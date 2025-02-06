using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.Image;


namespace PavonisInteractive.TerraInvicta
{
    public class patch_TIArmyState : TIArmyState
    {

        private float regionDamageScaling
        {
            get
            {
                if (this.armyType != ArmyType.AlienMegafauna)
                {
                    return 2E-05f;
                }
                return 0.002f;
            }
        }
        private TIArmyState lastEnemyArmy;
        public void FireAtEnemyArmy(TIArmyState defendingArmy)
        {
            if (this.atSea)
            {
                return;
            }
            this.lastEnemyArmy = defendingArmy;
            float attackValue = this.GetAttackValue();
            float enemyDefendValue = this.GetEnemyDefendValue(defendingArmy);
            float combatSuccessChance = this.GetCombatSuccessChance(attackValue, enemyDefendValue);
            int num = this.currentRegion.NumArmiesPresent(true, false, true, true);
            float num2 = 1f;
            if (!this.AlienMegafaunaArmy && !defendingArmy.AlienMegafaunaArmy && this.strength == 1f)
            {
                if (this.currentRegion.occupations.Count<KeyValuePair<TINationState, float>>() != 0)
                {
                    if (!this.currentRegion.occupations.All((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
                    {
                        goto IL_A8;
                    }
                }
                num2 = 3f;
                goto IL_C1;
            }
        IL_A8:
            if (UnityEngine.Random.value < 0.01f * (float)num * (float)num)
            {
                num2 = 3f;
            }
        IL_C1:
            defendingArmy.currentRegion.ApplyDamageToRegion((10f - attackValue) * this.regionDamageScaling * num2, this.faction, this.homeNation, false, false, false, false);
            if (UnityEngine.Random.value < combatSuccessChance)
            {
                if (!this.AlienMegafaunaArmy && defendingArmy.AlienMegafaunaArmy && UnityEngine.Random.value < TIEffectsState.SumEffectsModifiers(Context.MegafaunaMastery, this.faction, 0f))
                {
                    defendingArmy.AssignToFaction(this.faction, true);
                    return;
                }
                float num3 = attackValue * 0.001f * num2 * (0.8f + UnityEngine.Random.Range(0f, 0.4f));
                num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoAllArmies, this.faction, num3);
                switch (defendingArmy.armyType)
                {
                    case ArmyType.Human:
                        num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoHumanArmy, this.faction, num3);
                        break;
                    case ArmyType.AlienMegafauna:
                        num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoMegafauna, this.faction, num3);
                        break;
                    case ArmyType.AlienInvader:
                        num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoInvaderArmy, this.faction, num3);
                        num3 *= (1 + (TIGlobalValuesState.GlobalValues.earthAtmosphericCO2_ppm/100) );
                        break;
                }
                defendingArmy.TakeDamage(num3, this.faction, this.homeNation);
            }
        }

