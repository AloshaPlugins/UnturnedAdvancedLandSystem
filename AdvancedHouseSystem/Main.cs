using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Managers;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace AdvancedHouseSystem
{
    public class Main : RocketPlugin<Config>
    {
        public static Main Instance;
        
        protected override void Load()
        {
            Instance = this;
            EffectManager.onEffectButtonClicked += ControlManager.OnButtonClicked;
            EffectManager.onEffectTextCommitted += ControlManager.OnTyped;
            BarricadeManager.onDeployBarricadeRequested += OnDeployBarricadeRequested;
            BarricadeManager.onSalvageBarricadeRequested += OnSalvageBarricadeRequested;
            StructureManager.onDeployStructureRequested += OnDeployStructureRequested;
            StructureManager.onSalvageStructureRequested += OnSalvageStructureRequested;
        }

        protected override void Unload()
        {
            Instance = null;
            EffectManager.onEffectButtonClicked -= ControlManager.OnButtonClicked;
            EffectManager.onEffectTextCommitted -= ControlManager.OnTyped;
            BarricadeManager.onDeployBarricadeRequested -= OnDeployBarricadeRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;
            StructureManager.onSalvageStructureRequested -= OnSalvageStructureRequested;
        }
        private void OnSalvageBarricadeRequested(CSteamID steamıd, byte x, byte y, ushort plant, ushort index, ref bool shouldallow)
        {
            if (steamıd == CSteamID.Nil) return;
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region)) return;

            var barricade = region.barricades[index];

            var player = LandManager.GetPlayer(steamıd.m_SteamID);
            var land = LandManager.GetPositionToLand(barricade.point);
            if (land == null)
            {
                if (Configuration.Instance.DoNotLandOutsideDeployAllItems && !player.isAdmin && !player
                    .ToUnturnedPlayer()
                    .HasPermission(Configuration.Instance.LandOutsideDeployBypassPermission)) shouldallow = false;
                UnturnedChat.Say("1");
                return;
                
            }

            if (land.Author != steamıd.m_SteamID && !land.Members.Any(member => member.Id == steamıd.m_SteamID))
            {
                UnturnedChat.Say("2");
                shouldallow = false;
                ChatManager.serverSendMessage($"<size=20><color=red>BU BÖLGEDEN YAPI KALDIRMA İZNİN YOK</color></size> Eğer bu bölgeye bir yapı kaldırmak istiyorsan bölge sahibiyle iletişime geçmen gerekiyor.", Color.white, player);
                return;
            }
        }

        private void OnSalvageStructureRequested(CSteamID steamıd, byte x, byte y, ushort index, ref bool shouldallow)
        {
            if (steamıd == CSteamID.Nil) return;
            if (!StructureManager.tryGetRegion(x, y, out var region)) return;

            var barricade = region.structures[index];

            var player = LandManager.GetPlayer(steamıd.m_SteamID);
            var land = LandManager.GetPositionToLand(barricade.point);
            if (land == null)
            {
                if (Configuration.Instance.DoNotLandOutsideDeployAllItems && !player.isAdmin && !player
                    .ToUnturnedPlayer()
                    .HasPermission(Configuration.Instance.LandOutsideDeployBypassPermission)) shouldallow = false;
                UnturnedChat.Say("3");
                return;
            }

            if (land.Author != steamıd.m_SteamID && !land.Members.Any(member => member.Id == steamıd.m_SteamID))
            {
                UnturnedChat.Say("4");
                shouldallow = false;
                ChatManager.serverSendMessage($"<size=20><color=red>BU BÖLGEDEN YAPI KALDIRMA İZNİN YOK</color></size> Eğer bu bölgeye bir yapı kaldırmak istiyorsan bölge sahibiyle iletişime geçmen gerekiyor.", Color.white, player);
                return;
            }
        }

        private void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong @group, ref bool shouldallow)
        {
            if (owner == CSteamID.Nil.m_SteamID) return;
            var player = LandManager.GetPlayer(owner);
            var land = LandManager.GetPositionToLand(point);
            if (land == null)
            {
                if (Configuration.Instance.DoNotLandOutsideDeployAllItems && !player.isAdmin && !player
                    .ToUnturnedPlayer()
                    .HasPermission(Configuration.Instance.LandOutsideDeployBypassPermission)) shouldallow = false;

                UnturnedChat.Say("5");
                return;
                
            }

            if (land.Author != owner && !land.Members.Any(member => member.Id == player.playerID.steamID.m_SteamID))
            {
                UnturnedChat.Say("6");
                shouldallow = false;
                ChatManager.serverSendMessage($"<size=20><color=red>BU BÖLGEDE YAPI İZNİN YOK</color></size> Eğer bu bölgeye bir yapı yerleştirmek istiyorsan bölge sahibiyle iletişime geçmen gerekiyor.", Color.white, player);
                return;
            }
        }

        private void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong @group, ref bool shouldallow)
        {
            if (owner == CSteamID.Nil.m_SteamID) return;
            var player = LandManager.GetPlayer(owner);
            var land = LandManager.GetPositionToLand(point);
            if (land == null)
            {
                UnturnedChat.Say("7");
                if (Configuration.Instance.DoNotLandOutsideDeployAllItems && !player.isAdmin && !player
                    .ToUnturnedPlayer()
                    .HasPermission(Configuration.Instance.LandOutsideDeployBypassPermission)) shouldallow = false;
            }

            if (land.Author != owner && !land.Members.Any(member => member.Id == player.playerID.steamID.m_SteamID))
            {
                UnturnedChat.Say("8");
                shouldallow = false;
                ChatManager.serverSendMessage($"<size=20><color=red>BU BÖLGEDE YAPI İZNİN YOK</color></size> Eğer bu bölgeye bir yapı yerleştirmek istiyorsan bölge sahibiyle iletişime geçmen gerekiyor.", Color.white, player);
                return;
            }
        }
    }
}