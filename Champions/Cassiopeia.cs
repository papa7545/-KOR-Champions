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

            var lasthit_menu = new Menu("LastHit_Spell", "LastHit_Spell");
            lasthit_menu.AddItem(new MenuItem("lt_auto", "Auto").SetValue(false));
            lasthit_menu.AddItem(new MenuItem("lt_posion", "IsPosioned?").SetValue(true));
            ConfigManager.championMenu.AddSubMenu(lasthit_menu);

            championMenu.SubMenu("Drawings").AddItem(new MenuItem("draw_Qrange", "Draw Q").SetValue(new Circle(true, Color.Red)));
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("draw_Wrange", "Draw W").SetValue(new Circle(true, Color.Blue)));
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("draw_Erange", "Draw E").SetValue(new Circle(true, Color.Green)));
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("draw_Rrange", "Draw R").SetValue(new Circle(true, Color.White)));


            Q.SetSkillshot(1f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.6f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            CircleRendering(Player, Q.Range, championMenu.Item("draw_Qrange"), 5);
            CircleRendering(Player, E.Range, championMenu.Item("draw_Erange"), 5);
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                if (ConfigManager.championMenu.Item("lt_auto").GetValue<bool>())
                {
                    if (ConfigManager.championMenu.Item("lt_posion").GetValue<bool>())
                    {
                        if (ObjectManager.Get<Obj_AI_Minion>().Any(
                            t =>
                                !t.IsDead &&
                                t.IsEnemy &&
                                (t.HasBuff("CassiopeiaNoxiousBlast") || t.HasBuff("CassiopeiaMiasma")) &&
                                t.Distance(Player.Position) <= E.Range &&
                                t.Health + 5 < E.GetDamage(t)
                            ))
                        {
                            Lasthit_Spell(E, true,
                                ObjectManager.Get<Obj_AI_Minion>().First(
                                t =>
                                    !t.IsDead &&
                                    t.IsEnemy &&
                                    (t.HasBuff("CassiopeiaNoxiousBlast") || t.HasBuff("CassiopeiaMiasma")) &&
                                    t.Distance(Player.Position) <= E.Range &&
                                    t.Health + 5 < E.GetDamage(t)
                                ));
                        }
                    }
                    else
                        Lasthit_Spell(E);
                }
            }
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
            if (TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical) == null) return;

            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var eTime = Player.Distance(eTarget.Position) / 1900f;

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

            if (TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical) == null)  return;
            
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var eTime = Player.Distance(eTarget.Position) / 1900f;


            
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
                Cast(Q, TargetSelector.DamageType.Magical);
                Cast(W, TargetSelector.DamageType.Magical);
            }
            if (eTarget == null)
                return;

        }
   
    }
}
