using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta
{

    private void ManageNations(TIFactionState faction)
    {
        int day = this.gameTime.currentTime.day;
        if (day <= 28 && day % 14 == AIDailyFactionPlanner.factionAIData[faction].every14DaysOffset)
        {
            List<FactionGoal_Nation> list = faction.GoalsOfType(TIFactionGoalState.NationPriorityModifyingGoals, false, true).ConvertAll<FactionGoal_Nation>((TIFactionGoalState x) => (FactionGoal_Nation)x);
            using (List<TINationState>.Enumerator enumerator = faction.nationsWithMyControlPoints.GetEnumerator())
            {
                Func<TINationState, bool> <> 9__16;
                Func<TINationState, bool> <> 9__19;
                Func<FactionResource, float> <> 9__21;
                while (enumerator.MoveNext())
                {
                    TINationState nation = enumerator.Current;
                    List<TIControlPoint> list2 = nation.FactionControlPoints(faction, true, false, true);
                    int count = list2.Count;
                    List<FactionGoal_Nation> list3 = (from x in list
                                                      where x.target() == nation
                                                      select x).ToList<FactionGoal_Nation>();
                    int num = -1;
                    int num2 = 0;
                    bool flag = nation.executiveFaction == faction;
                    if (faction.IsAlienFaction)
                    {
                        using (List<TIControlPoint>.Enumerator enumerator2 = list2.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                TIControlPoint controlPoint = enumerator2.Current;
                                faction.playerControl.StartAction(new ApplyPriorityPresetToControlPoint(controlPoint, faction, faction.defaultPriorityPresetTemplateName));
                            }
                            continue;
                        }
                    }
                    Dictionary<PriorityType, int> dictionary = Enums.PriorityTypes.ToDictionary((PriorityType x) => x, (PriorityType x) => 0);
                    FactionGoal_Nation factionGoal_Nation = list3.FirstOrDefault((FactionGoal_Nation x) => x.GetGoalType() == GoalType.NeutralizeNation || x.GetGoalType() == GoalType.PillageNation);
                    if (factionGoal_Nation != null)
                    {
                        dictionary = factionGoal_Nation.prioritiesAsNation;
                    }
                    else
                    {
                        float IPs = (float)count * (nation.BaseInvestmentPoints_month() / (float)nation.numControlPoints);
                        Dictionary<PriorityType, bool> dictionary2 = Enums.PriorityTypes.ToDictionary((PriorityType x) => x, (PriorityType x) => nation.ValidPriority(x) && (nation.DeltaToInvestmentThreshhold(x) < IPs || nation.GetAccumulatedInvestmentPoints(x) / TemplateManager.global.GetRequiredInvestmentPoints(x) > 0.9f));
                        bool flag2 = faction.IsAlienFaction || IPs <= 5f || (float)nation.numNuclearWeapons >= 10f * faction.aiValues.wantEarthWarCapability || !flag;
                        bool flag3;
                        if (nation.wars.Count > 0)
                        {
                            Func<TIArmyState, bool> <> 9__11;
                            Func<TINationState, bool> <> 9__10;
                            flag3 = (from war in nation.currentWarStates
                                     where !war.stalemate
                                     select war).Any(delegate (TIWarState war)
                                     {
                                         IEnumerable<TINationState> source = war.EnemyAlliance(nation);
                                         Func<TINationState, bool> predicate4;
                                         if ((predicate4 = <> 9__10) == null)
                                         {
                                             predicate4 = (<> 9__10 = delegate (TINationState enemy)
                                             {
                                                 if (enemy.WinningWarAgainst(nation))
                                                 {
                                                     IEnumerable<TIArmyState> armies = enemy.armies;
                                                     Func<TIArmyState, bool> predicate5;
                                                     if ((predicate5 = <> 9__11) == null)
                                                     {
                                                         predicate5 = (<> 9__11 = ((TIArmyState enemyArmy) => enemyArmy.CanJourneyTo(nation.capital, null)));
                                                     }
                                                     return armies.Any(predicate5);
                                                 }
                                                 return false;
                                             });
                                         }
                                         return source.Any(predicate4);
                                     });
                        }
                        else
                        {
                            flag3 = false;
                        }
                        bool flag4 = flag3;
                        bool flag5 = nation.ArmiesThreateningCapital() > 0;
                        int num3 = 0;
                        if (flag4)
                        {
                            num3 = 10;
                            if (nation.ValidPriority(PriorityType.Military))
                            {
                                dictionary[PriorityType.Military] = 3;
                            }
                            if (!nation.civilWar && !flag2 && (dictionary2[PriorityType.InitiateNuclearProgram] || dictionary2[PriorityType.BuildNuclearWeapons]))
                            {
                                dictionary[PriorityType.InitiateNuclearProgram] = 3;
                                dictionary[PriorityType.BuildNuclearWeapons] = 3;
                            }
                            if (!flag5 || dictionary2[PriorityType.BuildArmy])
                            {
                                if (nation.ValidPriority(PriorityType.BuildArmy))
                                {
                                    dictionary[PriorityType.BuildArmy] = ((nation.controlPoints[nation.GetNextArmyControlPointIdx()].faction == faction || flag) ? 3 : 0);
                                }
                                else if (!flag5 && nation.ValidPriority(PriorityType.UpgradeArmy))
                                {
                                    dictionary[PriorityType.UpgradeArmy] = ((nation.controlPoints[nation.GetNextArmyControlPointIdx()].faction == faction || flag) ? 3 : 0);
                                }
                                else if (!flag5 && !flag2)
                                {
                                    dictionary[PriorityType.InitiateNuclearProgram] = 3;
                                    dictionary[PriorityType.BuildNuclearWeapons] = 3;
                                }
                            }
                        }
                        else
                        {
                            if (nation.civilWar)
                            {
                                if (!flag)
                                {
                                    if (!list.None((FactionGoal_Nation x) => x.GetGoalType() == GoalType.CaptureNationDirty))
                                    {
                                        goto IL_5F0;
                                    }
                                }
                                num3 = 9;
                                dictionary[PriorityType.Military] = 3;
                                if (IPs <= 2f)
                                {
                                    goto IL_1AEC;
                                }
                                if (nation.cohesionWarning)
                                {
                                    dictionary[PriorityType.Unity] = (((double)nation.democracy <= 6.5) ? 2 : 1);
                                }
                                if (IPs > 4f && nation.severeInequalityWarning)
                                {
                                    dictionary[PriorityType.Welfare] = ((faction.aiValues.protectHumanLife > 0.5f) ? 1 : 0) + (faction.cynical ? 0 : 2);
                                }
                                if (dictionary2[PriorityType.BuildArmy])
                                {
                                    dictionary[PriorityType.BuildArmy] = ((nation.controlPoints[nation.GetNextArmyControlPointIdx()].faction == faction) ? 3 : 1);
                                    goto IL_1AEC;
                                }
                                goto IL_1AEC;
                            }
                        IL_5F0:
                            if (nation.belligerentInActiveWar)
                            {
                                num3 = nation.wars.Sum((TINationState x) => x.numStandardArmies);
                                if (nation.ValidPriority(PriorityType.Military))
                                {
                                    dictionary[PriorityType.Military] = 3;
                                }
                                if (!nation.civilWar && !flag2)
                                {
                                    if (nation.wars.Any((TINationState x) => x.numNuclearWeapons > 0) && (IPs > 12f || dictionary2[PriorityType.InitiateNuclearProgram] || dictionary2[PriorityType.BuildNuclearWeapons]))
                                    {
                                        dictionary[PriorityType.InitiateNuclearProgram] = (dictionary2[PriorityType.BuildArmy] ? 1 : 3);
                                        dictionary[PriorityType.BuildNuclearWeapons] = (dictionary2[PriorityType.BuildArmy] ? 1 : 3);
                                    }
                                }
                                if (nation.regions.Any((TIRegionState x) => x.underBombardment))
                                {
                                    goto IL_770;
                                }
                                if (nation.wars.Any((TINationState x) => x.numNuclearWeapons > 1))
                                {
                                    goto IL_770;
                                }
                            IL_796:
                                if (dictionary2[PriorityType.BuildArmy] || IPs > 6f || flag)
                                {
                                    dictionary[PriorityType.BuildArmy] = (int)Mathf.Clamp(IPs / 4f, 0f, (float)((nation.controlPoints[nation.GetNextArmyControlPointIdx()].faction == faction) ? 3 : 1));
                                    dictionary[PriorityType.UpgradeArmy] = (int)Mathf.Clamp(IPs / 4f, 0f, (float)((nation.controlPoints[nation.GetNextArmyControlPointIdx()].faction == faction) ? 3 : 1));
                                }
                                if (nation.cohesionWarning)
                                {
                                    dictionary[PriorityType.Unity] = ((IPs > 2f && (double)nation.democracy <= 6.5) ? 1 : 0);
                                    dictionary[PriorityType.Knowledge] = (int)Mathf.Clamp(IPs / 6f, 0f, 3f);
                                    goto IL_BAB;
                                }
                                goto IL_BAB;
                            IL_770:
                                dictionary[PriorityType.BuildSpaceDefenses] = (int)Mathf.Clamp(IPs / 4f, 0f, 3f);
                                goto IL_796;
                            }
                            if (nation.severeInequalityWarning)
                            {
                                dictionary[PriorityType.Welfare] = ((faction.aiValues.protectHumanLife > 0.5f) ? 1 : 0) + (faction.cynical ? 1 : 2);
                                num3++;
                            }
                            if (nation.unrestMajorWarning)
                            {
                                dictionary[PriorityType.Military] = (int)Mathf.Clamp((10f - nation.democracy) / 3f, 0f, 3f);
                                if (nation.cohesionWarning)
                                {
                                    dictionary[PriorityType.Unity] = ((IPs > 5f && (double)nation.democracy <= 6.5) ? 2 : 1);
                                }
                                dictionary[PriorityType.Economy] = Mathf.Max(dictionary[PriorityType.Economy], (int)Mathf.Clamp(nation.democracy / 3f, 0f, 3f));
                                num3 += 2;
                            }
                            else if (nation.cohesionWarning)
                            {
                                if (nation.perCapitaGDP < 6000f)
                                {
                                    dictionary[PriorityType.Economy] = Mathf.Max(dictionary[PriorityType.Economy], (int)Mathf.Clamp(nation.democracy / 3f, 1f, 3f));
                                }
                                else if (nation.inequalityWarning)
                                {
                                    dictionary[PriorityType.Welfare] = Mathf.Max(dictionary[PriorityType.Welfare], ((faction.aiValues.protectHumanLife > 0.5f) ? 2 : 1) + (faction.cynical ? 0 : 1));
                                }
                                if (nation.majorCohesionWarning)
                                {
                                    dictionary[PriorityType.Unity] = 3;
                                    num3++;
                                }
                                else
                                {
                                    dictionary[PriorityType.Unity] = ((IPs > 2f && nation.education + nation.democracy <= 15f) ? ((int)Mathf.Clamp(IPs / 6f, 1f, 3f)) : ((IPs > 6f) ? 1 : 0));
                                }
                                dictionary[PriorityType.Knowledge] = ((IPs > 4f) ? ((int)Mathf.Clamp(IPs / 4f, 0f, 3f)) : 0);
                                num3++;
                            }
                        IL_BAB:
                            if (nation.corruption > 0.5f)
                            {
                                dictionary[PriorityType.Spoils] = Mathf.Max(dictionary[PriorityType.Spoils], 1);
                                num3++;
                            }
                            if (factionGoal_Nation == null && nation.CouncilControlPointFraction_DiscountNeutral(faction, true, false) > 0.5f)
                            {
                                if (flag4 || !nation.MajorGlobalPower)
                                {
                                    TIGameState nation2 = nation;
                                    TIFactionGoalState focusGoal = faction.focusGoal;
                                    if (!(nation2 == ((focusGoal != null) ? focusGoal.target() : null)))
                                    {
                                        goto IL_D5F;
                                    }
                                }
                                if (nation.GetPublicOpinionOfFaction(faction) < 0.25f)
                                {
                                    dictionary[PriorityType.Unity] = Mathf.Max(dictionary[PriorityType.Unity], (faction.cynical || faction.extremist) ? 3 : 2);
                                }
                                else if (nation.GetPublicOpinionOfFaction(faction) < 0.5f)
                                {
                                    dictionary[PriorityType.Unity] = Mathf.Max(dictionary[PriorityType.Unity], (faction.cynical || faction.extremist) ? 2 : 1);
                                }
                                else if (nation.GetPublicOpinionOfFaction(faction) < nation.singleIdeaCap - 0.05f)
                                {
                                    dictionary[PriorityType.Unity] = Mathf.Max(dictionary[PriorityType.Unity], (faction.cynical || faction.extremist) ? 1 : 0);
                                }
                            }
                        IL_D5F:
                            float num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                            bool flag6 = flag || (nation.CouncilControlPointFraction(faction, true, true) >= 0.5f && faction.FindGoals(TIFactionGoalState.CaptureNationGoals, faction, nation, TIFactionState.GoalFilter.none, false).Count > 0 && !faction.enemyWarFactions.Contains(nation.executiveFaction));
                            TIFactionGoalState tifactionGoalState = faction.GetManagementGoalForNation(nation, false);
                            if (tifactionGoalState == null && nation.executiveFaction == faction)
                            {
                                tifactionGoalState = faction.SetManagementGoalForNation(nation);
                            }
                            flag6 = (flag6 && tifactionGoalState != null && this.buildUpMilitaryGoals.Contains(tifactionGoalState.GetGoalType()));
                            bool flag7 = nation.controlPoints[nation.GetNextArmyControlPointIdx()].faction == faction;
                            bool flag8 = nation.GetNextNavy() != null && nation.GetNextNavy().faction == faction;
                            if (flag6 || flag7 || flag8)
                            {
                                IEnumerable<TINationState> rivals = nation.rivals;
                                Func<TINationState, bool> predicate;
                                if ((predicate = <> 9__16) == null)
                                {
                                    predicate = (<> 9__16 = ((TINationState x) => x.executiveFaction != faction));
                                }
                                int num5 = rivals.Where(predicate).Sum((TINationState x) => x.numStandardArmies) - nation.numStandardArmies - nation.allies.Sum((TINationState x) => x.numStandardArmies);
                                if (num5 > 0)
                                {
                                    if (flag6 && nation.ValidPriority(PriorityType.Military))
                                    {
                                        dictionary[PriorityType.Military] = Mathf.Max(dictionary[PriorityType.Military], (int)Mathf.Clamp((float)num5 * 0.5f, 1f, 3f));
                                    }
                                    if (num4 >= (float)(nation.HasExternalClaims() ? 8 : 12))
                                    {
                                        if ((flag6 || flag7) && nation.ValidPriority(PriorityType.BuildArmy))
                                        {
                                            dictionary[PriorityType.BuildArmy] = Mathf.Max(dictionary[PriorityType.BuildArmy], (int)Mathf.Clamp(Mathf.Min((float)num5, num4 / 8f), 0f, 3f));
                                        }
                                        if ((flag6 || flag8) && num4 >= (float)(nation.HasExternalClaims() ? 10 : 20) && nation.ValidPriority(PriorityType.UpgradeArmy))
                                        {
                                            dictionary[PriorityType.UpgradeArmy] = Mathf.Max(dictionary[PriorityType.UpgradeArmy], (int)Mathf.Clamp(Mathf.Min((float)num5, num4 / 8f), 0f, 3f));
                                        }
                                    }
                                    num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                                    if (num4 > 0f && flag6)
                                    {
                                        IEnumerable<TINationState> rivals2 = nation.rivals;
                                        Func<TINationState, bool> predicate2;
                                        if ((predicate2 = <> 9__19) == null)
                                        {
                                            predicate2 = (<> 9__19 = ((TINationState x) => x.executiveFaction != faction));
                                        }
                                        if (rivals2.Where(predicate2).Any((TINationState x) => x.numNuclearWeapons > 1) && nation.ValidPriority(PriorityType.BuildSpaceDefenses))
                                        {
                                            dictionary[PriorityType.BuildSpaceDefenses] = Mathf.Clamp(num5, 1, 3);
                                        }
                                        else if (!flag2 && nation.NumNuclearWeaponsDefendingMe() == 0)
                                        {
                                            if (nation.ValidPriority(PriorityType.InitiateNuclearProgram))
                                            {
                                                dictionary[PriorityType.InitiateNuclearProgram] = (int)Mathf.Clamp(num4, 1f, (float)Mathf.Min(3, num5));
                                            }
                                            else if (nation.ValidPriority(PriorityType.BuildNuclearWeapons))
                                            {
                                                dictionary[PriorityType.BuildNuclearWeapons] = (int)Mathf.Clamp(num4, 1f, (float)Mathf.Min(3, num5));
                                            }
                                        }
                                    }
                                    num3++;
                                    num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                                }
                            }
                            if (num4 > 0f && faction.resourceIncomeDeficiencies.Contains(FactionResource.Money))
                            {
                                if (nation.FactionControlPoints(faction, false, false, true).Count > 0 && (nation.democracy < 7.5f || faction.cynical))
                                {
                                    dictionary[PriorityType.Spoils] = Mathf.Max(dictionary[PriorityType.Spoils], Mathf.Clamp(1 + (faction.cynical ? 1 : -1) + ((nation.elitesHappy || (double)nation.corruption < 0.15) ? -2 : 1) + (faction.believers ? -1 : 1), 0, 3));
                                }
                                if (num4 - (float)num3 > 8f)
                                {
                                    dictionary[PriorityType.SpaceDevelopment] = Mathf.Max(dictionary[PriorityType.SpaceDevelopment], (int)Mathf.Clamp(num4, 0f, 3f));
                                }
                                num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                            }
                            int num6 = 0;
                            if (!AIEvaluators.Abundant(faction, FactionResource.Boost, faction.GetCurrentResourceAmount(FactionResource.Boost), faction.GetDailyIncome(FactionResource.Boost, false, false) > 0f, 1f) && (faction.GetDailyIncome(FactionResource.Boost, false, false) < 0f || num4 - (float)num3 >= 6f || faction.AvailableMissionControlMinusUnderConstruction >= 20))
                            {
                                Func<FactionResource, float> getIncomePerMonth;
                                if ((getIncomePerMonth = <> 9__21) == null)
                                {
                                    getIncomePerMonth = (<> 9__21 = ((FactionResource x) => AIEvaluators.EstimateFutureIncomePerMonth(faction, x, false, false, false)));
                                }
                                Dictionary<FactionResource, ValueTuple<bool, bool, bool>> spaceResourceIncomesChecklist = AIEvaluators.GetSpaceResourceIncomesChecklist(getIncomePerMonth);
                                if (spaceResourceIncomesChecklist.None(([TupleElementNames(new string[]
                                    {
                                        "MeetsMinimum",
                                        "MeetsRecommended",
                                        "MeetsGood"
                                    })] KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>> x) => x.Value.Item1))
                                {
                                    num6 = 3;
                                }
                                else if (spaceResourceIncomesChecklist.NotAll(([TupleElementNames(new string[]
                                    {
                                        "MeetsMinimum",
                                        "MeetsRecommended",
                                        "MeetsGood"
                                    })] KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>> x) => x.Value.Item1))
                                {
                                    num6 = 2;
                                }
                                else if (spaceResourceIncomesChecklist.None(([TupleElementNames(new string[]
                                    {
                                        "MeetsMinimum",
                                        "MeetsRecommended",
                                        "MeetsGood"
                                    })] KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>> x) => x.Value.Item2) || faction.GetDailyIncome(FactionResource.Boost, false, false) < 0f || faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
                                {
                                    num6 = 1;
                                }
                                if (num6 > 0)
                                {
                                    num3++;
                                    if (nation.ValidPriority(PriorityType.LaunchFacilities))
                                    {
                                        dictionary[PriorityType.LaunchFacilities] = (int)Mathf.Min(num4, (float)num6);
                                    }
                                    else if (nation.ValidPriority(PriorityType.SpaceflightProgram))
                                    {
                                        dictionary[PriorityType.SpaceflightProgram] = (int)Mathf.Clamp(num4, 0f, 3f);
                                    }
                                    num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                                }
                            }
                            if (!AIEvaluators.Abundant(faction, FactionResource.MissionControl, 0f, faction.AvailableMissionControlMinusUnderConstruction > 0, 1f) || faction.resourceIncomeDeficiencies.Contains(FactionResource.MissionControl))
                            {
                                num3 += ((num6 > 0) ? 0 : 1);
                                int num7 = 0;
                                if (faction.AvailableMissionControlMinusUnderConstruction < 10)
                                {
                                    num7 = 1;
                                }
                                if (nation.ValidPriority(PriorityType.MissionControl))
                                {
                                    dictionary[PriorityType.MissionControl] = (int)Mathf.Clamp(num4, (float)num7, 3f);
                                }
                                else if (nation.ValidPriority(PriorityType.SpaceflightProgram))
                                {
                                    dictionary[PriorityType.SpaceflightProgram] = Mathf.Max(dictionary[PriorityType.SpaceflightProgram], (int)Mathf.Clamp(num4, (float)num7, 3f));
                                }
                                else
                                {
                                    dictionary[PriorityType.Economy] = Mathf.Max(dictionary[PriorityType.Economy], (int)Mathf.Clamp(num4, 1f, 3f));
                                }
                                num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                            }
                            if (num3 <= 2 || dictionary.Values.Sum() <= 5)
                            {
                                dictionary[PriorityType.Economy] = (int)Mathf.Clamp(num4 / 2f - (float)num3, 0f, 3f);
                                if (faction.aiValues.gatherScience >= 0.25f || faction.resourceIncomeDeficiencies.Contains(FactionResource.Research))
                                {
                                    Dictionary<PriorityType, int> dictionary3 = dictionary;
                                    dictionary3[PriorityType.Knowledge] = dictionary3[PriorityType.Knowledge] + (1 - num3);
                                    if (faction.aiValues.gatherScience >= 1f || (faction.resourceIncomeDeficiencies.Contains(FactionResource.Research) && num4 > 1f))
                                    {
                                        dictionary3 = dictionary;
                                        dictionary3[PriorityType.Knowledge] = dictionary3[PriorityType.Knowledge] + 1;
                                        if (faction.aiValues.gatherScience > 1f && num4 > 2f)
                                        {
                                            dictionary3 = dictionary;
                                            dictionary3[PriorityType.Knowledge] = dictionary3[PriorityType.Knowledge] + 1;
                                        }
                                    }
                                }
                                dictionary[PriorityType.Knowledge] = Mathf.Clamp(dictionary[PriorityType.Knowledge], 0, 3);
                                num4 = IPs * (float)count / (float)Mathf.Max(1, num3) - (float)dictionary.Values.Sum();
                            }
                            if (num4 - 6f > 0f || num3 == 0)
                            {
                                using (List<FactionGoal_Nation>.Enumerator enumerator3 = list3.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        FactionGoal_Nation goal = enumerator3.Current;
                                        if (num4 > 0f)
                                        {
                                            IEnumerable<PriorityType> keys = goal.prioritiesAsNation.Keys;
                                            Func<PriorityType, int> keySelector;
                                            Func<PriorityType, int> <> 9__25;
                                            if ((keySelector = <> 9__25) == null)
                                            {
                                                keySelector = (<> 9__25 = ((PriorityType x) => goal.prioritiesAsNation[x]));
                                            }
                                            foreach (PriorityType priorityType in keys.OrderByDescending(keySelector))
                                            {
                                                if (!flag2 || (priorityType != PriorityType.BuildNuclearWeapons && priorityType != PriorityType.InitiateNuclearProgram))
                                                {
                                                    int num8 = dictionary[priorityType];
                                                    float a = Mathf.Min((float)goal.prioritiesAsNation[priorityType], num4);
                                                    dictionary[priorityType] = (int)Mathf.Max(a, (float)dictionary[priorityType]);
                                                    num4 -= (float)((dictionary[priorityType] - num8) * 2);
                                                    if (num4 <= 0f)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (num4 > 0f && num3 == 0)
                            {
                                foreach (PriorityType key in this.orderedPrioritiesForSpares)
                                {
                                    int b = 0;
                                    switch (key)
                                    {
                                        case PriorityType.Economy:
                                            b = faction.defaultPriorityPreset.economySetting;
                                            break;
                                        case PriorityType.Welfare:
                                            b = faction.defaultPriorityPreset.welfareSetting;
                                            break;
                                        case PriorityType.Knowledge:
                                            b = faction.defaultPriorityPreset.knowledgeSetting;
                                            break;
                                        case PriorityType.Unity:
                                            b = faction.defaultPriorityPreset.unitySetting;
                                            break;
                                        case PriorityType.Military:
                                            b = faction.defaultPriorityPreset.militarySetting;
                                            break;
                                        case PriorityType.Spoils:
                                            b = faction.defaultPriorityPreset.spoilsSetting;
                                            break;
                                        case PriorityType.SpaceDevelopment:
                                            b = faction.defaultPriorityPreset.spaceProgramSetting;
                                            break;
                                        case PriorityType.SpaceflightProgram:
                                            b = Mathf.Max(faction.defaultPriorityPreset.boostSetting, faction.defaultPriorityPreset.missionControlSetting);
                                            break;
                                        case PriorityType.LaunchFacilities:
                                            b = ((num6 > 0) ? num6 : 0);
                                            break;
                                        case PriorityType.MissionControl:
                                            b = faction.defaultPriorityPreset.missionControlSetting;
                                            break;
                                        case PriorityType.BuildArmy:
                                            b = faction.defaultPriorityPreset.armySetting;
                                            break;
                                        case PriorityType.UpgradeArmy:
                                            b = faction.defaultPriorityPreset.navySetting;
                                            break;
                                        case PriorityType.InitiateNuclearProgram:
                                        case PriorityType.BuildNuclearWeapons:
                                            b = (flag2 ? 0 : faction.defaultPriorityPreset.nuclearProgramSetting);
                                            break;
                                        case PriorityType.BuildSpaceDefenses:
                                            b = faction.defaultPriorityPreset.spaceDefenseSetting;
                                            break;
                                    }
                                    dictionary[key] = Mathf.Max(dictionary[key], b);
                                    num4 -= (float)(dictionary[key] * 3);
                                    if (num4 <= 0f)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    IL_1AEC:
                        if (dictionary.Values.Sum() == 0)
                        {
                            foreach (TIControlPoint controlPoint2 in list2)
                            {
                                faction.playerControl.StartAction(new ApplyPriorityPresetToControlPoint(controlPoint2, faction, faction.defaultPriorityPresetTemplateName));
                            }
                            foreach (PriorityType priorityType2 in Enums.PriorityTypes)
                            {
                                dictionary[priorityType2] = nation.controlPoints[0].GetControlPointPriority(priorityType2, true);
                            }
                        }
                        if (num3 < 10)
                        {
                            int num9 = nation.CountFactionControlPoints(faction, false, false, true);
                            float corruption = nation.corruption;
                            int num10 = dictionary.Sum((KeyValuePair<PriorityType, int> x) => x.Value);
                            float num11 = (float)(dictionary[PriorityType.Spoils] / Mathf.Max(1, num10));
                            int num12 = Mathf.RoundToInt((float)num10 * (corruption - num11));
                            if (num12 > 0 && !faction.resourceIncomeDeficiencies.Contains(FactionResource.Money))
                            {
                                num12 /= 2;
                            }
                            num = dictionary[PriorityType.Spoils] * num9 + num12;
                            if (num > 3 * num9 && !nation.elitesHappy)
                            {
                                num2 = 3 * num9 - num;
                            }
                        }
                    }
                    IEnumerable<PriorityType> priorityTypes2 = Enums.PriorityTypes;
                    Func<PriorityType, bool> predicate3;
                    Func<PriorityType, bool> <> 9__27;
                    if ((predicate3 = <> 9__27) == null)
                    {
                        predicate3 = (<> 9__27 = ((PriorityType x) => nation.ValidPriority(x)));
                    }
                    foreach (PriorityType priorityType3 in priorityTypes2.Where(predicate3))
                    {
                        foreach (TIControlPoint ticontrolPoint in nation.FactionControlPoints(faction, true, false, true))
                        {
                            if (priorityType3 == PriorityType.Spoils)
                            {
                                if (ticontrolPoint.benefitsDisabled)
                                {
                                    if (ticontrolPoint.GetControlPointPriority(priorityType3, false) != 0)
                                    {
                                        faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint, faction, priorityType3, 0, false));
                                    }
                                }
                                else if (num > -1)
                                {
                                    int num13 = Mathf.Clamp(num, 0, 3);
                                    faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint, faction, priorityType3, num13, false));
                                    num -= num13;
                                    if (num <= 0)
                                    {
                                        break;
                                    }
                                }
                                else if (dictionary[priorityType3] != ticontrolPoint.GetControlPointPriority(priorityType3, false))
                                {
                                    faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint, faction, priorityType3, dictionary[priorityType3], false));
                                }
                            }
                            else
                            {
                                int num14 = dictionary[priorityType3];
                                if (num2 > 0)
                                {
                                    num14--;
                                    num2--;
                                }
                                if (num14 != ticontrolPoint.GetControlPointPriority(priorityType3, false))
                                {
                                    faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint, faction, priorityType3, num14, false));
                                }
                            }
                        }
                    }
                }
            }
        }
        if (day <= 28 && this.gameTime.currentTime.day % 14 == AIDailyFactionPlanner.factionAIData[faction].every14DaysOffsetLate)
        {
            Dictionary<FactionResource, float> dictionary4 = new Dictionary<FactionResource, float>
                {
                    {
                        FactionResource.Money,
                        faction.GetCurrentResourceAmount(FactionResource.Money) * 0.75f
                    },
                    {
                        FactionResource.Influence,
                        (faction.GetCurrentResourceAmount(FactionResource.Influence) * (float)faction.councilors.Count == (float)faction.maxCouncilSize) ? 0.5f : 0.3f
                    }
                };
            List<TINationState> majorityControlNations = faction.majorityControlNations;
            foreach (TIFactionGoalState tifactionGoalState2 in (from x in faction.GoalsOfType(new List<GoalType>
                {
                    GoalType.DevelopNation,
                    GoalType.ExpandNation
                }, false, true)
                                                                orderby (float)x.importance / (float)x.target().ref_nation.numControlPoints descending
                                                                select x).ToList<TIFactionGoalState>())
            {
                TINationState ref_nation = tifactionGoalState2.target().ref_nation;
                if (majorityControlNations.Contains(ref_nation))
                {
                    if (ref_nation.belligerentInActiveWar || ref_nation.unrestWarning || ref_nation.civilWar)
                    {
                        this.AttemptFundPriority(faction, ref_nation, PriorityType.Military, ref dictionary4);
                    }
                    else if (!ref_nation.alienNation)
                    {
                        if (ref_nation.spaceFlightProgram)
                        {
                            if (!faction.AnyAvailableMissionControl)
                            {
                                this.AttemptFundPriority(faction, ref_nation, PriorityType.MissionControl, ref dictionary4);
                            }
                            else if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
                            {
                                this.AttemptFundPriority(faction, ref_nation, PriorityType.LaunchFacilities, ref dictionary4);
                            }
                        }
                        else if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
                        {
                            this.AttemptFundPriority(faction, ref_nation, PriorityType.SpaceflightProgram, ref dictionary4);
                        }
                        else if (ref_nation.numControlPoints >= 3 || ref_nation.cohesion < 3f)
                        {
                            if (faction.extremist)
                            {
                                this.AttemptFundPriority(faction, ref_nation, PriorityType.Unity, ref dictionary4);
                            }
                            else
                            {
                                this.AttemptFundPriority(faction, ref_nation, PriorityType.Knowledge, ref dictionary4);
                            }
                        }
                    }
                    else if (ref_nation.cohesion < 3f)
                    {
                        this.AttemptFundPriority(faction, ref_nation, PriorityType.Unity, ref dictionary4);
                    }
                }
            }
        }
    }
    public void AttemptFundPriority(TIFactionState faction, TINationState nation, PriorityType priority, ref Dictionary<FactionResource, float> resourcesAvailable)
    {
        if (nation.ValidPriority(priority))
        {
            TIResourcesCost tiresourcesCost = nation.InvestmentPointDirectPurchasePrice(priority, faction);
            int num = tiresourcesCost.CanAfford_Count(faction, resourcesAvailable);
            float num2 = nation.MaxDirectInvestIPsRemainingThisYear();
            if ((float)num > num2)
            {
                num = (int)Math.Truncate((double)num2);
            }
            if ((float)num > nation.DeltaToInvestmentThreshhold(priority))
            {
                faction.playerControl.StartAction(new DirectInvestAction(faction, nation, priority, (float)num));
                foreach (ResourceValue resourceValue in tiresourcesCost.resourceCosts)
                {
                    Dictionary<FactionResource, float> dictionary = resourcesAvailable;
                    FactionResource resource = resourceValue.resource;
                    dictionary[resource] -= resourceValue.value * (float)num;
                }
            }
        }
    }
    internal class patch_AIDailyFactionPlanner : AIDailyFactionPlanner
    {
        private readonly List<PriorityType> orderedPrioritiesForSpares = new List<PriorityType>
        {
            PriorityType.Knowledge,
            PriorityType.SpaceflightProgram,
            PriorityType.LaunchFacilities,
            PriorityType.MissionControl,
            PriorityType.Economy,
            PriorityType.Military,
            PriorityType.SpaceDevelopment,
            PriorityType.BuildSpaceDefenses,
            PriorityType.BuildArmy,
            PriorityType.Welfare,
            PriorityType.UpgradeArmy,
            PriorityType.Unity,
            PriorityType.Spoils,
            PriorityType.InitiateNuclearProgram,
            PriorityType.BuildNuclearWeapons
        };
    }
}

