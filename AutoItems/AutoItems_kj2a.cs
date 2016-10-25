using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using Ensage.Common.Menu;

namespace AutoItems
{
    class AutoItems_kj2a
    {
        private static Hero me;
	private static Hero mHero;
	private static Player mPlayer;
        private static bool heroCreateSideMessage, bearCreateSideMessage;
        private static Item item_bottle, item_phase_boots, item_magic_stick, item_magic_wand, item_cheese, item_arcane_boots;
        private static List<Hero> Alies;
        private static double PercentStickUse = 0.2;
	private static double PercentCheeseUse = 0.1;
	private static double PercentArcaneUse = 0.3;
        private static readonly uint WM_MOUSEWHEEL = 0x020A;
        private static readonly Menu Menu = new Menu("Auto Items", "Auto Items", true);
        private static readonly Menu _item_config = new Menu("Items", "Items");
        private static readonly Menu _percent_config = new Menu("Other Settings", "Other Settings");
        private static readonly Dictionary<string, bool> list_of_items = new Dictionary<string, bool>
	{
		{"item_bottle",true},
		{"item_phase_boots",true},
		{"item_arcane_boots",true},
		{"item_cheese",true},
		{"item_magic_stick",true},
		{"item_magic_wand",true},
		{"item_dust",true},
		{"item_hand_of_midas",true}
	};
        static void Main(string[] args)
        {
            Menu.AddSubMenu(_item_config);
            Menu.AddSubMenu(_percent_config);
            _item_config.AddItem(new MenuItem("Items: ", "Items: ").SetValue(new AbilityToggler(list_of_items)));
            _percent_config.AddItem(new MenuItem("Midas All", "Midas All").SetValue(true).SetTooltip("false = only creeps 5 lvl and > 950 HP."));
            _percent_config.AddItem(new MenuItem("Stick % HP", "Stick % HP").SetValue(new Slider(10, 1, 100)));
	    _percent_config.AddItem(new MenuItem("Cheese % HP", "Cheese % HP").SetValue(new Slider(10, 1, 100)));
	    _percent_config.AddItem(new MenuItem("Arcane Boots % MP", "Arcane Boots % MP").SetValue(new Slider(30, 1, 100)));
            Menu.AddToMainMenu();
            Game.OnUpdate += Tick;
            Game.OnUpdate += Game_OnUpdate;
            PrintSuccess(string.Format("> Auto Items by kj2a Loaded!"));
        }
        public static void Tick(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || Game.IsChatOpen)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            if (me.IsAlive)
            {
                FindItems();
                PercentStickUse = ((double)Menu.Item("Stick % HP").GetValue<Slider>().Value / 100);
		PercentCheeseUse = ((double)Menu.Item("Cheese % HP").GetValue<Slider>().Value / 100);
		PercentArcaneUse = ((double)Menu.Item("Arcane Boots % MP").GetValue<Slider>().Value / 100);
                if ( item_bottle != null && (!me.IsInvisible() || me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !me.IsChanneling() && me.Modifiers.Any(x => x.Name == "modifier_fountain_aura_buff") && _item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(item_bottle.Name) && Utils.SleepCheck("bottle"))
                {
                    if(!me.Modifiers.Any(x => x.Name == "modifier_bottle_regeneration") && (me.Health < me.MaximumHealth || me.Mana < me.MaximumMana))
                        item_bottle.UseAbility(false);
                    Alies = ObjectMgr.GetEntities<Hero>().Where(x => x.Team == me.Team && x != me && (x.Health < x.MaximumHealth || x.Mana < x.MaximumMana) && !x.Modifiers.Any(y => y.Name == "modifier_bottle_regeneration") && x.IsAlive && !x.IsIllusion && x.Distance2D(me) <= item_bottle.CastRange).ToList();
                    foreach (Hero v in Alies)
                        if (v != null)
                            item_bottle.UseAbility(v,false);
                    Utils.Sleep(300, "bottle");
                }
                if (item_phase_boots != null && (!me.IsInvisible() || me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !me.IsChanneling() && item_phase_boots.CanBeCasted() && me.NetworkActivity == NetworkActivity.Move && _item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(item_phase_boots.Name) && Utils.SleepCheck("phaseboots"))
                {
                    item_phase_boots.UseAbility(false);
                    Utils.Sleep(300, "phaseboots");
                }				
		if (item_arcane_boots != null && (!me.IsInvisible() || me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !me.IsChanneling() && item_arcane_boots.CanBeCasted() && (double)me.Mana / me.MaximumMana < PercentArcaneUse && _item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(item_arcane_boots.Name) && Utils.SleepCheck("arcaneboots"))
		{		
			item_arcane_boots.UseAbility(false);
			Utils.Sleep(300, "arcaneboots");
		}
						
		if (item_cheese != null && (!me.IsInvisible() || me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !me.IsChanneling() && item_cheese.CanBeCasted() && (double)me.Health / me.MaximumHealth < PercentCheeseUse && _item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(item_cheese.Name) && Utils.SleepCheck("item_cheese"))
		{	
			item_cheese.UseAbility(false);
			Utils.Sleep(300, "item_cheese");
		}
                if (item_magic_stick != null && (!me.IsInvisible() || me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !me.IsChanneling() && item_magic_stick.CanBeCasted() && item_magic_stick.CurrentCharges > 0 && (double)me.Health / me.MaximumHealth < PercentStickUse && _item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(item_magic_stick.Name) && Utils.SleepCheck("item_magic_stick"))
                {    
                    item_magic_stick.UseAbility(false);
                    Utils.Sleep(300, "item_magic_stick");
            	}
                if (item_magic_wand != null && (!me.IsInvisible() || me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !me.IsChanneling() &&  item_magic_wand.CanBeCasted() && item_magic_wand.CurrentCharges > 0 && (double)me.Health / me.MaximumHealth < PercentStickUse && _item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(item_magic_wand.Name) && Utils.SleepCheck("item_magic_wand"))
                {    
                    item_magic_wand.UseAbility(false);
                    Utils.Sleep(300, "item_magic_wand");
                }
            }
        }
        static void FindItems()
        {
            if (Utils.SleepCheck("Finditems"))
            {
                item_bottle = me.FindItem("item_bottle");
                item_phase_boots = me.FindItem("item_phase_boots");
                item_magic_stick = me.FindItem("item_magic_stick");
                item_magic_wand = me.FindItem("item_magic_wand");
		item_cheese = me.FindItem("item_cheese");
		item_arcane_boots = me.FindItem("item_arcane_boots");
                Utils.Sleep(500, "Finditems");
            }
        }
        private static void PrintSuccess(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.Green, arguments);
        }
        private static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
        
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || Game.IsChatOpen)
                return;

            if (mHero == null) mHero = ObjectMgr.LocalHero;
            if (mPlayer == null) mPlayer = ObjectMgr.LocalPlayer;
            if (mHero == null) return;
            if  (_item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_hand_of_midas") && Utils.SleepCheck("Auto Midas"))
            {
		var midas = FindMidas(mHero);
		var midasOwner =  midas != null ? (Unit) midas.Owner : null;
		if (midasOwner != null && !midasOwner.IsChanneling() && !midasOwner.IsInvisible())
		{
			UseMidas(midas, midasOwner);
			Utils.Sleep(500, "Auto Midas");
		}
            }
           if (_item_config.Item("Items: ").GetValue<AbilityToggler>().IsEnabled("item_dust"))
           {
          
		var dust = mHero.FindItem("item_dust");
            	if (dust==null|| !dust.CanBeCasted() || mHero.IsInvisible() || !Utils.SleepCheck("delay")) return;
            	var enemy = ObjectMgr.GetEntities<Hero>()
                	.Where(
                    	v =>
                        	!v.IsIllusion && v.Team != mPlayer.Team && v.IsAlive && v.IsVisible &&
                        	mHero.Distance2D(v) <= 1000);
            	foreach (var v in enemy)
            	{
                	if (v.Modifiers.Any(
                	 x =>
	                        (x.Name == "modifier_bounty_hunter_wind_walk" ||
	                         x.Name == "modifier_riki_permanent_invisibility" ||
	                         x.Name == "modifier_mirana_moonlight_shadow" || x.Name == "modifier_treant_natures_guise" ||
	                         x.Name == "modifier_weaver_shukuchi" ||
	                         x.Name == "modifier_broodmother_spin_web_invisible_applier" ||
	                         x.Name == "modifier_item_invisibility_edge_windwalk" || x.Name == "modifier_rune_invis" ||
	                         x.Name == "modifier_clinkz_wind_walk" || x.Name == "modifier_item_shadow_amulet_fade" ||
	                         x.Name == "modifier_bounty_hunter_track" || x.Name == "modifier_bloodseeker_thirst_vision" ||
	                         x.Name == "modifier_slardar_amplify_damage" || x.Name == "modifier_item_dustofappearance") ||
	                        x.Name == "modifier_invoker_ghost_walk_enemy"))
	                {
	                    dust.UseAbility();
	                    Utils.Sleep(250, "delay");
	                }
	
	                if ((v.Name == ("npc_dota_hero_templar_assassin") || v.Name == ("npc_dota_hero_sand_king")) &&
	                    (v.Health/v.MaximumHealth < 0.3))
	                {
	                    dust.UseAbility();
	                    Utils.Sleep(250, "delay");
	
	                }
	                if (v.Name != ("npc_dota_hero_nyx_assassin") || !v.Spellbook.Spell4.CanBeCasted()) continue;
	                dust.UseAbility();
	                Utils.Sleep(250, "delay");
		}
            }
        }

        // --→ Function: Find Midas
        private static Item FindMidas(Unit entity)
        {
            if (entity.ClassID.Equals(ClassID.CDOTA_Unit_Hero_LoneDruid))
            {
                var bear = ObjectMgr.GetEntities<Unit>().Where(unit => unit.ClassID.Equals(ClassID.CDOTA_Unit_SpiritBear) && unit.IsAlive && unit.Team.Equals(mHero.Team) && unit.IsControllable).ToList();
                var heroMidas = entity.FindItem("item_hand_of_midas");
                if (heroMidas.CanBeCasted()) return heroMidas;
                return bear.Any() ? bear.First().FindItem("item_hand_of_midas") : null;
            }
            else
            {
                var heroMidas = entity.FindItem("item_hand_of_midas");
                return heroMidas;
            }
        }

        // --→ Function: Use Midas
        private static void UseMidas(Ability midas, Unit owner)
        {
            if (midas.CanBeCasted() && owner.CanUseItems() && owner.Equals(mHero))
            {
                if (!heroCreateSideMessage)
                {
                    heroCreateSideMessage = true;
                    GenerateSideMessage(GetOwnerName(owner));
                }
                if (Menu.Item("Midas All").GetValue<bool>()) {
                	var creeps = ObjectMgr.GetEntities<Creep>().Where(creep => creep.Team != owner.Team && creep.IsAlive && creep.IsVisible && creep.IsSpawned && !creep.IsAncient && creep.Health > 0 && creep.Distance2D(owner) < midas.CastRange + 25).ToList();
                	if (!creeps.Any()) return;
			midas.UseAbility(creeps.First());
                } else {
                	var creeps = ObjectMgr.GetEntities<Creep>().Where(creep => creep.Team != owner.Team && creep.IsAlive && creep.IsVisible && creep.IsSpawned && !creep.IsAncient && creep.Health > 950 && creep.Distance2D(owner) < midas.CastRange + 25).ToList();
                	if (!creeps.Any()) return;
        		midas.UseAbility(creeps.First());
                }
            }
            else if(heroCreateSideMessage) heroCreateSideMessage = false;

            if (midas.CanBeCasted() && owner.CanUseItems() && owner.ClassID.Equals(ClassID.CDOTA_Unit_SpiritBear))
            {
                if (!bearCreateSideMessage)
                {
                    bearCreateSideMessage = true;
                    GenerateSideMessage(GetOwnerName(owner));
                }
                
                if (Menu.Item("Midas All").GetValue<bool>()) {
                	var creeps = ObjectMgr.GetEntities<Creep>().Where(creep => creep.Team != owner.Team && creep.IsAlive && creep.IsVisible && creep.IsSpawned && !creep.IsAncient && creep.Health > 0 && creep.Distance2D(owner) < midas.CastRange + 25).ToList();
                	if (!creeps.Any()) return;
			midas.UseAbility(creeps.First());
                } else {
                	var creeps = ObjectMgr.GetEntities<Creep>().Where(creep => creep.Team != owner.Team && creep.IsAlive && creep.IsVisible && creep.IsSpawned && !creep.IsAncient && creep.Health > 950 && creep.Distance2D(owner) < midas.CastRange + 25).ToList();
                	if (!creeps.Any()) return;
        		midas.UseAbility(creeps.First());
                }
            }
            else if(bearCreateSideMessage) bearCreateSideMessage = false;
        }

        // --→ Function: Get Midas Owner Name
        private static string GetOwnerName(Entity owner)
        {
            return owner.Equals(mHero) ? owner.Name.Replace("npc_dota_hero_", "") : "spirit_bear";
        }

        // --→ Function: Create Side Message
        private static void GenerateSideMessage(string unit)
        {
            var sideMessage = new SideMessage(unit, new Vector2(200, 60));
            sideMessage.AddElement(new Vector2(10, 10), new Vector2(72, 40), Drawing.GetTexture("materials/ensage_ui/heroes_horizontal/" + unit + ".vmat"));
            sideMessage.AddElement(new Vector2(85, 16), new Vector2(62, 31), Drawing.GetTexture("materials/ensage_ui/other/arrow_usual.vmat"));
            sideMessage.AddElement(new Vector2(145, 10), new Vector2(70, 40), Drawing.GetTexture("materials/ensage_ui/items/hand_of_midas.vmat"));
            sideMessage.CreateMessage();
        }
    }
}