        // Token: 0x06003323 RID: 13091 RVA: 0x0011AA18 File Offset: 0x00118C18
        public static float LocalForcesAdjacentRegionsBonus(TIRegionState currentRegion)
        {
            float num = 0f;
            IEnumerable<TIRegionState> enumerable = from x in currentRegion.AdjacentRegions(false)
                                                    where x.nation == currentRegion.nation
                                                    select x;
            float num2 = (float)enumerable.Count<TIRegionState>();
            if (num2 > 0f)
            {
                foreach (TIRegionState tiregionState in enumerable)
                {
                    if (tiregionState.occupations.Count != 0)
                    {
                        if (!tiregionState.occupations.All((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
                        {
                            continue;
                        }
                    }
                    if (tiregionState.NumArmiesPresent(false, false, true, false) == 0)
                    {
                        num += 1f;
                    }
                }
                return num / num2 * currentRegion.nation.militaryTechLevel * TemplateManager.global.adjacentFriendlyForcesRegionMiltechMultiplier * (1f + currentRegion.nation.unrest * TemplateManager.global.defenseUnrestMultiplier);
            }
            return 0f;
        }
        public bool TakeDamage(float amount, TIFactionState attacker, TINationState attackingNation)
        {
            if (this.strength <= 0f)
            {
                return true;
            }
            if (!this.AlienRegularArmy)
            {
                patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(amount / 200, true);
            }
            this.strength -= amount;
            this.strength = Mathf.Clamp(this.strength, 0f, 1f);
            if (amount > 0f)
            {
                GameControl.eventManager.TriggerEvent(new ArmyTakesDamage(this), this.armyDamageEventName, new object[]
                {
                    this,
                    this.currentRegion
                });
                if (attackingNation != null)
                {
                    foreach (TIWarState tiwarState in from x in this.homeNation.currentWarStates
                                                      where x.allBelligerents.Contains(attackingNation)
                                                      select x)
                    {
                        tiwarState.FightingOccurs();
                    }
                }
            }
            if (this.strength <= 0f)
            {
                TINotificationQueueState.LogArmyIsDestroyed(this, this.currentRegion, attacker);
                if (this.faction != null)
                {
                    if (!this.faction.armiesLost.ContainsKey(this.armyType))
                    {
                        this.faction.armiesLost.Add(this.armyType, 1);
                    }
                    else
                    {
                        Dictionary<ArmyType, int> armiesLost = this.faction.armiesLost;
                        ArmyType key = this.armyType;
                        armiesLost[key]++;
                    }
                }
                if ((this.faction == null || this.faction.IsActiveHumanFaction) && this.homeNation != null)
                {
                    this.homeNation.AddToCohesion(-this.homeNation.democracy / 10f, TINationState.CohesionChangeReason.CohesionReason_ArmyLost);
                    if (this.faction != null)
                    {
                        this.homeNation.PropagandaOnPop(this.faction.ideology, -this.homeNation.democracy);
                    }
                }
                if (attacker != null)
                {
                    switch (this.armyType)
                    {
                        case ArmyType.Human:
                            if (this.homeNation.alienNation && this.techLevel >= 6f)
                            {
                                attacker.CompleteMilestone(CampaignMilestone.AccessAlienTech);
                                if (UnityEngine.Random.value < 0.5f)
                                {
                                    attacker.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
                                }
                                else if (UnityEngine.Random.value < 0.1f)
                                {
                                    attacker.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
                                }
                            }
                            else if (this.currentRegion.nation == this.homeNation)
                            {
                                this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildArmy, 0.3f + UnityEngine.Random.value * 0.2f, true);
                                if (this.deploymentType == DeploymentType.Naval)
                                {
                                    if (this.homeNation.navalFreedom)
                                    {
                                        if (this.currentRegion.onTheWater)
                                        {
                                            this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.7f + UnityEngine.Random.value * 0.2f, true);
                                        }
                                        else
                                        {
                                            this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.9f + UnityEngine.Random.value * 0.2f, true);
                                        }
                                    }
                                    else
                                    {
                                        this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.2f + UnityEngine.Random.value * 0.1f, true);
                                    }
                                }
                            }
                            else
                            {
                                this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildArmy, 0.15f + UnityEngine.Random.value * 0.1f, true);
                                if (this.deploymentType == DeploymentType.Naval)
                                {
                                    if (this.homeNation.navalFreedom)
                                    {
                                        if (this.currentRegion.onTheWater)
                                        {
                                            this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.6f + UnityEngine.Random.value * 0.2f, true);
                                        }
                                        else
                                        {
                                            this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.8f + UnityEngine.Random.value * 0.2f, true);
                                        }
                                    }
                                    else
                                    {
                                        this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.1f + UnityEngine.Random.value * 0.1f, true);
                                    }
                                }
                            }
                            break;
                        case ArmyType.AlienMegafauna:
                            attacker.CompleteMilestone(CampaignMilestone.AccessAlienMegafauna);
                            if (attacker.isActivePlayer)
                            {
                                attacker.UnlockAchievement("destroyMegafauna");
                            }
                            break;
                        case ArmyType.AlienInvader:
                            attacker.CompleteMilestone(CampaignMilestone.AccessAlienTech);
                            attacker.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
                            attacker.CompleteMilestone(CampaignMilestone.AccessWarDogCorpus);
                            attacker.CompleteMilestone(CampaignMilestone.AlienArmyDestroyed);
                            if (UnityEngine.Random.value < 0.4f)
                            {
                                attacker.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
                            }
                            if (attacker.isActivePlayer)
                            {
                                attacker.UnlockAchievement("destroyAlienArmy");
                            }
                            break;
                    }
                }
                if (this.armyType == ArmyType.AlienInvader && GameStateManager.AlienNation().regions.Count == 0)
                {
                    if (GameStateManager.AlienFaction().armies.Count((TIArmyState x) => x.armyType == ArmyType.AlienInvader) <= 1)
                    {
                        foreach (TIWarState war in this.homeNation.currentWarStates)
                        {
                            TINationState.EndFullWar(GameStateManager.AlienFaction(), war, true);
                        }
                    }
                }
                if (attacker != null)
                {
                    attacker.RegisterKill(this, 1);
                }
                this.Disband();
                return true;
            }
            this.SetArmyDataDirty();
            return false;
        }

        public virtual void EngageLocalForcesAndOccupy(bool regionReturnFireOnly = false) //NEEDS TO BE updated for balance(not tested, is base atm)
        {
            if (this.atSea)
            {
                return;
            }
            List<TIArmyState> list = (from x in this.currentRegion.FilteredArmiesPresent(false, false, true, false, false)
                                      where x.CurrentOperations().Count == 0
                                      select x).ToList<TIArmyState>();
            int count = list.Count;
            List<TIArmyState> list2 = (from x in this.currentRegion.FilteredArmiesPresent(true, false, false, false, true)
                                       where x.CurrentOperations().Count == 0
                                       select x).ToList<TIArmyState>();
            int count2 = list2.Count;
            float num = list.Sum((TIArmyState x) => x.combatEffectiveness);
            float num2 = list2.Sum((TIArmyState x) => x.combatEffectiveness);
            bool inFriendlyRegion = this.InFriendlyRegion;
            bool flag = !inFriendlyRegion;
            List<TINationState> occupyingAlliance;
            TINationState tinationState;
            float highestWarAllianceOccupationValue = this.currentRegion.GetHighestWarAllianceOccupationValue(out tinationState, out occupyingAlliance);
            TINationState tinationState2 = (inFriendlyRegion && highestWarAllianceOccupationValue > 0f) ? (from x in this.currentRegion.occupations
                                                                                                           where occupyingAlliance.Contains(x.Key)
                                                                                                           select x).SelectRandomWeightedItem((KeyValuePair<TINationState, float> x) => this.currentRegion.occupations[x.Key], -1f, 1E-37f).Key : null;
            float num3;
            if (inFriendlyRegion)
            {
                num3 = 0.01f * (float)count2 * (float)count2 * (float)Mathf.Max(count, 1);
            }
            else
            {
                num3 = 0.01f * (float)count * (float)count * (float)Mathf.Max(count2, 1);
            }
            float num4 = ((this.strength == 1f && (!this.currentRegion.occupations.ContainsKey(this.homeNation) || this.currentRegion.occupations[this.homeNation] <= 0f)) || UnityEngine.Random.value < num3) ? 40f : 1f;
            if (this.strength == 1f)
            {
                if (this.currentRegion.occupations.Count<KeyValuePair<TINationState, float>>() != 0)
                {
                    if (!this.currentRegion.occupations.All((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
                    {
                        goto IL_23E;
                    }
                }
                num4 = 100f;
                goto IL_267;
            }
        IL_23E:
            float value = UnityEngine.Random.value;
            if (value < num3)
            {
                num4 = 40f;
            }
            else if (value < num3 * 2f)
            {
                num4 = 5f;
            }
        IL_267:
            float attackValue = this.GetAttackValue();
            float num5 = this.LocalForcesBaseDefenseLevel(true, tinationState2);
            float combatSuccessChance = this.GetCombatSuccessChance(num5, attackValue);
            this.currentRegion.ApplyDamageToRegion(Mathf.Max(1f, 12f - attackValue) * this.regionDamageScaling * num4, this.faction, this.homeNation, false, false, false, false);
            float num6 = this.currentRegion.RegionArmyActionMultiplier(true);
            if (UnityEngine.Random.value >= combatSuccessChance)
            {
                if (!regionReturnFireOnly)
                {
                    if (flag)
                    {
                        if (count2 == 0)
                        {
                            float num7 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                            float num8 = Mathf.Max(0f, attackValue - this.currentNation.militaryTechLevel - this.currentNation.adviserCommandBonus);
                            float num9 = (attackValue + num8) * 0.000225f * num6 * num4 * num7;
                            float num10 = (float)count - this.currentRegion.mapRegionTemplate.area_km2 / 100000f;
                            if (num10 > 0f)
                            {
                                num9 *= Mathf.Max(0.5f, 1f - num10 * 0.02f);
                            }
                            num9 = Mathf.Min(0.1f, num9);
                            this.currentRegion.IncreaseOccupationValue(this.homeNation, num9, this);
                        }
                    }
                    else if (count == 0)
                    {
                        if (tinationState2 != null)
                        {
                            float num11 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                            float[] array = new float[3];
                            array[1] = attackValue - tinationState2.militaryTechLevel - tinationState2.adviserCommandBonus;
                            float num12 = Mathf.Max(array);
                            float num13 = (attackValue + num12) * 0.000225f * num6 * num4 * ((count == 0) ? (1f + num11) : num11);
                            num13 = Mathf.Min(0.1f, num13);
                            this.currentRegion.IncreaseOccupationValue(tinationState2, -num13, this);
                        }
                        else
                        {
                            this.currentRegion.ValidateAndCleanOccupations();
                        }
                    }
                    if (this.armyType == ArmyType.AlienInvader && num4 > 1f)
                    {
                        this.currentRegion.ConductAbductions(this.faction, 1);
                    }
                }
                return;
            }
            float num14 = num5 * 0.000225f * num4 * (0.8f + UnityEngine.Random.Range(0f, 0.4f));
            num14 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoAllArmies, this.currentRegion.nation.executiveFaction, num14);
            if (flag)
            {
                ArmyType armyType = this.armyType;
                if (armyType != ArmyType.Human)
                {
                    if (armyType == ArmyType.AlienInvader)
                    {
                        num14 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoInvaderArmy, this.currentNation.executiveFaction, num14);
                    }
                }
                else
                {
                    num14 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoHumanArmy, this.currentNation.executiveFaction, num14);
                }
            }
            float num15 = inFriendlyRegion ? (num2 - num) : (num - num2);
            if (num15 > 0f)
            {
                num14 *= 1f - num15 / (num15 + 2f);
            }
            if (!flag)
            {
                this.TakeDamage((2f - this.currentNation.cohesion / 10f + this.currentNation.unrest / 10f) * num14, (tinationState2 != null) ? tinationState2.ref_faction : null, tinationState2);
                return;
            }
            float num16;
            if (!this.currentRegion.occupations.ContainsKey(this.homeNation) || this.currentRegion.occupations[this.homeNation] <= 0f)
            {
                num16 = 0f;
            }
            else if (count == 0)
            {
                num16 = 1f;
            }
            else
            {
                float num17 = this.currentRegion.occupations[this.homeNation];
                float num18 = num;
                num16 = num17 / num18;
            }
            if (UnityEngine.Random.value < num16)
            {
                float num19 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                float num20 = Mathf.Clamp(num5 * 0.000225f * num4 * num6 * num19 / (float)Mathf.Max(1, count), 0f, 0.1f);
                this.currentRegion.IncreaseOccupationValue(this.homeNation, -num20, null);
                this.TakeDamage(num14, this.currentRegion.ref_faction, this.currentRegion.nation);
                return;
            }
            this.TakeDamage(2f * num14, this.currentRegion.ref_faction, this.currentRegion.nation);
        }
        private int reachableRegionsCachedFrame = -1;
        private HashSet<TIRegionState> cachedReachableRegions = new HashSet<TIRegionState>();

        public bool Teleportable(TIRegionState destination, Func<TIRegionState, bool> IsRegionAllowed = null)
        {

            if (destination.template.oilResource && this.currentRegion.template.oilResource)
            {
                return true;
            }
            return false;
        }

        public static IList<TIRegionState> TeleportValid(TIArmyState army, patch_TIRegionState currentRegion)
        {
            List<TIRegionState> list = new List<TIRegionState>();
            List<TINationState> list2 = new List<TINationState>();
            list2.Add(army.homeNation);
            list2.AddRange(army.homeNation.allies);
            foreach (TINationState tinationState in list2)
            {
                foreach (patch_TIRegionState tiregionState in tinationState.regions)
                {
                    if (currentRegion.isTeleport && tiregionState.isTeleport)
                    {
                        list.Add(tiregionState);
                    }
                }
            }

            return list;
        }
    }
}
