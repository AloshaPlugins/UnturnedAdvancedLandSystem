using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Helper;
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

        public static IEnumerator TaxUpdater(float checkDuration)
        {
            var lastUpdate = DateTime.Now;
            while (true)
            {
                yield return new WaitForSeconds(checkDuration);
                if (!Main.Instance.Configuration.Instance.Lands.Any()) continue;
                var landTypes = Main.Instance.Configuration.Instance.LandTypes;
                var lands = Main.Instance.Configuration.Instance.Lands.Where(land => !land.TaxExpired && land.Author != 0);

                foreach (var land in lands)
                {
                    var type = landTypes.FirstOrDefault(landType => landType.Id == land.TypeId);
                    if (type == null) type = Main.Instance.Configuration.Instance.DefaultLandType;
                    if ((DateTime.Now - lastUpdate).TotalSeconds > type.TaxDurationForMinutes * 60)
                    {
                        land.Tax += type.UpTaxPrice;
                        if (land.Tax >= type.MaxTax)
                        {
                            land.TaxExpired = true;
                            Save();
                        }
                    }
                }
            }
        }

        public static IEnumerator ExpiredTaxUpdater(float checkDuration)
        {
            while (true)
            {
                yield return new WaitForSeconds(checkDuration);
                if (!Main.Instance.Configuration.Instance.Lands.Any()) continue;
                var landTypes = Main.Instance.Configuration.Instance.LandTypes;
                var lands = Main.Instance.Configuration.Instance.Lands.Where(land => land.TaxExpired);

                foreach (var land in lands)
                {
                    if ((DateTime.Now - land.LastTax).TotalDays >= 1)
                    {
                        var type = landTypes.FirstOrDefault(landType => landType.Id == land.TypeId);
                        if (type == null) type = Main.Instance.Configuration.Instance.DefaultLandType;
                        land.LastTax = DateTime.Now;
                        land.TaxDay += 1;
                        if (land.TaxDay >= type.AllowDay)
                        {
                            var radius = CustomMath.SqrtDistanceFromCube(land.X1, land.X2, land.Z1, land.Z2);

                            var center = new Vector3(((land.X1 - land.X2) / 2), (land.Y1 - land.Y2), ((land.Z1 - land.Z2) / 2));
                            List<RegionCoordinate> regionCoordinates = new List<RegionCoordinate>();
                            Regions.getRegionsInRadius(center, radius, regionCoordinates);

                            List<Transform> barricades = new List<Transform>();
                            BarricadeManager.getBarricadesInRadius(center, radius, regionCoordinates, barricades);
                            List<Transform> structers = new List<Transform>();
                            StructureManager.getStructuresInRadius(center, radius, regionCoordinates, structers);

                            foreach (var barricade in barricades)
                            {
                                if (!LandManager.InLand(barricade.position, land)) continue;
                                if(!BarricadeManager.tryGetInfo(barricade, out var x, out var y, out var plant,
                                    out var index, out var region)) continue;

                                DamageTool.getBarricadeRootTransform(barricade);
                                var barricadeData = region.barricades[index];
                                if (barricadeData.owner == land.Author) BarricadeManager.destroyBarricade(region, x, y, plant, index);
                            }

                            foreach (var @struct in structers)
                            {
                                if (!LandManager.InLand(@struct.position, land)) continue;
                                if (!StructureManager.tryGetInfo(@struct, out var x, out var y, out var index,
                                    out var region)) continue;

                                var structData = region.structures[index];
                                if (structData.owner == land.Author) StructureManager.destroyStructure(region, x, y, index, @struct.position);
                            }

                            land.TaxExpired = false;
                            land.TaxDay = 0;
                            land.LastTax = DateTime.Now;
                            land.Members = new List<Member>();
                            land.Author = 0;
                            land.Tax = 0;
                            land.Price = type.Price;
                            land.Sale = true;
                            land.Name = type.Name;
                            Save();
                        }
                    }
                }
            }
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