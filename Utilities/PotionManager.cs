using LeagueSharp;
using LeagueSharp.Common;
using System;

namespace Kor_AIO.Utilities
{
    internal class PotionManager
    {
        private static Menu _menu;

        public void Load(Menu config)
        {
            config.AddItem(new MenuItem("useHP", "Use Health Pot").SetValue(true));
            config.AddItem(new MenuItem("useHPPercent", "Health %").SetValue(new Slider(35, 1)));
            config.AddItem(new MenuItem("useMP", "Use Mana Pot").SetValue(true));
            config.AddItem(new MenuItem("useMPPercent", "Mana %").SetValue(new Slider(35, 1)));

            _menu = config;

            Game.OnGameUpdate += GameOnOnGameUpdate;
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            var useHp = _menu.Item("useHP").GetValue<bool>();
            var useMp = _menu.Item("useMP").GetValue<bool>();

            // CrystalFlask => 2041 
            // Biscuit => 2010
            // HPPot => 2003
            // MPPot => 2004

            if (!ObjectManager.Player.IsDead)
            {
                if (useHp && ObjectManager.Player.HealthPercentage() <= _menu.Item("useHPPercent").GetValue<Slider>().Value && !IsUsingHpPot())
                {
                    if (Items.HasItem(2041) && Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }
                    else if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }
                    else if (Items.HasItem(2003) && Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }
                }

                if (useMp && ObjectManager.Player.ManaPercentage() <= _menu.Item("useMPPercent").GetValue<Slider>().Value && !IsUsingManaPot())
                {
                    if (Items.HasItem(2004) && Items.CanUseItem(2004))
                    {
                        Items.UseItem(2004);
                    }
                }
            }
        }

        private static bool IsUsingHpPot()
        {
            return ObjectManager.Player.HasBuff("RegenerationPotion", true) ||
                   ObjectManager.Player.HasBuff("ItemCrystalFlask", true) ||
                   ObjectManager.Player.HasBuff("ItemMiniRegenPotion", true);
        }

        private static bool IsUsingManaPot()
        {
            return ObjectManager.Player.HasBuff("FlaskOfCrystalWater", true) ||
                   ObjectManager.Player.HasBuff("ItemCrystalFlask", true);
        }
    }
}