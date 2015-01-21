#region
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Text;
#endregion

namespace Kor_AIO.Champions
{
    class Karthus : Kor_AIO_Base
    {
        /// <summary>
        /// Jeon Karthus.
        /// </summary>
        ///         

        public static List<string> killableC = new List<string>();
        public static Render.Text UltText = new Render.Text("", Player, new Vector2(0, 50), (int)30, ColorBGRA.FromRgba(0xFF0000FF))
            {
                OutLined = true,
                VisibleCondition = c => killableC.Any() && R.IsReady() && championMenu.Item("r_notifier").GetValue<bool>()
            };
        public static Vector3 LastCast = Vector3.Zero;


        public Karthus()
        {
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, int.MaxValue);

            Q.SetSkillshot(1.2f, 160,int.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.3f, 10, int.MaxValue, false, SkillshotType.SkillshotCircle);

            championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_Q", "Q").SetValue(true));
            championMenu.SubMenu("Harass").AddItem(new MenuItem("harass_W", "W").SetValue(false));

            championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_auto", "AutoCombo(OnZombie)").SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_Q", "Q").SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("combo_W", "W").SetValue(true));

            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("clear_Q", "Q").SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("clear_E", "E").SetValue(true));

            var Emenu = new Menu("E - Defile", "E - Defile");
            Emenu.AddItem(new MenuItem("e_smartcast", "SmartCast").SetValue(true));
            Emenu.AddItem(new MenuItem("e_mana", "Require mana(%)").SetValue(new Slider(20,0,100)));
            championMenu.AddSubMenu(Emenu);

            var Rmenu = new Menu("R - Requiem", "R - Requiem");
            Rmenu.AddItem(new MenuItem("r_notifier", "Notifier").SetValue(true));
            Rmenu.AddItem(new MenuItem("r_autocast", "AutoCast").SetValue(true));
            Rmenu.AddItem(new MenuItem("r_distance", "Nobody in Range)").SetValue(new Slider(1000, 0, 2500)));
            championMenu.AddSubMenu(Rmenu);

            CircleRendering(Player, Q.Range, "draw_Qrange", 5);
            CircleRendering(Player, W.Range, "draw_Wrange", 5);
            UltText.Add();
        }
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo || (Player.IsZombie && championMenu.Item("combo_auto").GetValue<bool>()))
                combo();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();
            
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
                LaneClear();

            if (championMenu.Item("e_smartcast").GetValue<bool>())
                SmartE();

            if (R.IsReady() && championMenu.Item("r_notifier").GetValue<bool>())
                Ult();

        }


        public static void harass()
        {
            if (W.IsReady() && championMenu.Item("harass_W").GetValue<bool>())
            {
                Cast(W, TargetSelector.DamageType.Magical);
            }
            else if (Q.IsReady() && championMenu.Item("harass_Q").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    var temp = Q.GetPrediction(target);
                }
            }
        }
        public static void combo()
        {
            if (W.IsReady() && championMenu.Item("combo_W").GetValue<bool>())
            {
                Cast(W, TargetSelector.DamageType.Magical);
            }
            else if (Q.IsReady() && championMenu.Item("combo_Q").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q.Range,TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    var temp = Q.GetPrediction(target);
                    Q.Cast(temp.CastPosition);
                    return;
                }
            }

            if (Player.IsZombie)
                Q.Cast(ObjectManager.Get<Obj_AI_Minion>().First(t => !t.IsDead && t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= Q.Range));
        }
        public static void SmartE()
        {
            if (Player.ManaPercentage() >= championMenu.Item("e_mana").GetValue<Slider>().Value)
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Any(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= E.Range))
                {
                    if (!Player.HasBuff("KarthusDefile"))
                        E.Cast();
                }
                else if (Player.HasBuff("KarthusDefile") &&
                    !ObjectManager.Get<Obj_AI_Minion>().Any(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= E.Range))
                    E.Cast();

            }
        }
        public static void LaneClear()
        {
            if(ObjectManager.Get<Obj_AI_Minion>().Any(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= E.Range))
            {
                if (!Player.HasBuff("KarthusDefile"))
                    E.Cast();
                Cast(Q, ObjectManager.Get<Obj_AI_Minion>().First(t => !t.IsDead && t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= Q.Range));
            }
        }
        public static void Ult()
        {
            bool first = true;
            StringBuilder buffer = new StringBuilder();
            buffer.Append("Killable:");

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= R.Range &&
                t.Health <= R.GetDamage(t) && !t.IsZombie))
            {
                killableC.Add(hero.ChampionName);
                if (first)
                {
                    buffer.Append(hero.ChampionName);
                    first = false;
                }
                else
                {
                    buffer.Append(",");
                    buffer.Append(hero.ChampionName);
                }
            }

            UltText.text = buffer.ToString();

            if (UltText.text == "Killable:")
                killableC.Clear();

            if (killableC.Any() && championMenu.Item("r_autocast").GetValue<bool>()) // r_distance
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Any
                    (t => t.IsEnemy && !t.IsDead && t.IsVisible && t.Distance(Player.Position) <= R.Range && t.Health <= R.GetDamage(t) && !t.IsZombie) 
                    &&
                    !ObjectManager.Get<Obj_AI_Hero>().Any
                    (t => t.IsEnemy && !t.IsDead && Player.Distance(t.Position) <= championMenu.Item("r_distance").GetValue<Slider>().Value))
                    Cast(R);
            }
        }
    }
}
