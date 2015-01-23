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
        public static Render.Circle drawR = null;

        public Xerath()
        {
            Q = new Spell(SpellSlot.Q, 1500);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 1050);
            R = new Spell(SpellSlot.R, 3000);

            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750-50, 1550, 1.5f);
            W.SetSkillshot(0.7f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 60, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);


            Spell[] SpellList = new[] { Q, W, E };
            ConfigManager.SetCombo(SpellList, true, true, true);
            ConfigManager.SetHarass(SpellList, true, false, false);

            CircleRendering(Player, Q.Range, "draw_Qrange", 5);
            CircleRendering(Player, W.Range, "draw_Wrange", 5);
            CircleRendering(Player, E.Range, "draw_Erange", 5);

            drawR = new Render.Circle(Player, R.Range, championMenu.Item("draw_Rrange",true).GetValue<Circle>().Color, 5)
            {
                VisibleCondition = c => championMenu.Item("draw_Rrange", true).GetValue<Circle>().Active,
            };
            drawR.Add();
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (R.Level > 0 && R.Range != R.Level * 1000 + 2000)
                R.Range = R.Level * 1000 + 2000;
            if (R.Range != drawR.Radius)
                drawR.Radius = R.Range;


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
            if (GetBoolFromMenu(Q, false, true))
                Cast(Q, TargetSelector.DamageType.Magical);
            if (GetBoolFromMenu(W, false, true))
                Cast(W, TargetSelector.DamageType.Magical);
            if (GetBoolFromMenu(E, false, true))
                Cast(E, TargetSelector.DamageType.Magical);
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
                if(GetBoolFromMenu(Q,true))
                    Cast(Q, TargetSelector.DamageType.Magical);
                if (GetBoolFromMenu(W, true))
                    Cast(W, TargetSelector.DamageType.Magical);
                if (GetBoolFromMenu(E, true))
                    Cast(E, TargetSelector.DamageType.Magical);
            }
        }

    
    }
}
