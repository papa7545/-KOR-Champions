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
            Q = new Spell(SpellSlot.Q, 700f, TargetSelector.DamageType.Physical);
            W = new Spell(SpellSlot.W, float.MaxValue, TargetSelector.DamageType.Physical);
            E = new Spell(SpellSlot.E, 700f, TargetSelector.DamageType.Physical);
            R = new Spell(SpellSlot.R, 900f, TargetSelector.DamageType.Physical);
            Menu subMenu = new Menu("KillSteal", "KillSteal", false);
            subMenu.AddItem(new MenuItem("ks_enable", "Enable - Q", false).SetValue<bool>(true));
            championMenu.AddSubMenu(subMenu);

            Spell[] SpellList = new[] { Q, W, E };
            ConfigManager.SetCombo(SpellList, true, true, true);
            ConfigManager.SetHarass(SpellList, true, true, false);

            Menu menu2 = new Menu("E - TimeWarp", "E - TimeWarp", false);
            menu2.AddItem(new MenuItem("E_target", "Target", false).SetValue<StringList>(new StringList(new string[] { "Me", "Enemy" }, 0)));
            championMenu.AddSubMenu(menu2);
            Menu menu3 = new Menu("R - ChronoShift", "R - ChronoShift", false);
            menu3.AddItem(new MenuItem("R_mode", "Mode", false).SetValue<StringList>(new StringList(new string[] { "DetectDamage", "DetectHP" }, 0)));
            menu3.AddItem(new MenuItem("R_me", "Me", false).SetValue<bool>(true));
            menu3.AddItem(new MenuItem("R_ally", "Ally", false).SetValue<bool>(true));
            menu3.AddItem(new MenuItem("empty", "", false));
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(t => t.IsAlly && !t.IsMe))
            {
                menu3.AddItem(new MenuItem("R_ally_" + hero.ChampionName, hero.ChampionName, false).SetValue<bool>(true));
            }
            menu3.AddItem(new MenuItem("R_HP", "HP(%)", false).SetValue<Slider>(new Slider(10, 0, 100)));
            championMenu.AddSubMenu(menu3);

            CircleRendering(Player, Q.Range, "draw_Qrange", 5);
            CircleRendering(Player, R.Range, "draw_Rrange", 5);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {

            CastR();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();

            if (championMenu.Item("ks_enable").GetValue<bool>())
                KillSteal();
            
        }


        private static void harass()
        {
            if (Q.IsReady() && GetBoolFromMenu(Q, false, true))
                Cast(Q, TargetSelector.DamageType.Magical);
            else if (E.IsReady() && GetBoolFromMenu(E, false,true))
            {
                if (championMenu.Item("E_target").GetValue<StringList>().SelectedValue == "Me")
                    Cast(E);
                else
                    Cast(E, TargetSelector.DamageType.Magical);
            }
            else if (W.IsReady() &&
                Player.Mana >= Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost + Player.Spellbook.GetSpell(SpellSlot.W).ManaCost &&
                Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time > 2.0f &&
                GetBoolFromMenu(W, false, true))
                Cast(W);
        }

        private static void combo()
        {

            Game.PrintChat(Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time + "!");

            if (Q.IsReady() && GetBoolFromMenu(Q,true))
                Cast(Q, TargetSelector.DamageType.Magical);
            else if (E.IsReady() && GetBoolFromMenu(Q,true))
            {
                if (championMenu.Item("E_target").GetValue<StringList>().SelectedValue == "Me")
                    Cast(E);
                else
                    Cast(E, TargetSelector.DamageType.Magical);
            }
            else if (W.IsReady() &&
                Player.Mana >= Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost + Player.Spellbook.GetSpell(SpellSlot.W).ManaCost &&
                Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time > 2.0f &&
                GetBoolFromMenu(W, true))
                Cast(W);
        }
        private static void KillSteal()
        {
            foreach(var t in ObjectManager.Get<Obj_AI_Hero>().Where(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= Q.Range &&
                t.Health+(t.HPRegenRate*4) <= Q.GetDamage(t) && Q.IsReady() && !t.IsZombie))
            {
                Cast(Q, t);
            }
        }

        private static void CastR()
        {
            if (R.IsReady())
            {
                List<Obj_AI_Hero> SaveList = new List<Obj_AI_Hero>();

                if (championMenu.Item("R_mode").GetValue<StringList>().SelectedValue == "DetectHP")
                {
                    if (championMenu.Item("R_me").GetValue<bool>() && (Player.HealthPercentage() <= championMenu.Item("R_HP").GetValue<Slider>().Value))
                        SaveList.Add(Player);

                    if (championMenu.Item("R_ally").GetValue<bool>())
                    {
                        foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where<Obj_AI_Hero>
                            (t => !t.IsDead && !t.IsMe && t.IsAlly && 
                                t.HealthPercentage() <= championMenu.Item("R_HP").GetValue<Slider>().Value
                                &&
                                championMenu.Item("R_ally_" + t.ChampionName).GetValue<bool>()))
                        {
                            SaveList.Add(hero);
                        }
                    }
                    if (SaveList.Any<Obj_AI_Hero>())
                    {
                        foreach (Obj_AI_Hero hero in SaveList.Where(t => t.Distance(Player.Position) <= R.Range))
                        {
                            Cast(R, hero);
                        }
                    }
                }
                else
                {
                    if (championMenu.Item("R_me").GetValue<bool>())
                        SaveList.Add(Player);

                    if (championMenu.Item("R_ally").GetValue<bool>())
                    {
                        foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where<Obj_AI_Hero>(t => !t.IsDead && !t.IsMe && t.IsAlly && 
                            championMenu.Item("R_ally_" + t.ChampionName).GetValue<bool>()))
                        {
                            SaveList.Add(hero);
                        }
                    }
                    if (SaveList.Any<Obj_AI_Hero>())
                    {
                        foreach (Obj_AI_Hero hero in SaveList.Where(t => t.Distance(Player.Position) <= R.Range))
                        {
                            foreach (Obj_SpellMissile missile in ObjectManager.Get<Obj_SpellMissile>().Where(missile => missile.SpellCaster.IsValid<Obj_AI_Hero>() && missile.SpellCaster.IsEnemy))
                            {
                                float num = missile.StartPosition.Distance(hero.Position) / missile.SData.MissileSpeed;
                                float num2 = missile.SData.LineWidth / hero.MoveSpeed;
                                if (num > num2)
                                    break;

                                if (missile.SpellCaster.GetDamageSpell(hero, missile.SData.Name).CalculatedDamage >= hero.Health)
                                    Cast(R, hero);
                            }
                        }
                    }
                }
            }
        }
        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if ((unit.IsValid<Obj_AI_Hero>() && args.Target.IsValid<Obj_AI_Hero>()) && (championMenu.Item("R_mode", false).GetValue<StringList>().SelectedValue != "DetectDamage"))
            {
                Obj_AI_Hero caster = (Obj_AI_Hero)unit;
                Obj_AI_Hero target = (Obj_AI_Hero)args.Target;
                if ((unit.IsValid<Obj_AI_Hero>() && target.IsAlly) && (caster.IsEnemy && (caster.GetDamageSpell(target, args.SData.Name).CalculatedDamage >= target.Health)))
                {
                    Cast(R, target);
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
                    {
                        if (Q.GetDamage(hero) - hero.HPRegenRate * 2 >= hero.Health)
                            return;
                        dmg = (Q.GetDamage(hero) - hero.HPRegenRate * 2);
                    }

                    if (Player.Spellbook.CanUseSpell(Ignite.Slot) == SpellState.Ready && (hero.Health + hero.HPRegenRate * 2) <= dmg)
                        Player.Spellbook.CastSpell(Ignite.Slot, hero);
                }
            }
        }
    }
}
