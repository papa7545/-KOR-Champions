using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kor_AIO.Champions
{
    class Akali : Kor_AIO_Base
    {
        public Akali()
        {

        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 800);

            Q.SetSkillshot(0.15f, 60, 1200, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            //R.SetSkillshot();
        }

        private void LoadMenu()
        {
            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("ComboUseQ", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseE", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboUseR", "Use R", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(combo);
            }

            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("HarassUseQ", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("HarassUseE", "Use E", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(harass);
            }

            //Harass menu:
            var laneclear = new Menu("Lane Clear", "LaneClear");
            {
                laneclear.AddItem(new MenuItem("LaneClearUseQ", "Use Q", true).SetValue(true));
                laneclear.AddItem(new MenuItem("LaneClearUseE", "Use E", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(laneclear);
            }

            //Harass menu:
            var jungleclear = new Menu("Jungle Clear", "JungleClear");
            {
                jungleclear.AddItem(new MenuItem("JungleClearUseQ", "Use Q", true).SetValue(true));
                jungleclear.AddItem(new MenuItem("JungleClearUseE", "Use E", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(jungleclear);
            }
            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("usePacket", "Use Packet", true).SetValue(true));
                ConfigManager.championMenu.AddSubMenu(misc);
            }
        }
    }
}
