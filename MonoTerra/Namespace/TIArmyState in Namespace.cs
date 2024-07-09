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
        public void EngageLocalForcesAndOccupy(bool regionReturnFireOnly = false)
        {
            if (this.atSea)
            {
                return;
            }
            List<TIArmyState> list = this.currentRegion.FilteredArmiesPresent(false, false, true, false);
            int count = list.Count;
            List<TIArmyState> list2 = this.currentRegion.FilteredArmiesPresent(true, true, false, false);
            int count2 = list2.Count;
            TINationState getOccupierNation = this.GetOccupierNation;
            float num;
            if (this.InFriendlyRegion)
            {
                num = 0.01f * (float)count2 * (float)count2;
            }
            else
            {
                num = 0.01f * (float)count * (float)count;
            }
            float num2 = ((this.strength == 1f && (!this.currentRegion.occupations.ContainsKey(this.homeNation) || this.currentRegion.occupations[this.homeNation] <= 0f)) || UnityEngine.Random.value < num) ? 40f : 1f;
            if (!this.AlienMegafaunaArmy)
            {
                if (this.strength == 1f)
                {
                    if (this.currentRegion.occupations.Count<KeyValuePair<TINationState, float>>() != 0)
                    {
                        if (!this.currentRegion.occupations.All((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
                        {
                            goto IL_120;
                        }
                    }
                    num2 = 100f;
                    goto IL_149;
                }
            IL_120:
                float value = UnityEngine.Random.value;
                if (value < num)
                {
                    num2 = 40f;
                }
                else if (value < num * 2f)
                {
                    num2 = 5f;
                }
            }
        IL_149:
            float attackValue = this.GetAttackValue();
            float num3 = this.LocalForcesBaseDefenseLevel(true);
            bool inFriendlyRegion = this.InFriendlyRegion;
            float combatSuccessChance = this.GetCombatSuccessChance(inFriendlyRegion ? attackValue : num3, inFriendlyRegion ? num3 : attackValue);
            this.currentRegion.ApplyDamageToRegion(Mathf.Max(1f, 12f - attackValue) * this.regionDamageScaling, this.faction, this.homeNation, false, false, false, false);
            float num4 = Mathf.Pow(this.currentRegion.populationInMillions, 0.75f) / Mathf.Pow(TIGlobalValuesState.GlobalValues.averageRegionPopulation, 0.75f);
            float num5 = Mathf.Pow(this.currentRegion.mapRegionTemplate.area_km2, 0.75f) / Mathf.Pow(TIGlobalValuesState.GlobalValues.medianRegionArea, 0.75f);
            float num6 = 1f / ((num4 + num5) * 0.5f);
            if (UnityEngine.Random.value >= combatSuccessChance)
            {
                if (!regionReturnFireOnly)
                {
                    if (!inFriendlyRegion)
                    {
                        if (count2 == 0)
                        {
                            float num7 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                            float num8 = attackValue * 0.0005f * num6 * num2 * num7;
                            float num9 = (float)count - this.currentRegion.mapRegionTemplate.area_km2 / 100000f;
                            if (num9 > 0f)
                            {
                                num8 *= Mathf.Max(0.5f, 1f - num9 * 0.02f);
                            }
                            num8 = Mathf.Min(0.1f, num8);
                            this.currentRegion.IncreaseOccupationValue(this.homeNation, num8, this);
                        }
                    }
                    else
                    {
                        float num10 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                        float num11 = attackValue * 0.000225f * num6 * num2 * ((count == 0) ? (1f + num10) : num10);
                        num11 = Mathf.Min(0.1f, num11);
                        this.currentRegion.IncreaseOccupationValue(getOccupierNation, -num11, this);
                    }
                    if (this.armyType == ArmyType.AlienInvader && num2 > 1f)
                    {
                        this.currentRegion.ConductAbductions(this.faction, 1);
                    }
                }
                return;
            }
            float num12 = num3 * 0.000225f * num2 * (0.8f + UnityEngine.Random.Range(0f, 0.4f));
            num12 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoAllArmies, this.currentRegion.nation.executiveFaction, num12);
            if (!inFriendlyRegion)
            {
                ArmyType armyType = this.armyType;
                if (armyType != ArmyType.Human)
                {
                    if (armyType == ArmyType.AlienInvader)
                    {
                        num12 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoInvaderArmy, this.currentNation.executiveFaction, num12);
                    }
                }
                else
                {
                    num12 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoHumanArmy, this.currentNation.executiveFaction, num12);
                }
            }
            float num13 = (float)(inFriendlyRegion ? (count2 - count) : (count - count2));
            if (num13 > 0f)
            {
                num12 *= 1f - num13 / (num13 + 2f);
            }
            float num14;
            if (this.armyType == ArmyType.AlienMegafauna)
            {
                num14 = 0f;
            }
            else if (!inFriendlyRegion)
            {
                if (!this.currentRegion.occupations.ContainsKey(this.homeNation) || this.currentRegion.occupations[this.homeNation] <= 0f)
                {
                    num14 = 0f;
                }
                else if (count == 0 || list.Count == 0)
                {
                    num14 = 1f;
                }
                else
                {
                    float num15 = this.currentRegion.occupations[this.homeNation];
                    float num16 = (float)count * list.Average((TIArmyState x) => x.strength);
                    num14 = num15 / num16;
                }
            }
            else if (count == 0)
            {
                num14 = 1f;
            }
            else
            {
                float num17 = (float)count2 * list2.Average((TIArmyState x) => x.strength);
                float num18 = (float)count * list.Average((TIArmyState x) => x.strength) * ((getOccupierNation != null) ? this.currentRegion.occupations[getOccupierNation] : 1f);
                num14 = num17 / num18;
            }
            if (UnityEngine.Random.value < num14)
            {
                if (!inFriendlyRegion)
                {
                    float num19 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                    float num20 = Mathf.Min(0.1f, num3 * 0.000225f * num2 * num6 * num19);
                    this.currentRegion.IncreaseOccupationValue(this.homeNation, -num20, null);
                    this.TakeDamage(num12 / 4, this.currentRegion.ref_faction, this.currentRegion.nation);
                    return;
                }
                float num21 = 0.8f + UnityEngine.Random.Range(0f, 0.4f);
                float num22 = Mathf.Min(0.1f, Mathf.Max(1f, this.currentRegion.nation.cohesion - this.currentRegion.nation.unrest) * attackValue * 0.000225f * num2 * num6 * num21);
                this.currentRegion.IncreaseOccupationValue(getOccupierNation, -num22, null);
                this.TakeDamage(num12 / 4 * (1.1f - this.currentNation.cohesion), (getOccupierNation != null) ? getOccupierNation.ref_faction : null, getOccupierNation);
                return;
            }
            else
            {
                if (!inFriendlyRegion)
                {
                    this.TakeDamage(0.5f * num12, this.currentRegion.ref_faction, this.currentRegion.nation);
                    return;
                }
                this.TakeDamage((0.5f - this.currentNation.cohesion) * num12, (getOccupierNation != null) ? getOccupierNation.ref_faction : null, getOccupierNation);
                return;
            }
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
