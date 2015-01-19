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
        public static string championMenuName = "[Kor AIO] "  + ObjectManager.Player.ChampionName;
        public static string UtilityMenuName = "[Kor AIO] Utility";
        public static Menu championMenu = new Menu(championMenuName, championMenuName, true);
        public static Menu utilityMenu = new Menu(UtilityMenuName, UtilityMenuName, true);

        public static Orbwalking.Orbwalker Orbwalker;

        public static void LoadMenu()
        {
            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            orbwalkMenu.SubMenu("Misc").AddItem(new MenuItem("disMovement", "Disable Movement", true)).SetValue(false);
            orbwalkMenu.SubMenu("Misc").AddItem(new MenuItem("disAttack", "Disable Attack", true)).SetValue(false);
            championMenu.AddSubMenu(orbwalkMenu);

            var tsMenu = new Menu("TargetSelector", "TargetSelector");
            championMenu.AddSubMenu(tsMenu);
            TargetSelector.AddToMenu(tsMenu);

            var combo = new Menu("Combo", "Combo");
            championMenu.AddSubMenu(combo);

            var harass = new Menu("Harass", "Harass");
            championMenu.AddSubMenu(harass);

            var laneclear = new Menu("Lane Clear", "LaneClear");
            championMenu.AddSubMenu(laneclear);

            var jungleclear = new Menu("Jungle Clear", "JungleClear");
            championMenu.AddSubMenu(jungleclear);

            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("usePacket", "Packets", true)).SetValue(true);
            championMenu.AddSubMenu(misc);

            var DrawMenu = new Menu("Drawings", "Draw");
            championMenu.AddSubMenu(DrawMenu);

            DrawMenu.AddItem(new MenuItem("draw_Qrange","Draw Q").SetValue(new Circle(true, Color.Red)));
            DrawMenu.AddItem(new MenuItem("draw_Wrange", "Draw W").SetValue(new Circle(true, Color.Blue)));
            DrawMenu.AddItem(new MenuItem("draw_Erange", "Draw E").SetValue(new Circle(true, Color.Green)));
            DrawMenu.AddItem(new MenuItem("draw_Rrange", "Draw R").SetValue(new Circle(true, Color.White)));

            championMenu.AddToMainMenu();


            var potionMenu = new Menu("Potion Control", "Potion");
            new PotionManager().Load(potionMenu);
            utilityMenu.AddSubMenu(potionMenu);


            var IgniteMenu = new Menu("Ignite", "Ignite");
            IgniteMenu.AddItem(new MenuItem("ignite_enable", "Enable").SetValue(true));
            utilityMenu.AddSubMenu(IgniteMenu);



            utilityMenu.AddToMainMenu();
        }
    }
}
