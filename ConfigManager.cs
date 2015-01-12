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

            DrawMenu.AddItem(new MenuItem("draw_Qrange","Draw Q").SetValue(new Circle(true, Color.Red)));
            DrawMenu.AddItem(new MenuItem("draw_Wrange", "Draw W").SetValue(new Circle(true, Color.Blue)));
            DrawMenu.AddItem(new MenuItem("draw_Erange", "Draw E").SetValue(new Circle(true, Color.Green)));
            DrawMenu.AddItem(new MenuItem("draw_Rrange", "Draw R").SetValue(new Circle(true, Color.White)));
        }

        #region old things
        public static void set_menu()
        {
            var ComboMenu = new Menu("Combo", "Combo");
            championMenu.AddSubMenu(ComboMenu);
            ComboMenu.AddItem(new MenuItem("combo_active", "Active").SetValue(true));
            ComboMenu.AddItem(new MenuItem("combo_key", "Key :").SetValue(new KeyBind(32, KeyBindType.Press)));

            var HarassMenu = new Menu("Harass", "Harass");
            championMenu.AddSubMenu(HarassMenu);
            HarassMenu.AddItem(new MenuItem("harass_active", "Active").SetValue(true));
            HarassMenu.AddItem(new MenuItem("harass_key", "Key :").SetValue(new KeyBind('T', KeyBindType.Press)));

            var LasthitMenu = new Menu("LastHit", "LastHit");
            championMenu.AddSubMenu(LasthitMenu);
            LasthitMenu.AddItem(new MenuItem("lasthit_key", "Key :").SetValue(new KeyBind('C', KeyBindType.Press)));

        }

        public static baseMenuItem m_Items = new baseMenuItem();
        public class baseMenuItem
        {
            public string COMBO_ACTIVE = "combo_active";
            public string HARASS_ACTIVE = "harass_active";
            public string COMBO_KEY = "combo_key";
            public string HARASS_KEY = "harass_key";
        }
        #endregion
    }
}
