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

namespace Kor_AIO.Champions
{
    class Zilean : Kor_AIO_Base
    {
        /// <summary>
        /// Jeon Zilean.
        /// </summary>
        /// 



        public Zilean()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 900);

            var ks_menu = new Menu("KillSteal", "KillSteal");
            ks_menu.AddItem(new MenuItem("ks_enable", "Enable - Q").SetValue(true));
            championMenu.AddSubMenu(ks_menu);

            var E_menu = new Menu("E - TimeWarp", "E - TimeWarp");
            E_menu.AddItem(new MenuItem("E_combo", "Combo").SetValue(true));
            E_menu.AddItem(new MenuItem("E_target", "Target").SetValue(new StringList(new []{"Me","Enemy"})));
            championMenu.AddSubMenu(E_menu);

            var R_menu = new Menu("R - ChronoShift", "R - ChronoShift");
            R_menu.AddItem(new MenuItem("R_oncombo", "OnCombo").SetValue(true));
            R_menu.AddItem(new MenuItem("R_me", "Me").SetValue(true));
            R_menu.AddItem(new MenuItem("R_ally", "Ally").SetValue(true));
            R_menu.AddItem(new MenuItem("empty", ""));
            foreach(var temp in ObjectManager.Get<Obj_AI_Hero>().Where(t => t.IsAlly && !t.IsMe))
            {
                R_menu.AddItem(new MenuItem("R_ally_" + temp.ChampionName, temp.ChampionName).SetValue(true));
            }
            R_menu.AddItem(new MenuItem("R_HP", "HP(%)").SetValue(new Slider(10,0,100)));

            championMenu.AddSubMenu(R_menu);

            CircleRendering(Player, Q.Range, championMenu.Item("draw_Qrange"), 5);
            CircleRendering(Player, R.Range, championMenu.Item("draw_Rrange"), 5);


            IgniteSlot = Player.GetSpellSlot("SummonerDot");

        }
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();

            if (championMenu.Item("ks_enable").GetValue<bool>())
                KillSteal();
            
            CastR();

        }


        private static void harass()
        {

      
            if (Q.IsReady())
                Cast(Q, TargetSelector.DamageType.Magical);
            else if (W.IsReady())
                Cast(W);

        }

        private static void combo()
        {
            if (Q.IsReady())
                Cast(Q, TargetSelector.DamageType.Magical);
            else if (E.IsReady() && championMenu.Item("E_combo").GetValue<bool>())
            {
                if (championMenu.Item("E_target").GetValue<StringList>().SelectedValue == "Me")
                    Cast(E);
                else
                    Cast(E, TargetSelector.DamageType.Magical);
            }
            else if (W.IsReady())
                Cast(W);
        }
        private static void KillSteal()
        {
            foreach(var t in ObjectManager.Get<Obj_AI_Hero>().Where(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= Q.Range &&
                t.Health+(t.HPRegenRate*4) <= Q.GetDamage(t) && Q.IsReady()))
            {
                Cast(Q, t);
            }
        }
        private static void CastR()
        {
            bool _if = true;

            if (championMenu.Item("R_oncombo").GetValue<bool>())
                _if = (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo);

            if (R.IsReady() && _if)
            {
                var SaveList = new List<Obj_AI_Hero>();
                if (championMenu.Item("R_me").GetValue<bool>()
                    && Player.HealthPercentage() <= championMenu.Item("R_HP").GetValue<Slider>().Value)
                    SaveList.Add(Player);

                if (championMenu.Item("R_ally").GetValue<bool>())
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(t => !t.IsDead && !t.IsMe && t.IsAlly
                        && t.HealthPercentage() <= championMenu.Item("R_HP").GetValue<Slider>().Value
                        && championMenu.Item("R_ally_" + t.ChampionName).GetValue<bool>()))
                    {
                        SaveList.Add(hero);
                    }
                }

                if (SaveList.Any())
                {
                    foreach (var target in SaveList.Where(t => t.Distance(Player.Position) <= R.Range))
                    {
                        Cast(R, target);
                    }
                }
            }
        }

        public override void Game_Utility(EventArgs args) // ignite
        {
            if (utilityMenu.Item("ignite_enable").GetValue<bool>() && Ignite.Slot != SpellSlot.Unknown)
            {
                float dmg = 50 + 20 * Player.Level;

                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => !hero.IsDead && Player.ServerPosition.Distance(hero.ServerPosition) < Ignite.Range
                        && hero.IsEnemy))
                {
                    if (hero.Buffs.Any(c => c.Name == "timebombenemybuff"))
                        dmg += (Q.GetDamage(hero) - hero.HPRegenRate * 2);

                    if (Player.Spellbook.CanUseSpell(Ignite.Slot) == SpellState.Ready && (hero.Health + hero.HPRegenRate * 2) <= dmg)
                        Player.Spellbook.CastSpell(Ignite.Slot, hero);
                }
            }
        }
    }
}
