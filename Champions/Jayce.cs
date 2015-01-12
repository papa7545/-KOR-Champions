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

        public static bool isCannon = true;

        private readonly float[] _cannonQcd = { 8, 8, 8, 8, 8 };
        private readonly float[] _cannonWcd = { 14, 12, 10, 8, 6 };
        private readonly float[] _cannonEcd = { 16, 16, 16, 16, 16 };

        private readonly float[] _hammerQcd = { 16, 14, 12, 10, 8 };
        private readonly float[] _hammerWcd = { 10, 10, 10, 10, 10 };
        private readonly float[] _hammerEcd = { 14, 13, 12, 11, 10 };

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

        private void LoadMenu()
        {
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
                harass.AddItem(new MenuItem("HarassUseQ", "Use Cannon Q", true).SetValue(true));
                harass.AddItem(new MenuItem("HarassUseW", "Use Cannon W", true).SetValue(true));
                harass.AddItem(new MenuItem("manaH", "Mana > %", true).SetValue(new Slider(40)));
                ConfigManager.championMenu.AddSubMenu(harass);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("shootQE", "Shoot QE", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                misc.AddItem(new MenuItem("UseParallelE", "Use Parallel E", true).SetValue(true));
                misc.AddItem(new MenuItem("eAway", "Gate Distance", true).SetValue(new Slider(20, 3, 60)));
                misc.AddItem(new MenuItem("UseInterrupt", "Use E to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGapCloser", "Use E for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("UseQAlways", "Use Q When E onCD", true).SetValue(true));
                misc.AddItem(new MenuItem("autoE", "EPushInCombo HP < %", true).SetValue(new Slider(20)));
                misc.AddItem(new MenuItem("usePacket", "Use Packet", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(misc);
            }
        }

        public static void checkForm()
        {
            isCannon = Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name.Contains("jayceshockblast");
        }

        public static bool packets()
        {
            return ConfigManager.championMenu.Item("usePacket", true).GetValue<bool>();
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) 
                return;

            checkForm();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {

            }

            if (ConfigManager.championMenu.Item("shootQE", true).GetValue<KeyBind>().Active)
            {
                shootQE(Game.CursorPos);
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!ConfigManager.championMenu.Item("UseGap", true).GetValue<bool>())
                return;

            if (_hamEcd == 0 && gapcloser.Sender.IsValidTarget(E2.Range + gapcloser.Sender.BoundingRadius))
            {
                if (isCannon && R.IsReady())
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
                if (isCannon && R.IsReady())
                    R.Cast();

                if (Q2.IsReady())
                    Q2.Cast(unit, packets());
            }

            if (unit != null && (Player.Distance(unit) < E2.Range + unit.BoundingRadius && _hamEcd == 0))
            {
                if (isCannon && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(unit, packets());
            }
        }

#region ShootQE

        public static Vector2 getParalelVec(Vector3 pos)
        {
            if (ConfigManager.championMenu.Item("UseParallelE", true).GetValue<bool>())
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = ConfigManager.championMenu.Item("eAway", true).GetValue<Slider>().Value;
                away = (neg == 1) ? away : -away;
                var v2 = Vector3.Normalize(pos - Player.ServerPosition) * away;
                var bom = new Vector2(v2.Y, -v2.X);
                return Player.ServerPosition.To2D() + bom;
            }
            else
            {
                var v2 = Vector3.Normalize(pos - Player.ServerPosition) * 180;
                var bom = new Vector2(v2.X, v2.Y);
                return Player.ServerPosition.To2D() + bom;
            }
        }

        public static bool shootQE(Vector3 pos)
        {
            try
            {
                if (!E.IsReady() || !Q.IsReady() || !isCannon)
                    return false;

                if (!isCannon && R.IsReady())
                    R.Cast();

                if (ConfigManager.championMenu.Item("usePacket", true).GetValue<bool>())
                {
                    Q.Cast(pos.To2D(), packets());
                    E.Cast(getParalelVec(pos), packets());
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
            return true;
        }

#endregion

        private float _canQcd, _canWcd, _canEcd;
        private float _hamQcd, _hamWcd, _hamEcd;
        private float _canQcdRem, _canWcdRem, _canEcdRem;
        private float _hamQcdRem, _hamWcdRem, _hamEcdRem;

        private void ProcessCoolDowns()
        {
            _canQcd = ((_canQcdRem - Game.Time) > 0) ? (_canQcdRem - Game.Time) : 0;
            _canWcd = ((_canWcdRem - Game.Time) > 0) ? (_canWcdRem - Game.Time) : 0;
            _canEcd = ((_canEcdRem - Game.Time) > 0) ? (_canEcdRem - Game.Time) : 0;
            _hamQcd = ((_hamQcdRem - Game.Time) > 0) ? (_hamQcdRem - Game.Time) : 0;
            _hamWcd = ((_hamWcdRem - Game.Time) > 0) ? (_hamWcdRem - Game.Time) : 0;
            _hamEcd = ((_hamEcdRem - Game.Time) > 0) ? (_hamEcdRem - Game.Time) : 0;
        }

        private float CalculateCD(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        private void GetCooldowns(GameObjectProcessSpellCastEventArgs spell)
        {
            if (isCannon)
            {
                if (spell.SData.Name == "jayceshockblast")
                    _canQcdRem = Game.Time + CalculateCD(_cannonQcd[Q.Level - 1]);
                if (spell.SData.Name == "jaycehypercharge")
                    _canWcdRem = Game.Time + CalculateCD(_cannonWcd[W.Level - 1]);
                if (spell.SData.Name == "jayceaccelerationgate")
                    _canEcdRem = Game.Time + CalculateCD(_cannonEcd[E.Level - 1]);
            }
            else
            {
                if (spell.SData.Name == "JayceToTheSkies")
                    _hamQcdRem = Game.Time + CalculateCD(_hammerQcd[Q.Level - 1]);
                if (spell.SData.Name == "JayceStaticField")
                    _hamWcdRem = Game.Time + CalculateCD(_hammerWcd[W.Level - 1]);
                if (spell.SData.Name == "JayceThunderingBlow")
                    _hamEcdRem = Game.Time + CalculateCD(_hammerEcd[E.Level - 1]);
            }
        }

    }
}
