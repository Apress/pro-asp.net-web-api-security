using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Security.Cryptography;

namespace JWT
{
    public class JsonWebEncryptedToken
    {
        private const string TYPE_HEADER = "typ";
        private const string JSON_WEB_TOKEN = "JWT";
        private const string ENCRYPTION_ALGORITHM_HEADER = "alg";
        private const string ENCRYPTION_METHOD_HEADER = "enc";
        private const string RSA_OAEP = "RSA-OAEP";
        private const string AES_256_GCM = "A256GCM";
        private const string EXPIRATION_TIME_CLAIM = "exp";
        private const string ISSUER_CLAIM = "iss";
        private const string AUDIENCE_CLAIM = "aud";

        private static readonly TimeSpan lifeTime = new TimeSpan(0, 2, 0);
        private static readonly DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        private Dictionary<string, string> claims = new Dictionary<string, string>();

        public JsonWebEncryptedToken()
        {
            TimeSpan ts = DateTime.UtcNow - epochStart + lifeTime;
            this.ExpiresOn = Convert.ToUInt64(ts.TotalSeconds);
        }

        [JsonProperty(PropertyName = TYPE_HEADER)]
        public string Type
        {
            get { return JSON_WEB_TOKEN; }
        }

        [JsonProperty(PropertyName = ENCRYPTION_ALGORITHM_HEADER)]
        public string EncryptionAlgorithm
        {
            get { return RSA_OAEP; }
        }

        [JsonProperty(PropertyName = ENCRYPTION_METHOD_HEADER)]
        public string EncryptionMethod
        {
            get { return AES_256_GCM; }
        }


        [JsonIgnore]
        public string AsymmetricKey { get; set; }

        [JsonIgnore]
        public IList<Claim> Claims
        {
            get
            {
                return this.claims.Keys.SelectMany(key =>
                                            this.claims[key].Split(',')
                                                .Select(value => new Claim(key, value))).ToList();
            }
        }

        [JsonIgnore]
        public ulong ExpiresOn
        {
            get
            {
                return UInt64.Parse(this.claims[EXPIRATION_TIME_CLAIM]);
            }
            private set
            {
                this.claims.Add(EXPIRATION_TIME_CLAIM, value.ToString());
            }
        }

        [JsonIgnore]
        public string Issuer
        {
            get
            {
                return this.claims.ContainsKey(ISSUER_CLAIM) ? this.claims[ISSUER_CLAIM] : String.Empty;
            }
            set
            {
                this.claims.Add(ISSUER_CLAIM, value);
            }
        }

        [JsonIgnore]
        public string Audience
        {
            get
            {
                return this.claims.ContainsKey(AUDIENCE_CLAIM) ? this.claims[AUDIENCE_CLAIM] : String.Empty;
            }
            set
            {
                this.claims.Add(AUDIENCE_CLAIM, value);
            }
        }

        public void AddClaim(string claimType, string value)
        {
            if (this.claims.ContainsKey(claimType))
                this.claims[claimType] = this.claims[claimType] + "," + value;
            else
                this.claims.Add(claimType, value);
        }

        public override string ToString()
        {
            string header = JsonConvert.SerializeObject(this);
            string claims = JsonConvert.SerializeObject(this.claims);

            // Generate a 256 bit random Content Master Key and a 96 bit initialization vector
            byte[] masterKey = new byte[32];
            byte[] initVector = new byte[12];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(masterKey);
                provider.GetBytes(initVector);
            }

            byte[] encryptedMasterKey = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(this.AsymmetricKey);
                encryptedMasterKey = rsa.Encrypt(masterKey, true); // OAEP Padding
            }

            var authData = new EncryptedPayload()
            {
                Header = header,
                EncryptedMasterKey = encryptedMasterKey,
                InitializationVector = initVector
            };

            byte[] additionalAuthenticatedData = authData.ToAdditionalAuthenticatedData();

            byte[] tag = null;
            byte[] cipherText = null;

            using (var aes = new AuthenticatedAesCng())
            {
                aes.CngMode = CngChainingMode.Gcm; // Galois/Counter Mode
                aes.Key = masterKey;
                aes.IV = initVector;
                aes.AuthenticatedData = additionalAuthenticatedData;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (IAuthenticatedCryptoTransform encryptor = aes.CreateAuthenticatedEncryptor())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            // Encrypt the claims set
                            byte[] claimsSet = Encoding.UTF8.GetBytes(claims);
                            cs.Write(claimsSet, 0, claimsSet.Length);
                            cs.FlushFinalBlock();
                            tag = encryptor.GetTag();
                            cipherText = ms.ToArray();
                        }
                    }
                }
            }

            var payload = new EncryptedPayload()
            {
                Header = header,
                EncryptedMasterKey = encryptedMasterKey,
                InitializationVector = initVector,
                CipherText = cipherText,
                Tag = tag
            };

            string token = payload.ToString();

            return token;
        }

        public static JsonWebEncryptedToken Parse(string token, string privateKey)
        {
            byte[] claimSet = null;
            EncryptedPayload payload = null;

            try
            {
                payload = EncryptedPayload.Parse(token);

                byte[] masterKey = null;
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKey);
                    masterKey = rsa.Decrypt(payload.EncryptedMasterKey, true);
                }

                byte[] additionalAuthenticatedData = payload.ToAdditionalAuthenticatedData();
                using (AuthenticatedAesCng aes = new AuthenticatedAesCng())
                {
                    aes.CngMode = CngChainingMode.Gcm;
                    aes.Key = masterKey;
                    aes.IV = payload.InitializationVector;
                    aes.AuthenticatedData = additionalAuthenticatedData;
                    aes.Tag = payload.Tag;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(),
                                                                                                                  CryptoStreamMode.Write))
                        {
                            byte[] cipherText = payload.CipherText;
                            cs.Write(cipherText, 0, cipherText.Length);
                            cs.FlushFinalBlock();

                            claimSet = ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Invalid Token", ex);
            }

            var jwt = JsonConvert.DeserializeObject<JsonWebEncryptedToken>(payload.Header);
            jwt.AsymmetricKey = privateKey;
            jwt.claims = JsonConvert.DeserializeObject
                                                                  <Dictionary<string, string>>(Encoding.UTF8.GetString(claimSet));

            TimeSpan ts = DateTime.UtcNow - epochStart;

            if (jwt.ExpiresOn < Convert.ToUInt64(ts.TotalSeconds))
                throw new SecurityException("Token has expired");

            return jwt;
        }

    }

}
