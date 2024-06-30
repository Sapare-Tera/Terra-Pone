using AssetBundles;
using FullSerializer;
using Microsoft.CSharp.RuntimeBinder;
using MonoMod;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using TMPro;
using TMPro.Examples;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using PavonisInteractive.TerraInvicta;
using System.Diagnostics;
using Mono.Cecil;
using System.Reflection;

namespace PavonisInteractive.TerraInvicta
{
    public class patch_GeneralControlsController : GeneralControlsController
    {
        private Dictionary<patch_FactionResource, int> proposedResourceSales;

        public TMP_Text waterInfoText;

        public Transform MagicPanel;

        [Header("Resources Data")]
        public TMP_Text magicInfoText;

        private void UpdateResourceData(TIFactionState faction)
        {
            this.incomeInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Money), true);
            this.influenceInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Influence), true);
            this.operationInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Operations), true);
            this.boostInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Boost), true);
            this.researchInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Research), true);
            this.missionControlInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.MissionControl), true);
            //this.magicInfoText.SetText(GeneralControlsController.ResourceReportString(faction, (FactionResource)patch_FactionResource.Magic), true);
            this.controlPointMaintenanceText.SetText(GeneralControlsController.ControlPointMaintenanceString(faction), true);
            bool unlockedSpaceResources = faction.UnlockedSpaceResources;
            bool unlockedAntimatter = faction.UnlockedAntimatter;
            bool unlockedExotics = faction.UnlockedExotics;
            this.waterPanel.gameObject.SetActive(unlockedSpaceResources);
            this.volatilesPanel.gameObject.SetActive(unlockedSpaceResources);
            this.baseMetalsPanel.gameObject.SetActive(unlockedSpaceResources);
            this.nobleMetalsPanel.gameObject.SetActive(unlockedSpaceResources);
            this.fissilesPanel.gameObject.SetActive(unlockedSpaceResources);
            // this.MagicPanel.gameObject.SetActive(unlockedSpaceResources);
            this.antimatterPanel.gameObject.SetActive(unlockedAntimatter);
            this.exoticsPanel.gameObject.SetActive(unlockedExotics);
            if (unlockedSpaceResources)
            {
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Water, this.waterInfoText, this.waterPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Volatiles, this.volatilesInfoText, this.volatilesPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Metals, this.baseMetalsInfoText, this.baseMetalsPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.NobleMetals, this.nobleMetalsInfoText, this.nobleMetalsPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Fissiles, this.fissilesInfoText, this.fissilesPanel);
                //   this.SetSpaceResourceValuesInBar(faction, (FactionResource)patch_FactionResource.Magic, this.fissilesInfoText, this.fissilesPanel);
            }
            if (unlockedAntimatter)
            {
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Antimatter, this.antimatterInfoText, this.antimatterPanel);
            }
            if (unlockedExotics)
            {
                this.SetSpaceResourceValuesInBar(faction, (FactionResource)patch_FactionResource.Magic, this.exoticsInfoText, this.exoticsPanel);
            }
        }
        private void SetSpaceResourceValuesInBar(TIFactionState faction, FactionResource resourceType, TMP_Text reportText, Transform panel)
        {
            if ((float)Screen.width / (float)Screen.height >= 1.5f)
            {
                panel.gameObject.GetComponent<LayoutElement>().preferredWidth = 105f;
                reportText.SetText(GeneralControlsController.ResourceReportString(faction, resourceType), true);
                return;
            }
            panel.gameObject.GetComponent<LayoutElement>().preferredWidth = 65f;
            float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
            if (resourceType == FactionResource.Antimatter)
            {
                reportText.SetText(TIUtilities.FormatBigOrSmallNumber((double)currentResourceAmount, 1, 7, 0, false), true);
                return;
            }
            reportText.SetText(TIUtilities.FormatBigNumber((double)currentResourceAmount, 1), true);
        }

        private static bool showMonthlyIncomes
        {
            get
            {
                return GameControl.control.activePlayer.showMonthlyIncomesInTopBarAndIntel;
            }
        }

        public static string ResourceReportString(TIFactionState faction, FactionResource resourceType)
        {
            string result = string.Empty;
            switch (resourceType)
            {
                case FactionResource.Money:
                case FactionResource.Influence:
                case FactionResource.Operations:
                case FactionResource.Boost:
                case FactionResource.Water:
                case FactionResource.Volatiles:
                case FactionResource.Metals:
                case FactionResource.NobleMetals:
                case FactionResource.Fissiles:
                case FactionResource.Exotics:
                case (FactionResource)patch_FactionResource.Magic:
                    {
                        float num;
                        if (patch_GeneralControlsController.showMonthlyIncomes)
                        {
                            num = faction.GetMonthlyIncome(resourceType, false, false);
                        }
                        else
                        {
                            num = faction.GetDailyIncome(resourceType, false, false);
                        }
                        float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
                        if (num == 0f)
                        {
                            result = TIUtilities.FormatBigOrSmallNumber((double)currentResourceAmount, 1, 7, 0, false);
                        }
                        else if (num > 0f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesGain", new object[]
                            {
                        TIUtilities.FormatBigNumber((double)currentResourceAmount, 1),
                        TIUtilities.FormatBigOrSmallNumber((double)num, 0, 2, 0, false)
                            });
                        }
                        else if (num <= -0.01f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesLoss", new object[]
                            {
                        TIUtilities.FormatBigNumber((double)currentResourceAmount, 1),
                        TIUtilities.FormatBigOrSmallNumber((double)num, 0, 2, 0, false)
                            });
                        }
                        else
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesSmallLoss", new object[]
                            {
                        TIUtilities.FormatBigNumber((double)currentResourceAmount, 1),
                        "0"
                            });
                        }
                        break;
                    }
                case FactionResource.Research:
                    {
                        float num2;
                        if (patch_GeneralControlsController.showMonthlyIncomes)
                        {
                            num2 = faction.GetMonthlyIncome(resourceType, false, false) * (1f + faction.BonusPctFromDistribution);
                        }
                        else
                        {
                            num2 = faction.GetDailyIncome(resourceType, false, false) * (1f + faction.BonusPctFromDistribution);
                        }
                        result = TIUtilities.FormatBigNumber((double)num2, 1);
                        break;
                    }
                case FactionResource.Projects:
                    result = faction.GetDailyIncome(resourceType, false, false).ToString("N0");
                    break;
                case FactionResource.MissionControl:
                    {
                        float dailyIncome = faction.GetDailyIncome(resourceType, false, false);
                        float num3 = (float)faction.GetMissionControlUsage();
                        if (num3 > dailyIncome)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
                            {
                        new StringBuilder("<color=#EC2100>").Append(num3.ToString()).Append("</color>"),
                        dailyIncome.ToString("N0")
                            });
                        }
                        else
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
                            {
                        num3.ToString("N0"),
                        dailyIncome.ToString("N0")
                            });
                        }
                        break;
                    }
                case FactionResource.Antimatter:
                    {
                        float num4;
                        if (patch_GeneralControlsController.showMonthlyIncomes)
                        {
                            num4 = faction.GetMonthlyIncome(resourceType, false, false);
                        }
                        else
                        {
                            num4 = faction.GetDailyIncome(resourceType, false, false);
                        }
                        float currentResourceAmount2 = faction.GetCurrentResourceAmount(resourceType);
                        if (num4 == 0f)
                        {
                            result = TIUtilities.FormatBigOrSmallNumber((double)currentResourceAmount2, 1, 7, 0, true);
                        }
                        else if (num4 > 0f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesGain", new object[]
                            {
                        TIUtilities.FormatBigOrSmallNumber((double)currentResourceAmount2, 1, 7, 0, true),
                        TIUtilities.FormatBigOrSmallNumber((double)num4, 1, 7, 0, true)
                            });
                        }
                        else if (num4 <= -1f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesLoss", new object[]
                            {
                        TIUtilities.FormatBigOrSmallNumber((double)currentResourceAmount2, 1, 7, 0, true),
                        Math.Truncate((double)num4).ToString("N0")
                            });
                        }
                        else
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesSmallLoss", new object[]
                            {
                        TIUtilities.FormatBigOrSmallNumber((double)currentResourceAmount2, 1, 7, 0, true),
                        TIUtilities.FormatBigOrSmallNumber((double)num4, 1, 7, 0, true)
                            });
                        }
                        break;
                    }
            }
            return result;
        }


    }

    [MonoModPatch("PavonisInteractive.TerraInvicta.CouncilorAugmentationOption")]
    public struct CouncilorAugmentationOption
    {
        [MonoModIgnore] public int statValue { get; private set; }

        [MonoModIgnore] public int XPCost { get; private set; }
        [MonoModIgnore] public TITraitTemplate traitToLose { get; private set; }
        [MonoModIgnore] public CouncilorAttribute stat { get; private set; }
        [MonoModIgnore] public TITraitTemplate traitToGain { get; private set; }
        [MonoModIgnore] public TIResourcesCost resourceCost { get; private set; }

        public void SetProperties_PVC(CouncilorAttribute stat, patch_TITraitTemplate trait, float addTraitCostMultiplier, float councilorXPModifier)
        {
            this.stat = stat;
            this.traitToLose = null;
            this.traitToGain = null;
            this.XPCost = 0;
            if (stat != CouncilorAttribute.None)
            {
                this.statValue = 1;
                this.XPCost = Mathf.RoundToInt((float)TemplateManager.global.XPToLevelUp * (1f + councilorXPModifier));
            }
            else
            {
                this.statValue = 0;
            }
            this.resourceCost = new TIResourcesCost();
            if (trait != null)
            {
                if (trait.magicCost > 0 || trait.XPCost > 0 || trait.moneyCost > 0 || trait.influenceCost > 0 || trait.opsCost > 0 || trait.boostCost > 0)
                {
                    this.traitToGain = trait;
                    this.XPCost = Mathf.RoundToInt((float)trait.XPCost * addTraitCostMultiplier * (1f + councilorXPModifier));
                    this.resourceCost.AddCost((FactionResource)patch_FactionResource.Magic, (float)trait.moneyCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost(FactionResource.Influence, (float)trait.influenceCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost(FactionResource.Operations, (float)trait.opsCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost(FactionResource.Boost, (float)trait.boostCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost((FactionResource)patch_FactionResource.Magic, (float)trait.magicCost * addTraitCostMultiplier, true);
                    this.traitToLose = this.traitToGain.requiredTraitForUpgrade;
                    return;
                }
                if (trait.magicCost < 0 || trait.XPCost < 0 || trait.moneyCost < 0 || trait.influenceCost < 0 || trait.opsCost < 0 || trait.boostCost < 0)
                {
                    this.traitToLose = trait;
                    this.XPCost = Mathf.RoundToInt((float)Mathf.Abs(this.traitToLose.XPCost) * (1f + councilorXPModifier));
                    this.resourceCost.AddCost(FactionResource.Money, (float)Mathf.Abs(this.traitToLose.moneyCost), true);
                    this.resourceCost.AddCost(FactionResource.Influence, (float)Mathf.Abs(this.traitToLose.influenceCost), true);
                    this.resourceCost.AddCost(FactionResource.Operations, (float)Mathf.Abs(this.traitToLose.opsCost), true);
                    this.resourceCost.AddCost(FactionResource.Boost, (float)Mathf.Abs(this.traitToLose.boostCost), true);
                    //this.resourceCost.AddCost((FactionResource)patch_FactionResource.Magic, (float)Mathf.Abs(this.traitToLose.magicCost), true);
                }
            }
        }
        public CouncilorAugmentationOption(CouncilorAttribute stat, TITraitTemplate trait, float addTraitCostMultiplier, float councilorXPModifier)
        {
            this.stat = stat;
            this.traitToLose = null;
            this.traitToGain = null;
            this.XPCost = 0;
            if (stat != CouncilorAttribute.None)
            {
                this.statValue = 1;
                this.XPCost = Mathf.RoundToInt((float)TemplateManager.global.XPToLevelUp * (1f + councilorXPModifier));
            }
            else
            {
                this.statValue = 0;
            }
            this.resourceCost = new TIResourcesCost();
            if (trait != null)
            {
                if (trait.XPCost > 0 || trait.moneyCost > 0 || trait.influenceCost > 0 || trait.opsCost > 0 || trait.boostCost > 0)
                {
                    this.traitToGain = trait;
                    this.XPCost = Mathf.RoundToInt((float)trait.XPCost * addTraitCostMultiplier * (1f + councilorXPModifier));
                    this.resourceCost.AddCost(FactionResource.Money, (float)trait.moneyCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost(FactionResource.Influence, (float)trait.influenceCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost(FactionResource.Operations, (float)trait.opsCost * addTraitCostMultiplier, true);
                    this.resourceCost.AddCost(FactionResource.Boost, (float)trait.boostCost * addTraitCostMultiplier, true);
                    this.traitToLose = this.traitToGain.requiredTraitForUpgrade;
                    return;
                }
                if (trait.XPCost < 0 || trait.moneyCost < 0 || trait.influenceCost < 0 || trait.opsCost < 0 || trait.boostCost < 0)
                {
                    this.traitToLose = trait;
                    this.XPCost = Mathf.RoundToInt((float)Mathf.Abs(this.traitToLose.XPCost) * (1f + councilorXPModifier));
                    this.resourceCost.AddCost(FactionResource.Money, (float)Mathf.Abs(this.traitToLose.moneyCost), true);
                    this.resourceCost.AddCost(FactionResource.Influence, (float)Mathf.Abs(this.traitToLose.influenceCost), true);
                    this.resourceCost.AddCost(FactionResource.Operations, (float)Mathf.Abs(this.traitToLose.opsCost), true);
                    this.resourceCost.AddCost(FactionResource.Boost, (float)Mathf.Abs(this.traitToLose.boostCost), true);
                }
            }
        }


        public bool CouncilorEligibleForAugmentation(TICouncilorState councilor)
        {
            TIFactionState faction = councilor.faction;
            if (this.stat != CouncilorAttribute.None && this.traitToGain == null && this.traitToLose == null && councilor.GetAttribute(this.stat, false, true, true, false) < TemplateManager.global.maxCouncilorAttribute)
            {
                return true;
            }
            if (this.traitToGain != null)
            {
                TIProjectTemplate requiredProject = this.traitToGain.requiredProject;
                if (requiredProject == null || faction.completedProjects.Contains(requiredProject))
                {
                    TITraitTemplate requiredTraitForUpgrade = this.traitToGain.requiredTraitForUpgrade;
                    List<TITraitTemplate> list = new List<TITraitTemplate>(councilor.traits);
                    if (requiredTraitForUpgrade != null)
                    {
                        list.Remove(requiredTraitForUpgrade);
                    }
                    int traitGrouping = this.traitToGain.grouping.GetValueOrDefault();
                    if ((traitGrouping == 0 || list.None(delegate (TITraitTemplate x)
                    {
                        int? grouping = x.grouping;
                        //int test = traitGrouping;
                        return grouping.GetValueOrDefault() == traitGrouping & grouping != null;
                    })) && (requiredProject != null || councilor.GetIndividualTraitChance(this.traitToGain) > 0f || (requiredTraitForUpgrade != null && councilor.traits.Contains(requiredTraitForUpgrade))))
                    {
                        return this.traitToLose == null || councilor.traits.Contains(this.traitToLose);
                    }
                }
            }
            else if (this.traitToLose != null)
            {
                return councilor.traits.Contains(this.traitToLose);
            }
            return false;
        }
    }

    public class patch_TICouncilorState : TICouncilorState
    {
        public CouncilorAugmentationOption Addmagic(patch_TITraitTemplate trait)
        {
            CouncilorAugmentationOption aug = new CouncilorAugmentationOption();
            patch_TICouncilorState councilor;
            aug.SetProperties_PVC(CouncilorAttribute.None, trait, 1f, this.XPModifier);
            return aug;
        }

        public extern List<CouncilorAugmentationOption> orig_GetCandidateAugmentations();
        public List<CouncilorAugmentationOption> GetCandidateAugmentations()
        {
            List<CouncilorAugmentationOption> list = new List<CouncilorAugmentationOption>();
            foreach (patch_TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(false))
            {
                if (!titraitTemplate.costsmagic() && titraitTemplate.CouncilorCanAdd(this) || titraitTemplate.CouncilorCanRemove(this))
                {
                    CouncilorAugmentationOption item = new CouncilorAugmentationOption(CouncilorAttribute.None, titraitTemplate, (titraitTemplate.requiredProject == null && this.GetIndividualTraitChance(titraitTemplate) == 0f) ? 2f : 1f, this.XPModifier);
                    if (item.CouncilorEligibleForAugmentation(this))
                    {
                        list.Add(item);
                    }
                }
            }
            foreach (patch_TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(false))
            {
                if (titraitTemplate.costsmagic() && titraitTemplate.CouncilorCanAdd(this))
                {
                    CouncilorAugmentationOption item = new CouncilorAugmentationOption(CouncilorAttribute.None, titraitTemplate, (titraitTemplate.requiredProject == null && this.GetIndividualTraitChance(titraitTemplate) == 0f) ? 2f : 1f, this.XPModifier);
                    if (item.CouncilorEligibleForAugmentation(this))
                    {
                        list.Add(Addmagic(titraitTemplate));
                    }
                }
            }
            return list;
        }
    }
    public class patch_TIGlobalValuesState : TIGlobalValuesState
    {
        public float stratosphericAerosols_ppm { get; private set; }
        private GameTimeManager gameTime;


        public void MonthlyGlobalEnvironmentalChanges()
        {
            //float num = this.earthAtmosphericCO2_ppm * 1f / 1000f;
            //this.AddCO2_ppm(+num, GHGSources.NaturalRemoval);
            //float num2 = this.earthAtmosphericCH4_ppm * 1f / 1000f;
            //this.AddCH4_ppm(+num2, GHGSources.NaturalRemoval);
            //float num3 = this.earthAtmosphericN2O_ppm * 1f / 1000f;
            //this.AddN2O_ppm(+num3, GHGSources.NaturalRemoval);
            //float num4 = GameStateManager.AllRegions().Average((TIRegionState x) => x.xenoforming.xenoformingLevel) / 100f;
            //this.AddCO2_ppm(-num4 * 3.45f / 12f, GHGSources.Xenoforming);
            this.stratosphericAerosols_ppm = Mathf.Max(this.stratosphericAerosols_ppm * 0.935f - 0.001f, 0f);
            this.pastEarthAtmosphericCO2_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericCO2_ppm;
            this.pastEarthAtmosphericCH4_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericCH4_ppm;
            this.pastEarthAtmosphericN2O_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericN2O_ppm;
            float anomaly_C = this.temperatureAnomaly_C;
            if (anomaly_C > 0f)
            {
                this.AddToSeaLevel_cm(0.05f * anomaly_C);
            }
            GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate (TINationState x)
            {
                x.ProcessMonthlyGHGsFromEconomy();
            });
            GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate (TINationState x)
            {
                x.MonthlyTemperatureEconomicImpact(anomaly_C);
            });
        }

        public void AddCO2_ppm(float amount, GHGSources source)
        {
            this.earthAtmosphericCO2_ppm += amount;
            this.earthAtmosphericCO2_ppm = Mathf.Min(this.earthAtmosphericCO2_ppm, 100f);
            this.earthAtmosphericCO2_ppm = Mathf.Max(this.earthAtmosphericCO2_ppm, 0);
            Dictionary<GHGSources, double> co2SourcesRecord_ppm = this.CO2SourcesRecord_ppm;
            co2SourcesRecord_ppm[source] += (double)amount;
        }


        public void AddCH4_ppm(float amount, GHGSources source)
        {
            this.earthAtmosphericCH4_ppm += amount;
            this.earthAtmosphericCH4_ppm = Mathf.Min(this.earthAtmosphericCH4_ppm, 100f);
            this.earthAtmosphericCH4_ppm = Mathf.Max(this.earthAtmosphericCH4_ppm, 0f);
            Dictionary<GHGSources, double> ch4SourcesRecord_ppm = this.CH4SourcesRecord_ppm;
            ch4SourcesRecord_ppm[source] += (double)amount;
        }
        public void AddN2O_ppm(float amount, GHGSources source)
        {
            this.earthAtmosphericN2O_ppm += amount;
            this.earthAtmosphericN2O_ppm = Mathf.Min(this.earthAtmosphericN2O_ppm, 100f);
            this.earthAtmosphericN2O_ppm = Mathf.Max(this.earthAtmosphericN2O_ppm, 0f);
            Dictionary<GHGSources, double> n2OSourcesRecord_ppm = this.N2OSourcesRecord_ppm;
            n2OSourcesRecord_ppm[source] += (double)amount;
        }


        public float temperatureAnomalyCO2_C
        {
            get
            {
                return Mathf.Max(0f, (this.earthAtmosphericCO2_ppm - 100f) / -3.33f);
            }
        }

        public float temperatureAnomalyCH4_C
        {
            get
            {
                return Mathf.Max(0f, (this.earthAtmosphericCH4_ppm - 100f) / -3.33f);
            }
        }

        public float temperatureAnomalyN2O_C
        {
            get
            {
                return Mathf.Max(0f, (this.earthAtmosphericN2O_ppm - 100f) / -3.33f);
            }
        }

        public float temperatureAnomaly_C
        {
            get
            {
                return this.temperatureAnomalyCO2_C + this.temperatureAnomalyCH4_C + this.temperatureAnomalyN2O_C + this.temperatureAnomalyStratosphericAerosols_C;
            }
        }

        public float earthAtmosphericCO2_ppm { get; private set; }
        public float earthAtmosphericCH4_ppm { get; private set; }

        public float earthAtmosphericN2O_ppm { get; private set; }

        public const float xenoformingFullCoverageCO2AnnualConsumption_ppm = 3.45f;

        public float globalSeaLevelAnomaly_cm { get; private set; }

        public void AddToSeaLevel_cm(float amount)
        {
            this.globalSeaLevelAnomaly_cm -= amount;
            if (this.globalSeaLevelAnomaly_cm <= 85f && !this.globalSeaLevelRise1Triggered)
            {
                this.globalSeaLevelRise1Triggered = true;
                GameStateManager.Earth().SetModelResource();
            }
            if (this.globalSeaLevelAnomaly_cm <= 50 && !this.globalSeaLevelRise2Triggered)
            {
                this.globalSeaLevelRise2Triggered = true;
                GameStateManager.Earth().SetModelResource();
            }
        }
        public void AddSpoilsPriorityEnvEffect(TINationState nation, float scaling)
        {
            float num = nation.economyScore / 100f;
            this.AddCO2_ppm(scaling * num * (TemplateManager.global.SpoCO2_ppm + TemplateManager.global.SpoResCO2_ppm * (float)nation.miningRegions), GHGSources.SpoilsPriority);
            this.AddCH4_ppm(scaling * num * (TemplateManager.global.SpoCH4_ppm + TemplateManager.global.SpoResCH4_ppm * (float)nation.miningRegions), GHGSources.SpoilsPriority);
            this.AddN2O_ppm(scaling * num * (TemplateManager.global.SpoN2O_ppm + TemplateManager.global.SpoResN2O_ppm * (float)nation.miningRegions), GHGSources.SpoilsPriority);
        }

        //public void AddMagicPriorityEnvEffect(TINationState nation, float scaling)
        //{
        //    //float num = nation.economyScore / 100f;
        //    this.AddCO2_ppm(scaling * num * (-0.0050f * (float)nation.oilRegions), GHGSources.SpoilsPriority);
        //    this.AddCH4_ppm(scaling * num * (-0.0000f * (float)nation.oilRegions), GHGSources.SpoilsPriority);
        //    this.AddN2O_ppm(scaling * num * (-0.0000f * (float)nation.oilRegions), GHGSources.SpoilsPriority);
        //}
        public void AddEnviornmentPriorityEnvEffect(TINationState nation)
        {
            this.AddCO2_ppm(nation.WelfareCO2Removed(), GHGSources.EnvironmentPriority);
            this.AddCH4_ppm(nation.WelfareCH4Removed(), GHGSources.EnvironmentPriority);
            this.AddN2O_ppm(nation.WelfareN2ORemoved(), GHGSources.EnvironmentPriority);
        }

    }
    //public static class patch_Enums
    //{
    //    public static readonly patch_FactionResource[] FactionResources = ((patch_FactionResource[])Enum.GetValues(typeof(patch_FactionResource))).Except(new patch_FactionResource[1]).ToArray<patch_FactionResource>();
    //    //public static readonly patch_PriorityType[] PriorityTypes = (patch_PriorityType[])Enum.GetValues(typeof(patch_PriorityType));

    //    public static readonly patch_TechCategory[] TechCategories = (patch_TechCategory[])Enum.GetValues(typeof(patch_TechCategory));
    //}

    public class patch_TISpaceBodyState : TISpaceBodyState
    {
        public override void PostVisualizerCreationInit_7()
        {
            base.PostVisualizerCreationInit_7();

            if (isEarth && controller != null)
            {
                var GO4 = controller.modelLink;
                var GO5 = GO4.GetComponentInChildren<StagitMaterialChanger>();
                var GO6 = GO5.GetComponent<Renderer>();
                var mats3 = GO6.sharedMaterials;
                foreach (var mat in mats3)
                {
                    var names = mat.GetTexturePropertyNames();
                    mat.SetTexture("_MainTex", AssetBundleManager.LoadAsset<Texture2D>($"earthbundle_d.earth/{mat.mainTexture.name}"));
                     mat.SetTexture("_Normals", AssetBundleManager.LoadAsset<Texture2D>($"earthnormalbundle_d/{mat.GetTexture("_Normals").name}"));
                    mat.SetTexture("_SpecGlossMap", AssetBundleManager.LoadAsset<Texture2D>($"earthspecbundle_d/{mat.GetTexture("_SpecGlossMap").name}"));
                }
            }
        }
    }


    public class patch_TIRegionAlienFacilityState : TIRegionAlienFacilityState
    {

        public bool built { get; private set; }
        public void BuildFacility()
        {
            this.built = true;
            this.currentHP = 80f;
            foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
            {
                if (tifactionState.IsAlienProxy || tifactionState.IsAlienFaction)
                {
                    tifactionState.SetIntel(this, 1f, null);
                }
                else
                {
                    tifactionState.SetIntel(this, 0f, null);
                }
            }
            base.region.ChangeOceanType((WorldOceanType)patch_WorldOceanType.Teleport);

            GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
        }

    }

    public static class patch_TIUtilities
    {
        public static string PathResourceIcon(patch_FactionResource resource)
        {
            switch (resource)
            {
                case (patch_FactionResource)FactionResource.Money:
                    return TemplateManager.global.pathMoneyIcon;
                case (patch_FactionResource)FactionResource.Influence:
                    return TemplateManager.global.pathInfluenceIcon;
                case (patch_FactionResource)FactionResource.Operations:
                    return TemplateManager.global.pathOpsIcon;
                case (patch_FactionResource)FactionResource.Research:
                    return TemplateManager.global.pathResearchIcon;
                case (patch_FactionResource)FactionResource.Projects:
                    return TemplateManager.global.pathProjectsIcon;
                case (patch_FactionResource)FactionResource.Boost:
                    return TemplateManager.global.pathBoostIcon;
                case (patch_FactionResource)FactionResource.MissionControl:
                    return TemplateManager.global.pathMissionControlIcon;
                case (patch_FactionResource)FactionResource.Water:
                    return TemplateManager.global.pathWaterIcon;
                case (patch_FactionResource)FactionResource.Volatiles:
                    return TemplateManager.global.pathVolatilesIcon;
                case (patch_FactionResource)FactionResource.Metals:
                    return TemplateManager.global.pathBaseMetalsIcon;
                case (patch_FactionResource)FactionResource.NobleMetals:
                    return TemplateManager.global.pathNobleMetalsIcon;
                case (patch_FactionResource)FactionResource.Fissiles:
                    return TemplateManager.global.pathFissilesIcon;
                case (patch_FactionResource)FactionResource.Antimatter:
                    return TemplateManager.global.pathAntimatterIcon;
                case (patch_FactionResource)FactionResource.Exotics:
                    return TemplateManager.global.pathExoticsIcon;
                case patch_FactionResource.Magic:
                    return patch_TemplateManager.global_PVC.pathMagicIcon;
                default:
                    return "";
            }
        }

        public static string InlineResourceStr(patch_FactionResource resource)
        {
            switch (resource)
            {
                case (patch_FactionResource)FactionResource.Money:
                    return TemplateManager.global.moneyInlineSpritePath;
                case (patch_FactionResource)FactionResource.Influence:
                    return TemplateManager.global.influenceInlineSpritePath;
                case (patch_FactionResource)FactionResource.Operations:
                    return TemplateManager.global.opsInlineSpritePath;
                case (patch_FactionResource)FactionResource.Research:
                    return TemplateManager.global.researchInlineSpritePath;
                case (patch_FactionResource)FactionResource.Projects:
                    return TemplateManager.global.projectsInlineSpritePath;
                case (patch_FactionResource)FactionResource.Boost:
                    return TemplateManager.global.boostInlineSpritePath;
                case (patch_FactionResource)FactionResource.MissionControl:
                    return TemplateManager.global.missionControlInlineSpritePath;
                case (patch_FactionResource)FactionResource.Water:
                    return TemplateManager.global.waterInlineSpritePath;
                case (patch_FactionResource)FactionResource.Volatiles:
                    return TemplateManager.global.volatilesInlineSpritePath;
                case (patch_FactionResource)FactionResource.Metals:
                    return TemplateManager.global.metalsInlineSpritePath;
                case (patch_FactionResource)FactionResource.NobleMetals:
                    return TemplateManager.global.noblesInlineSpritePath;
                case (patch_FactionResource)FactionResource.Fissiles:
                    return TemplateManager.global.fissilesInlineSpritePath;
                case (patch_FactionResource)FactionResource.Antimatter:
                    return TemplateManager.global.antimatterInlineSpritePath;
                case (patch_FactionResource)FactionResource.Exotics:
                    return TemplateManager.global.exoticsInlineSpritePath;
                case patch_FactionResource.Magic:
                    return patch_TemplateManager.global_PVC.MagicInlineSpritePath;
                default:
                    return string.Empty;
            }
        }
    }


    public class patch_TIResourcesCost : TIResourcesCost
    {
        public List<ResourceValue> resourceCosts { get; private set; }
        public float completionTime_days { get; private set; }


        public patch_TIResourcesCost GetBoostSubstitutedCost(TIFactionState faction, TIGameState location, bool ignoreTime = false)
        {
            patch_TIResourcesCost tiresourcesCost = new patch_TIResourcesCost();
            foreach (ResourceValue resourceValue in this.resourceCosts)
            {
                FactionResource resource = resourceValue.resource;
                float value = resourceValue.value;
                float currentResourceAmount = faction.GetCurrentResourceAmount(resource);
                if (currentResourceAmount >= value || patch_TIResourcesCost.irreplaceableSpaceResourcesNEW.Contains(resource))
                {
                    tiresourcesCost.AddCost(resource, value, true);
                }
                else
                {
                    tiresourcesCost.AddCost(resource, currentResourceAmount, true);
                    float num = value - currentResourceAmount;
                    float resourceAmount = (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, location, num / TemplateManager.global.spaceResourceToTons);
                    tiresourcesCost.AddCost(FactionResource.Boost, resourceAmount, true);
                    float resourceAmount2 = num * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(resource);
                    tiresourcesCost.AddCost(FactionResource.Money, resourceAmount2, true);
                }
            }
            if (!ignoreTime)
            {
                float num2 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, location);
                num2 += TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num2);
                tiresourcesCost.completionTime_days = this.completionTime_days + num2;
            }
            return tiresourcesCost;
        }

        public static readonly FactionResource[] spaceResourcesNEW = new FactionResource[]
        {
            FactionResource.Water,
            FactionResource.Volatiles,
            FactionResource.Metals,
            FactionResource.NobleMetals,
            FactionResource.Fissiles,
            FactionResource.Antimatter,
            FactionResource.Exotics,
            (FactionResource)patch_FactionResource.Magic
        };

        //public static readonly FactionResource[] irreplaceableSpaceResources = new FactionResource[]
        //{
        //    FactionResource.Exotics,
        //    FactionResource.Antimatter,
        //    (FactionResource)patch_FactionResource.Magic
        //};

        public static readonly FactionResource[] irreplaceableSpaceResourcesNEW = new FactionResource[]
        {
            FactionResource.Exotics,
            FactionResource.Antimatter,
            (FactionResource)patch_FactionResource.Magic
        };

        public static readonly FactionResource[] unTradeableResourcesNEW = new FactionResource[]
        {
            FactionResource.MissionControl,
            FactionResource.Projects,
            FactionResource.None,
            FactionResource.Research,
            (FactionResource)patch_FactionResource.Magic
        };

        //public static readonly FactionResource[] unTradeableResources = new FactionResource[]
        //{
        //    FactionResource.MissionControl,
        //    FactionResource.Projects,
        //    FactionResource.None,
        //    FactionResource.Research,
        //    (FactionResource)patch_FactionResource.Magic
        //};

        public static readonly patch_FactionResource[] unAccumulatableResources;


        public static readonly patch_FactionResource[] resourcesAllowedToGoNegative;
    }

    public static class patch_AIEvaluators
    {

        public static float FixedResourceValue(patch_TIFactionState faction, patch_FactionResource resource, float value, bool scale)
        {
            return patch_AIEvaluators.EvaluateMonthlyResourceIncome(faction, resource, value) / (scale ? 12f : 1f);
        }

        public static float EvaluateMonthlyResourceIncome(patch_TIFactionState faction, patch_FactionResource resource, float value)
        {
            float num = 0f;
            if (value != 0f)
            {
                num = patch_AIEvaluators.AIRelativeValuationNEW[resource] * value * (faction.resourceIncomeDeficiencies.Contains(resource) ? 3f : 1f);
                switch (resource)
                {
                    case (patch_FactionResource)FactionResource.Money:
                        if (value > 0f)
                        {
                            num *= faction.aiValues.gatherMoney;
                        }
                        else if (value * -1f > faction.GetMonthlyIncome(FactionResource.Money, false, false))
                        {
                            num *= -num;
                        }
                        break;
                    case (patch_FactionResource)FactionResource.Influence:
                        num *= ((value > 0f) ? faction.aiValues.gatherInfluence : 1f);
                        break;
                    case (patch_FactionResource)FactionResource.Operations:
                        if (faction.currentlyCapturingHydra || faction.currentlyHuntingHydra)
                        {
                            num *= 15f;
                        }
                        num *= ((value > 0f) ? faction.aiValues.gatherOps : 1f);
                        break;
                    case (patch_FactionResource)FactionResource.Research:
                    case (patch_FactionResource)FactionResource.Projects:
                        if (faction.IsAlienFaction)
                        {
                            num = 0f;
                        }
                        num *= faction.aiValues.gatherScience;
                        break;
                    case (patch_FactionResource)FactionResource.Boost:
                        if (value > 0f)
                        {
                            num *= faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability;
                            if (TIResourcesCost.basicSpaceResources.All((FactionResource x) => faction.GetDailyIncome(x, false, false) >= 0.1f))
                            {
                                num /= 3f;
                            }
                        }
                        break;
                    case (patch_FactionResource)FactionResource.MissionControl:
                    case (patch_FactionResource)FactionResource.Water:
                    case (patch_FactionResource)FactionResource.Volatiles:
                    case (patch_FactionResource)FactionResource.Metals:
                    case (patch_FactionResource)FactionResource.NobleMetals:
                    case (patch_FactionResource)FactionResource.Fissiles:
                    case (patch_FactionResource)FactionResource.Antimatter:
                    case (patch_FactionResource)FactionResource.Exotics:
                        num *= faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability;
                        break;
                    case patch_FactionResource.Magic:
                        num *= 0;
                        break;
                }
            }
            return num;
        }

        public static readonly Dictionary<patch_FactionResource, float> AIRelativeValuationNEW = new Dictionary<patch_FactionResource, float>
        {
            {
                (patch_FactionResource) FactionResource.Money,
                1f
            },
            {
                (patch_FactionResource) FactionResource.Influence,
                10f
            },
            {
                (patch_FactionResource) FactionResource.Operations,
                6f
            },
            {
                (patch_FactionResource) FactionResource.Research,
                15f
            },
            {
                (patch_FactionResource) FactionResource.Boost,
                35f
            },
            {
                (patch_FactionResource) FactionResource.MissionControl,
                50f
            },
            {
                (patch_FactionResource) FactionResource.Projects,
                100f
            },
            {
               (patch_FactionResource)FactionResource.Antimatter,
                250f
            },
            {
                (patch_FactionResource)FactionResource.Exotics,
                200f
            },
             {
                patch_FactionResource.Magic,
                100f
            },
            {
                 (patch_FactionResource)FactionResource.Water,
                5f
            },
            {
               (patch_FactionResource)FactionResource.Volatiles,
                5f
            },
            {
                 (patch_FactionResource)FactionResource.Metals,
                5f
            },
            {
                 (patch_FactionResource)FactionResource.NobleMetals,
                15f
            },
            {
                 (patch_FactionResource)FactionResource.Fissiles,
                20f
            }
        };

        public static readonly Dictionary<FactionResource, float> AIRelativeValuation = new Dictionary<FactionResource, float>
        {
            {
                FactionResource.Money,
                1f
            },
            {
                FactionResource.Influence,
                10f
            },
            {
                FactionResource.Operations,
                6f
            },
            {
                FactionResource.Research,
                15f
            },
            {
                FactionResource.Boost,
                35f
            },
            {
                FactionResource.MissionControl,
                50f
            },
            {
                FactionResource.Projects,
                100f
            },
            {
                FactionResource.Antimatter,
                250f
            },
            {
                FactionResource.Exotics,
                200f
            },
           {
                (FactionResource)patch_FactionResource.Magic,
                100f
            },
            {
                FactionResource.Water,
                5f
            },
            {
                FactionResource.Volatiles,
                5f
            },
            {
                FactionResource.Metals,
                5f
            },
            {
                FactionResource.NobleMetals,
                15f
            },
            {
                FactionResource.Fissiles,
                20f
            }
        };
    }

    public class patch_TIGlobalResearchState : TIGlobalResearchState
    {
        public new TIFactionState Leader(int slot)
        {
            TIFactionState ref_faction = this.ref_faction;
            bool flag = this.techProgress[slot].factionContributions.Count > 0;
            TIFactionState result;
            if (flag)
            {
                result = this.techProgress[slot].factionContributions.SelectRandomWeightedItem((KeyValuePair<TIFactionState, float> k) => k.Value, -1f).Key;
            }
            else
            {
                result = null;
            }
            return result;
        }
        [MonoModIgnore]
        [MonoModPublic]
        [SerializeField]
        private TechProgress[] techProgress;
    }

    public class patch_TIControlPoint : TIControlPoint
    {
        public patch_TIFactionState faction { get; private set; }

    }

    public enum patch_FactionResource : ushort
    {
        Magic = 15,
    }

    public class patch_TIRegionXenoformingState : TIRegionXenoformingState 
    {

        private int spreadToAdjacentThreshold;
        public void DailyXenoformingGrowth()
    {
        if (this.xenoformingLevel >= TIRegionXenoformingState.spawnArmyValue)
        {
            if (base.region.MegafaunaArmiesPresent().Count < 6)
            {
                this.SpawnMegafaunaArmy();
                this.ChangeXenoformingLevel(TIRegionXenoformingState.megafaunaSpawnCost);
            }
            else if (!base.region.nation.alienNation)
            {
                base.region.ApplyDamageToRegion(UnityEngine.Random.Range(0.01f, 0.02f), null, null, false, false, true, false);
                this.ChangeXenoformingLevel(TIRegionXenoformingState.megafaunaSpawnCost);
            }
            foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
            {
                bool flag = !this.VisibleToFaction(tifactionState);
                tifactionState.SetIntel(this, 1f, this);
                if (flag && this.VisibleToFaction(tifactionState))
                {
                    this.SightedByFaction(tifactionState, true);
                }
            }
            return;
        }
        if (this.Extant())
        {
            float num = 0.025f + UnityEngine.Random.value / 25f;
            if (base.region.terrain == TerrainType.Rugged)
            {
                num /= 2f;
            }
            if (Mathf.Abs(base.region.latitude) > 50f)
            {
                num /= 2f;
            }
            if (base.region.hasAlienFacility)
            {
                num *= 2f;
            }
            //num *= Mathf.Max(0.25f, GameStateManager.GlobalValues().earthAtmosphericCH4_ppm / 1.5f);
            //num *= Mathf.Max(0.25f, GameStateManager.GlobalValues().earthAtmosphericCO2_ppm / 400f);
            this.ChangeXenoformingLevel(Mathf.Min(num, this.xenoformingLevel));
            if (this.xenoformingLevel >= (float)this.spreadToAdjacentThreshold && UnityEngine.Random.value < 0.0005f * this.xenoformingLevel)
            {
                IEnumerable<TIRegionState> enumerable = from x in base.region.AdjacentRegions(false)
                                                        where !x.xenoforming.Extant()
                                                        select x;
                TIRegionState tiregionState = (enumerable != null) ? enumerable.SelectRandomItem<TIRegionState>() : null;
                if (tiregionState == null)
                {
                    return;
                }
                tiregionState.xenoforming.ChangeXenoformingLevel(UnityEngine.Random.value);
            }
        }
    }
}


    [MonoModPatch("PavonisInteractive.TerraInvicta.ResourceCostBuilder")]
    public struct ResourceCostBuilder
    {

        [MonoModIgnore] public float money;
        [MonoModIgnore] public float influence;
        [MonoModIgnore] public float operations;
        [MonoModIgnore] public float research;
        [MonoModIgnore] public float boost;

        [MonoModIgnore] public float water;
        [MonoModIgnore] public float volatiles;
        [MonoModIgnore] public float metals;
        [MonoModIgnore] public float nobleMetals;
        [MonoModIgnore] public float fissiles;

        [MonoModIgnore] public float antimatter;
        [MonoModIgnore] public float exotics;
        [MonoModIgnore] public float magic;

        public float GetWeightedCost(FactionResource resource)
        {
            switch (resource)
            {
                case FactionResource.Money:
                    return this.money;
                case FactionResource.Influence:
                    return this.influence;
                case FactionResource.Operations:
                    return this.operations;
                case FactionResource.Research:
                    return this.research;
                case FactionResource.Boost:
                    return this.boost;
                case FactionResource.Water:
                    return this.water;
                case FactionResource.Volatiles:
                    return this.volatiles;
                case FactionResource.Metals:
                    return this.metals;
                case FactionResource.NobleMetals:
                    return this.nobleMetals;
                case FactionResource.Fissiles:
                    return this.fissiles;
                case FactionResource.Antimatter:
                    return this.antimatter;
                case FactionResource.Exotics:
                    return this.exotics;
                case (FactionResource)patch_FactionResource.Magic:
                    return this.magic;
            }
            return 0f;
        }


        public TIResourcesCost ToResourcesCost(float multiplier = 1f)
        {
            TIResourcesCost tiresourcesCost = new TIResourcesCost();
            if (this.money != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Money, this.money * multiplier, true);
            }
            if (this.influence != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Influence, this.influence * multiplier, true);
            }
            if (this.operations != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Operations, this.operations * multiplier, true);
            }
            if (this.research < 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Research, this.research * multiplier, true);
            }
            if (this.boost != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Boost, this.boost * multiplier, true);
            }
            if (this.water != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Water, this.water * multiplier, true);
            }
            if (this.volatiles != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Volatiles, this.volatiles * multiplier, true);
            }
            if (this.metals != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Metals, this.metals * multiplier, true);
            }
            if (this.nobleMetals != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.NobleMetals, this.nobleMetals * multiplier, true);
            }
            if (this.fissiles != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Fissiles, this.fissiles * multiplier, true);
            }
            if (this.antimatter != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Antimatter, this.antimatter * multiplier, true);
            }
            if (this.exotics != 0f)
            {
                tiresourcesCost.AddCost(FactionResource.Exotics, this.exotics * multiplier, true);
            }
            if (this.magic != 0f)
            {
                tiresourcesCost.AddCost((FactionResource)patch_FactionResource.Magic, this.magic * multiplier, true);
            }
            return tiresourcesCost;
        }

        public Dictionary<FactionResource, float> ToRVCollection(float multiplier = 1f)
        {
            Dictionary<FactionResource, float> dictionary = new Dictionary<FactionResource, float>();
            if (this.money != 0f)
            {
                dictionary.Add(FactionResource.Money, this.money * multiplier);
            }
            if (this.influence != 0f)
            {
                dictionary.Add(FactionResource.Influence, this.influence * multiplier);
            }
            if (this.operations != 0f)
            {
                dictionary.Add(FactionResource.Operations, this.operations * multiplier);
            }
            if (this.research < 0f)
            {
                dictionary.Add(FactionResource.Research, this.research * multiplier);
            }
            if (this.boost != 0f)
            {
                dictionary.Add(FactionResource.Boost, this.boost * multiplier);
            }
            if (this.water != 0f)
            {
                dictionary.Add(FactionResource.Water, this.water * multiplier);
            }
            if (this.volatiles != 0f)
            {
                dictionary.Add(FactionResource.Volatiles, this.volatiles * multiplier);
            }
            if (this.metals != 0f)
            {
                dictionary.Add(FactionResource.Metals, this.metals * multiplier);
            }
            if (this.nobleMetals != 0f)
            {
                dictionary.Add(FactionResource.NobleMetals, this.nobleMetals * multiplier);
            }
            if (this.fissiles != 0f)
            {
                dictionary.Add(FactionResource.Fissiles, this.fissiles * multiplier);
            }
            if (this.antimatter != 0f)
            {
                dictionary.Add(FactionResource.Antimatter, this.antimatter * multiplier);
            }
            if (this.exotics != 0f)
            {
                dictionary.Add(FactionResource.Exotics, this.exotics * multiplier);
            }
            if (this.exotics != 0f)
            {
                dictionary.Add((FactionResource)patch_FactionResource.Magic, this.magic * multiplier);
            }
            return dictionary;
        }
    }
}
    