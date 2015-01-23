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

namespace Kor_AIO.Utilities
{
    public static class Extention
    {
        public static bool getMenuBool(this Menu _menu, string id, bool madechampionUniq = false)
        {
            return _menu.Item(id, madechampionUniq).GetValue<bool>();
        }
        public static int getMenuValue(this Menu _menu, string id, bool madechampionUniq = false)
        {
            return _menu.Item(id, madechampionUniq).GetValue<Slider>().Value;
        }
    }

    internal class ItemManager
    {
        public static Menu _menu;
        private static Obj_AI_Hero Player = ObjectManager.Player;

        public void Load(Menu config)
        {

            config.AddItem(new MenuItem("useitem", "Active").SetValue(true));

            var item_zhonya = new Menu("Zhonya", "Zhonya");
            config.AddSubMenu(item_zhonya);
            item_zhonya.AddItem(new MenuItem("useitem_zhonya", "UseZhonya",true).SetValue(true));
            item_zhonya.AddItem(new MenuItem("useitem_p_zhonya", "Use On Hp(%)").SetValue(new Slider(15, 0, 100)));

            var item_seraph = new Menu("Seraph", "Seraph");
            config.AddSubMenu(item_seraph);
            item_seraph.AddItem(new MenuItem("useitem_seraph", "UseSeraph",true).SetValue(true));
            item_seraph.AddItem(new MenuItem("useitem_p_seraph", "Use On Hp(%)").SetValue(new Slider(20, 0, 100)));

            var item_Bilgewater = new Menu("Bilgewater", "Bilgewater");
            config.AddSubMenu(item_Bilgewater);
            item_Bilgewater.AddItem(new MenuItem("useitem_bilgewater", "UseBilgewater",true).SetValue(true));
            item_Bilgewater.AddItem(new MenuItem("useitem_p_bilgewater", "Use On Hp(%)").SetValue(new Slider(20, 0, 100)));
            item_Bilgewater.AddItem(new MenuItem("useitem_bilgewater_atg", "Anti-Gap-Closer").SetValue(true));
            item_Bilgewater.AddItem(new MenuItem("useitem_bilgewater_atg_p", "Gap :").SetValue(new Slider(250, 100, 450)));

            var item_botrk = new Menu("BOTRK", "BOTRK");
            config.AddSubMenu(item_botrk);
            item_botrk.AddItem(new MenuItem("useitem_botrk", "UseBOTRK",true).SetValue(true));
            item_botrk.AddItem(new MenuItem("useitem_p_botrk", "Use On Hp(%)").SetValue(new Slider(20, 0, 100)));
            item_botrk.AddItem(new MenuItem("useitem_botrk_atg", "Anti-Gap-Closer").SetValue(true));
            item_botrk.AddItem(new MenuItem("useitem_botrk_atg_p", "Gap :").SetValue(new Slider(250, 100, 450)));


            var item_mikaels = new Menu("Mikaels", "Mikaels");
            config.AddSubMenu(item_mikaels);
            item_mikaels.AddItem(new MenuItem("useitem_mikaels", "ON?",true).SetValue(true));
            item_mikaels.AddItem(new MenuItem("useitem_p_mikaels", "Use On Hp(%)").SetValue(new Slider(15, 0, 100)));
            item_mikaels.AddItem(new MenuItem("useitem_p_mikaels_delay", "Mikaels Delay(ms)").SetValue(new Slider(100, 0, 1000)));

            #region mikaels_cc
            var menu_mikaels_cc = new Menu("mikael_cc", "Use On CC");
            item_mikaels.AddSubMenu(menu_mikaels_cc);
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_bool", "On CC?").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_stun", "Stun").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_fear", "Fear(Flee)").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_charm", "Charm").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_taunt", "Taunt").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_snare", "Snare").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_silence", "Silence").SetValue(false));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_polymorph", "Polymorph").SetValue(true));
            #endregion


            var item_qs = new Menu("QuickSilver", "QuickSilver");
            config.AddSubMenu(item_qs);
            item_qs.AddItem(new MenuItem("useitem_qs_bool", "UseQS",true).SetValue(true));
            item_qs.AddItem(new MenuItem("useitem_p_qs_delay", "QuickSilver Delay(ms)").SetValue(new Slider(100, 0, 1000)));
            #region qs_cc
            var menu_quicksilver_cc = new Menu("Use On CC", "Use On CC");
            item_qs.AddSubMenu(menu_quicksilver_cc);
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_stun", "Stun").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_fear", "Fear(Flee)").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_charm", "Charm").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_taunt", "Taunt").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_snare", "Snare").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_silence", "Silence").SetValue(false));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_polymorph", "Polymorph").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_suppression", "Suppression").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_zedult", "ZedUlt").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_fizzult", "FizzUlt").SetValue(true));
            #endregion


            _menu = config;

            Game.OnGameUpdate += GameOnGameUpdate;
        }

        private static void GameOnGameUpdate(EventArgs args)
        {
            if (!Player.InShop() && _menu.getMenuBool("useitem"))
            {
                int tempItemid = 3157;
                if (_menu.getMenuBool("useitem_zhonya",true) && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
                {
                    foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Zhonyas_Hourglass))
                    {
                        if (Player.HealthPercentage() <= (float)_menu.getMenuValue("useitem_p_zhonya")
                            && !Player.Buffs.Any(buff => buff.DisplayName == "Chrono Shift"))
                            Player.Spellbook.CastSpell(p_item.SpellSlot);
                    }
                }

                tempItemid = 3040;
                if (_menu.getMenuBool("useitem_seraph",true) && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
                {

                    foreach (var p_item in Player.InventoryItems.Where(item => Convert.ToInt32(item.Id) == 3040))
                    {
                        if (Player.HealthPercentage() <= (float)_menu.getMenuValue("useitem_p_seraph")
                            && !Player.Buffs.Any(buff => buff.DisplayName == "Chrono Shift"))
                            Player.Spellbook.CastSpell(p_item.SpellSlot);
                    }
                }

                tempItemid = Convert.ToInt32(ItemId.Bilgewater_Cutlass);
                if (_menu.getMenuBool("useitem_bilgewater",true) && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
                {
                    Obj_AI_Hero target = null;
                    foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Bilgewater_Cutlass))
                    {
                        if (ObjectManager.Get<Obj_AI_Hero>().Any(h => h.IsEnemy && !h.IsDead && h.IsVisible &&
                            Vector3.Distance(h.Position, Player.Position) <= _menu.getMenuValue("useitem_bilgewater_atg_p")))
                        {
                            target = ObjectManager.Get<Obj_AI_Hero>().First(h => h.IsEnemy && !h.IsDead && h.IsVisible &&
                            Vector3.Distance(h.Position, Player.Position) <= _menu.getMenuValue("useitem_bilgewater_atg_p"));
                            Player.Spellbook.CastSpell(p_item.SpellSlot, target);
                        }

                        if (Player.HealthPercentage() <= (float)_menu.getMenuValue("useitem_p_bilgewater"))
                        {
                            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy && h.IsValid && h.IsVisible && Vector3.Distance(h.Position, Player.Position) <= 450))
                            {
                                Player.Spellbook.CastSpell(p_item.SpellSlot, hero);
                            }
                        }
                    }
                }

                tempItemid = Convert.ToInt32(ItemId.Blade_of_the_Ruined_King);
                if (_menu.getMenuBool("useitem_botrk", true) && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
                {
                    Obj_AI_Hero target = null;
                    Double max_healpoint = 0;

                    foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Blade_of_the_Ruined_King))
                    {
                        if (_menu.getMenuBool("useitem_botrk_atg"))
                        {
                            if (ObjectManager.Get<Obj_AI_Hero>().Any(h => h.IsEnemy && !h.IsDead && h.IsVisible &&
                                Vector3.Distance(h.Position, Player.Position) <= _menu.getMenuValue("useitem_botrk_atg_p")))
                            {
                                target = ObjectManager.Get<Obj_AI_Hero>().First(h => h.IsEnemy && !h.IsDead && h.IsVisible &&
                                Vector3.Distance(h.Position, Player.Position) <= _menu.getMenuValue("useitem_botrk_atg_p"));
                                Player.Spellbook.CastSpell(p_item.SpellSlot, target);
                            }
                        }

                        if (Player.HealthPercentage() <= (float)_menu.getMenuValue("useitem_p_botrk"))
                        {
                            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy && h.IsValid && h.IsVisible && Vector3.Distance(h.Position, Player.Position) <= 450))
                            {
                                var healpoint = Player.CalcDamage(hero, Damage.DamageType.Physical, hero.MaxHealth * 0.1);
                                if (max_healpoint < healpoint)
                                {
                                    max_healpoint = healpoint;
                                    target = hero;
                                }
                            }
                            Player.Spellbook.CastSpell(p_item.SpellSlot, target);
                        }
                    }
                }

                tempItemid = Convert.ToInt32(ItemId.Mikaels_Crucible);
                if (_menu.getMenuBool("useitem_mikaels", true) && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
                {
                    List<BuffType> bufflist = new List<BuffType>();
                    getbufflist(bufflist, ItemId.Mikaels_Crucible);
                    foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Mikaels_Crucible))
                    {
                        foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly && !h.IsMe && h.IsValid && h.IsVisible && Vector3.Distance(h.Position, Player.Position) <= 800))
                        {
                            if (hero.HealthPercentage() <= (float)_menu.getMenuValue("useitem_p_mikaels"))
                                Player.Spellbook.CastSpell(p_item.SpellSlot, hero);

                            if (_menu.getMenuBool("mikaels_cc_bool"))
                            {
                                foreach (var buff in hero.Buffs)
                                {
                                    if (bufflist.Any(b => b == buff.Type))
                                        Utility.DelayAction.Add(_menu.getMenuValue("useitem_p_mikaels_delay"), () => { Player.Spellbook.CastSpell(p_item.SpellSlot, hero); });
                                }
                            }
                        }
                    }
                }
                tempItemid = Convert.ToInt32(ItemId.Quicksilver_Sash);
                int tempItemid2 = Convert.ToInt32(ItemId.Mercurial_Scimitar);
                if (_menu.getMenuBool("useitem_qs_bool",true))
                {
                    if ((Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid)) || ((Items.HasItem(tempItemid2) && Items.CanUseItem(tempItemid2))))
                    {
                        List<BuffType> bufflist = new List<BuffType>();
                        getbufflist(bufflist, ItemId.Quicksilver_Sash);
                        foreach (var p_item in Player.InventoryItems.Where(item => (item.Id == ItemId.Quicksilver_Sash || item.Id == ItemId.Mercurial_Scimitar)))
                        {
                            foreach (var buff in Player.Buffs)
                            {
                                Utility.DelayAction.Add(_menu.getMenuValue("useitem_p_qs_delay"), () =>
                                {
                                    if (bufflist.Any(b => b == buff.Type))
                                        Player.Spellbook.CastSpell(p_item.SpellSlot);
                                    if (buff.DisplayName == "ZedUltExecute" && _menu.getMenuBool("qs_cc_zedult"))
                                        Player.Spellbook.CastSpell(p_item.SpellSlot);
                                    if (buff.DisplayName == "FizzChurnTheWatersCling" && _menu.getMenuBool("qs_cc_fizzult"))
                                        Player.Spellbook.CastSpell(p_item.SpellSlot);
                                });
                            }
                        }
                    }
                }
            }
        }

        public static void getbufflist(List<BuffType> list, ItemId whatitem)
        {
            if (whatitem == ItemId.Mikaels_Crucible)
            {
                if (_menu.getMenuBool("mikaels_cc_stun"))
                    list.Add(BuffType.Stun);
                if (_menu.getMenuBool("mikaels_cc_fear"))
                {
                    list.Add(BuffType.Fear);
                    list.Add(BuffType.Flee);
                }
                if (_menu.getMenuBool("mikaels_cc_charm"))
                    list.Add(BuffType.Charm);
                if (_menu.getMenuBool("mikaels_cc_taunt"))
                    list.Add(BuffType.Taunt);
                if (_menu.getMenuBool("mikaels_cc_snare"))
                    list.Add(BuffType.Snare);
                if (_menu.getMenuBool("mikaels_cc_silence"))
                    list.Add(BuffType.Silence);
                if (_menu.getMenuBool("mikaels_cc_polymorph"))
                    list.Add(BuffType.Polymorph);
            }
            if (whatitem == ItemId.Quicksilver_Sash)
            {
                if (_menu.getMenuBool("qs_cc_stun"))
                    list.Add(BuffType.Stun);
                if (_menu.getMenuBool("qs_cc_fear"))
                {
                    list.Add(BuffType.Fear);
                    list.Add(BuffType.Flee);
                }
                if (_menu.getMenuBool("qs_cc_charm"))
                    list.Add(BuffType.Charm);
                if (_menu.getMenuBool("qs_cc_taunt"))
                    list.Add(BuffType.Taunt);
                if (_menu.getMenuBool("qs_cc_snare"))
                    list.Add(BuffType.Snare);
                if (_menu.getMenuBool("qs_cc_silence"))
                    list.Add(BuffType.Silence);
                if (_menu.getMenuBool("qs_cc_polymorph"))
                    list.Add(BuffType.Polymorph);
                if (_menu.getMenuBool("qs_cc_suppression"))
                    list.Add(BuffType.Suppression);
            }

        }
    }
}