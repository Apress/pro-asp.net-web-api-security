using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace JWT
{
    public class EncryptedPayload
    {
        public string Header { get; set; }
        public byte[] EncryptedMasterKey { get; set; }
        public byte[] InitializationVector { get; set; }
        public byte[] CipherText { get; set; }
        public byte[] Tag { get; set; }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}.{4}", Header.ToBase64String(),
                                                            EncryptedMasterKey.ToBase64String(),
                                                            InitializationVector.ToBase64String(),
                                                            CipherText.ToBase64String(),
                                                            Tag.ToBase64String());
        }

        public byte[] ToAdditionalAuthenticatedData()
        {
            string data = String.Format("{0}.{1}.{2}", Header.ToBase64String(),
                                                            EncryptedMasterKey.ToBase64String(),
                                                            InitializationVector.ToBase64String());
            return Encoding.UTF8.GetBytes(data);
        }

        public static EncryptedPayload Parse(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 5)
                throw new SecurityException("Bad token");

            return new EncryptedPayload()
            {
                Header = Encoding.UTF8.GetString(parts[0].ToByteArray()),
                EncryptedMasterKey = parts[1].ToByteArray(),
                InitializationVector = parts[2].ToByteArray(),
                CipherText = parts[3].ToByteArray(),
                Tag = parts[4].ToByteArray()
            };
        }
    }

}
