using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using GoogleAuthWebApi.Helpers;

namespace GoogleAuthWebApi.Infrastructure
{
    public class Totp
    {
        public static IList<string> GetPastCurrentFutureOtp(string base32EncodedSecret)
        {
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

            long counter = (long)Math.Floor((DateTime.UtcNow - epochStart).TotalSeconds / 30);

            var otps = new List<string>();
            otps.Add(GetHotp(base32EncodedSecret, counter - 1)); // previous OTP
            otps.Add(GetHotp(base32EncodedSecret, counter)); // current OTP
            otps.Add(GetHotp(base32EncodedSecret, counter + 1)); // next OTP

            return otps;
        }

        private static string GetHotp(string base32EncodedSecret, long counter)
        {
            byte[] message = BitConverter.GetBytes(counter).Reverse().ToArray(); // Assuming Intel machine (little endian)

            byte[] secret = base32EncodedSecret.ToByteArray();

            string s = secret.ToBase32String();
            bool b = base32EncodedSecret.Equals(s);

            HMACSHA1 hmac = new HMACSHA1(secret, true);

            byte[] hash = hmac.ComputeHash(message);

            int offset = hash[hash.Length - 1] & 0xf;
            int truncatedHash = ((hash[offset] & 0x7f) << 24) |
                                    ((hash[offset + 1] & 0xff) << 16) |
                                        ((hash[offset + 2] & 0xff) << 8) |
                                            (hash[offset + 3] & 0xff);

            int hotp = truncatedHash % 1000000; // 6 digits code and hence 10 power 6, that is a million
            return hotp.ToString().PadLeft(6, '0');
        }
    }

}