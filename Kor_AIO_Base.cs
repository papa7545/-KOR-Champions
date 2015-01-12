using LeagueSharp;
using LeagueSharp.Common;
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



        public Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return Orbwalker.ActiveMode; }
        }

        public Kor_AIO_Base()
        {
            ConfigManager.LoadMenu();

            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            ConfigManager.baseMenu.AddSubMenu(orbwalkMenu);

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


        public virtual void Game_OnGameUpdate(EventArgs args)
        {
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
    }
}
