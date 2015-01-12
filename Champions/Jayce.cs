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
            LoadMenu();
            SetSpells();
        }

        public static bool isCannon = false;

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
        
        private readonly SpellDataInst _qdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);

        private void LoadMenu()
        {
            //Keys
            var key = new Menu("Keys", "Keys");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo", true).SetValue(new KeyBind("Space".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass", true).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "Harass (toggle)!", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("shootQEMouse", "Shoot QE Mouse", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                ConfigManager.championMenu.AddSubMenu(key);
            }

            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("ComboUseQ", "Use Cannon Q", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseW", "Use Cannon W", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseECombo", "Use Cannon E", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseQHammer", "Use Hammer Q", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseWHammer", "Use Hammer W", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseEHammer", "Use Hammer E", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseR", "Use R to Switch", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(combo);
            }

            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("HarassUseQ", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("HarassUseW", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("manaH", "Mana > %", true).SetValue(new Slider(40)));
                ConfigManager.championMenu.AddSubMenu(harass);
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
                ConfigManager.championMenu.AddSubMenu(misc);
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) 
                return;

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {

            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {

            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

            }

        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!ConfigManager.championMenu.Item("UseGap", true).GetValue<bool>()) return;

            if (_hamEcd == 0 && gapcloser.Sender.IsValidTarget(E2.Range + gapcloser.Sender.BoundingRadius))
            {
                if (!_hammerTime && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(gapcloser.Sender, packets());
            }
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!ConfigManager.championMenu.Item("UseInt", true).GetValue<bool>()) return;

            if (unit != null && Player.Distance(unit) < Q2.Range + unit.BoundingRadius && _hamQcd == 0 && _hamEcd == 0)
            {
                if (!_hammerTime && R.IsReady())
                    R.Cast();

                if (Q2.IsReady())
                    Q2.Cast(unit, packets());
            }

            if (unit != null && (Player.Distance(unit) < E2.Range + unit.BoundingRadius && _hamEcd == 0))
            {
                if (!_hammerTime && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(unit, packets());
            }
        }

        public static Vector2 getParalelVec(Vector3 pos)
        {
            if (ConfigManager.championMenu.Item("parlelE").GetValue<bool>())
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = ConfigManager.championMenu.Item("eAway").GetValue<Slider>().Value;
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
                if (!isCannon && R.IsReady())
                    R2.Cast();

                if (!E.IsReady() || !Q.IsReady() || !isCannon)
                    return false;

                //if (ConfigManager.championMenu.Item("packets").GetValue<bool>())
                {
                    Q.Cast(pos.To2D(), packets());
                    E.Cast(getParalelVec(pos), packets());
                }
                    /*
                else
                {
                    Vector3 bPos = Player.ServerPosition - Vector3.Normalize(pos - Player.ServerPosition) * 50;

                    Player.IssueOrder(GameObjectOrder.MoveTo, bPos);
                    Q.Cast(pos);
                    E.Cast(getParalelVec(pos));
                }*/

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }
    }
}
