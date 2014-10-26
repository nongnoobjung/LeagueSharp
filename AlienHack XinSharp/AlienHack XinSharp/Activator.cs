using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace AlienHack_XinSharp
{
    class Activator
    {

        public static Items.Item tiamat, hydra, blade, bilge, rand, lotis;
        public static Obj_AI_Hero Player = ObjectManager.Player;


        public static void Game_OnGameLoad(EventArgs args)
        {
            bilge = new Items.Item(3144, 475f);
            blade = new Items.Item(3153, 425f);
            hydra = new Items.Item(3074, 375f);
            tiamat = new Items.Item(3077, 375f);
            rand = new Items.Item(3143, 490f);
            lotis = new Items.Item(3190, 590f);

        }


        public static void addmenu(Menu menu)
        {

            var tempMenu = menu;

             //Items public static Int32 Tiamat = 3077, Hydra = 3074, Blade = 3153, Bilge = 3144, Rand = 3143, lotis = 3190;
            tempMenu.AddSubMenu(new Menu("items", "items"));
            tempMenu.SubMenu("items").AddSubMenu(new Menu("Offensive", "Offensive"));
            tempMenu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Tiamat", "Use Tiamat")).SetValue(true);
            tempMenu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Hydra", "Use Hydra")).SetValue(true);
            tempMenu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "Use Bilge")).SetValue(true);
            tempMenu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BilgeEnemyhp", "If Enemy Hp <").SetValue(new Slider(85, 1, 100)));
            tempMenu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Bilgemyhp", "Or your Hp < ").SetValue(new Slider(85, 1, 100)));
            tempMenu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "Use Blade")).SetValue(true);
            tempMenu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BladeEnemyhp", "If Enemy Hp <").SetValue(new Slider(85, 1, 100)));
            tempMenu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Blademyhp", "Or Your  Hp <").SetValue(new Slider(85, 1, 100)));
            tempMenu.SubMenu("items").AddSubMenu(new Menu("Deffensive", "Deffensive"));
            tempMenu.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omen", "Use Randuin Omen"))
                .SetValue(true);
            tempMenu.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omenenemys", "Randuin if enemys>").SetValue(new Slider(2, 1, 5)));
            tempMenu.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotis", "Use Iron Solari"))
                .SetValue(true);
            tempMenu.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotisminhp", "Solari if Ally Hp<").SetValue(new Slider(35, 1, 100)));

            Program.Config.AddSubMenu(tempMenu);

        }



        public static void UseItemes(Obj_AI_Hero target)
        {
            var iBilge = Program.Config.Item("Bilge").GetValue<bool>();
            var iBilgeEnemyhp = target.Health <=
                                (target.MaxHealth * (Program.Config.Item("BilgeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBilgemyhp = Player.Health <=
                             (Player.MaxHealth * (Program.Config.Item("Bilgemyhp").GetValue<Slider>().Value) / 100);
            var iBlade = Program.Config.Item("Blade").GetValue<bool>();
            var iBladeEnemyhp = target.Health <=
                                (target.MaxHealth * (Program.Config.Item("BladeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBlademyhp = Player.Health <=
                             (Player.MaxHealth * (Program.Config.Item("Blademyhp").GetValue<Slider>().Value) / 100);
            var iOmen = Program.Config.Item("Omen").GetValue<bool>();
            var iOmenenemys = ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(450)) >=
                              Program.Config.Item("Omenenemys").GetValue<Slider>().Value;
            var iTiamat = Program.Config.Item("Tiamat").GetValue<bool>();
            var iHydra = Program.Config.Item("Hydra").GetValue<bool>();
            var ilotis = Program.Config.Item("lotis").GetValue<bool>();


            if (Player.Distance(target) <= 450 && iBilge && (iBilgeEnemyhp || iBilgemyhp) && bilge.IsReady())
            {
                bilge.Cast(target);

            }
            if (Player.Distance(target) <= 450 && iBlade && (iBladeEnemyhp || iBlademyhp) && blade.IsReady())
            {
                blade.Cast(target);

            }
            if (Utility.CountEnemysInRange(350) >= 1 && iTiamat && tiamat.IsReady())
            {
                tiamat.Cast();

            }
            if (Utility.CountEnemysInRange(350) >= 1 && iHydra && hydra.IsReady())
            {
                hydra.Cast();

            }
            if (iOmenenemys && iOmen && rand.IsReady())
            {
                rand.Cast();

            }
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (Program.Config.Item("lotisminhp").GetValue<Slider>().Value) / 100) &&
                        hero.Distance(Player.ServerPosition) <= lotis.Range && lotis.IsReady())
                        lotis.Cast();
                }
            }
        }
    }
 }


