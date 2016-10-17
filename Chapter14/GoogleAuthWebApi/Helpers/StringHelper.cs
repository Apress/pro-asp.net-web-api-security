using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAuthWebApi
{
    public static class StringHelper
    {
        private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static byte[] ToByteArray(this string secret)
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            var bits = secret.ToUpper().ToCharArray().Select(c =>
                                                        Convert.ToString(alphabet.IndexOf(c), 2)
                                                                .PadLeft(5, '0'))
                                                                    .Aggregate((a, b) => a + b);

            return Enumerable.Range(0, bits.Length / 8)
                                    .Select(i => Convert.ToByte(bits.Substring(i * 8, 8), 2))
                                        .ToArray();
        }

        public static string ToBase32String(this byte[] secret)
        {
            var bits = secret.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Aggregate((a, b) => a + b);

            return Enumerable.Range(0, bits.Length / 5)
                                    .Select(i => alphabet.Substring(
                                                            Convert.ToInt32(
                                                                        bits.Substring(i * 5, 5), 2), 1))
                                        .Aggregate((a, b) => a + b);
        }
    }
}
