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

        public static Render.Text text_notifier = new Render.Text("Find Weaken Champion!", Player, new Vector2(0, 50), (int)32, ColorBGRA.FromRgba(0xFF00FFBB))
            {
                VisibleCondition =  c => textshow,
                OutLined = true
            };

        public TwistedFate()
        {

            text_notifier.Add();

            var menu_ts = new Menu("TargetSelector", "TargetSelector");
            var menu_q = new Menu("Q-Wild Cards", "Q-Wild Cards");
            var menu_autopicker = new Menu("W-Pick A Card", "W-Pick A Card");
            var menu_notifier = new Menu("Ult Notifier", "Ult Notifier");
            var menu_drawing = new Menu("drawing", "drawing");


            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 5500);

            Q.SetSkillshot(0.1f, 60, 1450, false, SkillshotType.SkillshotLine);



            ConfigManager.championMenu.AddSubMenu(menu_q);
            menu_q.AddItem(new MenuItem("TF_q_enable", "Enable").SetValue(true));
            menu_q.AddItem(new MenuItem("TF_q_key", "Key:").SetValue(new KeyBind(32, KeyBindType.Press)));

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

            Drawing.OnEndScene += OnDraw_EndScene;
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
                if (ConfigManager.championMenu.Item("TF_q_enable").GetValue<bool>() && ConfigManager.championMenu.Item("TF_q_key").GetValue<KeyBind>().Active)
                {
                    Kor_AIO_Base.Cast(Q, TargetSelector.DamageType.Magical);
                }
                #endregion
                #region pick a card
                if (ConfigManager.championMenu.Item("TF_Cardpicker").GetValue<bool>())
                {
                    Random delayt = new Random(DateTime.Now.Millisecond);
                    delayi = ConfigManager.championMenu.Item("TF_delay").GetValue<Slider>().Value;
                    if (ConfigManager.championMenu.Item("TF_delayrnd").GetValue<bool>())
                        delayi = delayt.Next(0, ConfigManager.championMenu.Item("TF_delay").GetValue<Slider>().Value);

                    if (stack >= 20)
                    {
                        cards = card.none;
                        stack = 0;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Cooldown)
                    {
                        stack++;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && cards == card.none
                        && Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Ready)
                    {
                        if (ConfigManager.championMenu.Item("TF_Goldkey").GetValue<KeyBind>().Active)
                        {
                            cards = card.gold;
                            stack = 0;
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                        if (ConfigManager.championMenu.Item("TF_Bluekey").GetValue<KeyBind>().Active)
                        {
                            cards = card.blue;
                            stack = 0;
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                        if (ConfigManager.championMenu.Item("TF_Redkey").GetValue<KeyBind>().Active)
                        {
                            cards = card.red;
                            stack = 0;
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock" && cards == card.gold)
                    {

                        Utility.DelayAction.Add(delayi, () => { Player.Spellbook.CastSpell(SpellSlot.W); });
                        cards = card.none;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock" && cards == card.blue)
                    {
                        Utility.DelayAction.Add(delayi, () => { Player.Spellbook.CastSpell(SpellSlot.W); });
                        cards = card.none;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock" && cards == card.red)
                    {
                        Utility.DelayAction.Add(delayi, () => { Player.Spellbook.CastSpell(SpellSlot.W); });
                        cards = card.none;
                    }
                }
                #endregion
                #region notifier
                if (ConfigManager.championMenu.Item("TF_notifier").GetValue<bool>() && R.IsReady())
                {
                    if (ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsEnemy && !hero.IsDead && hero.IsVisible && Player.Distance(hero.Position) > 1450))
                    {
                        string str = "";
                        foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => 
                            hero.IsEnemy && !hero.IsDead && hero.IsVisible && Player.Distance(hero.Position) > 1450))
                        {
                            if (hero.HealthPercentage() <= (float)ConfigManager.championMenu.Item("TF_notifier_HP").GetValue<Slider>().Value)
                            {
                                str += hero.ChampionName+" ";
                                textshow = true;
                            }
                        }

                        if (str == "")
                            textshow = false;
                        text_notifier.text = "Find Weaken Champion! :" + str;
                        text_notifier.TextUpdate();
                    }
                }
                else
                    textshow = false;
                #endregion
        }
        public static void OnDraw_EndScene(EventArgs args)
        {
            if (ConfigManager.championMenu.Item("draw_Rrange").GetValue<Circle>().Active)
            {
                J_DrawCircle(Player.Position, 5500, ConfigManager.championMenu.Item("draw_Rrange").GetValue<Circle>().Color, 1, 20, true);
                J_DrawCircle(Player.Position, 5500, ConfigManager.championMenu.Item("draw_Rrange").GetValue<Circle>().Color, 1);
            }
            if (ConfigManager.championMenu.Item("draw_Qrange").GetValue<Circle>().Active)
            {
                J_DrawCircle(Player.Position, 1450, ConfigManager.championMenu.Item("draw_Qrange").GetValue<Circle>().Color, 1);
            }
            if (ConfigManager.championMenu.Item("draw_Wrange").GetValue<Circle>().Active)
            {
                J_DrawCircle(Player.Position, Player.AttackRange, ConfigManager.championMenu.Item("draw_Wrange").GetValue<Circle>().Color, 1);
            }
            if (ConfigManager.championMenu.Item("TF_killable").GetValue<bool>() || ConfigManager.championMenu.Item("TF_damage").GetValue<bool>())
                    drawtarget();
        }


        #region 함수
        public static double getQDmg(Obj_AI_Base target, double AP, int s_level)
        {
            return Q.GetDamage(target);
        }
        public static double getWDmg(Obj_AI_Base target, double AP , double AD, int s_level,string name)
        {
            double[] spell_basedamage = { 0, 15, 22.5, 30, 37.5, 45 };
            switch (name)
            {
                case "goldcardlock":
                    spell_basedamage = new double[] { 0, 15, 22.5, 30, 37.5, 45 }; break;
                case "bluecardlock":
                    spell_basedamage = new double[] { 0, 40, 60, 80, 100, 120 }; break;
                case "redcardlock":
                    spell_basedamage = new double[] { 0, 30, 45, 60, 75, 90 }; break;
            }

            double eDmg = AP * 0.50 + AD * 1.00 + spell_basedamage[s_level];

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, eDmg);
        }
        public static double getEDmg(Obj_AI_Base target, double AP,double AD, int s_level)
        {
            double[] spell_basedamage = { 0, 55, 80, 105, 130, 155 };

            double eDmg = AP * 0.50 + spell_basedamage[s_level];

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, eDmg) + ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,AD);
        }

        public static void drawtarget()
        {
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && !hero.IsDead && hero.IsVisible))
            {
                #region get info
                float Player_bAD = Player.BaseAttackDamage;
                float Player_aAD = Player.FlatPhysicalDamageMod;
                float Player_totalAD = Player_bAD + Player_aAD;
                float Player_bAP = Player.BaseAbilityDamage;
                float Player_aAP = Player.FlatMagicDamageMod;
                float Player_totalAP = Player_bAP + Player_aAP;
                #endregion

                    double dmg = 0;
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Ready || Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                        dmg = getQDmg(target, Player_totalAP, Player.Spellbook.GetSpell(SpellSlot.Q).Level);

                    if (Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Ready || Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Surpressed)
                        dmg = dmg + getWDmg(target, Player_totalAP, Player_totalAD, Player.Spellbook.GetSpell(SpellSlot.W).Level, Player.Spellbook.GetSpell(SpellSlot.W).Name);

                    if (Player.HasBuff("CardMasterStackParticle"))
                        dmg = dmg + getEDmg(target, Player_totalAP, Player_totalAD, Player.Spellbook.GetSpell(SpellSlot.Q).Level);





                    if (target.Health <= dmg && ConfigManager.championMenu.Item("TF_killable").GetValue<bool>())
                        J_DrawCircle(target.Position, 100, Color.Peru, 50);

                    if (ConfigManager.championMenu.Item("TF_damage").GetValue<bool>())
                    {
                        var hpPercent = target.Health / target.MaxHealth;
                        var dmgPercent = (float)dmg / target.MaxHealth;

                        var barX = (target.HPBarPosition.X + 105 * hpPercent) +10 - (dmgPercent*105);
                        Drawing.DrawLine(Math.Max(barX, target.HPBarPosition.X + 10), target.HPBarPosition.Y + 20, Math.Max(barX, target.HPBarPosition.X + 10), target.HPBarPosition.Y + 28, 2, dmg > target.Health ? Color.White : Color.Blue);
                        Drawing.DrawText(target.HPBarPosition.X + 140, target.HPBarPosition.Y + 15, dmg > target.Health ? Color.Peru : Color.Blue, Convert.ToInt64(dmg).ToString());
                    }
                }
        }
       
        #endregion
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
