using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Models;
using Rocket.Unturned.Player;
using SDG.Framework.UI.Devkit;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace AdvancedHouseSystem.Managers
{
    public static class ControlManager
    {
        private static string Prefix = "ADH2_";

        public static List<Screen> Screens = new List<Screen>();

        public static void OnButtonClicked(Player player, string buttonName)
        {
            if (!buttonName.StartsWith(Prefix)) return;
            buttonName = buttonName.Substring(Prefix.Length);

            var screen = Screens.FirstOrDefault(se => se.Id == player.channel.owner.playerID.steamID);
            if (screen == null) return;



        }

        public static void OnTyped(Player player, string buttonName, string text)
        {
            if (!buttonName.StartsWith(Prefix)) return;
            buttonName = buttonName.Substring(Prefix.Length);
            var untPlayer = UnturnedPlayer.FromPlayer(player);
            var id = player.channel.owner.playerID.steamID;
            var screen = Screens.FirstOrDefault(s => s.Id == id);
            if (screen == null)
            {
                CloseMenu(player);
                return;
            }

            if (buttonName == "Sayfa_Ileri")
            {
                var nextPage = screen.Page + 1;
                var members = GetPageToMembers(screen.Land.Members, nextPage, 5);
                if (members.Count <= 0) return;
                screen.Page += 1;
                ShowMenu(player, screen.Land);
                return;
            }
            if (buttonName == "Sayfa_Geri")
            {
                var nextPage = screen.Page - 1;
                if (nextPage <= 0) return;
                var members = GetPageToMembers(screen.Land.Members, nextPage, 5);
                if (members.Count <= 0) return;
                screen.Page -= 1;
                ShowMenu(player, screen.Land);
                return;
            }

            if (buttonName == "VergiOde" && screen.Land.Author == id.m_SteamID)
            {
                var tax = screen.Land.Tax;
                if (untPlayer.Experience < tax)
                {
                    ChatManager.serverSendMessage("<size=20><color=red>YETERSIZ BAKIYE</color></size> Vergi borcunuzu ödemek için bakiyeniz yeterli değil.", Color.white, player.channel.owner, player.channel.owner, EChatMode.LOCAL, default, true);
                    return;
                }

                untPlayer.Experience -= tax;
                screen.Land.Tax = 0;
                screen.Land.Save();
                ShowMenu(player, screen.Land);
                return;
            }

            if (buttonName == "Sat" && screen.Land.Author == id.m_SteamID)
            {
                // TODO: SATMA FİYATLARI BELİRTİLMESİ İÇİN YER.
                return;
            }

            if (buttonName == "Devret" && screen.Land.Author == id.m_SteamID)
            {
                // TODO: Birisine evi devretmesi için kullanılacak yer.
                return;
            }
        }

        public static void ShowMenu(Player player, Land land)
        {
            CSteamID id = player.channel.owner.playerID.steamID;

            var screen = Screens.FirstOrDefault(s => s.Id == id);
            if (screen == null)
            {
                screen = new Screen()
                {
                    Id = id,
                    Land = land,
                    Page = 1
                };
                Screens.Add(screen);
            }
            SetModal(player, true);
            short key = 454;
            EffectManager.sendUIEffect(Main.Instance.Configuration.Instance.MainEffect, key, id, true, Main.Instance.Configuration.Instance.Name);

            EffectManager.sendUIEffectText(key, id, true, "Home_Name_Text", land.Name);
            EffectManager.sendUIEffectText(key, id, true, "Home_Price_Text", $"<color=green>$</color>{Humanize(land.Price)}");
            EffectManager.sendUIEffectText(key, id, true, "Count_Group_Text", $"{Provider.clients.Where(client => land.Members.Any(member => member.Id == client.player.channel.owner.playerID.steamID.m_SteamID)).Count()}/{land.Members.Count}");
            EffectManager.sendUIEffectText(key, id, true, "Tax_Text", $"<color=green>$</color>{Humanize(land.Tax)}");
            EffectManager.sendUIEffectText(key, id, true, "Tax_Text_0", $"<color=green>$</color>{Humanize(land.UpTax)}<COLOR=red>/dakika</color>");

            if (land.Author == id.m_SteamID)
            {
                EffectManager.sendUIEffectVisibility(key, id, true, "Owner", true);
                EffectManager.sendUIEffectVisibility(key, id, true, "Adam_Çıkar", true);
                EffectManager.sendUIEffectVisibility(key, id, true, "Adam_Ekle", true);
                EffectManager.sendUIEffectVisibility(key, id, true, "Adam_Sıfırla", true);
            }
            var members = GetPageToMembers(land.Members, screen.Page, 5);
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                EffectManager.sendUIEffectText(key, id, true, $"Item_{i}_T", member.Name);
                EffectManager.sendUIEffectVisibility(key, id, true, $"Item_{i}", true);
            }
        }

        public static void CloseMenu(Player player)
        {
            SetModal(player, false);
            CSteamID id = player.channel.owner.playerID.steamID;
            Screens.RemoveAll(s => s.Id == id);
            
            EffectManager.askEffectClearByID(Main.Instance.Configuration.Instance.MainEffect, id);
        }

        // helal lan
        public static void SetModal(Player player, bool activity = true)
        {
            player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, activity);
        }

        public static List<Member> GetPageToMembers(List<Member> list, int page, int count) =>
            list.Skip(page * count - count).Take(page * count).ToList();
        public static string Humanize(uint number)
        {
            string[] suffix = { "f", "a", "p", "n", "μ", "m", string.Empty, "k", "M", "G", "T", "P", "E" };

            var absnum = Math.Abs(number);

            int mag;
            if (absnum < 1)
            {
                mag = (int)Math.Floor(Math.Floor(Math.Log10(absnum)) / 3);
            }
            else
            {
                mag = (int)(Math.Floor(Math.Log10(absnum)) / 3);
            }

            var shortNumber = number / Math.Pow(10, mag * 3);

            return $"{shortNumber:0.###}{suffix[mag + 6]}";
        }
    }

    public class Screen
    {
        public CSteamID Id;
        public Land Land;
        public int Page;
    }
}
