using System;
using System.Collections.Generic;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using System.Threading.Tasks;

namespace AutoATitems
{
	internal class Program
	{
		private static readonly Menu Menu = new Menu("AutoATitems", "AutoATitems by kj2a", true, "npc_dota_hero_invoker", true);
		private static Item urn, dagon, halberd, ethereal, mjollnir, orchid, abyssal, mom, Shiva, mail, bkb, satanic, medall, blink, sheep, manta;
		
		private static bool keyCombo;
		private static Hero me;
		private static Hero target;

		
        private static void Main()
        {
            Events.OnLoad += OnLoadEvent;
            Events.OnClose += OnCloseEvent;
        }
		
        private static void OnCloseEvent(object sender, EventArgs args)
        {
            Game.OnUpdate -= Game_OnUpdate;
            target = null;
            me = null;
            Menu.RemoveFromMainMenu();
        }
	
		private static void OnLoadEvent(object sender, EventArgs args)
		{
			Menu.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_ethereal_blade", true},
                    {"item_blink", true},
                    {"item_heavens_halberd", true},
                    {"item_orchid", true}, {"item_bloodthorn", true},
                    {"item_urn_of_shadows", true},
                    {"item_abyssal_blade", true},
                    {"item_shivas_guard", true},
                    {"item_blade_mail", true},
                    {"item_black_king_bar", true},
                    {"item_medallion_of_courage", true},
                    {"item_solar_crest", true}
                })));
            Menu.AddItem(
               new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
               {
                   {"item_mask_of_madness", true},
                   {"item_sheepstick", true},
                   {"item_arcane_boots", true},
                   {"item_mjollnir", true},
                   {"item_satanic", true},
				   {"item_manta", true}
			   })));
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));


			Game.OnUpdate += Game_OnUpdate;
			Console.WriteLine("> AutoATitems# loaded!");

		}

		public static void Game_OnUpdate(EventArgs args)
		{
			var me = ObjectManager.LocalHero;
			if (!Game.IsInGame || me == null || Game.IsWatchingGame) return;
			
			keyCombo = Game.IsKeyDown(Menu.Item("ComboKey").GetValue<KeyBind>().Key);
			
			if (keyCombo)
			{
				var target = me.ClosestToMouseTarget(1200);
				if (target == null)
				{
					return;
				}
				if (target.IsAlive && !target.IsInvul() && !target.IsIllusion && !target.IsMagicImmune() && !me.IsInvisible())
				{
					Shiva = me.FindItem("item_shivas_guard");
					ethereal = me.FindItem("item_ethereal_blade");
					mom = me.FindItem("item_mask_of_madness");
					urn = me.FindItem("item_urn_of_shadows");
					manta = me.FindItem("item_manta");
					dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
					halberd = me.FindItem("item_heavens_halberd");
					mjollnir = me.FindItem("item_mjollnir");
					orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
					abyssal = me.FindItem("item_abyssal_blade");
					mail = me.FindItem("item_blade_mail");
					bkb = me.FindItem("item_black_king_bar");
					satanic = me.FindItem("item_satanic");
					blink = me.FindItem("item_blink");
					medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
					sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
					soul = me.FindItem("item_soul_ring");
					var qqqqqqqqqqqqq = ObjectManager.GetEntities<Hero>().Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune()).ToList();	
					var stoneModif = target.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
					
					if (target.IsVisible && me.Distance2D(target) <= 1200)
					{
						
					    var InvForgeds = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit) && x.IsAlive && x.IsControllable);
					    if (InvForgeds != null) {
							foreach (var v in InvForgeds)
							{
								if (target.Position.Distance2D(v.Position) < 1200 && !target.IsAttackImmune() &&
									Utils.SleepCheck(v.Handle.ToString()))
								{
									v.Attack(target);
									Utils.Sleep(300, v.Handle.ToString());
								}
							}
						}
	
						if (me.CanUseItems())
						{
							// use items.
							if ((manta != null 
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)) 
								&& manta.CanBeCasted() && me.IsSilenced() && Utils.SleepCheck("manta"))
							{
								manta.UseAbility();
								Utils.Sleep(400, "manta");
							}
							if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name))
								&& manta.CanBeCasted() && (target.Position.Distance2D(me.Position) <= me.GetAttackRange()+me.HullRadius)
								&& Utils.SleepCheck("manta"))
							{
								manta.UseAbility();
								Utils.Sleep(150, "manta");
							}
							if ( // MOM
								mom != null
								&& mom.CanBeCasted()
								&& me.CanCast()
								&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mom.Name)
								&& Utils.SleepCheck("mom")
								&& me.Distance2D(target) <= 700
								)
							{
								mom.UseAbility();
								Utils.Sleep(250, "mom");
							}
							if ( // Hellbard
								halberd != null
								&& halberd.CanBeCasted()
								&& me.CanCast()
								&& !target.IsMagicImmune()
								&& (target.NetworkActivity == NetworkActivity.Attack
									|| target.NetworkActivity == NetworkActivity.Crit
									|| target.NetworkActivity == NetworkActivity.Attack2)
								&& Utils.SleepCheck("halberd")
								&& me.Distance2D(target) <= 700
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
								)
							{
								halberd.UseAbility(target);
								Utils.Sleep(250, "halberd");
							}
							if ( // Mjollnir
								mjollnir != null
								&& mjollnir.CanBeCasted()
								&& me.CanCast()
								&& !target.IsMagicImmune()
								&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
								&& Utils.SleepCheck("mjollnir")
								&& me.Distance2D(target) <= 900
								)
							{
								mjollnir.UseAbility(me);
								Utils.Sleep(250, "mjollnir");
							} // Mjollnir Item end
							if ( // Medall
								medall != null
								&& medall.CanBeCasted()
								&& Utils.SleepCheck("Medall")
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
								&& me.Distance2D(target) <= 700
								)
							{
								medall.UseAbility(target);
								Utils.Sleep(250, "Medall");
							} // Medall Item end

							if ( // sheep
								sheep != null
								&& sheep.CanBeCasted()
								&& me.CanCast()
								&& !target.IsLinkensProtected()
								&& !target.IsMagicImmune()
								&& me.Distance2D(target) <= 1400
								&& !stoneModif
								&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
								&& Utils.SleepCheck("sheep")
								)
							{
								sheep.UseAbility(target);
								Utils.Sleep(250, "sheep");
							} // sheep Item end
							if ( // Abyssal Blade
								abyssal != null
								&& abyssal.CanBeCasted()
								&& me.CanCast()
								&& !target.IsStunned()
								&& !target.IsHexed()
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
								&& Utils.SleepCheck("abyssal")
								&& me.Distance2D(target) <= 400
								)
							{
								abyssal.UseAbility(target);
								Utils.Sleep(250, "abyssal");
							} // Abyssal Item end
							if (orchid != null && orchid.CanBeCasted() && me.Distance2D(target) <= 900
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
								&& Utils.SleepCheck("orchid"))
							{
								orchid.UseAbility(target);
								Utils.Sleep(100, "orchid");
							}

							if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(target) <= 600
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
								&& !target.IsMagicImmune() && Utils.SleepCheck("Shiva"))
							{
								Shiva.UseAbility();
								Utils.Sleep(100, "Shiva");
							}
							if ( // ethereal
								ethereal != null
								&& ethereal.CanBeCasted()
								&& me.CanCast()
								&& !target.IsLinkensProtected()
								&& !target.IsMagicImmune()
								&& !stoneModif
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
								&& Utils.SleepCheck("ethereal")
								)
							{
								ethereal.UseAbility(target);
								Utils.Sleep(200, "ethereal");
							} // ethereal Item end
							if (
								blink != null
								&& me.CanCast()
								&& blink.CanBeCasted()
								&& me.Distance2D(target) >= 450
								&& me.Distance2D(target) <= 1150
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
								&& Utils.SleepCheck("blink")
								)
							{
								blink.UseAbility(target.Position);
								Utils.Sleep(250, "blink");
							}
							if ( // Dagon
								me.CanCast()
								&& dagon != null
								&& (ethereal == null
									|| (target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
										|| ethereal.Cooldown < 17))
								&& !target.IsLinkensProtected()
								&& dagon.CanBeCasted()
								&& !target.IsMagicImmune()
								&& !stoneModif
								&& Utils.SleepCheck("dagon")
								)
							{
								dagon.UseAbility(target);
								Utils.Sleep(200, "dagon");
							} // Dagon Item end
							if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(target) <= 400
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
							{
								urn.UseAbility(target);
								Utils.Sleep(240, "urn");
							}
							if ( // Satanic 
								satanic != null 
								&& me.Health <= (me.MaximumHealth * 0.3) 
								&& satanic.CanBeCasted() 
								&& me.Distance2D(target) <= me.AttackRange + 50
								&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
								&& Utils.SleepCheck("satanic")
								)
							{
								satanic.UseAbility();
								Utils.Sleep(240, "satanic");
							} // Satanic Item end
							if (mail != null && mail.CanBeCasted() && (qqqqqqqqqqqqq.Count(x => x.Distance2D(me) <= 650) >=
																	   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
								Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
							{
								mail.UseAbility();
								Utils.Sleep(100, "mail");
							}
							if (bkb != null && bkb.CanBeCasted() && (qqqqqqqqqqqqq.Count(x => x.Distance2D(me) <= 650) >=
																	 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
								Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
							{
								bkb.UseAbility();
								Utils.Sleep(100, "bkb");
							}							
						}
					}
				}
			}
		}
	}
}
