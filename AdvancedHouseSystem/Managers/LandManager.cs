using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Models;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace AdvancedHouseSystem.Managers
{
    public static class LandManager
    {
        public static Land GetPositionToLand(Vector3 position) =>
            Main.Instance.Configuration.Instance.Lands.FirstOrDefault(land =>
                land.X1 <= position.x && position.x <= land.X2 && land.Z1 <= position.z && position.z < land.Z2);

        public static Land GetPositionToLand(float x, float z) =>
            Main.Instance.Configuration.Instance.Lands.FirstOrDefault(land =>
                land.X1 <= x && x <= land.X2 && land.Z1 <= z && z < land.Z2);
        public static bool InLand(Vector3 pos, Land land) =>
            land.X1 <= pos.x && pos.x <= land.X2 && land.Z1 <= pos.z && pos.z < land.Z2;
        public static SteamPlayer GetPlayer(ulong id) =>
            Provider.clients.FirstOrDefault(client => client.playerID.steamID.m_SteamID == id);

        public static int NewId()
        {
            if (Main.Instance.Configuration.Instance.Lands.Count <= 0) return 1;
            else return Main.Instance.Configuration.Instance.Lands.Last().Id + 1;
        }


        public static void RemoveMember(Land land, ulong id)
        {
            land.Members.RemoveAll(m => m.Id == id);
        }
        public static bool AddMember(Land land, ulong id, string name)
        {
            if (land.Members.Any(member => member.Id == id)) return false;
            land.Members.Add(new Member()
            {
                Name = name,
                Id = id
            });
            return true;
        }
        public static void Save() => Main.Instance.Configuration.Save();

    }
}
