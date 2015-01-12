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
#endregion
namespace JeonChampions
{
    class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static baseMenuItem m_Items = new baseMenuItem();
        public static Spell Q = new Spell(SpellSlot.Q, Jproject_base.GetSpellRange(Jproject_base.Qdata));
        public static Spell W = new Spell(SpellSlot.W, Jproject_base.GetSpellRange(Jproject_base.Wdata));
        public static Spell E = new Spell(SpellSlot.E, Jproject_base.GetSpellRange(Jproject_base.Edata));
        public static Spell R = new Spell(SpellSlot.R, Jproject_base.GetSpellRange(Jproject_base.Rdata));
        public static String[] SupportChamps = { "Cassiopea", "Xerath", "TwistedFate" };
        public class baseMenuItem
        {
            public string COMBO_ACTIVE = "combo_active";
            public string HARASS_ACTIVE = "harass_active";
            public string COMBO_KEY = "combo_key";
            public string HARASS_KEY = "harass_key";
        }




        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
        public static void OnLoad(EventArgs args)
        {
            if (SupportChamps.Contains(Player.ChampionName))
            {
                if (Player.ChampionName == "Cassiopea")
                    Cassiopea._Cassiopea();
                if (Player.ChampionName == "Xerath")
                    Xerath._Xerath();
                if (Player.ChampionName == "TwistedFate")
                    TwistedFate._TF();
            }
            else
                Game.PrintChat("Jeon : There are not support your Champion(" + Player.ChampionName + ")");
        }
    }
}
