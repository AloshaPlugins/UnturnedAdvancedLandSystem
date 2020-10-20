using System;
using System.Collections;
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
        private const short key = 454;
        private static string Prefix = "ADH2_";

        public static List<Screen> Screens = new List<Screen>();

        public static void OnButtonClicked(Player player, string buttonName)
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

            if (buttonName == "Return")
            {
                ShowMenu(player, screen.Land);
                return;
            }
            if (buttonName == "Close")
            {
                CloseMenu(player);
                return;
            }

            if (buttonName == "SayfaIleri")
            {
                var nextPage = screen.Page + 1;
                var members = GetPageToMembers(screen.Land.Members, nextPage, 5);
                if (members.Count <= 0) return;
                screen.Page += 1;
                ShowMenu(player, screen.Land);
                return;
            }

            if (buttonName == "SayfaGeri")
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
                LandManager.Save();
                ShowMenu(player, screen.Land);
                return;
            }

            if (buttonName == "Player_Remove" && screen.Land.Author == id.m_SteamID)
            {
                SendInput(player, "OYUNCU ÇIKARMA", "Evinizden birini çıkarmak istiyorsanız onun adını yazın.", "Oyuncu Adı..", "Onayla");
                screen.Action = IAction.RemovePlayer;
                return;
            }

            if (buttonName == "Player_Add" && screen.Land.Author == id.m_SteamID)
            {
                SendInput(player, "OYUNCU EKLEME", "Evinize birini eklemek istiyorsanız onun adını yazın.", "Oyuncu Adı..", "Onayla");
                screen.Action = IAction.AddPlayer;
                return;
            }

            if (buttonName == "Player_Reset" && screen.Land.Author == id.m_SteamID)
            {
                screen.Land.Members = new List<Member>();
                LandManager.Save();
                ShowMenu(player, screen.Land);
                return;
            }

            if (buttonName == "Sat" && screen.Land.Author == id.m_SteamID)
            {
                SendInput(player, "EV SATMA", "Evi satmak için aşağıya bir fiyat girin.", "Fiyat Girin...", "Onayla");
                screen.Action = IAction.Sell;
                return;
            }

            if (buttonName == "Devret" && screen.Land.Author == id.m_SteamID)
            {
                SendInput(player, "EV DEVRETME", "Evi devretmek için aşağıya eve kayıtlı olan bir oyuncu adı girin.", "Oyuncu Adı...", "Onayla");
                screen.Action = IAction.Transfer;
                return;
            }

            if (buttonName == "InputSucces")
            {
                switch (screen.Action)
                {
                    case IAction.Sell:
                        if (!uint.TryParse(screen.Input, out var result))
                        {
                            SendError(player, "Bir sayı girmelisin.", 5);
                            return;
                        }

                        screen.Land.Members = new List<Member>();
                        screen.Land.Sale = true;
                        screen.Land.Price = result;
                        LandManager.Save();
                        CloseMenu(player);
                        break;
                    case IAction.Transfer:
                        var victimPlayer = UnturnedPlayer.FromName(screen.Input);
                        if (victimPlayer == null)
                        {
                            SendError(player, "Böyle bir oyuncu yok..", 5);
                            return;
                        }

                        screen.Land.Author = victimPlayer.CSteamID.m_SteamID;
                        LandManager.Save();
                        ShowMenu(player, screen.Land);
                        break;
                    case IAction.AddPlayer:
                        var vP = UnturnedPlayer.FromName(screen.Input);
                        if (vP == null)
                        {
                            SendError(player, "Böyle bir oyuncu yok..", 5);
                            return;
                        }

                        LandManager.AddMember(screen.Land, vP.CSteamID.m_SteamID, vP.DisplayName);
                        LandManager.Save();
                        ShowMenu(player, screen.Land);
                        break;
                    case IAction.RemovePlayer:
                        var vp = UnturnedPlayer.FromName(screen.Input);
                        if (vp == null)
                        {
                            SendError(player, "Böyle bir oyuncu yok..", 5);
                            return;
                        }

                        if (!screen.Land.Members.Any(member => member.Id == vp.CSteamID.m_SteamID))
                        {
                            SendError(player, "Bu oyuncu senin arsana kayıtlı değil.", 5);
                            return;
                        }

                        LandManager.RemoveMember(screen.Land,vp.CSteamID.m_SteamID);
                        LandManager.Save();
                        ShowMenu(player, screen.Land);
                        break;
                    default:
                        ShowMenu(player, screen.Land);
                        return;
                }
            }
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

            if (buttonName == "VariableText")
            {
                if (string.IsNullOrEmpty(text))
                {
                    SendError(player, "Alanın içerisine boş bir şey yazamazsınız.", 10);
                    return;
                }
                screen.Input = text;
            }
        }

        public static void SendError(Player player, string text, float time)
        {
            CSteamID id = player.channel.owner.playerID.steamID;

            EffectManager.sendUIEffectText(key, id, true, "ErrorMessage", text);
            EffectManager.sendUIEffectVisibility(key, id, true, "ErrorMessage", true);
            Main.Instance.StartCoroutine(StartErrorLifetime(time, player));
        }

        private static IEnumerator StartErrorLifetime(float second, Player player)
        {
            yield return new WaitForSeconds(second);
            if (player == null) yield break;

            CSteamID id = player.channel.owner.playerID.steamID;
            EffectManager.sendUIEffectVisibility(key, id, true, "ErrorMessage", false);
        }

        public static void SendInput(Player player, string title, string description, string inputboxdesc, string succesbuttonname)
        {
            SetModal(player, true);
            CSteamID id = player.channel.owner.playerID.steamID;
            EffectManager.sendUIEffect(Main.Instance.Configuration.Instance.InputEffect, key, id, true, title, description, inputboxdesc, succesbuttonname);
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
            EffectManager.sendUIEffect(Main.Instance.Configuration.Instance.MainEffect, key, id, true, Main.Instance.Configuration.Instance.Name);

            EffectManager.sendUIEffectText(key, id, true, "Home_Name_Text", land.Name);
            EffectManager.sendUIEffectText(key, id, true, "Home_Price_Text", $"<color=green>$</color>{land.Price}");
            EffectManager.sendUIEffectText(key, id, true, "Count_Group_Text", $"{Provider.clients.Where(client => land.Members.Any(member => member.Id == client.player.channel.owner.playerID.steamID.m_SteamID)).Count()}/{land.Members.Count}");
            EffectManager.sendUIEffectText(key, id, true, "Tax_Text", $"<color=green>$</color>{land.Tax}");
            EffectManager.sendUIEffectText(key, id, true, "Tax_Text_0", $"<color=green>$</color>{land.UpTax}<COLOR=red>/dakika</color>");

            if (land.Author == id.m_SteamID)
            {
                EffectManager.sendUIEffectVisibility(key, id, true, "Owner", true);
                EffectManager.sendUIEffectVisibility(key, id, true, "ADH2_Player_Remove", true);
                EffectManager.sendUIEffectVisibility(key, id, true, "ADH2_Player_Add", true);
                EffectManager.sendUIEffectVisibility(key, id, true, "ADH2_Player_Reset", true);
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
        public static void SetModal(Player player, bool activity = true)
        {
            player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, activity);
        }

        public static List<Member> GetPageToMembers(List<Member> list, int page, int count) =>
            list.Skip(page * count - count).Take(page * count).ToList();
    }

    public class Screen
    {
        public CSteamID Id;
        public Land Land;
        public int Page;
        public IAction Action;
        public string Input = string.Empty;
    }
}