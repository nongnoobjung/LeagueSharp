using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LX_Orbwalker;
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

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Xin Zhao") return;

            //Spells
            QSpell = new Spell(SpellSlot.Q, 375);
            WSpell = new Spell(SpellSlot.W, 20);
            ESpell = new Spell(SpellSlot.E, 650);
            RSpell = new Spell(SpellSlot.R, 500);

            SpellList.Add(QSpell);
            SpellList.Add(WSpell);
            SpellList.Add(ESpell);
            SpellList.Add(RSpell);

            //Make the menu
            Config = new Menu("XinSharp", "The Dragon Talon", true);


            //Orbwalker submenu

            var orbwalkerMenu = new Menu("My Orbwalker", "LX_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);

            //Add the target selector to the menu as submenu.
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);


            //LaneClear
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear/Jungle Clear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQLaneClear", "Use Q").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseWLaneClear", "Use W").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "Use E").SetValue(true));


            //Combo menu:
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R Kill Secured").SetValue(true));


            Config.AddSubMenu(new Menu("Misc", "Misc"));
            //Config.SubMenu("Misc").AddItem(new MenuItem("AutoTiamat", "Auto Tiamat").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem("packet", "Use Packets").SetValue(true));

            Config.AddToMainMenu();
            // end menu


            Game.PrintChat("AlienHack [XinSharp - The Dragon Talon] Loaded!");
            Game.OnGameUpdate += Game_OnGameUpdate;
            //Drawing.OnDraw += Drawing_OnDraw;
        }

        public static bool packets()
        {
            return Config.Item("packet").GetValue<bool>();
        }

        static void Drawing_OnDraw(EventArgs args)
        {

        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            //LXorbwalk
            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.Combo:
                    Combo();
                    break;
                case LXOrbwalker.Mode.Flee:
                    Flee();
                    break;
                case LXOrbwalker.Mode.LaneClear:
                    LaneClear();
                    break;
            }

        }

        static void Combo()
        {
            var target = SimpleTs.GetTarget(1500, SimpleTs.DamageType.Physical);
            if (Config.Item("UseECombo").GetValue<bool>() && ESpell.IsReady() && target.IsValidTarget(ESpell.Range) && !LXOrbwalker.InAutoAttackRange(target)) 
            {
                ESpell.Cast(target, packets());
            }
            if (Config.Item("UseWCombo").GetValue<bool>() && WSpell.IsReady() && LXOrbwalker.InAutoAttackRange(target))
            {
                WSpell.Cast();
            }
            if (Config.Item("UseQCombo").GetValue<bool>() && QSpell.IsReady() && LXOrbwalker.InAutoAttackRange(target))
            {
                QSpell.Cast();  
            }
            if (Config.Item("UseRCombo").GetValue<bool>() && RSpell.IsReady() && RSpell.IsKillable(target) && target.IsValidTarget(RSpell.Range))
            {
                RSpell.Cast();
            }
        }

        static void LaneClear()
        {

        }

        static void Flee()
        {

        }
    }

}