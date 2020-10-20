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

            var selected = args[0];

            if (selected == "create")
            {
                var name = args[1];
                var price = uint.Parse(args[2]);
                var sale = bool.Parse(args[3]);
                var id = LandManager.NewId();
                var land = new Land()
                {
                    Members = new List<Member>(),
                    Author = 0,
                    Id = id,
                    Z1 = 0,
                    Tax = 0,
                    X1 = 0,
                    X2 = 0,
                    UpTax = 0,
                    Name = name,
                    Z2 = 0,
                    Sale = sale,
                    ForRent = false,
                    Price = price
                };
                Main.Instance.Configuration.Instance.Lands.Add(land);
                LandManager.Save();
                UnturnedChat.Say($"ev oluşturdun {id} numaralı ev oluşturdun.");
                return;
            }

            else if (selected == "modify")
            {
                var id = int.Parse(args[1]);
                var land = Main.Instance.Configuration.Instance.Lands.FirstOrDefault(l => l.Id == id);


                var selected2 = args[2];
                if (selected2 == "position")
                {
                    var selected3 = args[3];
                    if (selected3 == "1")
                    {
                        land.X1 = player.Position.x;
                        land.Z1 = player.Position.z;
                        LandManager.Save();
                        UnturnedChat.Say($"yeni ev güncellendi {land.X1}, {land.Z1}");
                        return;
                    }
                    else if (selected3 == "2")
                    {
                        land.X2 = player.Position.x;
                        land.Z2 = player.Position.z;
                        LandManager.Save();
                        UnturnedChat.Say($"yeni ev güncellendi {land.X2}, {land.Z2}");
                        return;
                    }
                }
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "land";

        public string Help => "Arsa işlemlerini burada yapabilirsin.";

        public string Syntax => "land";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>(){"alosha.land"};
    }
}
