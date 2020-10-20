using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Steamworks;

namespace AdvancedHouseSystem.Models
{
    public class Land
    {
        public string Name;
        public int Id;
        public ulong Author;
        public List<Member> Members = new List<Member>();

        public bool Sale, ForRent;
        public uint Price, Tax, UpTax;

        public float X1, Z1, X2, Z2;
    }
}
