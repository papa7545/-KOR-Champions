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
    class Blitzcrank : Kor_AIO_Base
    {
        /// <summary>
        /// Jeon Cassiopeia.
        /// Cassiopeia Posion Buff Name : CassiopeiaNoxiousBlast ,CassiopeiaMiasma
        /// </summary>
        /// 
        public Blitzcrank()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0, 70, 1800f, true, SkillshotType.SkillshotLine);
        }
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();

            if (ObjectManager.Get<Obj_AI_Hero>().Any(t=> t.HasBuff("RocketGrab") && t.IsEnemy && t.IsVisible && !t.IsDead))
                E.Cast();
        }


        public static void harass()
        {
            Obj_AI_Hero qTarget = null;
            if(ObjectManager.Get<Obj_AI_Hero>().Any(t=> t.Distance(Player.Position) <= Q.Range && t.IsEnemy && !t.IsDead
                && t.IsVisible))
            {
                qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            }
            

            if (qTarget == null)
                return;
            else if (Q.IsReady())
                Kor_AIO_Base.Cast(Q,qTarget);
                
        }

        public static void combo()
        {
            Obj_AI_Hero qTarget = null;
            if (ObjectManager.Get<Obj_AI_Hero>().Any(t => t.Distance(Player.Position) <= Q.Range && t.IsEnemy && !t.IsDead
                && t.IsVisible))
            {
                qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            }


            if (qTarget == null)
                return;
            else if (Q.IsReady())
                Kor_AIO_Base.Cast(Q, qTarget);

        }
    }
}
