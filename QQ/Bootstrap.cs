using CarrySharp.Settings;

namespace CarrySharp
{
    using System;
    using Ensage;
    using Ensage.Common;
    using System.Threading;

    class Bootstrap
    {
        public static void Initialize()
        {
            Events.OnLoad += OnLoadEvent;
            Events.OnClose += OnCloseEvent;
        }

        private static void OnUpdateEvent(EventArgs args)
        {
            try
            {
                MenuStructure.Update();
                HeroSelect.Combo();
            }
            catch (Exception ee)
            {
                // Console.WriteLine("ErrorOnUpdateEvent" + ee)
            }
            finally
            {
                if (MenuStructure.MenuVar.LastHitEnable)
                {
                    if (MenuStructure.MenuVar.Test)
                        Lasthit.Attack_Calc();

                    if ((Game.IsKeyDown(MenuStructure.MenuVar.LastHitKey) || MenuStructure.MenuVar.SummonsAutoLasthit) &&
                        MenuStructure.MenuVar.SummonsEnable)
                    {
                        Lasthit.SummonLastHit();
                    }
                    else if ((Game.IsKeyDown(MenuStructure.MenuVar.FarmKey) || MenuStructure.MenuVar.SummonsAutoFarm) &&
                        MenuStructure.MenuVar.SummonsEnable)
                    {
                        Lasthit.SummonFarm();
                    }
                    else
                    {
                        if (!Variables.Var.SummonsAutoAttackTypeDef)
                        {
                            Common.AutoattackSummons(-1);
                            Variables.Var.SummonsDisableAaKeyPressed = false;
                            Variables.Var.SummonsAutoAttackTypeDef = true;
                        }
                        Variables.Var.CreeptargetS = null;
                    }

                    if (Game.IsKeyDown(MenuStructure.MenuVar.LastHitKey))
                    {
                        Lasthit.LastHit();
                    }
                    else if (Game.IsKeyDown(MenuStructure.MenuVar.FarmKey))
                    {
                        Lasthit.Farm();
                    }
                    else if (Game.IsKeyDown(MenuStructure.MenuVar.CombatKey))
                    {
                        Lasthit.Combat();
                    }
                    else if (Game.IsKeyDown(MenuStructure.MenuVar.KiteKey))
                    {
                        Lasthit.Kite();
                    }
                    else
                    {
                        if (!Variables.Var.AutoAttackTypeDef)
                        {
                            Variables.Var.Me.Hold();
                            Common.Autoattack(MenuStructure.MenuVar.AutoAttackMode);
                            Variables.Var.DisableAaKeyPressed = false;
                            Variables.Var.AutoAttackTypeDef = true;
                        }
                        Variables.Var.CreeptargetH = null;
                    }
                }
            }
        }

        private static void OnLoadEvent(object sender, EventArgs e)
        {
            try
            {
                Variables.Var.Me = ObjectManager.LocalHero;
                HeroSelect.Load();
                HeroSelect.HeroesLoadEvent();
                MenuStructure.Load();
                Game.OnUpdate += OnUpdateEvent;

            }
            catch (Exception ee)
            {
                // Console.WriteLine("ErrorOnLoad" + ee)
            }
        } 

        private static void OnCloseEvent(object sender, EventArgs e)
        {
            try
            {
                Game.OnUpdate -= OnUpdateEvent;
                HeroSelect.ControllerCloseEvent();
                MenuStructure.Unload();
                HeroSelect.Unload();


                Common.Print("CarrySharp by beminee. Please upvote on Assembly Database if you liked it!");
            }
            catch (Exception ee)
            {
                // Console.WriteLine("ErrorOnClose" + ee)
            }
        } 
    }
}
