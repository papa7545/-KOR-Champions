using Kor_AIO.Utilities;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Drawing;
using System.Linq;

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
            LoadUtilityMenu();
            LoadChampionMenu();
        }

        public static void LoadUtilityMenu()
        {
            var potionMenu = new Menu("Potion Control", "Potion");
            new PotionManager().Load(potionMenu);
            utilityMenu.AddSubMenu(potionMenu);

            var IgniteMenu = new Menu("Ignite", "Ignite");
            IgniteMenu.AddItem(new MenuItem("ignite_enable", "Enable").SetValue(true));
            utilityMenu.AddSubMenu(IgniteMenu);

            utilityMenu.AddToMainMenu();
        }

        public static void LoadChampionMenu()
        {
            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");

            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);


            orbwalkMenu.SubMenu("Misc").AddItem(new MenuItem("disMovement", "Disable Movement", true)).SetValue(false);
            orbwalkMenu.SubMenu("Misc").AddItem(new MenuItem("disAttack", "Disable Attack", true)).SetValue(false);
            championMenu.AddSubMenu(orbwalkMenu);

            var tsMenu = new Menu("TargetSelector", "TargetSelector");
            championMenu.AddSubMenu(tsMenu);
            TargetSelector.AddToMenu(tsMenu);

            championMenu.AddSubMenu(new Menu("Combo", "Combo"));
            championMenu.AddSubMenu(new Menu("Harass", "Harass"));
            championMenu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));


            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("usePacket", "Packets", true)).SetValue(true);
            misc.AddItem(new MenuItem("useInterrupt", "Use Interrupt", true).SetValue(true));
            misc.AddItem(new MenuItem("useAntiGapCloser", "Use Anti-GapCloser", true).SetValue(true));
            championMenu.AddSubMenu(misc);

            var DrawMenu = new Menu("Drawings", "Drawings");
            championMenu.AddSubMenu(DrawMenu);
            DrawMenu.AddItem(new MenuItem("draw_Qrange", "Draw Q", true).SetValue(new Circle(true, Color.Red)));
            DrawMenu.AddItem(new MenuItem("draw_Wrange", "Draw W", true).SetValue(new Circle(true, Color.Blue)));
            DrawMenu.AddItem(new MenuItem("draw_Erange", "Draw E", true).SetValue(new Circle(true, Color.Green)));
            DrawMenu.AddItem(new MenuItem("draw_Rrange", "Draw R", true).SetValue(new Circle(true, Color.White)));

            championMenu.AddToMainMenu();
        }

        #region SetComboMethod
        public static void SetCombo(bool Q,bool W,bool E,bool R,bool UseQ = true,bool UseW = true,bool UseE = true,bool UseR = true)
        {
            if (UseQ)
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_Q", "Q",true).SetValue(Q));
            if (UseW)
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_W", "W", true).SetValue(W));
            if (UseE)
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_E", "E", true).SetValue(E));
            if (UseR)
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_R", "R", true).SetValue(R));
        }
        public static void SetCombo(bool[] SpellBoolList, bool[] SpellUseList = null)
        {
            if (SpellUseList == null)
                SpellUseList = new[] { true, true, true, true };

            if (SpellBoolList.Length < 4)
            {
                Array.Resize(ref SpellBoolList, 4);
                for (var i = 0; i < 4 - SpellBoolList.Length; i++)
                {
                    SpellBoolList[SpellBoolList.Length] = false;
                }
            }
            if (SpellUseList.Length < 4)
            {
                Array.Resize(ref SpellUseList, 4);
                for (var i = 0; i < 4 - SpellUseList.Length; i++)
                {
                    SpellUseList[SpellUseList.Length] = false;
                }
            }


            if (SpellUseList[0])
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_Q", "Q", true).SetValue(SpellBoolList[0]));
            if (SpellUseList[1])
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_W", "W", true).SetValue(SpellBoolList[1]));
            if (SpellUseList[2])
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_E", "E", true).SetValue(SpellBoolList[2]));
            if (SpellUseList[3])
                championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_R", "R", true).SetValue(SpellBoolList[3]));
        }

        public static void SetCombo(Spell[] SpellList,bool StateQ = true,bool StateW = true,bool StateE = true,bool StateR = true)
        {
            SpellSlot[] Support = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

            if (SpellList.Count() > 4)
                Program.PrintChat("Error! SpellList is so much. Resize SpellList", true, "SetCombo");


            foreach (var Spell in SpellList.Where(t => Support.Contains(t.Slot)))
            {
                if (Spell.Slot.ToString() == "Q")
                    championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_Q", "Q", true).SetValue(StateQ));
                if (Spell.Slot.ToString() == "W")
                    championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_W", "W",true).SetValue(StateW));
                if (Spell.Slot.ToString() == "E")
                    championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_E", "E",true).SetValue(StateE));
                if (Spell.Slot.ToString() == "R")
                    championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_R", "R",true).SetValue(StateR));
            }
        }
        #endregion SetComboMethod

        #region SetHarassMethod
        public static void SetHarass(bool Q, bool W, bool E, bool R, bool UseQ = true, bool UseW = true, bool UseE = true, bool UseR = true)
        {
            if (UseQ)
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_Q", "Q", true).SetValue(Q));
            if (UseW)
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_W", "W", true).SetValue(W));
            if (UseE)
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_E", "E", true).SetValue(E));
            if (UseR)
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_R", "R", true).SetValue(R));
        }
        public static void SetHarass(bool[] SpellBoolList, bool[] SpellUseList = null)
        {
            if (SpellUseList == null)
                SpellUseList = new[] { true, true, true, true };

            if (SpellBoolList.Length < 4)
            {
                Array.Resize(ref SpellBoolList, 4);
                for (var i = 0; i < 4 - SpellBoolList.Length; i++)
                {
                    SpellBoolList[SpellBoolList.Length] = false;
                }
            }
            if (SpellUseList.Length < 4)
            {
                Array.Resize(ref SpellUseList, 4);
                for (var i = 0; i < 4 - SpellUseList.Length; i++)
                {
                    SpellUseList[SpellUseList.Length] = false;
                }
            }


            if (SpellUseList[0])
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_Q", "Q", true).SetValue(SpellBoolList[0]));
            if (SpellUseList[1])
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_W", "W", true).SetValue(SpellBoolList[1]));
            if (SpellUseList[2])
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_E", "E", true).SetValue(SpellBoolList[2]));
            if (SpellUseList[3])
                championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_R", "R", true).SetValue(SpellBoolList[3]));
        }

        public static void SetHarass(Spell[] SpellList, bool StateQ = true, bool StateW = true, bool StateE = true, bool StateR = true)
        {
            SpellSlot[] Support = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

            if (SpellList.Count() > 4)
                Program.PrintChat("Error! SpellList is so much. Resize SpellList", true, "SetHarass");


            foreach (var Spell in SpellList.Where(t => Support.Contains(t.Slot)))
            {
                if (Spell.Slot.ToString() == "Q")
                    championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_Q", "Q", true).SetValue(StateQ));
                if (Spell.Slot.ToString() == "W")
                    championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_W", "W", true).SetValue(StateW));
                if (Spell.Slot.ToString() == "E")
                    championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_E", "E", true).SetValue(StateE));
                if (Spell.Slot.ToString() == "R")
                    championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_R", "R", true).SetValue(StateR));
            }
        }
        #endregion SetHarassMethod

    }
}
