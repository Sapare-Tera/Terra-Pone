using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


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
                            float num8 = attackValue * 0.000225f * num6 * num2 * num7;
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

        //    //private HashSet<TIRegionState> cachedJourneyableRegions = new HashSet<TIRegionState>();

        //    //private int journeyableRegionsCachedFrame = -1;

        //    //public IEnumerable<TIRegionState> JourneyableRegions
        //    //{
        //    //    get
        //    //    {
        //    //        if (this.journeyableRegionsCachedFrame != Time.frameCount)
        //    //        {
        //    //            this.cachedJourneyableRegions.Clear();
        //    //            this.cachedJourneyableRegions.UnionWith(from x in TIRegionState.Regions
        //    //                                                    where this.CanJourneyTo(x, false)
        //    //                                                    select x);
        //    //            this.journeyableRegionsCachedFrame = Time.frameCount;
        //    //        }
        //    //        return this.cachedJourneyableRegions;
        //    //    }
        //    //}

        //    public static DeploymentType GetRequiredDeploymentType(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
        //    {
        //        DeploymentType result;
        //        TIArmyState.IsTraversible(origin, destination, out result, army);
        //        return result;
        //    }

        //    public static List<TIRegionState> AllValidDestinationRegions(TIArmyState army, patch_TIRegionState currentRegion, bool includeCurrentRegion)
        //    {
        //        List<TIRegionState> list = new List<TIRegionState>();
        //        bool navalFreedom = army.homeNation.navalFreedom;
        //        List<TINationState> list2 = new List<TINationState>();
        //        list2.Add(army.homeNation);
        //        list2.AddRange(army.homeNation.allies);
        //        IEnumerable<TINationState> enumerable = army.homeNation.wars.Distinct<TINationState>();
        //        list2.AddRange(enumerable);
        //        if (includeCurrentRegion)
        //        {
        //            list.Add(currentRegion);
        //        }
        //        List<TIRegionState> list3 = new List<TIRegionState>();
        //        foreach (TINationState tinationState in list2)
        //        {
        //            bool iamAnInvadingArmy = enumerable.Contains(tinationState);
        //            foreach (patch_TIRegionState tiregionState in tinationState.regions)
        //            {
        //                if (tiregionState.IsAdjacent(currentRegion, iamAnInvadingArmy) || (tiregionState != currentRegion && navalFreedom && army.deploymentType == DeploymentType.Naval && currentRegion.onTheWater && tiregionState.onTheWater) || (tiregionState != currentRegion && currentRegion.MagicResource && tiregionState.MagicResource))
        //                {
        //                    list.Add(tiregionState);
        //                }
        //                else
        //                {
        //                    list3.Add(tiregionState);
        //                }
        //            }
        //        }
        //        int num = 0;
        //        bool flag;
        //        do
        //        {
        //            flag = false;
        //            foreach (TIRegionState tiregionState2 in list3.ToList<TIRegionState>())
        //            {
        //                if (tiregionState2.AdjacentRegions(enumerable.Contains(tiregionState2.nation)).Intersect(list).Any<TIRegionState>())
        //                {
        //                    list.Add(tiregionState2);
        //                    list3.Remove(tiregionState2);
        //                    flag = true;
        //                }
        //            }
        //            num++;
        //        }
        //        while (flag && num < 1000);
        //        if (num >= 1000)
        //        {
        //            Log.Error("Something wrong with the loop", Array.Empty<object>());
        //        }
        //        return list;
        //    }

        //    public static IList<TIRegionState> OneStepValidDestinationRegions(TIArmyState army, patch_TIRegionState currentRegion, bool includeCurrentRegion)
        //    {
        //        List<TIRegionState> list = new List<TIRegionState>();
        //        if (army.AlienMegafaunaArmy)
        //        {
        //            list.AddRange(army.ref_region.AdjacentRegions(true));
        //            if (!includeCurrentRegion && list.Contains(currentRegion))
        //            {
        //                list.Remove(currentRegion);
        //            }
        //        }
        //        else
        //        {
        //            bool flag = army.homeNation.navalFreedom && army.deploymentType == DeploymentType.Naval && currentRegion.onTheWater;

        //            bool teleport = currentRegion.MagicResource;

        //            List<TINationState> list2 = new List<TINationState>();
        //            list2.Add(army.homeNation);
        //            list2.AddRange(army.homeNation.allies);

        //            if (includeCurrentRegion)
        //            {
        //                list.Add(currentRegion);
        //            }
        //            foreach (TINationState tinationState in list2)
        //            {
        //                foreach (patch_TIRegionState tiregionState in tinationState.regions)
        //                {
        //                    if (tiregionState.IsAdjacent(currentRegion, false) || (teleport && tiregionState != currentRegion && tiregionState.MagicResource) || (flag && tiregionState != currentRegion && tiregionState.onTheWater))
        //                    {
        //                        list.Add(tiregionState);
        //                    }
        //                }
        //            }
        //            foreach (TINationState tinationState2 in army.homeNation.wars.Distinct<TINationState>())
        //            {
        //                foreach (TIRegionState tiregionState2 in tinationState2.regions)
        //                {
        //                    if (tiregionState2.IsAdjacent(currentRegion, true) || (tiregionState2 != currentRegion && flag && tiregionState2.onTheWater))
        //                    {
        //                        list.Add(tiregionState2);
        //                    }
        //                }
        //            }
        //        }
        //        return list;
        //    }


        //    public static bool IsTraversible(patch_TIRegionState origin, patch_TIRegionState destination, out patch_DeploymentType deploymentTypeRequired, TIArmyState army = null)
        //    {
        //        deploymentTypeRequired = (patch_DeploymentType)DeploymentType.None;
        //        bool flag = origin.onTheWater && destination.onTheWater;
        //        if (flag && army != null)
        //        {
        //            flag = (army.deploymentType == DeploymentType.Naval && army.homeNation.navalFreedom);
        //        }
        //        bool flag2 = origin.Neighbors.Contains(destination);
        //        if (flag2 && army != null)
        //        {
        //            switch (origin.GetAdjacencyType(destination))
        //            {
        //                case TerrestrialAdjacencyType.None:
        //                    flag2 = false;
        //                    break;
        //                case TerrestrialAdjacencyType.FriendlyCrossingOnly:
        //                    flag2 = (!army.AlienMegafaunaArmy && !destination.nation.wars.Contains(army.homeNation));
        //                    break;
        //            }
        //        }
        //        bool flag3 = origin.MagicResource && destination.MagicResource;

        //        if (flag3)
        //        {
        //            deploymentTypeRequired = (patch_DeploymentType)DeploymentType.Standard;
        //            return true;
        //        }
        //        else
        //            if (flag2)
        //        {
        //            deploymentTypeRequired = (patch_DeploymentType)DeploymentType.Standard;
        //        }
        //        else
        //        {
        //            if (!flag)
        //            {
        //                return false;
        //            }
        //            deploymentTypeRequired = (patch_DeploymentType)DeploymentType.Naval;
        //        }
        //        return true;
        //    }
        //}
        //public class patch_OrgItemView : OrgItemView

        //{
        //    public void OnRightClickItem()
        //    {
        //        if (this.dragDestination == this.councilorController.availableDragDestination || this.dragDestination == this.councilorController.councilDragDestination)
        //        {
        //            this.councilorController.ShowInfoMyOrg(this.orgName.text, this.org.description(false), this.orgIcon.sprite);
        //            this.councilorController.selectedOrgTop = this;
        //            this.councilorController.orgActionButtonTextTop.SetText(Loc.T("UI.Council.UnequipOrg"), true);
        //            this.councilorController.orgActionButtonTextTop2.SetText(Loc.T("UI.Council.SellOrg"), true);
        //        }
        //        if (this.dragDestination == this.councilorController.councilorDragDestination)
        //        {
        //            this.councilorController.ShowInfoEquipOrg(this.org.displayName, this.org.hasFaction ? this.org.GetSalePrice().ToString("Relevant", false, false, null, false) : this.org.GetPurchaseCost(GameControl.control.activePlayer).ToString("Relevant", false, false, null, false), this.org.description(false), this.orgIcon.sprite);
        //            this.councilorController.selectedOrgBottom = this;
        //            if (this.org.hasFaction)
        //            {
        //                this.councilorController.orgActionButtonTextBottom.SetText(Loc.T("UI.Council.EquipOrg"), true);
        //                this.councilorController.orgActionButtonTextBottom2.SetText(Loc.T("UI.Council.SellOrg"), true);
        //                this.councilorController.orgActionButtonBottom2.gameObject.SetActive(true);
        //                return;
        //            }
        //            this.councilorController.orgActionButtonTextBottom.SetText(Loc.T("UI.Council.PurchaseOrg"), true);
        //            this.councilorController.orgActionButtonBottom2.gameObject.SetActive(false);
        //        }
        //    }
        //    private TIOrgState org;
        //    private DragDestination dragDestination;

        //    //     public enum patch_OrgStatus : ushort
        //    //    {
        //    //        Special = 5
        //    //    }
        //}
    }
}
