﻿using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kor_AIO
{
    internal class Kor_AIO_Base
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Obj_AI_Hero SelectedTarget = null;
        public static Orbwalking.Orbwalker Orbwalker;
        public List<Spell> SpellList = new List<Spell>();

        public static Spell P, Q, Q2, QCharged, W, W2, E, E2, R, R2;

        public static Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return Orbwalker.ActiveMode; }
        }

        public Kor_AIO_Base()
        {
            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            ConfigManager.championMenu.AddSubMenu(orbwalkMenu);

            ConfigManager.championMenu.SubMenu("Orbwalker").SubMenu("Misc").AddItem(new MenuItem("disMovement", "Disable Movement", true)).SetValue(false);
            ConfigManager.championMenu.SubMenu("Orbwalker").SubMenu("Misc").AddItem(new MenuItem("disAttack", "Disable Attack", true)).SetValue(false);
            ConfigManager.LoadMenu();

            
            Game.OnGameUpdate += Orbwalker_Setting;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnGameSendPacket += Game_OnSendPacket;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
        }


        #region Virtual Event
        public virtual void Game_OnGameUpdate(EventArgs args)
        {
        }

        public virtual void Orbwalker_Setting(EventArgs args)
        {
            if (ConfigManager.championMenu.SubMenu("Orbwalker").SubMenu("Misc").Item("disMovement", true).GetValue<bool>())
            {
                Orbwalker.SetMovement(false);
            }
            else
            {
                Orbwalker.SetMovement(true);
            }

            if (ConfigManager.championMenu.SubMenu("Orbwalker").SubMenu("Misc").Item("disAttack", true).GetValue<bool>())
            {
                Orbwalker.SetAttack(false);
            }
            else
            {
                Orbwalker.SetAttack(true);
            }
        }

        public virtual void Drawing_OnDraw(EventArgs args)
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
        #endregion

        #region Cast
        public static void Cast(Spell spell, Obj_AI_Base target, HitChance hitChance = HitChance.VeryHigh, bool aoe = false)
        {
            if (spell.IsReady())
            {
                spell.Cast(target, false, aoe);
            }
        }
        public static void Cast(Spell spell, Vector3 position, HitChance hitChance = HitChance.VeryHigh, bool aoe = false)
        {
            if (spell.IsReady())
            {
                spell.Cast(position);
            }
        }

        public static void Cast(Spell spell, TargetSelector.DamageType damageType)
        {
            if (spell.IsReady())
            {
                Obj_AI_Hero target = null;
                if (Player.ChampionName == "TwistedFate" && spell.Slot == SpellSlot.Q)
                    target = TargetSelector.GetTarget(1200, damageType);
                else
                    target = TargetSelector.GetTarget(spell.Range, damageType);

                if (target == null) return;

                spell.Cast(target);
            }
        }

        public static void Cast(Spell spell)
        {
            if (spell.IsReady())
                spell.Cast();
        }
        #endregion

        #region LiastHit

        public static void Lasthit_Spell(Spell LastHitSpell, bool boolean = true, Obj_AI_Minion target = null)
        {
            if (target == null)
            {
                var Target = ObjectManager.Get<Obj_AI_Minion>().Where(t => t.Health + 5 < LastHitSpell.GetDamage(t)
                    && t.Distance(Player.Position) <= LastHitSpell.Range && !t.IsDead && t.IsEnemy);

                if (Target.Any() && boolean)
                    Cast(LastHitSpell, Target.First());
            }
            else
            {
                if (boolean && target.Health + 5 < LastHitSpell.GetDamage(target))
                    Cast(LastHitSpell, target);
            }
        }

        private static double GetDmgWithItem(Obj_AI_Base target)
        {
            double Dmg = 0;
            Dmg = Damage.GetAutoAttackDamage(Player, target);

            if (Items.HasItem(Convert.ToInt32(ItemId.Iceborn_Gauntlet)) && (Items.CanUseItem(Convert.ToInt32(ItemId.Iceborn_Gauntlet)) || Player.HasBuff("sheen", true)))
                Dmg = Damage.GetAutoAttackDamage(Player, target);

            if (Items.HasItem(Convert.ToInt32(ItemId.Iceborn_Gauntlet)) && (Items.CanUseItem(Convert.ToInt32(ItemId.Iceborn_Gauntlet)) || Player.HasBuff("itemfrozenfist", true)))
                Dmg = Damage.GetAutoAttackDamage(Player, target) * 1.25f;

            if (Items.HasItem(Convert.ToInt32(ItemId.Trinity_Force)) && (Items.CanUseItem(Convert.ToInt32(ItemId.Trinity_Force)) || Player.HasBuff("sheen", true)))
                Dmg = Damage.GetAutoAttackDamage(Player, target) * 2.00f;

            return Dmg;
        }

        #endregion
    }
}
