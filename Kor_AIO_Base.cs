using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;


namespace Kor_AIO
{
    internal class Kor_AIO_Base
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu championMenu = ConfigManager.championMenu;
        public static Menu utilityMenu = ConfigManager.utilityMenu;
        public static Obj_AI_Hero CurrentTarget = null;
        public static List<SpellSlot> Combolist = new List<SpellSlot>();
        public static Font font = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 18, });
        
        public static List<RenderInfo> RenderCircleList = new List<RenderInfo>();
        public class RenderInfo
        {
            public Render.Circle _Circle;
            public MenuItem ColorItem = null;
        }

        //Spells
        public static SpellSlot IgniteSlot, SmiteSlot;
        public static Spell Ignite { get { return new Spell(IgniteSlot, 600); }}
        public static Spell P, Q, Q2, QCharged, W, W2, E, E2, R, R2,Smite;
        
        public static Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return ConfigManager.Orbwalker.ActiveMode; }
        }

        public Kor_AIO_Base()
        {
            ConfigManager.LoadMenu();

            var soundMenu = new Menu("Sounds", "Sounds");
            utilityMenu.AddSubMenu(soundMenu);
            soundMenu.AddItem(new MenuItem("onLoad", "onLoad")).SetValue(true);

            if (utilityMenu.Item("onLoad").GetValue<bool>())
            {
                var a = new SoundPlayer(Properties.Resources.StartUp);
                a.Play();
                a.Dispose();
            }

            Game.OnUpdate += Orbwalker_Setting;
            Game.OnUpdate += Game_Utility;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnDraw += Drawing_ForRender;
            Drawing.OnEndScene += Drawing_OnDrawEndSence;
            Drawing.OnEndScene += DrawComboDamage;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        #region Virtual Event
        public virtual void Game_OnGameUpdate(EventArgs args)
        {
        }
        public virtual void Game_Utility(EventArgs args)
        {
            if (utilityMenu.Item("ignite_enable").GetValue<bool>() && Ignite.Slot != SpellSlot.Unknown)
            {
                float dmg = 50 + 20 * Player.Level;
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero != null && hero.IsValid && !hero.IsDead && Player.ServerPosition.Distance(hero.ServerPosition) < Ignite.Range
                        && !hero.IsMe && !hero.IsAlly))
                {
                    if (Player.Spellbook.CanUseSpell(Ignite.Slot) == SpellState.Ready && (hero.Health + hero.HPRegenRate * 2) <= dmg)
                        Player.Spellbook.CastSpell(Ignite.Slot, hero);
                }
            }
        }
        public virtual void Orbwalker_Setting(EventArgs args)
        {
            ConfigManager.Orbwalker.SetMovement(!ConfigManager.championMenu.SubMenu("Orbwalker").SubMenu("Misc").Item("disMovement", true).GetValue<bool>());
            ConfigManager.Orbwalker.SetAttack(!ConfigManager.championMenu.SubMenu("Orbwalker").SubMenu("Misc").Item("disAttack", true).GetValue<bool>());
        }
        public virtual void Drawing_ForRender(EventArgs args)
        {
            if(RenderCircleList.Any())
                foreach(var _t in RenderCircleList.Where(t => t._Circle.Visible
                    && t._Circle.Color != t.ColorItem.GetValue<Circle>().Color))
                {
                    _t._Circle.Color = _t.ColorItem.GetValue<Circle>().Color;
                }
        }
        public virtual void Drawing_OnDraw(EventArgs args)
        {
        }
        public virtual void Drawing_OnDrawEndSence(EventArgs args)
        {
        }
        public virtual void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
        }
        public virtual void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
        }
        public virtual void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
        }
        public virtual void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
        }
        public virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
        }
        public virtual void Game_OnSendPacket(GamePacketEventArgs args)
        {
        }
        public virtual void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
        }
        public virtual void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
        }
        public virtual void DrawComboDamage(EventArgs args)
        {
            if (!Combolist.Any() || !championMenu.Item("draw_combo", true).GetValue<bool>()) return;

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
                var _list = Combolist.Where(t => t.IsReady());
                var dmg = Player.GetComboDamage(target, _list);
                dmg += GetDmgWithItem(target);


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
        #endregion

        #region methods

        #region Cast
        public static bool Packets()
        {
            return championMenu.Item("usePacket", true).GetValue<bool>();
        }

        public static void Cast(Spell spell, Obj_AI_Base target, HitChance hitChance = HitChance.VeryHigh, bool aoe = false)
        {
            if (target == null)
                return;
            if (!target.IsZombie && !target.IsDead && target.IsEnemy && target.IsHPBarRendered)
            {
                spell.Cast(target, Packets(), aoe);
            }
        }
        public static void Cast(Spell spell, Vector3 position, HitChance hitChance = HitChance.VeryHigh, bool aoe = false)
        {
            if (spell.IsReady())
            {
                spell.Cast(position, Packets());
            }
        }

        public static void Cast(Spell spell, TargetSelector.DamageType damageType)
        {
            if (spell.IsReady())
            {
                Obj_AI_Hero target = null;
                if (Player.ChampionName == "TwistedFate" && spell.Slot == SpellSlot.Q)
                    target = TargetSelector.GetTarget(1450, damageType);
                else
                    target = TargetSelector.GetTarget(spell.Range, damageType);

                if (target == null)
                    return;

                if(!target.IsZombie && !target.IsDead && target.IsEnemy && target.IsHPBarRendered)
                    spell.Cast(target, Packets());
            }
        }

        public static void Cast(Spell spell)
        {
            if (spell.IsReady())
                spell.Cast();
        }
        #endregion

        #region LastHit

        public static void Lasthit_Spell(Spell LastHitSpell, bool boolean = true, Obj_AI_Minion target = null)
        {
            if (target == null)
            {
                var Target = MinionManager.GetMinions(LastHitSpell.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                     .FirstOrDefault(t => t.Health < Player.CalcDamage(t,Damage.DamageType.Magical,LastHitSpell.GetDamage(t)) &&
                     HealthPrediction.GetHealthPrediction(t, (int)(Player.Distance(t, false) / LastHitSpell.Speed), (int)(LastHitSpell.Delay * 1000 + Game.Ping / 2)) > 0);


               
                if (Target != null)
                    Cast(LastHitSpell, Target);
            }
            else
            {
                if (boolean && target.Health < Player.CalcDamage(target, Damage.DamageType.Magical, LastHitSpell.GetDamage(target)) &&
                     HealthPrediction.GetHealthPrediction(target, (int)(Player.Distance(target, false) / LastHitSpell.Speed), (int)(LastHitSpell.Delay * 1000 + Game.Ping / 2)) > 0)
                    Cast(LastHitSpell, target);
            }
        }

        public static double GetDmgWithItem(Obj_AI_Base target)
        {
            double Dmg = 0;
            float Player_bAD = Player.BaseAttackDamage;
            float Player_aAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_bAD + Player_aAD;
            float Player_bAP = Player.BaseAbilityDamage;
            float Player_aAP = Player.FlatMagicDamageMod;
            float Player_totalAP = Player_bAP + Player_aAP;

            if (Items.HasItem(Convert.ToInt32(ItemId.Sheen)) && (Items.CanUseItem(Convert.ToInt32(ItemId.Sheen)) && Player.HasBuff("sheen", true)))
                return Dmg = Damage.CalcDamage(Player, target, Damage.DamageType.Physical, Player_bAD);

            if (Items.HasItem(Convert.ToInt32(ItemId.Iceborn_Gauntlet)) && (Items.CanUseItem(Convert.ToInt32(ItemId.Iceborn_Gauntlet)) && Player.HasBuff("itemfrozenfist", true)))
                return Dmg = Damage.CalcDamage(Player, target, Damage.DamageType.Physical, Player_bAD * 1.25f);

            if (Items.HasItem(Convert.ToInt32(ItemId.Trinity_Force)) && (Items.CanUseItem(Convert.ToInt32(ItemId.Trinity_Force)) && Player.HasBuff("sheen", true)))
                return Dmg = Damage.CalcDamage(Player, target, Damage.DamageType.Physical, Player_bAD * 2f);

            if (Player.HasBuff("lichbane", true))
                return Dmg = Damage.CalcDamage(Player, target, Damage.DamageType.Magical, Player_bAD * 0.75f + Player_totalAP * 0.50f);

            return Dmg;
        }

        #endregion

        #region Render Circle
        public static void CircleRendering(GameObject target, float Radius,string coloritem, int tickness = 1)
        {
            var menu = championMenu.Item(coloritem, true);
            Render.Circle Rendering = null;

            Rendering = new Render.Circle(target, Radius, menu.GetValue<Circle>().Color, tickness)
            {
                VisibleCondition = c => menu.GetValue<Circle>().Active,
            };
            
            
            
            Rendering.Add();
            RenderCircleList.Add(new RenderInfo()
            {
                _Circle = Rendering,
                ColorItem = menu
            });
        }
        #endregion

        #region GetBoolFromMenu
        public static bool GetBoolFromMenu(Menu rootmenu, string ID, string submenu = null, bool MadeChampionuniq = false)
        {
            if (submenu == null)
                return rootmenu.Item(ID, MadeChampionuniq).GetValue<bool>();
            else
                return rootmenu.SubMenu(submenu).Item(ID, MadeChampionuniq).GetValue<bool>();
        }
        public static bool GetBoolFromMenu(Spell Spell, bool IsCombo = false, bool IsHarass = false)
        {
            SpellSlot[] Support = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

            if (IsCombo == true && IsHarass == true)
            {
                Game.PrintChat("<font color='#FF0000'>[GetBoolFromMenu]:</font> Error! Don't use IsCombo with IsHarass Returened False");
                return false;
            }
            if (!Support.Contains(Spell.Slot))
            {
                Game.PrintChat("<font color='#FF0000'>[GetBoolFromMenu]:</font> Error! SpellSlot is wrong type");
                return false;
            }

            
            if (IsCombo)
                return championMenu.SubMenu("Combo").Item("combo_" + Spell.Slot.ToString(),true).GetValue<bool>();
            if (IsHarass)
                return championMenu.SubMenu("Harass").Item("harass_" + Spell.Slot.ToString(), true).GetValue<bool>();

            Game.PrintChat("<font color='#FF0000'>[GetBoolFromMenu]:</font> Error! You must use IsCombo or IsHarass");
            return false;
        }
        #endregion GetBoolFromMenu

        #endregion methods
    }
}