namespace PavonisInteractive.TerraInvicta.Systems.PeriodicUpdates {
    [UpdateInGroup(typeof(PipelineStages.SimulationStage))]
    public class patch_NationPeriodicUpdate : NationPeriodicUpdate
    {
        private void PeriodicNationUpdateTask(TINationState nation)
        {
            foreach (TIControlPoint ticontrolPoint in nation.NativeControlPoints)
            {
                if (nation.atWar)
                {
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.BuildArmy, 3, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.UpgradeArmy, 3, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.BuildNuclearWeapons, (nation.numNuclearWeapons == 0 && nation.numControlPoints >= 5) ? 2 : 0, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.InitiateNuclearProgram, (nation.numControlPoints >= 5) ? 2 : 0, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.BuildSpaceDefenses, 3, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Knowledge, 0, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.SpaceDevelopment, 0, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.SpaceflightProgram, 0, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.MissionControl, 0, false));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.LaunchFacilities, 0, false));
                }
                else
                {
                    nation.ApplyInvestmentTemplateToControlPoint(ticontrolPoint.positionInNation, nation.template.initialPriorityPreset[ticontrolPoint.positionInNation]);
                }
                int num = 0;
                if (nation.inequalityWarning)
                {
                    num++;
                }
                if (nation.cohesionWarning)
                {
                    num++;
                }
                if (nation.unrestMajorWarning)
                {
                    num++;
                }
                if (num > 0)
                {
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Welfare, num + (int)nation.democracy / 4, true));
                    GameControl.StartSimulationAction(new NationAI_SetPriority(ticontrolPoint, PriorityType.Unity, num + (10 - (int)nation.democracy) / 4, true));
                }
            }
        }
    }
}