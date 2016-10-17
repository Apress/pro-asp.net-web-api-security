using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using MyContacts.Helpers;

namespace MyContacts.Infrastructure
{
    public class JsonWebToken
    {
        private const string TYPE_HEADER = "typ";
        private const string JSON_WEB_TOKEN = "JWT";
        private const string SIGNING_ALGORITHM_HEADER = "alg";
        private const string HMAC_SHA256 = "HS256";
        private const string EXPIRATION_TIME_CLAIM = "exp";
        private const string ISSUER_CLAIM = "iss";
        private const string AUDIENCE_CLAIM = "aud";

        private static readonly TimeSpan lifeTime = new TimeSpan(0, 2, 0);
        private static readonly DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        private byte[] keyBytes = null;
        private Dictionary<string, string> claims = new Dictionary<string, string>();

        public JsonWebToken()
        {
            TimeSpan ts = DateTime.UtcNow - epochStart + lifeTime;
            this.ExpiresOn = Convert.ToUInt64(ts.TotalSeconds);
        }

        [JsonProperty(PropertyName = TYPE_HEADER)]
        public string Type
        {
            get { return JSON_WEB_TOKEN; }
        }

        [JsonProperty(PropertyName = SIGNING_ALGORITHM_HEADER)]
        public string SignatureAlgorithm
        {
            get { return HMAC_SHA256; }
        }


        [JsonIgnore]
        public string SymmetricKey
        {
            get
            {
                return Convert.ToBase64String(keyBytes);
            }
            set
            {
                keyBytes = Convert.FromBase64String(value);
            }
        }

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
            string header = JsonConvert.SerializeObject(this).ToBase64String();
            string claims = JsonConvert.SerializeObject(this.claims).ToBase64String();
            string signature = String.Empty;

            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                string data = String.Format("{0}.{1}", header, claims);
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                signature = signatureBytes.ToBase64String();
            }

            return String.Format("{0}.{1}.{2}", header, claims, signature);
        }

        public static JsonWebToken Parse(string token, string secretKey)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                throw new SecurityException("Bad token");

            string header = Encoding.UTF8.GetString(parts[0].ToByteArray());
            string claims = Encoding.UTF8.GetString(parts[1].ToByteArray());
            byte[] incomingSignature = parts[2].ToByteArray();
            string computedSignature = String.Empty;

            var jwt = JsonConvert.DeserializeObject<JsonWebToken>(header);
            jwt.SymmetricKey = secretKey;
            jwt.claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(claims);

            using (HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(secretKey)))
            {
                string data = String.Format("{0}.{1}", parts[0], parts[1]);
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                computedSignature = signatureBytes.ToBase64String();
            }

            if (!computedSignature.Equals(incomingSignature.ToBase64String(), StringComparison.Ordinal))
                throw new SecurityException("Signature is invalid");

            TimeSpan ts = DateTime.UtcNow - epochStart;

            if (jwt.ExpiresOn < Convert.ToUInt64(ts.TotalSeconds))
                throw new SecurityException("Token has expired");

            return jwt;
        }
    }

}