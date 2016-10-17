using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyContacts.Models
{
    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal DollarsSpentInStore { get; set; }
        public int LoyaltyPoints { get; set; }
        public string Owner { get; set; }

        public static IEnumerable<Contact> GenerateContacts()
        {
            yield return new Contact()
            {
                Name = "Tom",
                Email = "tom@nowhere.com",
                Address = "123 Oak Circle, GermanTown, AB 12345",
                DollarsSpentInStore = 1234.56M,
                LoyaltyPoints = 1000,
                Owner = "jqhuman"
            };

            yield return new Contact()
            {
                Name = "Dick",
                Email = "dick@anywhere.com",
                Address = "987 Cedar Circle, DutchTown, YZ 98765",
                DollarsSpentInStore = 1784.96M,
                LoyaltyPoints = 1500,
                Owner = "jqhuman"
            };

            yield return new Contact()
            {
                Name = "Harry",
                Email = "harry@somewhere.com",
                Address = "567 Birch Circle, FrenchTown, UA 34589",
                DollarsSpentInStore = 14567.43M,
                LoyaltyPoints = 12000,
                Owner = "jqhuman"
            };

            yield return new Contact()
            {
                Name = "Tom",
                Email = "tom@missing.com",
                Address = "493 Hemlock Circle, SpanishTown, MB 53293",
                DollarsSpentInStore = 145.47M,
                LoyaltyPoints = 100,
                Owner = "jqlaw"
            };
        }
    }
}