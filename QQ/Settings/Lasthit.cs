//Credits to redickowii

namespace CarrySharp.Settings
{
    using Ensage;
    using Ensage.Common;
    using CarrySharp.Settings;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;


   internal class Lasthit
   {
       private static float _attackRange;

       #region TEST
       public static int Attack(Entity unit)
        {
            var count = 0;
            try
            {
                var creeps =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                x.Distance2D(unit) <= x.AttackRange + 100 && x.IsAttacking() && x.IsAlive &&
                                x.Handle != unit.Handle && x.Team != unit.Team)
                        .ToList();
                count += (from creep in creeps
                          let angle = creep.Rotation < 0 ? Math.Abs(creep.Rotation) : 360 - creep.Rotation
                          where Math.Abs(angle - creep.FindAngleForTurnTime(unit.Position)) <= 3
                          select creep).Count();
            }
            catch (Exception ee)
            {
                //Console.WriteLine("ErrorAttack" + ee);
            }
            return count;
        }

        public static void Attack_Calc()
        {
            if (!ObjectManager.GetEntities<Unit>().Any(x => x.Distance2D(Variables.Var.Me) <= 2000 && x.IsAlive && x.Health > 0))
                return;
            var creeps =
                ObjectManager.GetEntities<Unit>()
                    .Where(x => x.Distance2D(Variables.Var.Me) <= 2000 && x.IsAlive && x.Health > 0)
                    .ToList();
            foreach (var creep in creeps)
            {
                if (!Variables.Var.CreepsDic.Any(x => x.AHealth(creep)))
                {
                    Variables.Var.CreepsDic.Add(new Variables.DictionaryUnit { Unit = creep, Ht = new List<Variables.Ht>() });
                }
            }

            Clear();
        }

        public static double Healthpredict(Unit unit, double time)
        {
            if (Variables.Var.CreepsDic.Count != 0 && MenuStructure.MenuVar.Test)
            {
                if (Variables.Var.CreepsDic.All(x => x.Unit.Handle != unit.Handle)) return 0;
                try
                {
                    var hta = Variables.Var.CreepsDic.First(x => x.Unit.Handle == unit.Handle).Ht.ToArray();
                    var length = hta.Length - 1;
                    if (hta.Length - hta[length].ACreeps >= 0)
                    {
                        var aCreeps = hta[length].ACreeps;

                        if (time <=
                            hta[length - aCreeps + 1].Time -
                            hta[length - aCreeps].Time)
                        {
                            return hta[length - aCreeps].Health -
                                    hta[length - aCreeps + 1].Health - 10;
                        }
                    }
                }
                catch (Exception)
                {
                        Common.PrintError("Health predict Error");
                }
            }
            return 0;
        }

