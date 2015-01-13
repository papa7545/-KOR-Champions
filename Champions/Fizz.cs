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

        
        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            /*
            float dmg = ComboDamage(target);

            if (dmg > target.Health + +20)
            {
                if (Dfg.IsReady() && xMenu.Item("useItems").GetValue<bool>() == true)
                {
                    Dfg.Cast(target);
                }
                if (target.IsValidTarget(Q.Range) && W.IsReady() && xMenu.Item("useW").GetValue<bool>() == true)
                {
                    W.Cast(Player, xMenu.Item("Packet").GetValue<bool>());
                }
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && xMenu.Item("useQ").GetValue<bool>() == true)
                {
                    Q.CastOnUnit(target, xMenu.Item("Packet").GetValue<bool>());
                }
                if (target.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(target, xMenu.Item("Packet").GetValue<bool>());
                }

                if (target.IsValidTarget(E.Range) && E.IsReady() && xMenu.Item("useE").GetValue<bool>() == true)
                {
                    E.Cast(target, xMenu.Item("Packet").GetValue<bool>());
                    E2.Cast(target);
                }

            }
            else
            {
                if (xMenu.Item("QR").GetValue<bool>())
                {
                    if (target.IsValidTarget(Q.Range) && Q.IsReady() && xMenu.Item("useQ").GetValue<bool>() == true)
                    {
                        Q.CastOnUnit(target, xMenu.Item("Packet").GetValue<bool>());
                        R.Cast(target, xMenu.Item("Packet").GetValue<bool>());
                    }
                    else
                    {

                        if (target.IsValidTarget(Q.Range) && Q.IsReady() && xMenu.Item("useQ").GetValue<bool>() == true)
                        {
                            Q.CastOnUnit(target, xMenu.Item("Packet").GetValue<bool>());

                        }
                    }
                }
                if (target.IsValidTarget(E.Range) && E.IsReady() && xMenu.Item("useE").GetValue<bool>() == true)
                {
                    E.Cast(target, xMenu.Item("Packet").GetValue<bool>());
                    E2.Cast(target);
                }
            }*/
        }
    }
}
