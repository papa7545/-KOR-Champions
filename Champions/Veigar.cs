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
    class Veigar : Kor_AIO_Base
    {
        /// <summary>
        /// Jeon Veigar.
        /// </summary>
        private static float ERidus = 700/2;

        public Veigar()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 650);

            Q.SetTargetted(0.25f, 1500);
            W.SetSkillshot(1.25f, 225, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 1400);

            Spell[] SpellList = new[] { Q, W, E,R };
            ConfigManager.SetCombo(SpellList, true, true, true,true);
            ConfigManager.SetHarass(SpellList, true, true, true,false);


            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("lt_Auto", "Auto").SetValue(false));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("lt_enable", "Enable").SetValue(true));


            Menu menu2 = new Menu("W - Dark Matter", "W - Dark Matter");
            menu2.AddItem(new MenuItem("W_Stunned", "CheckStunned?", false).SetValue(true));
            championMenu.AddSubMenu(menu2);

            CircleRendering(Player, Q.Range, "draw_Qrange", 5);
            CircleRendering(Player, W.Range, "draw_Wrange", 5);
            CircleRendering(Player, E.Range, "draw_Erange", 5);
            CircleRendering(Player, R.Range, "draw_Rrange", 5);


        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            Combolist = new List<SpellSlot>();
            if (Q.IsReady() && GetBoolFromMenu(Q, true))
                Combolist.Add(SpellSlot.Q);
            if (W.IsReady() && GetBoolFromMenu(W, true))
                Combolist.Add(SpellSlot.W);
            if (R.IsReady() && GetBoolFromMenu(R, true))
                Combolist.Add(SpellSlot.R);

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                combo();

            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
                harass();

            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LastHit || ConfigManager.championMenu.Item("lt_Auto").GetValue<bool>())
                Lasthit_Spell(Q);
        }

        public static void harass()
        {
            if (GetBoolFromMenu(E, false,true))
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical, false);
                if (target != null)
                {
                    var Predic = Prediction.GetPrediction(target, 0.2f);
                    E.Cast(Predic.CastPosition.Extend(Player.Position, ERidus), Packets());
                }
            }
            if (championMenu.Item("W_Stunned").GetValue<bool>())
            {
                if (GetBoolFromMenu(W, false,true) && TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical, false).HasBuffOfType(BuffType.Stun))
                    Cast(W, TargetSelector.DamageType.Magical);
            }
            else
                if (GetBoolFromMenu(W, false, true))
                    Cast(W, TargetSelector.DamageType.Magical);
            if (GetBoolFromMenu(Q, false, true))
                Cast(Q, TargetSelector.DamageType.Magical);
            if (GetBoolFromMenu(R, false, true))
                Cast(R, TargetSelector.DamageType.Magical);
        }

        public static void combo()
        {
            if (GetBoolFromMenu(E, true))
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical, false);
                if (target != null)
                {
                    var Predic = Prediction.GetPrediction(target, 0.2f);
                    E.Cast(Predic.CastPosition.Extend(Player.Position, ERidus), Packets());
                }
            }
            if (championMenu.Item("W_Stunned").GetValue<bool>())
            {
                if (GetBoolFromMenu(W, true) && TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical, false).HasBuffOfType(BuffType.Stun))
                    Cast(W, TargetSelector.DamageType.Magical);
            }
            else
                if (GetBoolFromMenu(W, true))
                    Cast(W, TargetSelector.DamageType.Magical);
            if (GetBoolFromMenu(Q, true))
                Cast(Q, TargetSelector.DamageType.Magical);
            if (GetBoolFromMenu(R, true))
                Cast(R, TargetSelector.DamageType.Magical);
        }
    }
}
