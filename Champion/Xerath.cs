#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonChampions
{
    class Xerath:Program
    {
        /// <summary>
        /// Jeon Cassiopeia.
        /// Cassiopeia Posion Buff Name : CassiopeiaNoxiousBlast ,CassiopeiaMiasma
        /// </summary>
        public static Jproject_base Xeraths;
        public static float rTime;

        public static void _Xerath()
        {
            Xeraths = new Jproject_base();

            Q = new Spell(SpellSlot.Q, 1500);
            W = new Spell(SpellSlot.W, Jproject_base.GetSpellRange(Jproject_base.Wdata));
            E = new Spell(SpellSlot.E, Jproject_base.GetSpellRange(Jproject_base.Edata));
            R = new Spell(SpellSlot.R, Jproject_base.GetSpellRange(Jproject_base.Rdata));

            
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750-50, 1550, 1.5f);
            W.SetSkillshot(0.7f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 60, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Game.OnGameUpdate += OnGameUpdate;
        }

        public static void OnGameUpdate(EventArgs args)
        {

            if (Jproject_base.baseMenu.Item(m_Items.COMBO_KEY).GetValue<KeyBind>().Active)
                combo();

            if (Jproject_base.baseMenu.Item(m_Items.HARASS_KEY).GetValue<KeyBind>().Active)
                harass();
        }

        public static void harass()
        {
            Jproject_base.Cast(Q, TargetSelector.DamageType.Magical);
        }

        public static void combo()
        {
            if (Player.HasBuff("XerathR"))
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Any(t => t.Distance(Game.CursorPos) <= 100 && !t.IsDead && t.IsEnemy))
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().First(t => t.Distance(Game.CursorPos) <= 100 && !t.IsDead && t.IsEnemy);
                    if (Environment.TickCount - rTime >= 100)
                    {
                        Jproject_base.Cast(R, target);
                        rTime = Environment.TickCount;
                    }
                }
            }
            else
            {
                Jproject_base.Cast(Q, TargetSelector.DamageType.Magical);
                Jproject_base.Cast(W, TargetSelector.DamageType.Magical);
                Jproject_base.Cast(E, TargetSelector.DamageType.Magical);
            }

        }


        public static void lasthit()
        {
        
        }
    
    }
}
