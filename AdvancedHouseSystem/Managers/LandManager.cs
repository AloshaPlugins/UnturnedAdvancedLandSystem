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

        public static SteamPlayer GetPlayer(ulong id) =>
            Provider.clients.FirstOrDefault(client => client.playerID.steamID.m_SteamID == id);

    }
}
