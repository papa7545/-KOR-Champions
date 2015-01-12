using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kor_AIO
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            PrintChat("Loading...");

            var plugin = Type.GetType("Kor_AIO.Champions." + ObjectManager.Player.ChampionName);

            if (plugin == null)
            {
                PrintChat(ObjectManager.Player.ChampionName + " is not supported.");
                return;
            }

            Activator.CreateInstance(plugin);
        }

        public static void PrintChat(string msg)
        {
            Game.PrintChat("<font color='#3492EB'>[Kor AIO] : </font><font color='#FFFFFF'>" + msg + "</font>");
        }
    }
}