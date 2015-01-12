#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonChampions
{
    class Jproject_base:Program
    {


        public static string menuName = "Jeon"+ObjectManager.Player.ChampionName;
        public static Menu baseMenu = new Menu(menuName, menuName, true);

        public static Menu qMenu, wMenu, eMenu, rMenu;
        public static SpellDataInst Qdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);


           
        public Jproject_base()
        {
            set_menu();
            Drawing.OnEndScene += OnEndSence_base;
        }

        public static void set_menu()
        {
            baseMenu.AddToMainMenu();
            var tsMenu = new Menu("TargetSelector", "TargetSelector"); //TargetSelector Menu
            baseMenu.AddSubMenu(tsMenu);
            TargetSelector.AddToMenu(tsMenu);

            var ComboMenu = new Menu("Combo", "Combo");
            baseMenu.AddSubMenu(ComboMenu);
            ComboMenu.AddItem(new MenuItem("combo_active", "Active").SetValue(true));
            ComboMenu.AddItem(new MenuItem("combo_key", "Key :").SetValue(new KeyBind(32,KeyBindType.Press)));

            var HarassMenu = new Menu("Harass", "Harass");
            baseMenu.AddSubMenu(HarassMenu);
            HarassMenu.AddItem(new MenuItem("harass_active", "Active").SetValue(true));
            HarassMenu.AddItem(new MenuItem("harass_key", "Key :").SetValue(new KeyBind('T', KeyBindType.Press)));

            var LasthitMenu = new Menu("LastHit", "LastHit");
            baseMenu.AddSubMenu(LasthitMenu);
            LasthitMenu.AddItem(new MenuItem("lasthit_key", "Key :").SetValue(new KeyBind('C', KeyBindType.Press)));


            var MiscMenu = new Menu("Misc","Misc");
            baseMenu.AddSubMenu(MiscMenu);

            MiscMenu.AddItem(new MenuItem("misc_q_range", "DrawRange_Q").SetValue(new Circle(true,Color.Red)));
            MiscMenu.AddItem(new MenuItem("misc_w_range", "DrawRange_W").SetValue(new Circle(true, Color.Blue)));
            MiscMenu.AddItem(new MenuItem("misc_e_range", "DrawRange_E").SetValue(new Circle(true, Color.Green)));
            MiscMenu.AddItem(new MenuItem("misc_r_range", "DrawRange_R").SetValue(new Circle(true, Color.White)));


        }



        public static void OnEndSence_base(EventArgs args)
        {
            if (baseMenu.Item("misc_q_range").GetValue<Circle>().Active)
                Drawing.DrawCircle(ObjectManager.Player.Position, GetSpellRange(Qdata,Q.IsChargedSpell),
            baseMenu.Item("misc_q_range").GetValue<Circle>().Color);
            if (baseMenu.Item("misc_w_range").GetValue<Circle>().Active)
                Drawing.DrawCircle(ObjectManager.Player.Position, GetSpellRange(Wdata, W.IsChargedSpell),
                    baseMenu.Item("misc_w_range").GetValue<Circle>().Color);
            if (baseMenu.Item("misc_e_range").GetValue<Circle>().Active)
                Drawing.DrawCircle(ObjectManager.Player.Position, GetSpellRange(Edata,E.IsChargedSpell),
                    baseMenu.Item("misc_e_range").GetValue<Circle>().Color);

            if (Player.ChampionName == "Xerath" && R.Level > 0)
                Drawing.DrawCircle(ObjectManager.Player.Position, 2000+(R.Level*1200),
                    baseMenu.Item("misc_r_range").GetValue<Circle>().Color);
            else if (baseMenu.Item("misc_r_range").GetValue<Circle>().Active)
                Drawing.DrawCircle(ObjectManager.Player.Position, GetSpellRange(Rdata, R.IsChargedSpell),
                    baseMenu.Item("misc_r_range").GetValue<Circle>().Color);
            
            if (Player.ChampionName == "Xerath" && R.Level > 0)
                Utility.DrawCircle(ObjectManager.Player.Position, 2000 + (R.Level * 1200),
                    baseMenu.Item("misc_r_range").GetValue<Circle>().Color,1,20,true);
            else if(R.Range > 1500)
                Utility.DrawCircle(ObjectManager.Player.Position, GetSpellRange(Rdata, R.IsChargedSpell),
                    baseMenu.Item("misc_r_range").GetValue<Circle>().Color,1,20,true);
        }

        private static void addskillmenu(SpellDataInst spell)
        {

            string str = "(" + spell.Slot.ToString() + ") " + spell.Name.Replace(ObjectManager.Player.ChampionName, "");
            Menu tempmenu = new Menu("", "");

            switch (spell.Slot.ToString())
            {
                case "Q":
                    qMenu = new Menu(str, str);
                    tempmenu = qMenu;
                    break;
                case "W":
                    wMenu = new Menu(str, str);
                    tempmenu = wMenu;
                    break;
                case "E":
                    eMenu = new Menu(str, str);
                    tempmenu = eMenu;
                    break;
                case "R":
                    rMenu = new Menu(str, str);
                    tempmenu = rMenu;
                    break;
                default:
                    Game.PrintChat("error");
                    break;
            }


            baseMenu.AddSubMenu(tempmenu);
        }

        public static float GetSpellRange(SpellDataInst targetSpell,bool IsChargedSkill = false)
        {
            if (targetSpell.SData.CastRangeDisplayOverride[0] <= 0)
            {
                if (targetSpell.SData.CastRange[0] <= 0)
                {
                    return
                        targetSpell.SData.CastRadius[0];
                }
                else
                {
                    if (!IsChargedSkill)
                        return
                            targetSpell.SData.CastRange[0];
                    else
                        return
                            targetSpell.SData.CastRadius[0];
                }
            }
            else
                return
                    targetSpell.SData.CastRangeDisplayOverride[0];

        }
        
        public static void Cast(Spell spell, Obj_AI_Base target, HitChance hitChance = HitChance.VeryHigh, bool aoe = false)
        {
            if (spell.IsReady())
            {
                spell.Cast(target, false, aoe);
            }
        }

        public static void Cast(Spell spell, TargetSelector.DamageType damageType)
        {
            if (spell.IsReady())
            {
                Obj_AI_Hero target = null;
                if(Player.ChampionName == "TwistedFate" && spell.Slot == SpellSlot.Q)
                    target = TargetSelector.GetTarget(1200, damageType);
                else
                    target = TargetSelector.GetTarget(spell.Range, damageType);

                if (target == null) return;

                spell.Cast(target);
            }
        }

        public static void Cast(Spell spell)
        {
            if (spell.IsReady())
                spell.Cast();
        }
    }
}
