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
        public List<Member> Members;

        public bool Sale, ForRent;
        public uint Price, Tax, UpTax;

        public float X1, Z1, X2, Z2;

        public Land(string name, int id, ulong author, List<Member> members, bool sale, bool forRent, uint price, float x1, float z1, float x2, float z2)
        {
            Name = name;
            Id = id;
            Author = author;
            Members = members;
            Sale = sale;
            ForRent = forRent;
            Price = price;
            X1 = x1;
            Z1 = z1;
            X2 = x2;
            Z2 = z2;
        }


        public bool RemoveMember(ulong id)
        {
            var member = this.Members.FirstOrDefault(m => m.Id == id);
            if (member == null) return false;
            this.Members.Remove(member);
            return true;
        }
        public bool AddMember(ulong id, string name)
        {
            if (this.Members.Any(member => member.Id == id)) return false;
            this.Members.Add(new Member(name, id));
            return true;
        }
        public void Save() => Main.Instance.Configuration.Save();

    }
}
