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

        public static Obj_SpellMissile myCastedQ = null;

        public static Vector3 castQon = new Vector3(0, 0, 0);

        /* COOLDOWN STUFF */
        public static float[] _cannonQcd = { 8, 8, 8, 8, 8 };
        public static float[] _cannonWcd = { 14, 12, 10, 8, 6 };
        public static float[] _cannonEcd = { 16, 16, 16, 16, 16 };

        public static float[] _hammerQcd = { 16, 14, 12, 10, 8 };
        public static float[] _hammerWcd = { 10, 10, 10, 10, 10 };
        public static float[] _hammerEcd = { 14, 12, 12, 11, 10 };

        public static float _canQcd = 0, _canWcd = 0, _canEcd = 0;
        public static float _hamQcd = 0, _hamWcd = 0, _hamEcd = 0;

        public static float _canQcdRem = 0, _canWcdRem = 0, _canEcdRem = 0;
        public static float _hamQcdRem = 0, _hamWcdRem = 0, _hamEcdRem = 0;

        /* COOLDOWN STUFF END */
        public static bool isCannon = true;

        public Jayce()
        {
            LoadMenu();
            SetSpells();
        }

        public void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            QCharged = new Spell(SpellSlot.Q, 1650);
            Q2 = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            W2 = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 650);
            E2 = new Spell(SpellSlot.E, 240);
            R = new Spell(SpellSlot.R);
            R2 = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.15f, 60, 1200, true, SkillshotType.SkillshotLine);
            QCharged.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            Q2.SetTargetted(0.25f, float.MaxValue);
            E.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E2.SetTargetted(.25f, float.MaxValue);
        }

        public void LoadMenu()
        {
            //Combo
            championMenu.SubMenu("Combo").AddItem(new MenuItem("comboUseQCannon", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("comboUseWCannon", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("comboUseECannon", "Use Cannon E", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("comboUseQHammer", "Use Hammer Q", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("comboUseWHammer", "Use Hammer W", true).SetValue(true));
            championMenu.SubMenu("Combo").AddItem(new MenuItem("comboUseEHammer", "Use Hammer E", true).SetValue(true));

            //Harass
            championMenu.SubMenu("Harass").AddItem(new MenuItem("HarassUseQCannon", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("Harass").AddItem(new MenuItem("HarassUseWCannon", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("Harass").AddItem(new MenuItem("manaH", "Mana > %", true).SetValue(new Slider(40)));

            //Lane Clear
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseQ", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseW", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseE", "Use Cannon E", true).SetValue(true));
            championMenu.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearMana", "Mana > %", true).SetValue(new Slider(40)));

            //Jungle Clear
            championMenu.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearUseQ", "Use Cannon Q", true).SetValue(true));
            championMenu.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearUseW", "Use Cannon W", true).SetValue(true));
            championMenu.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearMana", "Mana > %", true).SetValue(new Slider(40)));

            //Misc
            championMenu.SubMenu("Misc").AddItem(new MenuItem("shootQE", "Shoot QE", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("useEtype", "E Type", true).SetValue<StringList>(new StringList(new string[] { "Normal", "Parallel" }, 1)));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("eAway", "Gate Distance", true).SetValue(new Slider(20, 3, 60)));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("useCollision", "useCollision", true).SetValue(true));
            championMenu.SubMenu("Misc").AddItem(new MenuItem("useQwhenEonCD", "Use Q When E on CD", true).SetValue(true));
            
            //Draw
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("drawCannonQ", "Draw Cannon Q", true).SetValue(new Circle(true, Color.Red)));
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("drawCannonQCharged", "Draw Cannon Q Charged", true).SetValue(new Circle(true, Color.Blue)));
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("drawHammerQ", "Draw Hammer Q", true).SetValue(new Circle(true, Color.Green)));
            championMenu.SubMenu("Drawings").AddItem(new MenuItem("drawHammerE", "Draw Hammer E", true).SetValue(new Circle(true, Color.White)));
        }
        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var useWCombo = championMenu.Item("comboUseWCannon", true).GetValue<bool>();

            if (unit.IsMe && isCannon)
            {
                if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (_canWcd == 0 && Player.Distance(target) < 600 && isCannon && W.Level > 0 && W.IsReady())
                    {
                        if (useWCombo)
                        {
                            Orbwalking.ResetAutoAttackTimer();
                            W.Cast();
                        }
                    }
                }
            }
        }
        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            checkForm();
            processCDs();



            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                activateMura();
                Combo();
            }

            else
                deActivateMura();

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {

            }

            if (championMenu.Item("shootQE", true).GetValue<KeyBind>().Active)
            {
                shootQE(Game.CursorPos);
            }

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

            }
        }
        public static void checkForm()
        {
            isCannon = Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name.Contains("jayceshockblast");
        }
        public static void Combo()
        {
            var qTarget = TargetSelector.GetTarget(QCharged.Range, TargetSelector.DamageType.Physical);
            var qChargedTarget = TargetSelector.GetTarget(QCharged.Range, TargetSelector.DamageType.Physical);
            var q2Target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);
            var e2Target = TargetSelector.GetTarget(E2.Range, TargetSelector.DamageType.Physical);

            if (isCannon)
            {
                if (E.IsReady() && Q.IsReady() && Player.Distance(qChargedTarget) <= QCharged.Range && qChargedTarget != null)
                {
                    castQEPred(qChargedTarget);
                }
                else if (Q.IsReady() && Player.Distance(qTarget) <= Q.Range && qTarget != null && championMenu.Item("useQwhenEonCD", true).GetValue<bool>())
                {
                    castQPred(qTarget);
                }

                if (!Jayce.E.IsReady())
                    castQon = new Vector3(0, 0, 0);

                else if (Jayce.castQon.X != 0)
                    shootQE(Jayce.castQon);
            }

            else
            {
                if (E2.IsReady() && Player.Distance(e2Target) <= E2.Range + q2Target.BoundingRadius)
                {
                    E2.Cast(e2Target);
                }
                if (Q2.IsReady() && Player.Distance(q2Target) <= Q2.Range + q2Target.BoundingRadius)
                {
                    Q2.Cast(q2Target);
                }
                if (W2.IsReady() && Player.Distance(q2Target) > W2.Range)
                {
                    W2.Cast();
                }
            }
        }

        public static void castQEPred(Obj_AI_Hero target)
        {
            if (!isCannon)
                return;

            PredictionOutput po = QCharged.GetPrediction(target);
            if (po.Hitchance >= HitChance.Low && Player.Distance(po.UnitPosition) < (QCharged.Range + target.BoundingRadius))
            {
                castQon = po.CastPosition;
            }
            else if (po.Hitchance == HitChance.Collision && championMenu.Item("useCollision").GetValue<bool>())
            {
                Obj_AI_Base fistCol = po.CollisionObjects.OrderBy(unit => unit.Distance(Player.ServerPosition)).First();
                if (fistCol.Distance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.Distance(target.ServerPosition) < (180 - fistCol.BoundingRadius / 2))
                {
                    castQon = po.CastPosition;
                }
            }
        }

        public static void castQPred(Obj_AI_Hero target)
        {
            if (!isCannon)
                return;
            PredictionOutput po = Q.GetPrediction(target);
            if (po.Hitchance >= HitChance.High && Player.Distance(po.UnitPosition) < (Q.Range + target.BoundingRadius))
            {
                Q.Cast(po.CastPosition);
            }
            else if (po.Hitchance == HitChance.Collision && championMenu.Item("useCollision").GetValue<bool>())
            {
                Obj_AI_Base fistCol = po.CollisionObjects.OrderBy(unit => unit.Distance(Player.ServerPosition)).First();
                if (fistCol.Distance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.Distance(target.ServerPosition) < (100 - fistCol.BoundingRadius / 2))
                {
                    Q.Cast(po.CastPosition);
                }

            }
        }

        public static bool shootQE(Vector3 pos)
        {
            try
            {
                if (!isCannon && R2.IsReady())
                    R2.Cast();

                if (!E.IsReady() || !Q.IsReady() || !isCannon)
                    return false;

                Q.Cast(pos);

                E.Cast(getParalelVec(pos));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public static bool gotSpeedBuff()//jaycehypercharge
        {
            return Player.Buffs.Any(bi => bi.Name.Contains("jaycehypercharge"));
        }

        public static Vector2 getParalelVec(Vector3 pos)
        {
            if (championMenu.Item("useEtype", true).GetValue<StringList>().SelectedValue == "Parallel")
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = championMenu.SubMenu("Misc").Item("eAway", true).GetValue<Slider>().Value;
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
        #region CD Calculator
        public static float calcRealCD(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        public static void processCDs()
        {
            _hamQcdRem = ((_hamQcd - Game.Time) > 0) ? (_hamQcd - Game.Time) : 0;
            _hamWcdRem = ((_hamWcd - Game.Time) > 0) ? (_hamWcd - Game.Time) : 0;
            _hamEcdRem = ((_hamEcd - Game.Time) > 0) ? (_hamEcd - Game.Time) : 0;

            _canQcdRem = ((_canQcd - Game.Time) > 0) ? (_canQcd - Game.Time) : 0;
            _canWcdRem = ((_canWcd - Game.Time) > 0) ? (_canWcd - Game.Time) : 0;
            _canEcdRem = ((_canEcd - Game.Time) > 0) ? (_canEcd - Game.Time) : 0;
        }

        public static void GetCooldowns(GameObjectProcessSpellCastEventArgs spell)
        {
            try
            {
                if (spell.SData.Name == "JayceToTheSkies")
                    _hamQcd = Game.Time + calcRealCD(_hammerQcd[Q2.Level - 1]);
                if (spell.SData.Name == "JayceStaticField")
                    _hamWcd = Game.Time + calcRealCD(_hammerWcd[W2.Level - 1]);
                if (spell.SData.Name == "JayceThunderingBlow")
                    _hamEcd = Game.Time + calcRealCD(_hammerEcd[E2.Level - 1]);

                if (spell.SData.Name == "jayceshockblast")
                    _canQcd = Game.Time + calcRealCD(_cannonQcd[Q.Level - 1]);
                if (spell.SData.Name == "jaycehypercharge")
                    _canWcd = Game.Time + calcRealCD(_cannonWcd[W.Level - 1]);
                if (spell.SData.Name == "jayceaccelerationgate")
                    _canEcd = Game.Time + calcRealCD(_cannonEcd[E.Level - 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        public static void drawRange()
        {
            if (isCannon)
            {
                if (championMenu.SubMenu("Drawings").Item("drawCannonQ", true).GetValue<Circle>().Active)
                {
                    if (_canQcd == 0)
                        Render.Circle.DrawCircle(Player.Position, Q.Range, championMenu.SubMenu("Drawings").Item("drawCannonQ", true).GetValue<Circle>().Color);
                    else
                        Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Red);
                }

                if (championMenu.SubMenu("Drawings").Item("drawCannonQCharged", true).GetValue<Circle>().Active)
                {
                    if ((_canQcd == 0) && (_canEcd == 0))
                        Render.Circle.DrawCircle(Player.Position, QCharged.Range, championMenu.SubMenu("Drawings").Item("drawCannonQCharged", true).GetValue<Circle>().Color);
                    else
                        Render.Circle.DrawCircle(Player.Position, QCharged.Range, Color.Red);
                }
            }
            else
            {
                if (championMenu.SubMenu("Drawings").Item("drawHammerQ", true).GetValue<Circle>().Active)
                {
                    if (_hamQcd == 0)
                        Render.Circle.DrawCircle(Player.Position, Q2.Range, championMenu.SubMenu("Drawings").Item("drawHammerQ", true).GetValue<Circle>().Color);
                    else
                        Render.Circle.DrawCircle(Player.Position, Q2.Range, Color.Red);
                }
                if (championMenu.SubMenu("Drawings").Item("drawHammerE", true).GetValue<Circle>().Active)
                {
                    if (_hamEcd == 0)
                        Render.Circle.DrawCircle(Player.Position, E2.Range, championMenu.SubMenu("Drawings").Item("drawHammerE", true).GetValue<Circle>().Color);
                    else
                        Render.Circle.DrawCircle(Player.Position, E2.Range, Color.Red);
                }
            }
        }

        public static void drawCD()
        {
            var pScreen = Drawing.WorldToScreen(Player.Position);
            pScreen[0] -= 20;

            if (!isCannon)
            {
                if (_canQcdRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Red, format: "Q: " + _canQcdRem.ToString("0.0"));

                if (_canWcdRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Red, "W: " + _canWcdRem.ToString("0.0"));

                if (_canEcdRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Red, "E: " + _canEcdRem.ToString("0.0"));
            }
            else
            {
                if (_hamQcdRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Red, "Q: " + _hamQcdRem.ToString("0.0"));

                if (_hamWcdRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Red, "W: " + _hamWcdRem.ToString("0.0"));

                if (_hamEcdRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Red, "E: " + _hamEcdRem.ToString("0.0"));
            }
        }

        public static float getEQDmg(Obj_AI_Base target)
        {
            return
                (float)
                    Player.CalcDamage(target, Damage.DamageType.Physical,
                        (7 + (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 77)) +
                        (1.68 * ObjectManager.Player.FlatPhysicalDamageMod));
        }


        public static void activateMura()
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                Items.UseItem(3042);
        }

        public static void deActivateMura()
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") != 0)
                Items.UseItem(3042);
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
            if (!championMenu.Item("useAntiGapCloser", true).GetValue<bool>())
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
            drawCD();
            drawRange();
        }
    }
}
