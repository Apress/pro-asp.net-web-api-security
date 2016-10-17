using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TalentManagerBasic.Models;

namespace TalentManagerBasic
{
    public class Repository : IRepository
    {
        public Employee Add(Employee human)
        {
            human.Id = 12345; // simulating data store generating the Id
            return human;
        }
    }
}