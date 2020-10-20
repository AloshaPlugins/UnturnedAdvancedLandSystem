using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedHouseSystem.Models
{
    public class Member
    {
        public Member(string name, ulong id)
        {
            Name = name;
            Id = id;
        }

        public string Name;
        public ulong Id;
    }
}
