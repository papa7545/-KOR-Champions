using Kor_AIO.Utilities;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kor_AIO
{
    internal class ConfigManager
    {
        public static string championMenuName = "[Kor AIO] " + ObjectManager.Player.ChampionName;
        public static string UtilityMenuName = "[Kor AIO] Utility";
        public static Menu championMenu = new Menu(championMenuName, championMenuName, true);
        public static Menu utilityMenu = new Menu(UtilityMenuName, UtilityMenuName, true);

        public static void LoadMenu()
        {
            utilityMenu.AddToMainMenu();
            championMenu.AddToMainMenu();

            var tsMenu = new Menu("TargetSelector", "TargetSelector"); //TargetSelector Menu
            championMenu.AddSubMenu(tsMenu);
            TargetSelector.AddToMenu(tsMenu);

            var potionMenu = new Menu("Potion Control", "Potion");
            new PotionManager().Load(potionMenu);
            utilityMenu.AddSubMenu(potionMenu);

            var DrawMenu = new Menu("Drawings", "Draw");
            championMenu.AddSubMenu(DrawMenu);

            DrawMenu.AddItem(new MenuItem("Draw Q", "draw_Qrange").SetValue(new Circle(true, Color.Red)));
            DrawMenu.AddItem(new MenuItem("Draw W", "draw_Wrange").SetValue(new Circle(true, Color.Blue)));
            DrawMenu.AddItem(new MenuItem("Draw E", "draw_Erange").SetValue(new Circle(true, Color.Green)));
            DrawMenu.AddItem(new MenuItem("Draw R", "draw_Rrange").SetValue(new Circle(true, Color.White)));
        }
    }
}
