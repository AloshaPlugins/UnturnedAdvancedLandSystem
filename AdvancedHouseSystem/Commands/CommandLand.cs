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
    public class CommandLand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] args)
        {
            var player = caller as UnturnedPlayer;
            if (args.Length <= 0) return;

        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "land";

        public string Help => "Arsa işlemlerini burada yapabilirsin.";

        public string Syntax => "land";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>(){"alosha.land"};
    }
}
