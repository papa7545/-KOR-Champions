using LeagueSharp;
using LeagueSharp.Common;
using System;

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

            PrintChat(ObjectManager.Player.ChampionName + " Loaded!");

            Activator.CreateInstance(plugin);
        }

        public static void PrintChat(string msg, bool Error = false,string ErrorMethod = "")
        {
            if (!Error)
                Game.PrintChat("<font color='#3492EB'>[Kor AIO] : </font><font color='#FFFFFF'>" + msg + "</font>");
            else
                Game.PrintChat("<font color='#FF0000'>[" + ErrorMethod + "]:</font> " + msg);
        }
    }
}