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
    class Xerath : Kor_AIO_Base
    {
        /// <summary>
        /// Jeon Cassiopeia.
        /// Cassiopeia Posion Buff Name : CassiopeiaNoxiousBlast ,CassiopeiaMiasma
        /// </summary>
        public static float rTime;

        public Xerath()
        {
            Q = new Spell(SpellSlot.Q, 1500);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 1050);
            R = new Spell(SpellSlot.R, 3200);

            
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750-50, 1550, 1.5f);
            W.SetSkillshot(0.7f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 60, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Q.IsCharging)
                Orbwalking.Attack = false;
            else
                Orbwalking.Attack = true;

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();


        }

        public static void harass()
        {
            Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
        }

        public static void combo()
        {
            if (Player.HasBuff("XerathR"))
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Any(t => t.Distance(Game.CursorPos) <= 100 && !t.IsDead && t.IsEnemy))
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().First(t => t.Distance(Game.CursorPos) <= 100 && !t.IsDead && t.IsEnemy);
                    if (Environment.TickCount - rTime >= 100)
                    {
                        Kor_AIO_Base.Cast(R, target);
                        rTime = Environment.TickCount;
                    }
                }
            }
            else
            {
                Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
                Kor_AIO_Base.Cast(W, TargetSelector.DamageType.Magical);
                Kor_AIO_Base.Cast(E, TargetSelector.DamageType.Magical);
            }

        }

    
    }
}
