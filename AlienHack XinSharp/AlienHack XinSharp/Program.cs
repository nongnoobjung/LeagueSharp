using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AlienHack_XinSharp
{
    class Program
    {
        private static Menu Config;

        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell QSpell;
        public static Spell WSpell;
        public static Spell ESpell;
        public static Spell RSpell;
        public static List<Spell> SpellList = new List<Spell>();
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static SpellSlot IgniteSlot; //= ObjectManager.Player.GetSpellSlot("SummonerDot");
        public static Items.Item Tiamat = new Items.Item(3077,400);
        public static Items.Item Hydra = new Items.Item(3074, 400);

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Xin Zhao") return;

            //Spells
            QSpell = new Spell(SpellSlot.Q ,375f);

            WSpell = new Spell(SpellSlot.W, 375f);
            
            ESpell = new Spell(SpellSlot.E, 600f);
            //ESpell.SetTargetted();

            RSpell = new Spell(SpellSlot.R, 375f);

            SpellList.Add(QSpell);
            SpellList.Add(WSpell);
            SpellList.Add(ESpell);
            SpellList.Add(RSpell);
            
            //Make the menu
            Config = new Menu("XinSharp", "The Dragon Talon", true);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Load the orbwalker and add it to the menu as submenu.
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //LaneClear
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "Use Q").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseWLaneClear", "Use W").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "Use E").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearActive", "LaneClear!").SetValue(new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Harass menu:
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "Use W").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Combo menu:
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoTiamat", "Auto Tiamat").SetValue(true));

            Config.AddToMainMenu();
            // end menu

            var ignite = Player.Spellbook.GetSpell(Player.GetSpellSlot("SummonerDot"));
            IgniteSlot = ignite.Slot;

            Game.PrintChat("AlienHack [XinSharp - The Dragon Talon] Loaded!");
            Game.OnGameUpdate += Game_OnGameUpdate;
            //Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();

                if (menuItem.Active)
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            //LaneClear
            if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
            {
                DoLaneClear();
            }

            //Harass
            if (Config.Item("HarassActive").GetValue<KeyBind>().Active)
            {
                DoHarass();
            }

            //Combo
            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                DoCombo();
            }

        }

      
        private static void DoCombo()
        {
            var target = SimpleTs.GetTarget(ESpell.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

                if (ESpell.IsReady() && Player.Distance(target) < ESpell.Range)
                    ESpell.CastOnUnit(target);

                if (Tiamat.IsReady() && Player.Distance(target) < Tiamat.Range)
                    Tiamat.Cast();

                if (Hydra.IsReady() && Player.Distance(target) < Hydra.Range)
                    Hydra.Cast();

                if (QSpell.IsReady() && Player.Distance(target) < QSpell.Range)
                    QSpell.Cast(Player);

                if (WSpell.IsReady() && Player.Distance(target) < WSpell.Range)
                    WSpell.Cast(Player);

                if (RSpell.IsReady() && Player.Distance(target) < RSpell.Range)
                    RSpell.Cast();

                /*if (IgniteSlot != SpellSlot.Unknown && Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)  // 
                    ObjectManager.Player.SummonerSpellbook.CastSpell(IgniteSlot, target);*/

          
        }

        private static void DoHarass()
        {
            var target = SimpleTs.GetTarget(ESpell.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

           
                if (ESpell.IsReady() && Player.Distance(target) < ESpell.Range)
                    ESpell.CastOnUnit(target);

                if (Tiamat.IsReady() && Player.Distance(target) < Tiamat.Range)
                    Tiamat.Cast();

                if (Hydra.IsReady() && Player.Distance(target) < Hydra.Range)
                    Hydra.Cast();

                if (QSpell.IsReady() && Player.Distance(target) < QSpell.Range)
                    QSpell.Cast(Player);

                if (WSpell.IsReady() && Player.Distance(target) < WSpell.Range)
                    WSpell.Cast(Player);

        }

        private static void DoLaneClear()
        {
            //Find All Minion
            var allMinions = MinionManager.GetMinions(Player.ServerPosition, ESpell.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var jungleMinions = MinionManager.GetMinions(Player.ServerPosition, ESpell.Range, MinionTypes.All, MinionTeam.Neutral);
            allMinions.AddRange(jungleMinions);

            //Q
            if (Config.Item("UseQLaneClear").GetValue<bool>() && QSpell.IsReady() && allMinions.Count > 0)
            {
                QSpell.Cast(Player);
            }

            //W
            if (Config.Item("UseWLaneClear").GetValue<bool>() && WSpell.IsReady() && allMinions.Count > 0)
            {
                WSpell.Cast(Player);
            }

            //E
            if (Config.Item("UseELaneClear").GetValue<bool>() && ESpell.IsReady() && allMinions.Count > 0)
            {
                foreach (var minion in allMinions.Where(minion => minion.IsValidTarget()))
                {
                    if(ESpell.IsReady())
                        ESpell.Cast(minion);
                }
            }
            if (Config.Item("AutoTiamat").GetValue<bool>() && Tiamat.IsReady() && allMinions.Count > 0)
            {
                foreach (var minion in allMinions.Where(minion => minion.IsValidTarget()))
                {
                    if (Tiamat.IsReady() && Tiamat.Range > Player.Distance(minion))
                        Tiamat.Cast();
                }

            }
            if (Config.Item("AutoTiamat").GetValue<bool>() && Hydra.IsReady() && allMinions.Count > 0)
            {
                foreach (var minion in allMinions.Where(minion => minion.IsValidTarget()))
                {
                    if (Hydra.IsReady() && Hydra.Range > Player.Distance(minion))
                        Hydra.Cast();
                }

            }


        }
    }
}
