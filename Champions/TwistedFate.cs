#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
#endregion

namespace Kor_AIO.Champions
{
    class TwistedFate : Kor_AIO_Base
    {
        public static card cards= card.none;
        public enum card {
            none,
            blue,
            red,
            gold
        }

        public static int stack=0;
        public static int pastTime = 0;
        public static int delayi = 0;
        public static int x = 0;
        public static bool textshow = false;


        public static string ultText = "";
        public static Render.Text text_notifier = new Render.Text("Find Weaken Champion!", Player, new Vector2(0, 50), (int)32, ColorBGRA.FromRgba(0xFF00FFBB))
            {
                VisibleCondition = c => ultText != "",
                OutLined = true
            };
        
        public TwistedFate()
        {
            var menu_autopicker = new Menu("W-Pick A Card", "W-Pick A Card");
            var menu_notifier = new Menu("Ult Notifier", "Ult Notifier");


            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 5500);

            Q.SetSkillshot(0.1f, 60, 1450, false, SkillshotType.SkillshotLine);

            Spell[] SpellList = new[] { Q };
            ConfigManager.SetCombo(SpellList, true);
            ConfigManager.SetHarass(SpellList, true);
            
            ConfigManager.championMenu.AddSubMenu(menu_autopicker);
            menu_autopicker.AddItem(new MenuItem("TF_Cardpicker", "Pick").SetValue(true));
            menu_autopicker.AddItem(new MenuItem("TF_Goldkey", "GoldKey:").SetValue(new KeyBind('T', KeyBindType.Press)));
            menu_autopicker.AddItem(new MenuItem("TF_Bluekey", "BlueKey:").SetValue(new KeyBind('E', KeyBindType.Press)));
            menu_autopicker.AddItem(new MenuItem("TF_Redkey", "RedKey").SetValue(new KeyBind('W', KeyBindType.Press)));
            menu_autopicker.AddItem(new MenuItem("TF_delay", "Delay X(ms)").SetValue(new Slider(100, 0, 400)));
            menu_autopicker.AddItem(new MenuItem("TF_delayrnd", "Random Delay(0~Xms)").SetValue(false));


            ConfigManager.championMenu.AddSubMenu(menu_notifier);
            menu_notifier.AddItem(new MenuItem("TF_notifier", "UltNotifier").SetValue(true));
            menu_notifier.AddItem(new MenuItem("TF_notifier_HP", "Notice On HP(%)").SetValue(new Slider(10, 0, 100)));


            CircleRendering(Player, Q.Range, "draw_Qrange", 5);
            CircleRendering(Player, W.Range, "draw_Wrange", 5);
            CircleRendering(Player, R.Range, "draw_Rrange", 5);


            text_notifier.Add();
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            
            #region get info
            float Player_bAD = Player.BaseAttackDamage;
            float Player_aAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_bAD + Player_aAD;
            float Player_bAP = Player.BaseAbilityDamage;
            float Player_aAP = Player.FlatMagicDamageMod;
            float Player_totalAP = Player_bAP + Player_aAP;
            #endregion
            #region wildcards

            if (GetBoolFromMenu(Q,true) && OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                Cast(Q, TargetSelector.DamageType.Magical);

            #endregion
            #region pick a card
            if (ConfigManager.championMenu.Item("TF_Cardpicker").GetValue<bool>())
            {
                Random delayt = new Random(DateTime.Now.Millisecond);
                delayi = ConfigManager.championMenu.Item("TF_delay").GetValue<Slider>().Value;
                if (ConfigManager.championMenu.Item("TF_delayrnd").GetValue<bool>())
                    delayi = delayt.Next(0, ConfigManager.championMenu.Item("TF_delay").GetValue<Slider>().Value);

                if (stack >= 20)
                    cards = card.none;
                    stack = 0;

                if (Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Cooldown)
                    stack++;

                if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && cards == card.none
                    && Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Ready)
                {
                    if (ConfigManager.championMenu.Item("TF_Goldkey").GetValue<KeyBind>().Active)
                    {
                        cards = card.gold;
                        stack = 0;
                        Cast(W);
                    }
                    if (ConfigManager.championMenu.Item("TF_Bluekey").GetValue<KeyBind>().Active)
                    {
                        cards = card.blue;
                        stack = 0;
                        Cast(W);
                    }
                    if (ConfigManager.championMenu.Item("TF_Redkey").GetValue<KeyBind>().Active)
                    {
                        cards = card.red;
                        stack = 0;
                        Cast(W);
                    }
                }
                if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock" && cards == card.gold)
                {
                    Utility.DelayAction.Add(delayi, () => { Cast(W); });
                    cards = card.none;
                }
                if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock" && cards == card.blue)
                {
                    Utility.DelayAction.Add(delayi, () => { Cast(W); });
                    cards = card.none;
                }
                if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock" && cards == card.red)
                {
                    Utility.DelayAction.Add(delayi, () => { Cast(W); });
                    cards = card.none;
                }
            }
            #endregion
            #region notifier
            if (championMenu.Item("TF_notifier").GetValue<bool>() && R.IsReady())
            {
                ultText = "";
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero =>
                    hero.IsEnemy && hero.IsHPBarRendered && Player.Distance(hero.Position) > 1450))
                    {
                    if (hero.HealthPercentage() <= (float)ConfigManager.championMenu.Item("TF_notifier_HP").GetValue<Slider>().Value)
                    {
                        ultText += hero.ChampionName + " ";
                    }
                }
                if (ultText == "")
                    return;

