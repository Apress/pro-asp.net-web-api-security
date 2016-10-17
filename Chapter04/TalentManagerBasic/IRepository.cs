using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentManagerBasic.Models;

namespace TalentManagerBasic
{
    public interface IRepository
    {
        Employee Add(Employee human);
    }
}
