using System;
using System.Security.Permissions;

namespace CarrySharp.Settings
{

    using Ensage.Common.Menu;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class MenuStructure
    {

        private static readonly StringList Attack = new StringList(new[] { "Never", "Standart", "Always" }, 1);
        private static readonly Slider BonusRange = new Slider(100, 100, 500);
        private static readonly Slider BonusWindUpSlider = new Slider(500, 100, 2000);

        private static Menu mainMenu;
        private static Menu orbwalkMenu;

        private static readonly Menu lasthitMenu = new Menu("LastHit", "LastHit");
        public static Menu Menu { get { return mainMenu; } }
        public static Menu OrbWalkMenu { get { return orbwalkMenu; } }

        public static Menu LastHitMenu { get { return lasthitMenu; } }


        internal class MenuVar
        {
            public static bool Aoc;
            public static int AutoAttackMode;
            public static int BonusWindUp;
            public static uint CombatKey;
            public static bool Denie;
            public static uint FarmKey;
            public static bool Harass;
            public static uint KiteKey;
            public static bool LastHitEnable;
            public static uint LastHitKey;
            public static int Outrange;
            public static bool Sapport;
            public static bool ShowHp;
            public static bool SummonsAoc;
            public static bool SummonsAutoFarm;
            public static bool SummonsAutoLasthit;
            public static bool SummonsDenie;
            public static bool SummonsEnable;
            public static bool SummonsHarass;
            public static bool Test;
            public static bool UseSpell;
        }

    
		public static void Load()
		{

			mainMenu = new Menu("CarrySharp", "menuu", true);
			orbwalkMenu = new Menu("Lasthit", "LastHit");

		    
            orbwalkMenu.AddItem(new MenuItem("enableLasthit", "Enable").SetValue(false));
            orbwalkMenu.AddItem(new MenuItem("autoAttackMode", "Auto Attack").SetValue(Attack));
            orbwalkMenu.AddItem(new MenuItem("bonuswindup", "Bonus WindUp time on kitting").SetValue(BonusWindUpSlider));
            orbwalkMenu.AddItem(new MenuItem("hpleftcreep", "Mark hp ?").SetValue(true));
            orbwalkMenu.AddItem(new MenuItem("sapp", "Support").SetValue(false));
            orbwalkMenu.AddItem(new MenuItem("usespell", "Use spell ?").SetValue(true));
            orbwalkMenu.AddItem(new MenuItem("harassheroes", "Harass in lasthit mode ?").SetValue(true));
            orbwalkMenu.AddItem(new MenuItem("denied", "Deny creep ?").SetValue(true));
            orbwalkMenu.AddItem(new MenuItem("AOC", "Atteck own creeps ?").SetValue(false));
            orbwalkMenu.AddItem(new MenuItem("test", "Test Attack_Calc").SetValue(false));
            orbwalkMenu.AddItem(new MenuItem("outrange", "Bonus range").SetValue(BonusRange));
            
            var subMenu = new Menu("Summons", "Summons", false);
            subMenu.AddItem(new MenuItem("enable", "Enable").SetValue(false).SetTooltip("Test!"));
            subMenu.AddItem(new MenuItem("harassheroes_sub", "Harass in lasthit mode ?").SetValue(false));
            subMenu.AddItem(new MenuItem("denied_sub", "Deny creep ?").SetValue(false));
            subMenu.AddItem(new MenuItem("AOC_sub", "Atteck own creeps ?").SetValue(false));
            subMenu.AddItem(new MenuItem("autoD", "Auto lasthit").SetValue(false).SetTooltip("Dont work properly!!!"));
            subMenu.AddItem(new MenuItem("autoF", "Auto farm").SetValue(false).SetTooltip("Dont work properly!!!"));
            orbwalkMenu.AddSubMenu(subMenu);
            
            subMenu = new Menu("Keys", " Keys", false);
            subMenu.AddItem(new MenuItem("combatkey", "Chase mode").SetValue(new KeyBind(32, KeyBindType.Press)));
            subMenu.AddItem(new MenuItem("lasthit", "Lasthit mode").SetValue(new KeyBind('C', KeyBindType.Press)));
            subMenu.AddItem(new MenuItem("farmKey", "Farm mode").SetValue(new KeyBind('V', KeyBindType.Press)));
            subMenu.AddItem(new MenuItem("kitekey", "Kite mode").SetValue(new KeyBind('H', KeyBindType.Press)));
            orbwalkMenu.AddSubMenu(subMenu);
            
			if (!HeroSelect.IsSelected)
			{
                Common.Print("is succesfully loaded!"); return;
			}


            mainMenu.AddSubMenu((Menu)HeroSelect.HeroClass.GetField("Menu").GetValue(HeroSelect.HeroInstance));


			mainMenu.AddToMainMenu();
            
            Common.Print("is succesfully loaded!");
		}

        public static void Update()
        {
            MenuVar.LastHitEnable = orbwalkMenu.Item("enableLasthit").GetValue<bool>();
            if (MenuVar.LastHitEnable)
            {
                MenuVar.ShowHp = orbwalkMenu.Item("hpleftcreep").GetValue<bool>();
                MenuVar.Sapport = orbwalkMenu.Item("sapp").GetValue<bool>();
                MenuVar.UseSpell = orbwalkMenu.Item("usespell").GetValue<bool>();
                MenuVar.Harass = orbwalkMenu.Item("harassheroes").GetValue<bool>();
                MenuVar.Denie = orbwalkMenu.Item("denied").GetValue<bool>();
                MenuVar.Aoc = orbwalkMenu.Item("AOC").GetValue<bool>();
                MenuVar.Test = orbwalkMenu.Item("test").GetValue<bool>();
                MenuVar.SummonsEnable = orbwalkMenu.Item("enable").GetValue<bool>();
                if (MenuVar.SummonsEnable)
                {
                    MenuVar.SummonsHarass = orbwalkMenu.Item("harassheroes_sub").GetValue<bool>();
                    MenuVar.SummonsDenie = orbwalkMenu.Item("denied_sub").GetValue<bool>();
                    MenuVar.SummonsAoc = orbwalkMenu.Item("AOC_sub").GetValue<bool>();
                    MenuVar.SummonsAutoLasthit = orbwalkMenu.Item("autoD").GetValue<bool>();
                    MenuVar.SummonsAutoFarm = orbwalkMenu.Item("autoF").GetValue<bool>();
                }
                MenuVar.LastHitKey = orbwalkMenu.Item("lasthit").GetValue<KeyBind>().Key;
                MenuVar.FarmKey = orbwalkMenu.Item("farmKey").GetValue<KeyBind>().Key;
                MenuVar.CombatKey = orbwalkMenu.Item("combatkey").GetValue<KeyBind>().Key;
                MenuVar.KiteKey = orbwalkMenu.Item("kitekey").GetValue<KeyBind>().Key;

                MenuVar.AutoAttackMode =
                    orbwalkMenu.Item("autoAttackMode").GetValue<StringList>().SelectedIndex;
                MenuVar.Outrange = orbwalkMenu.Item("outrange").GetValue<Slider>().Value;
                MenuVar.BonusWindUp = orbwalkMenu.Item("bonuswindup").GetValue<Slider>().Value;
            }
        }


		public static void Unload()
		{
			if (mainMenu == null) return;

			mainMenu.RemoveFromMainMenu();
			mainMenu = null;
		}
    }
}
