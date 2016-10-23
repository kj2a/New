using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using SharpDX;
using Ensage.Common.Menu;


namespace OrbWalker
{
    class OrbWalker
    {
        private static readonly Menu Menu = new Menu("OrbWalker", "OrbWalker", true, "npc_dota_hero_Invoker", true);
        private static int OrbMinDist => Menu.Item("orbwalk.minDistance").GetValue<Slider>().Value;
    
        private static Hero me, target;
        private static bool target_magic_imune, target_isinvul,forge_in_my_side;
        private static float distance_me_target;
        private static ParticleEffect targetParticle;
        private static List<Unit> myunits;

        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("Combo Mode", "Combo Mode").SetValue(new KeyBind('T', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Target Type: ", "Target Type: ").SetValue(new StringList(new[] { "Target Selector", "Closest to mouse" }))).SetTooltip("On target selector you can get a better position while comboing. but closest to mouse is more easier");
            if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 0)
                Menu.AddItem(new MenuItem("Target Select", "Target Select").SetValue(new KeyBind('G', KeyBindType.Press)));

            var orbmenu = new Menu("OrbChanging", "Orb Menu");
            Menu.AddSubMenu(orbmenu);
            orbmenu.AddItem(new MenuItem("Enable OrbChanging", "Enable OrbChanging").SetValue(true).SetTooltip("Enable/Disable automatic orb Changing."));
            orbmenu.AddItem(new MenuItem("quas threshold health", "Quas Threshold Health").SetValue(new Slider(90, 1, 100)).SetTooltip("Percentage of HP threshold to change orbs for quas while not attacking."));

            Menu.AddItem(new MenuItem("orbwalk.minDistance", "Orbwalk min distance").SetValue(new Slider(250, 0, 700)).SetTooltip("the min distance to stop orbwalking and just auto attack."));
            Menu.AddToMainMenu();
            Game.OnWndProc += Exploding;
            Drawing.OnDraw += Target_esp;
            Orbwalking.Load();
        }
        public static void Target_esp(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            if (targetParticle == null && target != null)
            {
                targetParticle = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
            }
            if ((target == null || !target.IsVisible || !target.IsAlive) && targetParticle != null)
            {
                targetParticle.Dispose();
                targetParticle = null;
            }
            if (target != null && targetParticle != null)
            {
                targetParticle.SetControlPoint(2, me.Position);
                targetParticle.SetControlPoint(6, new Vector3(1, 0, 0));
                targetParticle.SetControlPoint(7, target.Position);
            }
        }
        public static void Exploding(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame || Game.IsPaused)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            for (uint i = 48; i <= 90; i++)
            {
                if (Game.IsKeyDown(keyCode: i))
                {
                    Utils.Sleep(2000, "KEYPRESSED");
                }
            }

            if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 0 && Game.IsKeyDown(Menu.Item("Target Select").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
                target = me.ClosestToMouseTarget(1000);
            if (Game.IsKeyDown(Menu.Item("Combo Mode").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (target != null && (!target.IsAlive || target.IsIllusion || distance_me_target > 3000 || !target.IsVisible))
                        target = null;
                    if (target == null)
                        target = me.BestAATarget(1000);
                }
                else if (Menu.Item("Target Type: ").GetValue<StringList>().SelectedIndex == 1)
                    target = me.ClosestToMouseTarget(1000);
                if (target != null && target.IsValid && !target.IsIllusion)
                {
                    //Console.WriteLine(target.Modifiers.LastOrDefault().Name);
                    if (Utils.SleepCheck("Variable Checker"))
                    {
                        distance_me_target = target.NetworkPosition.Distance2D(me.NetworkPosition);
                        myunits = ObjectMgr.GetEntities<Unit>().Where(x => x.Team == me.Team && x.IsControllable && x != null && x.IsAlive && x.Distance2D(target) <= 2000 && x.IsControllable && x.IsValid).ToList();
                        forge_in_my_side = ObjectMgr.GetEntities<Unit>().Where(x => x.Team == me.Team && x.IsControllable && x != null && x.IsAlive && x.Distance2D(target) <= 700 && x.Name.Contains("npc_dota_invoker_forged_spirit") && x.IsValid).Any();
                        
                        target_magic_imune = target.IsMagicImmune();
                        target_isinvul = target.IsInvul();
                        Utils.Sleep(200, "Variable Checker");
                    }
					
					
					if (Utils.SleepCheck("orbwalker"))
					{
						if (me.Distance2D(target) >= OrbMinDist)
							Orbwalking.Orbwalk(target);
						else
							me.Attack(target, false);
						Utils.Sleep(200, "orbwalker");
					}
					
                    if (myunits != null)
                    {
                        foreach (Unit unit in myunits)
                        {
                            if (unit == null && unit.IsAlive) continue;
                            if (unit.Name.Contains("necronomicon") || unit.Name.Contains("npc_dota_invoker_forged_spirit"))
                            {
                                Ability spell;
                                if (Utils.SleepCheck("attack" + unit.Handle))
                                {
                                    unit.Attack(target);
                                    if (unit.Name.Contains("npc_dota_necronomicon_archer"))
                                    {
                                        spell = unit.Spellbook.Spell1;
                                        if (spell != null && spell.CanBeCasted(target))
                                            spell.UseAbility(target);
                                    }
                                    Utils.Sleep(1000, "attack" + unit.Handle);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Utils.SleepCheck("moving_idle"))
                    {
                        me.Move(Game.MousePosition, false);
                        Utils.Sleep(300, "moving_idle");
                    }
                }
            }
        }
     
    }
}
