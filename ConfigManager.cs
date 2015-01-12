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
        public static string menuName = "[Kor AIO] " + ObjectManager.Player.ChampionName;
        public static Menu baseMenu = new Menu(menuName, menuName, true);
        public static Orbwalking.Orbwalker Orbwalker;

        public static Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return Orbwalker.ActiveMode; }
        }

        public static void LoadMenu()
        {
            baseMenu.AddToMainMenu();
            var tsMenu = new Menu("TargetSelector", "TargetSelector"); //TargetSelector Menu
            baseMenu.AddSubMenu(tsMenu);
            TargetSelector.AddToMenu(tsMenu);

            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            baseMenu.AddSubMenu(orbwalkMenu);

            var potionMenu = new Menu("Potion Control", "Potion");
            new PotionManager().Load(potionMenu);
            baseMenu.AddSubMenu(potionMenu);

            var DrawMenu = new Menu("Drawings", "Draw");
            baseMenu.AddSubMenu(DrawMenu);

            DrawMenu.AddItem(new MenuItem("Draw Q", "draw_Qrange").SetValue(new Circle(true, Color.Red)));
            DrawMenu.AddItem(new MenuItem("Draw W", "draw_Wrange").SetValue(new Circle(true, Color.Blue)));
            DrawMenu.AddItem(new MenuItem("Draw E", "draw_Erange").SetValue(new Circle(true, Color.Green)));
            DrawMenu.AddItem(new MenuItem("Draw R", "draw_Rrange").SetValue(new Circle(true, Color.White)));
        }
    }
}