        private static void Clear()
        {
            if (!Utils.SleepCheck("Lasthit.Clear")) return;
            var creeps = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive).ToList();
            Variables.Var.CreepsDic = (from creep in creeps
                             where Variables.Var.CreepsDic.Any(x => x.Unit.Handle == creep.Handle)
                             select Variables.Var.CreepsDic.Find(x => x.Unit.Handle == creep.Handle)).ToList();
            Utils.Sleep(10000, "Lasthit.Clear");
        }

        private static void UpdateCreeps()
        {
            try
            {
                Variables.Var.Creeps = ObjectManager.GetEntities<Unit>()
                    .Where(
                        x =>
                            (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                             x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Additive
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Barracks
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Building
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creature) && x.IsAlive && x.IsVisible
                            && x.Distance2D(Variables.Var.Me) < MenuStructure.MenuVar.Outrange + AttackRange()).ToList();
            }
            catch (Exception)
            {
                    Common.PrintError("Update Creeps Error");
            }
        }

        #endregion TEST

        #region Main

        public static void Combat()
        {
            Orbwalking.Orbwalk(Variables.Var.Target, attackmodifiers: true);
        }

        public static void Drawhpbar()
        {
            try
            {
                UpdateCreeps();
                if (Variables.Var.Creeps.Count == 0) return;
                foreach (var creep in Variables.Var.Creeps)
                {
                    if ((MenuStructure.MenuVar.Sapport && Variables.Var.Me.Team != creep.Team) ||
                        (!MenuStructure.MenuVar.Sapport && Variables.Var.Me.Team == creep.Team))
                        continue;
                    var health = creep.Health;
                    var maxHealth = creep.MaximumHealth;
                    if (health == maxHealth) continue;
                    var damge = (float) GetDamageOnUnitForDrawhpbar(creep, 0);
                    var hpperc = health / maxHealth;

                    var hbarpos = HUDInfo.GetHPbarPosition(creep);

                    Vector2 screenPos;
                    var enemyPos = creep.Position + new Vector3(0, 0, creep.HealthBarOffset);
                    if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

                    var start = screenPos;

                    hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep) / 2;
                    hbarpos.Y = start.Y;
                    var hpvarx = hbarpos.X;
                    var a = (float) Math.Floor(damge * HUDInfo.GetHPBarSizeX(creep) / creep.MaximumHealth);
                    var position = hbarpos + new Vector2(hpvarx * hpperc + 10, -12);
                    if (creep.ClassID == ClassID.CDOTA_BaseNPC_Tower)
                    {
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep) * 6;
                        position = hbarpos + new Vector2(hpvarx * hpperc - 5, -1);
                    }
                    else if (creep.ClassID == ClassID.CDOTA_BaseNPC_Barracks)
                    {
                        hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep);
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep) * 6;
                        position = hbarpos + new Vector2(hpvarx * hpperc + 10, -1);
                    }
                    else if (creep.ClassID == ClassID.CDOTA_BaseNPC_Building)
                    {
                        hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep) / 2;
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep);
                        position = hbarpos + new Vector2(hpvarx * hpperc + 10, -1);
                    }

                    Drawing.DrawRect(
                        position,
                        new Vector2(a, HUDInfo.GetHpBarSizeY(creep) - 4),
                        creep.Health > damge
                            ? creep.Health > damge * 2 ? new Color(180, 205, 205, 40) : new Color(255, 0, 0, 60)
                            : new Color(127, 255, 0, 80));
                    Drawing.DrawRect(position, new Vector2(a, HUDInfo.GetHpBarSizeY(creep) - 4), Color.Black, true);

                    if (!MenuStructure.MenuVar.Test) continue;
                    var time = Variables.Var.Me.IsRanged == false
                        ? Variables.Var.HeroAPoint / 1000 + Variables.Var.Me.GetTurnTime(Variables.Var.CreeptargetH.Position)
                        : Variables.Var.HeroAPoint / 1000 + Variables.Var.Me.GetTurnTime(creep.Position) +
                          Variables.Var.Me.Distance2D(creep) / GetProjectileSpeed(Variables.Var.Me);
                    var damgeprediction = Healthpredict(creep, time);
                    var b = (float) Math.Round(damgeprediction * 1 * HUDInfo.GetHPBarSizeX(creep) / creep.MaximumHealth);
                    var position2 = position + new Vector2(a, 0);
                    Drawing.DrawRect(position2, new Vector2(b, HUDInfo.GetHpBarSizeY(creep) - 4), Color.YellowGreen);
                    Drawing.DrawRect(position2, new Vector2(b, HUDInfo.GetHpBarSizeY(creep) - 4), Color.Black, true);
                }
            }
            catch (Exception)
            {
                    Common.PrintError("Draw Hpbar Error");
            }
        }

        public static void Farm()
        {
            if (!Utils.SleepCheck("Lasthit.Cast")) return;
            if (!Variables.Var.DisableAaKeyPressed)
            {
                Common.Autoattack(0);
                Variables.Var.DisableAaKeyPressed = true;
                Variables.Var.AutoAttackTypeDef = false;
            }
            Variables.Var.CreeptargetH = KillableCreep(Variables.Var.Me, false, false, 99);
            if (Variables.Var.CreeptargetH != null && Variables.Var.CreeptargetH.IsValid && Variables.Var.CreeptargetH.IsVisible && Variables.Var.CreeptargetH.IsAlive)
            {
                if (MenuStructure.MenuVar.UseSpell && Utils.SleepCheck("Lasthit.Cooldown"))
                    SpellCast();
                Orbwalking.Orbwalk(Variables.Var.CreeptargetH);
            }
        }

        public static List<Unit> GetNearestCreep(Unit source, float range)
        {
            try
            {
                return ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Additive ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Barracks ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Building ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creature) &&
                                 x.IsAlive && x.IsVisible && x.Distance2D(source) < range)
                                 .OrderBy(creep => creep.Health)
                                 .ToList();
            }
            catch (Exception)
            {
                    Common.PrintError("GetNearestCreep Error");
            }
            return null;
        }

        public static void Kite()
        {
            Orbwalking.Orbwalk(
                Variables.Var.Target,
                attackmodifiers: true,
                bonusWindupMs: MenuStructure.MenuVar.BonusWindUp);
        }

        public static void LastHit()
        {
            if (!Utils.SleepCheck("Lasthit.Cast")) return;
            if (!Variables.Var.DisableAaKeyPressed)
            {
                Common.Autoattack(0);
                Variables.Var.DisableAaKeyPressed = true;
                Variables.Var.AutoAttackTypeDef = false;
            }
            Variables.Var.CreeptargetH = KillableCreep(Variables.Var.Me, false, false, 2);
            if (MenuStructure.MenuVar.UseSpell && Utils.SleepCheck("Lasthit.Cooldown"))
                SpellCast();
            if (Variables.Var.CreeptargetH != null && Variables.Var.CreeptargetH.IsValid &&
                Variables.Var.CreeptargetH.IsVisible && Variables.Var.CreeptargetH.IsAlive)
            {
                var time = Variables.Var.Me.IsRanged == false
                    ? Variables.Var.HeroAPoint / 1000 + Variables.Var.Me.GetTurnTime(Variables.Var.CreeptargetH.Position)
                    : Variables.Var.HeroAPoint / 1000 + Variables.Var.Me.GetTurnTime(Variables.Var.CreeptargetH.Position) +
                      Variables.Var.Me.Distance2D(Variables.Var.CreeptargetH) / GetProjectileSpeed(Variables.Var.Me);
                var getDamage = GetDamageOnUnit(Variables.Var.Me, Variables.Var.CreeptargetH, 0);
                if (Variables.Var.CreeptargetH.Distance2D(Variables.Var.Me) <= AttackRange())
                {
                    if (Variables.Var.CreeptargetH.Health < getDamage ||
                        Variables.Var.CreeptargetH.Health < getDamage && Variables.Var.CreeptargetH.Team == Variables.Var.Me.Team &&
                        (MenuStructure.MenuVar.Denie || MenuStructure.MenuVar.Aoc))
                    {
                        if (!Variables.Var.Me.IsAttacking())
                            Variables.Var.Me.Attack(Variables.Var.CreeptargetH);
                    }
                    else if (Variables.Var.CreeptargetH.Health < getDamage * 2 && Variables.Var.CreeptargetH.Health >= getDamage &&
                             Variables.Var.CreeptargetH.Team != Variables.Var.Me.Team && Utils.SleepCheck("Lasthit.Stop"))
                    {
                        Variables.Var.Me.Hold();
                        Variables.Var.Me.Attack(Variables.Var.CreeptargetH);
                        Utils.Sleep((float) Variables.Var.HeroAPoint / 2 + Game.Ping, "Lasthit.Stop");
                    }
                }
                else if (Variables.Var.Me.Distance2D(Variables.Var.CreeptargetH) >= AttackRange() && Utils.SleepCheck("Lasthit.Walk"))
                {
                    Variables.Var.Me.Move(Variables.Var.CreeptargetH.Position);
                    Utils.Sleep(100 + Game.Ping, "Lasthit.Walk");
                }
            }
            else
            {
                Variables.Var.Target = MenuStructure.MenuVar.Harass ? Variables.Var.Me.BestAATarget() : null;
                Orbwalking.Orbwalk(Variables.Var.Target, 500);
            }
        }

        public static void SummonFarm()
        {
            if (Variables.Var.Summons.Count == 0) return;
            foreach (var summon in Variables.Var.Summons)
            {
                Variables.Var.CreeptargetS = KillableCreep(summon.Key, false, true, 99);

                if (Variables.Var.CreeptargetS != null && Variables.Var.CreeptargetS.IsValid && Variables.Var.CreeptargetS.IsVisible &&
                    Variables.Var.CreeptargetS.IsAlive && Utils.SleepCheck(summon.Key.Handle + "Lasthit.Attack"))
                {
                    summon.Key.Attack(Variables.Var.CreeptargetS);
                    Utils.Sleep(summon.Key.SecondsPerAttack * 300 + Game.Ping, summon.Key.Handle + "Lasthit.Attack");
                }
            }
        }

        public static void SummonLastHit()
        {
            if (Variables.Var.Summons.Count == 0 || Variables.Var.Summons.All(x => !x.Key.IsAlive)) return;
            if (!Variables.Var.SummonsDisableAaKeyPressed)
            {
                Common.AutoattackSummons(0);
                Variables.Var.SummonsDisableAaKeyPressed = true;
                Variables.Var.SummonsAutoAttackTypeDef = false;
            }
            foreach (var summon in Variables.Var.Summons.Where((x => x.Key.IsAlive)))
            {
                var attackRange = summon.Key.AttackRange;
                Variables.Var.CreeptargetS = KillableCreep(summon.Key, false, true, 3);
                if (Variables.Var.CreeptargetS != null && Variables.Var.CreeptargetS.IsValid && Variables.Var.CreeptargetS.IsVisible &&
                    Variables.Var.CreeptargetS.IsAlive &&
                    Variables.Var.CreeptargetS.Health < GetDamageOnUnit(summon.Key, Variables.Var.CreeptargetS, 0) * 3)
                {
                    var getDamage = GetDamageOnUnit(summon.Key, Variables.Var.CreeptargetS, 0);
                    if (Variables.Var.CreeptargetS.Distance2D(summon.Key) <= attackRange)
                    {
                        if (Variables.Var.CreeptargetS.Health < getDamage)
                        {
                            if (summon.Key.NetworkActivity != NetworkActivity.Attack &&
                                Utils.SleepCheck("Lasthit.Attack" + summon.Key.Handle) ||
                                !Utils.SleepCheck("Lasthit.Harass" + summon.Key.Handle))
                                summon.Key.Attack(Variables.Var.CreeptargetS);
                            Utils.Sleep(summon.Key.SecondsPerAttack * 1000 + Game.Ping,"Lasthit.Attack" + summon.Key.Handle);
                        }
                        else if (Variables.Var.CreeptargetS.Health > getDamage && Utils.SleepCheck("Lasthit.Stop"+ summon.Key.Handle))
                        {
                            summon.Key.Hold();
                            Utils.Sleep(300 + Game.Ping, "Lasthit.Stop" + summon.Key.Handle);
                        }
                    }
                    else if (summon.Key.Distance2D(Variables.Var.CreeptargetS) >= attackRange &&
                                Utils.SleepCheck("Lasthit.Walk" + summon.Key.Handle))
                    {
                        summon.Key.Move(Variables.Var.CreeptargetS.Position);
                        Utils.Sleep(300 + Game.Ping, "Lasthit.Walk" + summon.Key.Handle);
                    }
                }
                else if (MenuStructure.MenuVar.SummonsHarass && summon.Key.Distance2D(Variables.Var.Target) < 1000 &&
                            Utils.SleepCheck("Lasthit.Harass" + summon.Key.Handle))
                {
                    summon.Key.Attack(Variables.Var.Target);
                    Utils.Sleep(1000 + Game.Ping, "Lasthit.Harass" + summon.Key.Handle);
                }
            }
        }

        private static double GetDamageOnUnit(Unit unit, Unit minion, double bonusdamage)
        {
            double modif = 1;
            double magicdamage = 0;
            double physDamage = unit.MinimumDamage + unit.BonusDamage;
            if (unit.Handle == Variables.Var.Me.Handle)
            {
                var quellingBlade = unit.FindItem("item_quelling_blade");
                if (quellingBlade != null && minion.Team != unit.Team)
                {
                    if (unit.IsRanged)
                    {
                        physDamage = unit.MinimumDamage * 1.15 + unit.BonusDamage;
                    }
                    else
                    {
                        physDamage = unit.MinimumDamage * 1.4 + unit.BonusDamage;
                    }
                }
                switch (unit.ClassID)
                {
                    case ClassID.CDOTA_Unit_Hero_AntiMage:
                        if (minion.MaximumMana > 0 && minion.Mana > 0 && Variables.Var.Q.Level > 0 && minion.Team != unit.Team)
                            bonusdamage += (Variables.Var.Q.Level - 1) * 12 + 28 * 0.6;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Viper:
                        if (Variables.Var.W.Level > 0 && minion.Team != unit.Team)
                        {
                            var nethertoxindmg = Variables.Var.W.Level * 2.5;
                            //var percent = Math.Floor((double) unit.Health / unit.MaximumHealth * 100);
                            //if (percent > 80 && percent <= 100)
                            //    bonusdamage2 = nethertoxindmg * 0.5;
                            //else if (percent > 60 && percent <= 80)
                            //    bonusdamage2 = nethertoxindmg * 1;
                            //else if (percent > 40 && percent <= 60)
                            //    bonusdamage2 = nethertoxindmg * 2;
                            //else if (percent > 20 && percent <= 40)
                            //    bonusdamage2 = nethertoxindmg * 4;
                            //else if (percent > 0 && percent <= 20)
                            //    bonusdamage2 = nethertoxindmg * 8;
                        }
                        break;

                    case ClassID.CDOTA_Unit_Hero_Ursa:
                        var furymodif = 0;
                        if (unit.Modifiers.Any(x => x.Name == "modifier_ursa_fury_swipes_damage_increase"))
                            furymodif =
                                minion.Modifiers.Find(x => x.Name == "modifier_ursa_fury_swipes_damage_increase")
                                    .StackCount;
                        if (Variables.Var.E.Level > 0)
                        {
                            if (furymodif > 0)
                                bonusdamage += furymodif * ((Variables.Var.E.Level - 1) * 5 + 15);
                            else
                                bonusdamage += (Variables.Var.E.Level - 1) * 5 + 15;
                        }
                        break;

                    case ClassID.CDOTA_Unit_Hero_BountyHunter:
                        if (Variables.Var.W.Level > 0 && Variables.Var.W.AbilityState == AbilityState.Ready)
                            bonusdamage += physDamage * ((Variables.Var.W.Level - 1) * 0.25 + 0.50);
                        break;

                    case ClassID.CDOTA_Unit_Hero_Weaver:
                        if (Variables.Var.E.Level > 0 && Variables.Var.E.AbilityState == AbilityState.Ready)
                            bonusdamage += physDamage;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Kunkka:
                        if (Variables.Var.W.Level > 0 && Variables.Var.W.AbilityState == AbilityState.Ready && Variables.Var.W.IsAutoCastEnabled)
                            bonusdamage += (Variables.Var.W.Level - 1) * 15 + 25;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Juggernaut:
                        if (Variables.Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Brewmaster:
                        if (Variables.Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage;
                        break;

                    case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                        if (Variables.Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage * ((Variables.Var.E.Level - 1) * 0.5 + 0.25);
                        break;

                    case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                        if (Variables.Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage * ((Variables.Var.E.Level - 1) * 0.5 + 0.5);
                        break;

                    case ClassID.CDOTA_Unit_Hero_Life_Stealer:
                        if (Variables.Var.E.Level > 0)
                            bonusdamage += minion.Health * ((Variables.Var.E.Level - 1) * 0.01 + 0.045);
                        break;

                    case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                        if (Variables.Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage * ((Variables.Var.E.Level - 1) * 1.1 + 1.3);
                        break;
                }
                if (unit.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload"))
                {
                    magicdamage += (Variables.Var.E.Level - 1) * 20 + 30;
                }
                if (unit.Modifiers.Any(x => x.Name == "modifier_chilling_touch"))
                {
                    magicdamage += (Variables.Var.E.Level - 1) * 20 + 50;
                }
                if (unit.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && Variables.Var.W.Level > 0 &&
                    MenuStructure.MenuVar.UseSpell &&
                    unit.Distance2D(minion) <= AttackRange())
                {
                    magicdamage += (Variables.Var.W.Level - 1) * 6 + 6;
                }
            }
            if (unit.Modifiers.Any(x => x.Name == "modifier_bloodseeker_bloodrage"))
            {
                modif = modif *
                        (ObjectManager.GetEntities<Hero>()
                            .First(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker)
                            .Spellbook.Spell1.Level - 1) * 0.05 + 1.25;
            }
            if (Variables.Var.Summons != null && Variables.Var.Summons.Count > 1 && Variables.Var.Summons.Any(x => x.Key.IsAlive) && 
                Variables.Var.Summons.Any(x => x.Key.Handle != unit.Handle) && Variables.Var.CreeptargetS != null)
            {
                if (Variables.Var.CreeptargetH == null ||
                    (Variables.Var.CreeptargetH.Handle != Variables.Var.CreeptargetS.Handle && Variables.Var.Me.Handle != unit.Handle))
                {
                    bonusdamage =
                        Variables.Var.Summons.Where(
                            x =>
                                x.Key.Handle != unit.Handle && x.Key.CanAttack() &&
                                (Math.Abs(x.Key.Distance2D(minion) - x.Key.AttackRange) < 100 ||
                                 Math.Abs(x.Key.Distance2D(minion) - unit.Distance2D(minion)) < 100))
                            .Aggregate(bonusdamage, (current, x) => current + x.Key.MinimumDamage + x.Key.BonusDamage);
                }
                else if (Variables.Var.CreeptargetH.Handle == Variables.Var.CreeptargetS.Handle && Variables.Var.Me.Handle != unit.Handle)
                {
                    if (Variables.Var.Summons.Any(x => x.Key.Handle != unit.Handle))
                        bonusdamage +=
                            Variables.Var.Summons.Where(
                                x =>
                                    x.Key.Handle != unit.Handle && x.Key.CanAttack() &&
                                    x.Key.Distance2D(minion) < x.Key.AttackRange)
                                .Aggregate(bonusdamage,
                                    (current, x) => current + x.Key.MinimumDamage + x.Key.BonusDamage);
                    if (Variables.Var.Me.Distance2D(minion) < Variables.Var.Me.AttackRange && Variables.Var.Me.CanAttack())
                        bonusdamage += Variables.Var.Me.MinimumDamage + Variables.Var.Me.BonusDamage;
                }
                else if (Variables.Var.Me.Handle == unit.Handle && Variables.Var.CreeptargetH.Handle == Variables.Var.CreeptargetS.Handle)
                {
                    bonusdamage +=
                        Variables.Var.Summons.Where(x => x.Key.Distance2D(minion) < x.Key.AttackRange && x.Key.CanAttack())
                            .Aggregate(bonusdamage, (current, x) => current + x.Key.MinimumDamage + x.Key.BonusDamage);
                }
            }

            var damageMp = 1 - 0.06 * minion.Armor / (1 + 0.06 * Math.Abs(minion.Armor));
            magicdamage = magicdamage * (1 - minion.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage) * damageMp + magicdamage) * modif;
            if (minion.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                minion.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }
            if (realDamage > minion.MaximumHealth)
                realDamage = minion.Health + 10;

            return realDamage;
        }

        private static double GetDamageOnUnitForDrawhpbar(Unit unit, double bonusdamage)
        {
            var quellingBlade = Variables.Var.Me.FindItem("item_quelling_blade");
            double modif = 1;
            double magicdamage = 0;
            double physDamage = Variables.Var.Me.MinimumDamage + Variables.Var.Me.BonusDamage;
            if (quellingBlade != null && unit.Team != Variables.Var.Me.Team)
            {
                if (Variables.Var.Me.IsRanged)
                {
                    physDamage = Variables.Var.Me.MinimumDamage * 1.15 + Variables.Var.Me.BonusDamage;
                }
                else
                {
                    physDamage = Variables.Var.Me.MinimumDamage * 1.4 + Variables.Var.Me.BonusDamage;
                }
            }
            double bonusdamage2 = 0;
            switch (Variables.Var.Me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_AntiMage:
                    if (unit.MaximumMana > 0 && unit.Mana > 0 && Variables.Var.Q.Level > 0 && unit.Team != Variables.Var.Me.Team)
                        bonusdamage2 = (Variables.Var.Q.Level - 1) * 12 + 28 * 0.6;
                    break;

                case ClassID.CDOTA_Unit_Hero_Viper:
                    if (Variables.Var.W.Level > 0 && unit.Team != Variables.Var.Me.Team)
                    {
                        var nethertoxindmg = Variables.Var.W.Level * 2.5;
                        //var percent = Math.Floor((double) unit.Health / unit.MaximumHealth * 100);
                        //if (percent > 80 && percent <= 100)
                        //    bonusdamage2 = nethertoxindmg * 0.5;
                        //else if (percent > 60 && percent <= 80)
                        //    bonusdamage2 = nethertoxindmg * 1;
                        //else if (percent > 40 && percent <= 60)
                        //    bonusdamage2 = nethertoxindmg * 2;
                        //else if (percent > 20 && percent <= 40)
                        //    bonusdamage2 = nethertoxindmg * 4;
                        //else if (percent > 0 && percent <= 20)
                        //    bonusdamage2 = nethertoxindmg * 8;
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Ursa:
                    var furymodif = 0;
                    if (Variables.Var.Me.Modifiers.Any(x => x.Name == "modifier_ursa_fury_swipes_damage_increase"))
                        furymodif =
                            unit.Modifiers.Find(x => x.Name == "modifier_ursa_fury_swipes_damage_increase").StackCount;
                    if (Variables.Var.E.Level > 0)
                    {
                        if (furymodif > 0)
                            bonusdamage2 = furymodif * ((Variables.Var.E.Level - 1) * 5 + 15);
                        else
                            bonusdamage2 = (Variables.Var.E.Level - 1) * 5 + 15;
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_BountyHunter:
                    if (Variables.Var.W.Level > 0 && Variables.Var.W.AbilityState == AbilityState.Ready)
                        bonusdamage2 = physDamage * ((Variables.Var.W.Level - 1) * 0.25 + 0.50);
                    break;

                case ClassID.CDOTA_Unit_Hero_Weaver:
                    if (Variables.Var.E.Level > 0 && Variables.Var.E.AbilityState == AbilityState.Ready)
                        bonusdamage2 = physDamage;
                    break;

                case ClassID.CDOTA_Unit_Hero_Kunkka:
                    if (Variables.Var.W.Level > 0 && Variables.Var.W.AbilityState == AbilityState.Ready && Variables.Var.W.IsAutoCastEnabled)
                        bonusdamage2 = (Variables.Var.W.Level - 1) * 15 + 25;
                    break;

                case ClassID.CDOTA_Unit_Hero_Juggernaut:
                    if (Variables.Var.E.Level > 0)
                        if (Variables.Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage;
                    break;

                case ClassID.CDOTA_Unit_Hero_Brewmaster:
                    if (Variables.Var.E.Level > 0)
                        if (Variables.Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage;
                    break;

                case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                    if (Variables.Var.E.Level > 0)
                        if (Variables.Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage * ((Variables.Var.E.Level - 1) * 0.5 + 0.25);
                    break;

                case ClassID.CDOTA_Unit_Hero_Clinkz:
                    if (Variables.Var.W.Level > 0 && Variables.Var.W.IsAutoCastEnabled && unit.Team != Variables.Var.Me.Team)
                        bonusdamage2 = (Variables.Var.W.Level - 1) * 10 + 30;
                    break;

                case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                    if (Variables.Var.E.Level > 0)
                        if (Variables.Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage * ((Variables.Var.E.Level - 1) * 0.5 + 0.5);
                    break;

                case ClassID.CDOTA_Unit_Hero_Life_Stealer:
                    if (Variables.Var.W.Level > 0)
                        bonusdamage2 = unit.Health * ((Variables.Var.W.Level - 1) * 0.01 + 0.045);
                    break;

                case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                    if (Variables.Var.R.Level > 0)
                        if (Variables.Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage * ((Variables.Var.R.Level - 1) * 1.1 + 1.3);
                    break;
            }

            if (Variables.Var.Me.Modifiers.Any(x => x.Name == "modifier_bloodseeker_bloodrage"))
            {
                modif = modif *
                        (ObjectManager.GetEntities<Hero>()
                            .First(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker)
                            .Spellbook.Spell1.Level - 1) * 0.05 + 1.25;
            }
            if (Variables.Var.Me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload"))
            {
                magicdamage = magicdamage + (Variables.Var.E.Level - 1) * 20 + 30;
            }
            if (Variables.Var.Me.Modifiers.Any(x => x.Name == "modifier_chilling_touch"))
            {
                magicdamage = magicdamage + (Variables.Var.E.Level - 1) * 20 + 50;
            }
            if (Variables.Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && Variables.Var.W.Level > 0 && MenuStructure.MenuVar.UseSpell &&
                Variables.Var.Me.Distance2D(unit) <= AttackRange())
            {
                magicdamage = magicdamage + (Variables.Var.W.Level - 1) * 6 + 6;
            }

            bonusdamage = bonusdamage + bonusdamage2;
            var damageMp = 1 - 0.06 * unit.Armor / (1 + 0.06 * Math.Abs(unit.Armor));
            magicdamage = magicdamage * (1 - unit.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage) * damageMp + magicdamage) * modif;
            if (unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                unit.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }
            if (realDamage > unit.MaximumHealth)
                realDamage = unit.Health + 10;

            return realDamage;
        }

        private static Unit GetLowestHpCreep(Unit source, float range)
        {
            try
            {
                var lowestHp =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Additive ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Barracks ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Building ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creature) &&
                                 x.IsAlive && x.IsVisible && x.Team != source.Team &&
                                 x.Distance2D(source) < range)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                return lowestHp;
            }
            catch (Exception)
            {

                    Common.PrintError("GetLowestHpCreep Error");
            }
            return null;
        }

        private static Unit GetMyLowestHpCreep(Unit source, float range)
        {
            try
            {
                return ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Additive ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Barracks ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Building ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creature) &&
                                 x.IsAlive && x.IsVisible && x.Team == source.Team &&
                                 x.Distance2D(source) < range)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
            }
            catch (Exception)
            {
                Common.PrintError("Error GetAllLowestHpCreep");
            }
            return null;
        }

        private static Unit KillableCreep(Unit unit, bool islaneclear, bool summon, double x)
        {
            try
            {
                Unit minion, miniondenie;
                if (summon)
                {
                    minion = Variables.Var.Summons[unit].DefaultIfEmpty(null).FirstOrDefault(s => s.Team != Variables.Var.Me.Team);
                    miniondenie = Variables.Var.Summons[unit].DefaultIfEmpty(null).FirstOrDefault(s => s.Team == Variables.Var.Me.Team);
                }
                else
                {
                    minion = GetLowestHpCreep(unit, Common.GetOutRange(unit));
                    miniondenie = GetMyLowestHpCreep(unit, Common.GetOutRange(unit));
                }
                if (minion == null && miniondenie == null) return null;
                var percent = minion.Health / minion.MaximumHealth * 100;
                if ((miniondenie.Health > GetDamageOnUnit(unit, miniondenie, 0) ||
                    minion.Health < GetDamageOnUnit(unit, minion, 0) + 30) &&
                    (percent < 90 || GetDamageOnUnit(unit, minion, 0) > minion.MaximumHealth) &&
                    minion.Health < GetDamageOnUnit(unit, minion, 0) * x && !MenuStructure.MenuVar.Sapport)
                {
                    if (unit.CanAttack())
                        return minion;
                }
                else if (islaneclear)
                {
                    return minion;
                }

                if (MenuStructure.MenuVar.Denie && !summon || MenuStructure.MenuVar.SummonsDenie && summon)
                {
                    if (miniondenie.Health <= GetDamageOnUnit(unit, miniondenie, 0) * x * 0.75 &&
                        miniondenie.Health <= miniondenie.MaximumHealth / 2 &&
                        miniondenie.Team == unit.Team)
                    {
                        if (unit.CanAttack())
                            return miniondenie;
                    }
                }

                if (MenuStructure.MenuVar.Aoc && !summon || MenuStructure.MenuVar.SummonsAoc && summon)
                {
                    if (miniondenie.Health <= miniondenie.MaximumHealth / 2 &&
                        miniondenie.Health > GetDamageOnUnit(unit, miniondenie, 0) * x * 0.75 &&
                        miniondenie.Team == unit.Team)
                        if (unit.CanAttack())
                            return miniondenie;
                }
                return null;
            }
            catch (Exception)
            {
                //
            }
            return null;
        }

        private static void SpellCast()
        {
            try
            {
                foreach (var creep in Variables.Var.Creeps.Where(x => x.Team != Variables.Var.Me.Team)
                    .OrderByDescending(creep => creep.Health))
                {
                    double damage = 0;
                    switch (Variables.Var.Me.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Zuus:
                            if (Variables.Var.Q.Level > 0 && Variables.Var.Q.CanBeCasted() && Variables.Var.Me.Distance2D(creep) > AttackRange())
                            {
                                damage = ((Variables.Var.Q.Level - 1) * 15 + 85) * (1 - creep.MagicDamageResist);
                                if (damage > creep.Health)
                                {
                                    Variables.Var.Q.UseAbility(creep);
                                    Utils.Sleep(Variables.Var.Q.GetCastPoint(Variables.Var.Q.Level) * 1000 + 50 + Game.Ping, "Lasthit.Cast");
                                    Utils.Sleep(Variables.Var.Q.GetCooldown(Variables.Var.Q.Level) * 1000 + 50 + Game.Ping, "Lasthit.Cooldown");
                                }
                            }
                            break;

                        case ClassID.CDOTA_Unit_Hero_Bristleback:
                            if (Variables.Var.W.Level > 0 && Variables.Var.W.CanBeCasted() && Variables.Var.Me.Distance2D(creep) > AttackRange())
                            {
                                double quillSprayDmg = 0;
                                if (creep.Modifiers.Any(
                                        x =>
                                            x.Name == "modifier_bristleback_quill_spray_stack" ||
                                            x.Name == "modifier_bristleback_quill_spray"))
                                    quillSprayDmg =
                                        creep.Modifiers.Find(
                                            x =>
                                                x.Name == "modifier_bristleback_quill_spray_stack" ||
                                                x.Name == "modifier_bristleback_quill_spray").StackCount * 30 +
                                        (Variables.Var.W.Level - 1) * 2;
                                damage = ((Variables.Var.W.Level - 1) * 20 + 20 + quillSprayDmg) *
                                         (1 - 0.06 * creep.Armor / (1 + 0.06 * creep.Armor));
                                if (damage > creep.Health && Variables.Var.W.CastRange > Variables.Var.Me.Distance2D(creep))
                                {
                                    Variables.Var.W.UseAbility();
                                    Utils.Sleep(Variables.Var.W.GetCastPoint(Variables.Var.W.Level) * 1000 + 50 + Game.Ping, "Lasthit.Cast");
                                    Utils.Sleep(Variables.Var.W.GetCooldown(Variables.Var.W.Level) * 1000 + 50 + Game.Ping, "Lasthit.Cooldown");
                                }
                            }
                            break;

                        case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                            if (Variables.Var.Q.Level > 0 && Variables.Var.Q.CanBeCasted() && Variables.Var.Me.Distance2D(creep) > AttackRange())
                            {
                                var time = 300 + Variables.Var.Me.Distance2D(creep) / Variables.Var.Q.GetProjectileSpeed();
                                if (time < creep.SecondsPerAttack * 1000)
                                    damage = ((Variables.Var.Q.Level - 1) * 40 + 60) * (1 - 0.06 * creep.Armor / (1 + 0.06 * creep.Armor));
                                if (damage > creep.Health)
                                {
                                    Variables.Var.Q.UseAbility(creep);
                                    Utils.Sleep(Variables.Var.Q.GetCastPoint(Variables.Var.Q.Level) * 1000 + 50 + Game.Ping, "Lasthit.Cast");
                                    Utils.Sleep(6 * 1000 + Game.Ping, "Lasthit.Cooldown");
                                }
                            }
                            break;

                        case ClassID.CDOTA_Unit_Hero_Pudge:
                            if (Variables.Var.W.Level > 0)
                            {
                                if (Variables.Var.CreeptargetH != null && creep.Handle == Variables.Var.CreeptargetH.Handle &&
                                    Variables.Var.Me.Distance2D(creep) <= AttackRange())
                                {
                                    damage = GetDamageOnUnit(Variables.Var.Me, creep, 0);
                                    if (damage > creep.Health && !Variables.Var.W.IsToggled && Variables.Var.Me.IsAttacking())
                                    {
                                        Variables.Var.W.ToggleAbility();
                                        Utils.Sleep(200 + Game.Ping, "Lasthit.Cooldown");
                                    }
                                }
                                if (Variables.Var.W.IsToggled)
                                {
                                    Variables.Var.W.ToggleAbility();
                                    Utils.Sleep((float) Variables.Var.HeroAPoint + Game.Ping, "Lasthit.Cooldown");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        #endregion Main

        public static float GetProjectileSpeed(Entity unit)
        {
            return Variables.Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                ? 800
                : UnitDatabase.GetByName(unit.Name).ProjectileSpeed;
        }

        public static float AttackRange()
        {
            if (!Utils.SleepCheck("AttackRange"))
            {
                return _attackRange;
            }
            Utils.Sleep(1000, "AttackRange");

            if (Variables.Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord)
                _attackRange = Variables.Var.Q.IsToggled ? 128 : Variables.Var.Me.GetAttackRange();
            else
                _attackRange = Variables.Var.Me.GetAttackRange();

            return _attackRange;
        }
    }
}
