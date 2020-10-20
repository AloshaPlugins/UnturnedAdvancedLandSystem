using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Models;
using SDG.Framework.UI.Devkit;
using SDG.Unturned;
using Steamworks;

namespace AdvancedHouseSystem.Managers
{
    public static class ControlManager
    {
        private static string Prefix = "ADH2";

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


        }

        public static void ShowMenu(Player player, Land land)
        {
            SetModal(player, true);
            CSteamID id = player.channel.owner.playerID.steamID;
            short key = 454;
            EffectManager.sendUIEffect(Main.Instance.Configuration.Instance.MainEffect, key, id, true, Main.Instance.Configuration.Instance.Name);

            EffectManager.sendUIEffectText(key, id, true, "Home_Name_Text", land.Name);
            EffectManager.sendUIEffectText(key, id, true, "Home_Price_Text", $"<color=green>$</color>{Humanize(land.Price)}");
            EffectManager.sendUIEffectText(key, id, true, "Count_Group_Text", $"{Provider.clients.Where(client => land.Members.Any(member => member.Id == client.player.channel.owner.playerID.steamID.m_SteamID)).Count()}/{land.Members.Count}");
            EffectManager.sendUIEffectText(key, id, true, "Tax_Text", $"<color=green>$</color>{Humanize(land.Tax)}");
            EffectManager.sendUIEffectText(key, id, true, "Tax_Text_0", $"<color=green>$</color>{Humanize(land.UpTax)}<COLOR=red>/dakika</color>");

            if (land.Author == id.m_SteamID)
            {
                // TODO: Eğer evin sahibiyse ev sahibi özellikleri.
            }
        }

        public static void CloseMenu(Player player)
        {
            SetModal(player, false);

            // TODO: Menü listeden kaldırılacak. UI kapatılacak.
        }

        // helal lan
        public static void SetModal(Player player, bool activity = true)
        {
            player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, activity);
        }

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
        public Land land;
        public int page;
    }
}
