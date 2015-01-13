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
        /// Jeon BlitzCrank.
        /// </summary>
        /// 
        public Blitzcrank()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0, 70, 1800f, true, SkillshotType.SkillshotLine);
            var ks_menu = new Menu("KillSteal", "KillSteal");
            ks_menu.AddItem(new MenuItem("ks_enable", "Enable - R").SetValue(true));
            ConfigManager.championMenu.AddSubMenu(ks_menu);
        }
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();

            if (ObjectManager.Get<Obj_AI_Hero>().Any(t=> t.HasBuff("RocketGrab") && t.IsEnemy && t.IsVisible && !t.IsDead))
                E.Cast();

            if (ConfigManager.championMenu.Item("ks_enable").GetValue<bool>())
                KillSteal();
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (ConfigManager.championMenu.Item("draw_Qrange").GetValue<Circle>().Active)
                Drawing.DrawCircle(Player.Position, Q.Range, ConfigManager.championMenu.Item("draw_Qrange").GetValue<Circle>().Color);
            if (ConfigManager.championMenu.Item("draw_Rrange").GetValue<Circle>().Active)
                Drawing.DrawCircle(Player.Position, R.Range, ConfigManager.championMenu.Item("draw_Rrange").GetValue<Circle>().Color);
            
        }

        public static void harass()
        {
            Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
        }

        public static void combo()
        {
            Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
        }
        public static void KillSteal()
        {
            if (ObjectManager.Get<Obj_AI_Hero>().Any(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= R.Range &&
                t.Health <= R.GetDamage(t)))
                R.Cast();
        }
    }
}
