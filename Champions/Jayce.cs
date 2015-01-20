using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
namespace Kor_AIO.Champions
{
    class Jayce : Kor_AIO_Base
    {


        public static bool isCannon = true;


        public Jayce()
        {
            LoadMenu();
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

        private void LoadMenu()
        {
            //Combo Menu:
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseQ", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseW", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseE", "Use Cannon E", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseQHammer", "Use Hammer Q", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseWHammer", "Use Hammer W", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseEHammer", "Use Hammer E", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("ComboUseR", "Use R to Switch", true).SetValue(true));

            //Harass menu:
            championMenu.SubMenu("Harass").AddItem(new MenuItem("HarassUseQ", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("Harass").AddItem(new MenuItem("HarassUseW", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("Harass").AddItem(new MenuItem("manaH", "Mana > %", true).SetValue(new Slider(40)));

            //Lane Clear menu:
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseQ", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseW", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseE", "Use Cannon E", true).SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearMana", "Mana > %", true).SetValue(new Slider(40)));

            //Jungle Clear menu:
            championMenu.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearUseQ", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearUseW", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearMana", "Mana > %", true).SetValue(new Slider(40)));

            //Misc Menu:
            championMenu.SubMenu("Misc").AddItem(new MenuItem("shootQE", "Shoot QE", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("useEscape", "Escape", true).SetValue(new KeyBind('~', KeyBindType.Press)));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("UseParallelE", "Use Parallel E", true).SetValue(true));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("eAway", "Gate Distance", true).SetValue(new Slider(20, 3, 60)));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("UseQAlways", "Use Q When E onCD", true).SetValue(true));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("autoE", "EPushInCombo HP < %", true).SetValue(new Slider(20)));
        }

        public static void checkForm()
        {
            isCannon = Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name.Contains("jayceshockblast");
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            checkForm();
            ProcessCoolDowns();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                //Combo();
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {

            }

            if (championMenu.Item("shootQE", true).GetValue<KeyBind>().Active)
            {
                shootQE(Game.CursorPos);
            }

            if (championMenu.Item("useEscape", true).GetValue<KeyBind>().Active)
            {
                MoveTo(Game.CursorPos);

                if (_canEcd == 0)
                {
                    if (isCannon)
                        E.Cast(getParalelVec(Game.CursorPos), Packets());
                    else
                        R.Cast();
                }
                else
                {
                    if (R.IsReady())
                        R.Cast();
                }
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

            }
        }

        private static int _lastMovement;
        private static void MoveTo(Vector3 position)
        {
            if (Environment.TickCount - _lastMovement < 80)
                return;

            _lastMovement = Environment.TickCount;

            if (Player.ServerPosition.Distance(position) < 50)
            {
                if (Player.Path.Count() > 1)
                    Player.IssueOrder(GameObjectOrder.HoldPosition, Player.Position);
                return;
            }
            var point = Player.ServerPosition +
            300 * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
            Player.IssueOrder(GameObjectOrder.MoveTo, point);
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (unit.IsMe)
            {
                GetCooldowns(args);
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!championMenu.SubMenu("Misc").Item("useAntiGapCloser", true).GetValue<bool>())
                return;

            if (_hamEcd == 0 && gapcloser.Sender.IsValidTarget(E2.Range + gapcloser.Sender.BoundingRadius))
            {
                if (isCannon && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(gapcloser.Sender, Packets());
            }
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!championMenu.SubMenu("Misc").Item("UseInterrupt", true).GetValue<bool>())
                return;

            if (unit != null && Player.Distance(unit) < Q2.Range + unit.BoundingRadius && _hamQcd == 0 && _hamEcd == 0)
            {
                if (isCannon && R.IsReady())
                    R.Cast();

                if (Q2.IsReady())
                    Q2.Cast(unit, Packets());
            }

            if (unit != null && (Player.Distance(unit) < E2.Range + unit.BoundingRadius && _hamEcd == 0))
            {
                if (isCannon && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(unit, Packets());
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (isCannon)
            {
                if (championMenu.SubMenu("Drawings").Item("draw_Qrange").GetValue<bool>())
                {
                    if (_canQcd == 0)
                        Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
                    else
                        Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Red);
                }

                if ((_canQcd == 0) && (_canEcd == 0))
                    Render.Circle.DrawCircle(Player.Position, QCharged.Range, Color.Aqua);
                else
                    Render.Circle.DrawCircle(Player.Position, QCharged.Range, Color.Red);
            }
            else
            {
                if (_hamQcd == 0)
                    Render.Circle.DrawCircle(Player.Position, Q2.Range, Color.Aqua);
                else
                    Render.Circle.DrawCircle(Player.Position, Q2.Range, Color.Red);
                if (_hamEcd == 0)
                    Render.Circle.DrawCircle(Player.Position, E2.Range, Color.Aqua);
                else
                    Render.Circle.DrawCircle(Player.Position, E2.Range, Color.Red);
            }
        }

        #region ShootQE

        public static Vector2 getParalelVec(Vector3 pos)
        {
            if (championMenu.Item("UseParallelE", true).GetValue<bool>())
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = championMenu.Item("eAway", true).GetValue<Slider>().Value;
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

                if (Packets())
                {
                    Q.Cast(pos.To2D(), Packets());
                    E.Cast(getParalelVec(pos), Packets());
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

        private float _canQcd, _canWcd, _canEcd, _canRcd;
        private float _hamQcd, _hamWcd, _hamEcd, hamRcd;
        private float _canQcdRem, _canWcdRem, _canEcdRem, _canRcdRem;
        private float _hamQcdRem, _hamWcdRem, _hamEcdRem, _hamRcdRem;

        private readonly float[] _cannonQcd = { 8, 8, 8, 8, 8 };
        private readonly float[] _cannonWcd = { 14, 12, 10, 8, 6 };
        private readonly float[] _cannonEcd = { 16, 16, 16, 16, 16 };

        private readonly float[] _hammerQcd = { 16, 14, 12, 10, 8 };
        private readonly float[] _hammerWcd = { 10, 10, 10, 10, 10 };
        private readonly float[] _hammerEcd = { 14, 13, 12, 11, 10 };

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