                text_notifier.text = "Find Weaken Champion! :" + ultText;
                text_notifier.TextUpdate();
            }
            #endregion
        }
        public override void Drawing_OnDrawEndSence(EventArgs args)
        {
            if (championMenu.Item("draw_Rrange", true).GetValue<Circle>().Active)
                J_DrawCircle(Player.Position, R.Range, championMenu.Item("draw_Rrange", true).GetValue<Circle>().Color, 1, 20, true);
            
        }

        public static double getEDmg(Obj_AI_Base target, double AP, double AD, int s_level)
        {
            double[] spell_basedamage = { 0, 55, 80, 105, 130, 155 };

            double eDmg = AP * 0.50 + spell_basedamage[s_level];

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, eDmg) + ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, AD);
        }
        public override void DrawComboDamage(EventArgs args)
        {
            if (!championMenu.Item("draw_combo",true).GetValue<bool>()) return;
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsHPBarRendered))
            {
                #region get info
                float Player_bAD = Player.BaseAttackDamage;
                float Player_aAD = Player.FlatPhysicalDamageMod;
                float Player_totalAD = Player_bAD + Player_aAD;
                float Player_bAP = Player.BaseAbilityDamage;
                float Player_aAP = Player.FlatMagicDamageMod;
                float Player_totalAP = Player_bAP + Player_aAP;
                #endregion

                List<SpellSlot> Combolist = new List<SpellSlot>();
                if (Q.IsReady())
                    Combolist.Add(SpellSlot.Q);
                if (W.IsReady())
                    Combolist.Add(SpellSlot.W);

                var dmg = Player.GetComboDamage(target, Combolist);

                dmg += GetDmgWithItem(target);

                if (Player.HasBuff("CardMasterStackParticle"))
                    dmg += getEDmg(target, Player_totalAP, Player_totalAD, Player.Spellbook.GetSpell(SpellSlot.E).Level);

                var hpPercent = target.Health / target.MaxHealth;
                var dmgPercent = (float)dmg / target.MaxHealth;
                var x = (int)target.HPBarPosition.X;
                var y = (int)target.HPBarPosition.Y;


                font.DrawText(null, dmg.ToString("0"), x + 1, y + 1, SharpDX.Color.Black);
                font.DrawText(null, dmg.ToString("0"), x, y + 1, SharpDX.Color.Black);
                font.DrawText(null, dmg.ToString("0"), x - 1, y - 1, SharpDX.Color.Black);
                font.DrawText(null, dmg.ToString("0"), x, y - 1, SharpDX.Color.Black);
                font.DrawText(null, dmg.ToString("0"), x, y, SharpDX.Color.White);
                var barX = (target.HPBarPosition.X + 105 * hpPercent) + 10 - (dmgPercent * 105);
                Drawing.DrawLine(Math.Max(barX, target.HPBarPosition.X + 10), target.HPBarPosition.Y + 20, 
                    Math.Max(barX, target.HPBarPosition.X + 10), target.HPBarPosition.Y + 28, 2,
                    Color.Blue);

                if (target.Health <= dmg)
                    Render.Circle.DrawCircle(target.Position, 100, Color.Peru, 50);
            }
        }
       
        public static void J_DrawCircle(Vector3 center,
            float radius,
            Color color,
            int thickness = 5,
            int quality = 30,
            bool onMinimap = false)
        {
            if (!onMinimap)
            {
                Render.Circle.DrawCircle(center, radius, color, thickness);
                return;
            }

            var pointList = new List<Vector3>();
            for (var i = 0; i < quality; i++)
            {
                var angle = i * Math.PI * 2 / quality;
                pointList.Add(
                    new Vector3(
                        center.X + radius * (float)Math.Cos(angle), center.Y + radius * (float)Math.Sin(angle),
                        center.Z));
            }

            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);

                Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, thickness, color);
            }
        }
    }
}
