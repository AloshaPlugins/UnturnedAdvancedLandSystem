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
        public int Id, TypeId, TaxDay;
        public ulong Author;
        public List<Member> Members = new List<Member>();

        public bool Sale, TaxExpired;
        public DateTime LastTax;
        public uint Price, Tax;

        public float X1, Y1, Z1, X2, Y2, Z2;
    }
}
