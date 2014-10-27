using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LX_Orbwalker;


namespace Lets_be_friends_forever
{
    internal class Program
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static SpellDataInst smiteSlot;
        public static Int32 lastSkinId = 0;
        public static Items.Item rand, lotis;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
           if (ObjectManager.Player.ChampionName != "Amumu") return;

            Q = new Spell(SpellSlot.Q, 1100);
            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);


            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 550);
            smiteSlot = Player.SummonerSpellbook.GetSpell(Player.GetSpellSlot("summonersmite"));
            rand = new Items.Item(3143, 490f);
            lotis = new Items.Item(3190, 590f);

            //Make the menu
            Config = new Menu("Amumu", "Friend Forever", true);

            //Lxorbwalker
            var orbwalkerMenu = new Menu("Orbwalker", "LX_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);

            //Add the targer selector to the menu.
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Combo menu
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo1", "R if enemys>=").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("qHit", "Q HitChance").SetValue(new Slider(3, 1, 4)));

            //LaneClear menu
            Config.AddSubMenu(new Menu("LaneClear/Jungle Clear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseWLaneClear", "Use W").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseELaneClear", "Use E").SetValue(true));

            //Misc Menu
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("packet", "Use Packets").SetValue(true));

            //SummonerSpell Menu
            Config.AddSubMenu(new Menu("SummonerSpell", "SummonerSpell"));
            Config.SubMenu("SummonerSpell").AddItem(new MenuItem("usesmite", "Use Smite(Toggle)").SetValue(new KeyBind("N".ToCharArray()[0],
        KeyBindType.Toggle)));


            //Item
            Config.AddSubMenu(new Menu("items", "items"));
            Config.SubMenu("items").AddSubMenu(new Menu("Deffensive", "Deffensive"));
            Config.SubMenu("items")
               .SubMenu("Deffensive")
               .AddItem(new MenuItem("Omen", "Use Randuin Omen"))
               .SetValue(true);
            Config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("Omenenemys", "Randuin if enemys>").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotis", "Use Iron Solari"))
                .SetValue(true);
            Config.SubMenu("items")
                .SubMenu("Deffensive")
                .AddItem(new MenuItem("lotisminhp", "Solari if Ally Hp<").SetValue(new Slider(35, 1, 100)));
            
            //Skin Changer
            Config.AddSubMenu(new Menu("Skin Changer", "SkinChanger"));
            Config.SubMenu("SkinChanger").AddItem(new MenuItem("skin", "Use Custom Skin").SetValue(true));
            Config.SubMenu("SkinChanger").AddItem(new MenuItem("skin1", "Skin Changer").SetValue(new Slider(4, 1, 8)));

            //DrawEmenu
            Config.AddSubMenu(new Menu("Draw", "DrawSettings"));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawQ", "Q Range").SetValue(false));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawW", "W Range").SetValue(false));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawE", "E Range").SetValue(false));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem("DrawR", "R Range").SetValue(false));

            Config.AddToMainMenu();

            if (Config.Item("skin").GetValue<bool>())
            {
                Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(Player.NetworkId, Config.Item("skin1").GetValue<Slider>().Value, Player.ChampionName)).Process();
                lastSkinId = Config.Item("skin1").GetValue<Slider>().Value;
            }

            Game.PrintChat("AlienHack [Amumu - Let's Be Friend Forever] Loaded!");
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static bool Packets()
        {
            return Config.Item("packet").GetValue<bool>();
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (LXOrbwalker.CurrentMode == LXOrbwalker.Mode.LaneClear)
            {
                LaneClear();
            }
            if (LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
            {
                Combo();
            }

            if (Config.Item("usesmite").GetValue<KeyBind>().Active)
            {
                Smite();
            }

            if (Config.Item("skin").GetValue<bool>() && Config.Item("skin1").GetValue<Slider>().Value != lastSkinId)
            {
                Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(Player.NetworkId, Config.Item("skin1").GetValue<Slider>().Value, Player.ChampionName)).Process();
                lastSkinId = Config.Item("skin1").GetValue<Slider>().Value;
            }

          
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Config.Item("DrawQ").GetValue<bool>() && Q.Level > 0) Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Red);
            if (Config.Item("DrawW").GetValue<bool>() && W.Level > 0) Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Red);
            if (Config.Item("DrawE").GetValue<bool>() && E.Level > 0) Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Red);
            if (Config.Item("DrawR").GetValue<bool>() && R.Level > 0) Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Red);
        }

        static void Combo() 
        {
            var target = SimpleTs.GetTarget(Q.Range , SimpleTs.DamageType.Magical);

            var rCount =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        en =>
                            en.Team != Player.Team && en.IsValid && !en.IsDead &&
                            en.Distance(Player.Position) < R.Range);

            if (target == null)
            {
                if (W.Instance.ToggleState == 2)
                {
                    W.Cast();
                }
                return;
            }

            var qPred = Q.GetPrediction(target);

            if (Q.GetPrediction(target).Hitchance >= getHit() && Player.Distance(target) < Q.Range && Config.Item("UseQCombo").GetValue<bool>() && Q.IsReady())
            {
                Q.Cast(target, Packets());
            }

            if (W.IsReady() && W.Instance.ToggleState == 1 && Player.Distance(target) < 300 && Config.Item("UseWCombo").GetValue<bool>())
            {
                W.Cast();
            }

            if (E.IsReady() && Config.Item("UseECombo").GetValue<bool>() && Player.Distance(target) < E.Range)
            {
                E.Cast();
            }

            if (rCount.Count() >= Config.Item("UseRCombo1").GetValue<Slider>().Value)
            {
                if (R.IsReady() && Config.Item("UseRCombo").GetValue<bool>() )
                {
                    R.Cast();
                }
            }

            UseItemes(target);

            
        }

        static void LaneClear()
        {
                        var minion = MinionManager.GetMinions(Player.ServerPosition, W.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);




                        if (minion.Count > 0)
                        {
                            var minions = minion[0];

                            if (Config.Item("UseWLaneClear").GetValue<bool>() && W.IsReady() && W.Instance.ToggleState == 1 && minions.IsValidTarget(W.Range) )
                            {
                                W.Cast();
                            }
                            if (Config.Item("UseELaneClear").GetValue<bool>() && E.IsReady() && minions.IsValidTarget(E.Range))
                            {
                                E.Cast();
                            }
                        }
                        else
                        {
                            if ( W.Instance.ToggleState != 1)
                            {
                                W.Cast();
                            }
                        }


        }

        static void Smite()
        {
            string[] jungleMinions;
            if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[] { "AncientGolem", "LizardElder", "Worm", "Dragon" };
            }

            var minions = MinionManager.GetMinions(Player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Count() > 0)
            {
                int smiteDmg = getSmiteDmg();
                foreach (Obj_AI_Base minion in minions)
                {

                    Boolean b;
                    if (Utility.Map.GetMap()._MapType.Equals(Utility.Map.MapType.TwistedTreeline))
                    {
                        b = minion.Health <= smiteDmg &&
                            jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name));
                    }
                    else
                    {
                        b = minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name));
                    }

                    if (b)
                    {
                        Player.SummonerSpellbook.CastSpell(smiteSlot.Slot, minion);
                    }
                }
            }
        }

        private static int getSmiteDmg()
        {
            int level = Player.Level;
            int index = Player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        private static void UseItemes(Obj_AI_Hero target)
        {
            var iOmen = Config.Item("Omen").GetValue<bool>();
            var iOmenenemys = ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(450)) >=
                              Config.Item("Omenenemys").GetValue<Slider>().Value;
            var ilotis = Config.Item("lotis").GetValue<bool>();
            //var ihp = Config.Item("Hppotion").GetValue<bool>();
            // var ihpuse = Player.Health <= (Player.MaxHealth * (Config.Item("Hppotionuse").GetValue<Slider>().Value) / 100);
            //var imp = Config.Item("Mppotion").GetValue<bool>();
            //var impuse = Player.Health <= (Player.MaxHealth * (Config.Item("Mppotionuse").GetValue<Slider>().Value) / 100);

            
            if (iOmenenemys && iOmen && rand.IsReady())
            {
                rand.Cast();

            }
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (Config.Item("lotisminhp").GetValue<Slider>().Value) / 100) &&
                        hero.Distance(Player.ServerPosition) <= lotis.Range && lotis.IsReady())
                        lotis.Cast();
                }
            }
        }

        public static HitChance getHit()
        {
            var hitC = HitChance.High;
            var qHit = Config.Item("qHit").GetValue<Slider>().Value;

            // HitChance.Low = 3, Medium , High .... etc..
                switch (qHit)
                {
                    case 1:
                        hitC = HitChance.Low;
                        break;
                    case 2:
                        hitC = HitChance.Medium;
                        break;
                    case 3:
                        hitC = HitChance.High;
                        break;
                    case 4:
                        hitC = HitChance.VeryHigh;
                        break;
                }

            return hitC;
        }
    }
}
