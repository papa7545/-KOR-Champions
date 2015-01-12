using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kor_AIO.Champions
{
    class Jayce : Kor_AIO_Base
    {
        public Jayce()
        {
            Program.PrintChat("Jayce Loaded!");
            SetSpells();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            QCharged = new Spell(SpellSlot.Q, 1650);
            Q2 = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            W2 = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 650);
            E2 = new Spell(SpellSlot.E, 240);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.15f, 60, 1200, true, SkillshotType.SkillshotLine);
            QCharged.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            Q2.SetTargetted(0.25f, float.MaxValue);
            E.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E2.SetTargetted(.25f, float.MaxValue);
        }
        private bool _hammerTime;
        private readonly SpellDataInst _qdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);
        private void LoadMenu()
        {
            //Keys
            var key = new Menu("Keys", "Keys");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo!", true).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass!", true).SetValue(new KeyBind("S".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "Harass (toggle)!", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("shootMouse", "Shoot QE Mouse", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                ConfigManager.baseMenu.AddSubMenu(key);
            }

            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Cannon Q", true).SetValue(true));
                combo.AddItem(new MenuItem("qSpeed", "QE Speed, Higher = Faster, Lower = Accurate", true).SetValue(new Slider(1600, 400, 2500)));
                combo.AddItem(new MenuItem("UseWCombo", "Use Cannon W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use Cannon E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseQComboHam", "Use Hammer Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWComboHam", "Use Hammer W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseEComboHam", "Use Hammer E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R to Switch", true).SetValue(true));
                ConfigManager.baseMenu.AddSubMenu(combo);
            }

            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("UseQHarassHam", "Use Q Hammer", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarassHam", "Use W Hammer", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarassHam", "Use E Hammer", true).SetValue(true));
                harass.AddItem(new MenuItem("UseRHarass", "Use R to switch", true).SetValue(true));
                harass.AddItem(new MenuItem("manaH", "Mana > %", true).SetValue(new Slider(40)));
                ConfigManager.baseMenu.AddSubMenu(harass);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("UseInt", "Use E to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use E for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("forceGate", "Force Gate After Q", true).SetValue(false));
                misc.AddItem(new MenuItem("gatePlace", "Gate Distance", true).SetValue(new Slider(300, 50, 600)));
                misc.AddItem(new MenuItem("UseQAlways", "Use Q When E onCD", true).SetValue(true));
                misc.AddItem(new MenuItem("autoE", "EPushInCombo HP < %", true).SetValue(new Slider(20)));
                misc.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                ConfigManager.baseMenu.AddSubMenu(misc);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) 
                return;

            _hammerTime = !_qdata.Name.Contains("jayceshockblast");

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Program.PrintChat("Combo");
                shootQE(Game.CursorPos);
            }

        }


        public static Vector2 getParalelVec(Vector3 pos)
        {
            if (ConfigManager.baseMenu.Item("parlelE").GetValue<bool>())
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = ConfigManager.baseMenu.Item("eAway").GetValue<Slider>().Value;
                away = (neg == 1) ? away : -away;
                var v2 = Vector3.Normalize(pos - Player.ServerPosition) * away;
                var bom = new Vector2(v2.Y, -v2.X);
                return Player.ServerPosition.To2D() + bom;
            }
            else
            {
                var v2 = Vector3.Normalize(pos - Player.ServerPosition) * 300;
                var bom = new Vector2(v2.X, v2.Y);
                return Player.ServerPosition.To2D() + bom;
            }
        }

        public static bool shootQE(Vector3 pos)
        {
            try
            {
                if (isHammer && R.IsReady())
                    R2.Cast();
                if (!E.IsReady() || !Q.IsReady() || isHammer)
                    return false;

                if (ConfigManager.baseMenu.Item("packets").GetValue<bool>())
                {
                    Q.Cast(pos.To2D());
                    E.Cast(getParalelVec(pos));
                }
                else
                {
                    Vector3 bPos = Player.ServerPosition - Vector3.Normalize(pos - Player.ServerPosition) * 50;

                    Player.IssueOrder(GameObjectOrder.MoveTo, bPos);
                    Q.Cast(pos);

                    E.Cast(getParalelVec(pos));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
