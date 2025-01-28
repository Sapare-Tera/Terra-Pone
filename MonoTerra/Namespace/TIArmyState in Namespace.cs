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
