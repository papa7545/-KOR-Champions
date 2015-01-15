using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kor_AIO.Champions
{
    internal class Fizz : Kor_AIO_Base
    {
        public static Items.Item Dfg;

        public Fizz()
        {
            IgniteSlot = Player.GetSpellSlot("SummonerDot");
            SetSpells();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 400);
            E2 = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 1200);

            E.SetSkillshot(0.5f, 120, 1300, false, SkillshotType.SkillshotCircle);
            E2.SetSkillshot(0.5f, 400, 1300, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 250f, 1200f, false, SkillshotType.SkillshotLine);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {

            }

            if (ConfigManager.championMenu.Item("ShroudSelf", true).GetValue<KeyBind>().Active)
            {
                W.Cast(Player.Position);
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

            }
        }

        private static float ComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (Dfg.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            if (Dfg.IsReady())
                damage = damage * 1.2;

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                damage += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            if (Items.HasItem(3155, (Obj_AI_Hero)enemy))
            {
                damage = damage - 250;
            }

            if (Items.HasItem(3156, (Obj_AI_Hero)enemy))
            {
                damage = damage - 400;
            }
            return (float)damage;
        }

        public static void Combo()
        {
            CurrentTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (CurrentTarget == null)
                return;

            float dmg = ComboDamage(CurrentTarget);

            if (dmg > CurrentTarget.Health + +20)
            {
                if (Dfg.IsReady() && championMenu.Item("useItems").GetValue<bool>() == true)
                {
                    Dfg.Cast(CurrentTarget);
                }
                if (CurrentTarget.IsValidTarget(Q.Range) && W.IsReady() && championMenu.Item("useW").GetValue<bool>() == true)
                {
                    W.Cast(Player, Packets());
                }
                if (CurrentTarget.IsValidTarget(Q.Range) && Q.IsReady() && championMenu.Item("useQ").GetValue<bool>() == true)
                {
                    Q.CastOnUnit(CurrentTarget, Packets());
                }
                if (CurrentTarget.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(CurrentTarget, Packets());
                }

                if (CurrentTarget.IsValidTarget(E.Range) && E.IsReady() && championMenu.Item("useE").GetValue<bool>() == true)
                {
                    E.Cast(CurrentTarget, Packets());
                    E2.Cast(CurrentTarget);
                }

            }
            else
            {
                if (championMenu.Item("QR").GetValue<bool>())
                {
                    if (CurrentTarget.IsValidTarget(Q.Range) && Q.IsReady() && championMenu.Item("useQ").GetValue<bool>() == true)
                    {
                        Q.CastOnUnit(CurrentTarget, Packets());
                        R.Cast(CurrentTarget, Packets());
                    }
                    else
                    {

                        if (CurrentTarget.IsValidTarget(Q.Range) && Q.IsReady() && championMenu.Item("useQ").GetValue<bool>() == true)
                        {
                            Q.CastOnUnit(CurrentTarget, Packets());

                        }
                    }
                }
                if (CurrentTarget.IsValidTarget(E.Range) && E.IsReady() && championMenu.Item("useE").GetValue<bool>() == true)
                {
                    E.Cast(CurrentTarget, Packets());
                    E2.Cast(CurrentTarget);
                }
            }
        }
    }
}
