using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Managers;
using AdvancedHouseSystem.Models;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Framework.Debug;

namespace AdvancedHouseSystem.Commands
{
    public class CommandShow : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] args)
        {
            var player = caller as UnturnedPlayer;
            var land = LandManager.GetPositionToLand(player.Position);
            if (land == null) return;
            if (land.Author != player.CSteamID.m_SteamID &&
                !land.Members.Any(e => e.Id == player.CSteamID.m_SteamID)) return;
            if (land.Sale && land.Author == player.CSteamID.m_SteamID) return;
            ControlManager.ShowMenu(player.Player, land);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "show";

        public string Help => "Arsa işlemlerini burada yapabilirsin.";

        public string Syntax => "show";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>(){ "alosha.show" };
    }
}
