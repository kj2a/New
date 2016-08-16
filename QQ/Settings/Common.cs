using System.Collections.Generic;

namespace CarrySharp.Settings
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Linq;

    class Common
    {

        #region Methods

        public static void Autoattack(int a)
        {
            //if (Game.GetConsoleVar("dota_player_units_auto_attack_mode").GetInt() != aa)
            Game.ExecuteCommand("dota_player_units_auto_attack_mode " + a);
        }

        public static void AutoattackSummons(int a)
        {
            //if (Game.GetConsoleVar("dota_player_units_auto_attack_mode").GetInt() != aa)
            Game.ExecuteCommand("dota_summoned_units_auto_attack_mode " + a);
        }

        public static float RadiansToFace(Unit StartUnit, dynamic Target) 
        {
            if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("RadiansToFace -> INVALID PARAMETERS!", "Target");
            if (Target is Unit) Target = Target.Position;

            float deltaY = StartUnit.Position.Y - Target.Y;
            float deltaX = StartUnit.Position.X - Target.X;
            float angle = (float)(Math.Atan2(deltaY, deltaX));

            return (float)(Math.PI - Math.Abs(Math.Atan2(Math.Sin(StartUnit.RotationRad - angle), Math.Cos(StartUnit.RotationRad - angle))));
        }
    

        public static float GetOutRange(Unit unit)
        {
            if (unit.Handle == Variables.Var.Me.Handle)
            {
                return Lasthit.AttackRange() + MenuStructure.MenuVar.Outrange;
            }
            else
            {
                if (unit.IsRanged)
                    return 500 + MenuStructure.MenuVar.Outrange;
                else
                    return 200 + MenuStructure.MenuVar.Outrange;
            }
        }

        public static bool InvisibleHero(Hero z)
        {
            if (z.Modifiers.Any(
                x =>
                    (x.Name == "modifier_bounty_hunter_wind_walk" ||
                     x.Name == "modifier_riki_permanent_invisibility" ||
                     x.Name == "modifier_mirana_moonlight_shadow" || x.Name == "modifier_treant_natures_guise" ||
                     x.Name == "modifier_weaver_shukuchi" ||
                     x.Name == "modifier_broodmother_spin_web_invisible_applier" ||
                     x.Name == "modifier_item_invisibility_edge_windwalk" || x.Name == "modifier_rune_invis" ||
                     x.Name == "modifier_clinkz_wind_walk" || x.Name == "modifier_item_shadow_amulet_fade" ||
                     x.Name == "modifier_item_silver_edge_windwalk" ||
                     x.Name == "modifier_item_edge_windwalk" ||
                     x.Name == "modifier_nyx_assassin_vendetta" ||
                     x.Name == "modifier_invisible" ||
                     x.Name == "modifier_invoker_ghost_walk_enemy")))
                return true;
            return false;
        }

        public static bool HasStun(Hero x)
        {
            if (x.FindSpell("dragon_knight_dragon_tail") != null &&
                x.FindSpell("dragon_knight_dragon_tail").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("dragon_knight_dragon_tail").GetCastRange()
                ||
                x.FindSpell("earthshaker_echo_slam") != null && x.FindSpell("earthshaker_echo_slam").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("earthshaker_echo_slam").GetCastRange()
                ||
                x.FindSpell("legion_commander_duel") != null && x.FindSpell("legion_commander_duel").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("legion_commander_duel").GetCastRange()
                ||
                x.FindSpell("leshrac_split_earth") != null && x.FindSpell("leshrac_split_earth").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("leshrac_split_earth").GetCastRange()
                ||
                x.FindSpell("leoric_hellfire_blast") != null && x.FindSpell("leoric_hellfire_blast").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("leoric_hellfire_blast").GetCastRange()
                ||
                x.FindSpell("lina_light_strike_array") != null && x.FindSpell("lina_light_strike_array").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("lina_light_strike_array").GetCastRange()
                ||
                x.FindSpell("lion_impale") != null && x.FindSpell("lion_impale").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("lion_impale").GetCastRange()
                ||
                x.FindSpell("magnataur_reverse_polarity") != null &&
                x.FindSpell("magnataur_reverse_polarity").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("magnataur_reverse_polarity").GetCastRange()
                ||
                x.FindSpell("nyx_assassin_impale") != null && x.FindSpell("nyx_assassin_impale").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("nyx_assassin_impale").GetCastRange()
                ||
                x.FindSpell("ogre_magi_fireblast") != null && x.FindSpell("ogre_magi_fireblast").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("ogre_magi_fireblast").GetCastRange()
                ||
                x.FindSpell("skeleton_king_hellfire_blast") != null &&
                x.FindSpell("skeleton_king_hellfire_blast").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("skeleton_king_hellfire_blast").GetCastRange()
                ||
                x.FindSpell("sven_storm_bolt") != null && x.FindSpell("sven_storm_bolt").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("sven_storm_bolt").GetCastRange()
                ||
                x.FindSpell("tiny_avalanche") != null && x.FindSpell("tiny_avalanche").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("tiny_avalanche").GetCastRange()
                ||
                x.FindSpell("lion_voodoo") != null && x.FindSpell("lion_voodoo").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("lion_voodoo").GetCastRange()
                ||
                x.FindSpell("alchemist_unstable_concoction_throw") != null && x.FindSpell("alchemist_unstable_concoction_throw").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("alchemist_unstable_concoction_throw").GetCastRange()
                ||
                x.FindSpell("tusk_walrus_punch") != null && x.FindSpell("tusk_walrus_punch").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("tusk_walrus_punch").GetCastRange()
                ||
                x.FindSpell("vengefulspirit_magic_missile") != null &&
                x.FindSpell("vengefulspirit_magic_missile").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("vengefulspirit_magic_missile").GetCastRange()
                ||
                x.FindSpell("windrunner_shackleshot") != null && x.FindSpell("windrunner_shackleshot").Cooldown <= 0 &&
                Variables.Var.Me.Distance2D(x) <= x.FindSpell("windrunner_shackleshot").GetCastRange()
                )
                return true;
            else
            {
                return false;
            }

        }

      /*  public static readonly List<string> Disables = new List<string> 
        {
           "vengefulspirit_magic_missile",
           "dragon_knight_dragon_tail",
            "leshrac_split_earth",
            "leoric_hellfire_blast",
            "lina_light_strike_array",
            "lion_impale",
            "magnataur_reverse_polarity",
            "nyx_assassin_impale",
            "ogre_magi_fireblast",
            "skeleton_king_hellfire_blast",
            "sven_storm_bolt",
            "tiny_avalanche",
            "lion_voodoo",
            "alchemist_unstable_concoction_throw",
            "tusk_walrus_punch",
            "vengefulspirit_magic_missile",
            "windrunner_shackleshot",
            "drow_ranger_silence",
            "pudge_dismember",
            "slardar_slithereen_crush",
            "earthshaker_fissure",
            "chaos_knight_chaos_bolt",
            "chaos_knight_chaos_strike",
            "beastmaster_primal_roar",
            "batrider_flaming_lasso",
            "bane_nightmare",
            "axe_berserkers_call"
            

        };*/

        public static void Print(string s, MessageType chatMessage = MessageType.ChatMessage)
        {
            if (true)
                Game.PrintMessage("CarrySharp " + s, chatMessage);
        }

        public static void PrintError(string s)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("CarrySharp " + s);
        }

        public static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
        #endregion Methods
    }
}
