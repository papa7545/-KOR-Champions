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
    class Chogath:Program
    {
        /// <summary>
        /// Jeon Cassiopeia.
        /// Cassiopeia Posion Buff Name : CassiopeiaNoxiousBlast ,CassiopeiaMiasma
        /// </summary>
        /// 
        public static Jproject_base ChoGaths;
        public static float rTime;

        public static void _ChoGath()
        {
            ChoGaths = new Jproject_base();

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 675);
            R = new Spell(SpellSlot.R, 175);


            Q.SetSkillshot(0.75f, 175f, 1000f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.60f, 300f, 1750f, false, SkillshotType.SkillshotCone);

            Game.OnGameUpdate += OnGameUpdate;
        }

        public static void OnGameUpdate(EventArgs args)
        {

            if (Jproject_base.baseMenu.Item(m_Items.COMBO_KEY).GetValue<KeyBind>().Active)
                combo();

            if (Jproject_base.baseMenu.Item(m_Items.HARASS_KEY).GetValue<KeyBind>().Active)
                harass();

            KillSteal();
        }

        public static void harass()
        {
            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (W.IsReady() && Player.Distance(wTarget.Position) <= W.Range)
                Jproject_base.Cast(W, wTarget);

            if (Q.IsReady() && Player.Distance(qTarget.Position) <= Q.Range)
                Jproject_base.Cast(Q, qTarget);
        }

        public static void combo()
        {
            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.True);

            if (W.IsReady() && Player.Distance(wTarget.Position) <= W.Range)
                Jproject_base.Cast(W, wTarget);

            if (Q.IsReady() && Player.Distance(qTarget.Position) <= Q.Range)
                Jproject_base.Cast(Q,qTarget);

            if (R.IsReady() && Player.GetSpellDamage(rTarget, SpellSlot.R) > rTarget.Health)
                Jproject_base.Cast(R,rTarget);
        }


        public static void KillSteal()
        {
            foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(t => !t.IsDead && t.IsEnemy && t.IsVisible))
            {
                if (Player.GetSpellDamage(hero, SpellSlot.R) > hero.Health && Player.Distance(hero.Position) < R.Range)
                    Jproject_base.Cast(R, hero);

                if (Player.GetSpellDamage(hero, SpellSlot.Q) > hero.Health && Player.Distance(hero.Position) < Q.Range)
                    Jproject_base.Cast(Q, hero);

                if (Player.GetSpellDamage(hero, SpellSlot.W) > hero.Health && Player.Distance(hero.Position) < W.Range)
                    Jproject_base.Cast(W, hero);

            }
        }

        static void autoignite()
        {
            //if (Jproject_base.baseMenu.Item("autoIgnite").GetValue<bool>() && SIgnite != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(SIgnite) == SpellState.Ready)
            //{
            //    float ignitedamage = 50 + 20 * Player.Level;

            //    foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x != null && x.IsValid && !x.IsDead && Player.ServerPosition.Distance(x.ServerPosition) < 600 && !x.IsMe && !x.IsAlly && (x.Health + x.HPRegenRate * 1) <= ignitedamage))
            //    {
            //        Player.Spellbook.CastSpell(SIgnite, target);
            //    }
            //}
        }
    }
}
