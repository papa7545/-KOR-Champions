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
    class Cassiopeia : Kor_AIO_Base
    {
        /// <summary>
        /// Jeon Cassiopeia.
        /// Cassiopeia Posion Buff Name : CassiopeiaNoxiousBlast ,CassiopeiaMiasma
        /// </summary>
        /// 

        public Cassiopeia()
        {
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 850);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 825);

            Q.SetSkillshot(1f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.6f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            ConfigManager.set_menu();

            Game.OnGameUpdate += OnGameUpdate;
        }
        public static void OnGameUpdate(EventArgs args)
        {
            if (ConfigManager.championMenu.Item(ConfigManager.m_Items.COMBO_KEY).GetValue<KeyBind>().Active)
                combo();

            if (ConfigManager.championMenu.Item(ConfigManager.m_Items.HARASS_KEY).GetValue<KeyBind>().Active)
                harass();
        }

        //-----------------------------------Cassiopeia-------------------------------//
        /// <summary>
        /// 속력 = 거리/시간
        /// 시간 = 거리/속력
        /// 
        /// E스킬이 적에게 도달하는 시간
        /// 시간 = Player.Distance(eTarget)/E.MissileSpeed;
        /// 
        /// 버프 경과 시간 =
        /// 시간 = Game.Time - Buff.CastTime;
        /// 
        /// 카시오페아 독 지속시간
        /// 3초
        /// 
        /// 조건식
        /// 버프경과시간+E스킬이 적에게 도달하는 시간 <= 3초
        /// </summary>
        public static void harass()
        {
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var eTime = Player.Distance(eTarget.Position) / 1900f;
            var qPred = Q.GetPrediction(eTarget);
            var wPred = W.GetPrediction(eTarget);

            if (eTarget.Buffs.Any(b => b.DisplayName == "CassiopeiaNoxiousBlast" || b.DisplayName == "CassiopeiaMiasma"))
            {
                foreach (var buff in eTarget.Buffs.Where(b => b.DisplayName == "CassiopeiaNoxiousBlast" || b.DisplayName == "CassiopeiaMiasma"))
                {
                    var buffTime = Game.Time - buff.StartTime;
                    if (buffTime + eTime <= 3.5f)
                        E.CastOnUnit(eTarget);
                }
            }
            else
            {
                Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
                Kor_AIO_Base.Cast(W, TargetSelector.DamageType.Magical);
            }
            if (eTarget == null)
                return;
        }

        public static void combo()
        {


            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var eTime = Player.Distance(eTarget.Position) / 1900f;
            var qPred = Q.GetPrediction(eTarget);
            var wPred = W.GetPrediction(eTarget);

            if (eTarget.Buffs.Any(b => b.DisplayName == "CassiopeiaNoxiousBlast" || b.DisplayName == "CassiopeiaMiasma"))
            {
                foreach (var buff in eTarget.Buffs.Where(b => b.DisplayName == "CassiopeiaNoxiousBlast" || b.DisplayName == "CassiopeiaMiasma"))
                {
                    var buffTime = Game.Time - buff.StartTime;
                    if (buffTime + eTime <= 3.5f)
                        E.CastOnUnit(eTarget);
                }
            }
            else
            {
                Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
                Kor_AIO_Base.Cast(W, TargetSelector.DamageType.Magical);
            }
            if (eTarget == null)
                return;
        }


        public static void lasthit()
        {

            var eTarget = ObjectManager.Get<Obj_AI_Minion>();

            foreach(var t in eTarget)
            {
                if (t.Health <= E.GetDamage(t))
                    E.CastOnUnit(t);
            }

        }
    
    }
}
