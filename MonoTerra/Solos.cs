using HarmonyLib;
using UnityEngine;
using System.Reflection;
using PavonisInteractive.TerraInvicta;
using AssetBundles;
using System;
using MonoMod;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Actions;
using System.Linq;
using System.Text;
using System.Collections;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using static PavonisInteractive.TerraInvicta.Audio.SoundEvents;
using PavonisInteractive.TerraInvicta.Entities;
using UnityEngine.UI;


//public class patch_TISpaceShipTemplate : TISpaceShipTemplate
//{
//    public TIResourcesCost earthResourceConstructionCost(TIFactionState faction, TIHabModuleState shipyard)
//    {
//        TIResourcesCost tiresourcesCost = this.spaceResourceConstructionCost(false, shipyard, true, false, false);
//        TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
//        float num = 0f;
//        float num2 = 0f;
//        foreach (FactionResource resource in TIResourcesCost.replaceableSpaceResources)
//        {
//            num += tiresourcesCost.GetSingleCostValue(resource) * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(resource);
//            num2 += tiresourcesCost.GetSingleCostValue(resource);
//        }
//        tiresourcesCost2.AddCost(FactionResource.Money, num, true);
//        tiresourcesCost2.AddCost(FactionResource.Boost, (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, shipyard.hab.IsBase ? shipyard.ref_spaceBody : shipyard.ref_orbit, num2 / TemplateManager.global.spaceResourceToTons), true);
//        foreach (FactionResource factionResource in TIResourcesCost.irreplaceableSpaceResources)
//        {
//            tiresourcesCost2.AddCost(factionResource, tiresourcesCost.GetSingleCostValue(factionResource), true);
//        }
//        tiresourcesCost2.SetCompletionTime_Days((shipyard == null) ? this.hullTemplate.noShipyardConstructionTime_Days(faction) : (this.hullTemplate.constructionTime_Days(shipyard) + TISpaceObjectState.GenericTransferTime_d(shipyard.ref_faction, GameStateManager.Earth(), shipyard)));
//        return tiresourcesCost2;
//    }

//}


public abstract class patch_TIShipPartTemplate : TIShipPartTemplate
{
    [MonoModIgnore] public patch_TIShipPartTemplate() : base() { }

    [MonoModIgnore] public ResourceCostBuilder weightedBuildMaterials;
    [MonoModOriginal] public extern void orig_TIShipPartTemplate();
    [MonoModConstructor]
    public void TIShipPartTemplate()
    {
        orig_TIShipPartTemplate();

       ResourceCostBuilder weightedBuildMaterials = new ResourceCostBuilder();
    }
    public override abstract TIResourcesCost buildCost(float value = 0f, float value2 = 0f);
}

public class patch_TIPowerPlantTemplate : TIPowerPlantTemplate
{

    public override TIResourcesCost buildCost(float power_GW, float value2 = 0f)
    {
        float num = this.buildMass_tons(power_GW, value2);
        return this.weightedBuildMaterials.ToResourcesCost(num * TemplateManager.global.spaceResourceToTons);
    }
}



public struct patch_TechBonus
{
    public patch_TechCategory category;

    public float bonus;
}

public abstract class patch_TIGenericTechTemplate : TIGenericTechTemplate
{

    public patch_TechCategory techCategory;

    public static extern string orig_PathTechCategoryIcon(TechCategory category);
    public static string PathTechCategoryIcon(TechCategory category)
    {
        switch ((patch_TechCategory)category)
        {
            case patch_TechCategory.MagicScience: return patch_TemplateManager.global_PVC.pathMagicScienceIcon;
            default:
                return orig_PathTechCategoryIcon(category);
        }
    }
    public static extern string orig_categoryInlineSprite(TechCategory category);
    public static string categoryInlineSprite(TechCategory category)
    {
        switch ((patch_TechCategory)category)
        {
            case patch_TechCategory.MagicScience: return patch_TemplateManager.global_PVC.MagicScienceInlineSpritePath;
            default:
                return orig_categoryInlineSprite(category);
        }
    }
}

    public class TIMissionModifier_LeftoverCPMaintenance_Defender : TIMissionModifier
{
    public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
    {
        TIFactionState ref_faction = target.ref_faction;
        float? num = (ref_faction != null) ? new float?(ref_faction.GetBaselineControlPointMaintenanceCost(false)) : null;
        TIFactionState ref_faction2 = target.ref_faction;
        float valueOrDefault = (num - ((ref_faction2 != null) ? new float?(ref_faction2.GetControlPointMaintenanceFreebieCap()) : null)).GetValueOrDefault();
        if (valueOrDefault < 0f)
        {
            return valueOrDefault * patch_TemplateManager.global_PVC.TIMissionModifier_ControlPointUnder_Multiplier;
        }
        return 0f;
    }
}

public class patch_TIHabModuleTemplate : TIHabModuleTemplate
{

    [MonoModIgnore] public patch_TIHabModuleTemplate() : base() { }

    [MonoModIgnore] public ResourceCostBuilder weightedBuildMaterials;
    [MonoModOriginal] public extern void orig_TIHabModuleTemplate();
    [MonoModConstructor]
    public void TIHabModuleTemplate()
    {
        orig_TIHabModuleTemplate();

        ResourceCostBuilder weightedBuildMaterials = new ResourceCostBuilder();
    }

    public ResourceCostBuilder BuildMaterials(float irradiatedValue, TISpaceBodyState spaceBody, TIFactionState faction, float multiplier)
    {
        float num = this.Mass_tons(1f, spaceBody, faction);
        float num2 = this.Mass_tons(irradiatedValue, spaceBody, faction) - num;
        return new ResourceCostBuilder
        {
            water = this.weightedBuildMaterials.water * num * TemplateManager.global.spaceResourceToTons * multiplier,
            volatiles = this.weightedBuildMaterials.volatiles * num * TemplateManager.global.spaceResourceToTons * multiplier,
            metals = (this.weightedBuildMaterials.metals * num + num2) * TemplateManager.global.spaceResourceToTons * multiplier,
            nobleMetals = this.weightedBuildMaterials.nobleMetals * num * TemplateManager.global.spaceResourceToTons * multiplier,
            fissiles = this.weightedBuildMaterials.fissiles * num * TemplateManager.global.spaceResourceToTons * multiplier,
            antimatter = this.weightedBuildMaterials.antimatter * num * TemplateManager.global.spaceResourceToTons * multiplier,
            exotics = this.weightedBuildMaterials.exotics * num * TemplateManager.global.spaceResourceToTons * multiplier,
            magic = this.weightedBuildMaterials.magic * num * TemplateManager.global.spaceResourceToTons * multiplier
        };
    }

    //public override abstract TIResourcesCost buildCost(float value = 0f, float value2 = 0f);
    public TIResourcesCost CostFromEarth(TIFactionState faction, TIGameState destinationState, bool isUpgrade)
    {
        float irradiatedValue = TIUtilities.IrradiatedMultiplier(destinationState);
        float num = isUpgrade ? this.upgradeDiscount : 1f;
        TIResourcesCost tiresourcesCost = new TIResourcesCost();
        TISpaceBodyState spaceBody = destinationState.ref_spaceBody;
        float num2 = 1f;
        TIHabState tihabState = destinationState as TIHabState;
        if (tihabState != null)
        {
            num2 = tihabState.GetModuleConstructionTimeModifier(false);
            if (tihabState.IsBase)
            {
                spaceBody = tihabState.habSite.parentBody;
            }
        }
        else
        {
            TIHabSiteState tihabSiteState = destinationState as TIHabSiteState;
            if (tihabSiteState != null)
            {
                spaceBody = tihabSiteState.parentBody;
            }
        }
        tiresourcesCost.AddCost(FactionResource.Boost, this.BoostCostFromEarth(irradiatedValue, spaceBody, faction, destinationState, num, null), true);
        tiresourcesCost.AddCost(FactionResource.Money, this.MoneyCost(irradiatedValue, spaceBody, faction, num, null), true);
        TIResourcesCost tiresourcesCost2 = this.BuildMaterials(irradiatedValue, spaceBody, faction, num).ToResourcesCost(1f);
        foreach (FactionResource factionResource in patch_TIResourcesCost.irreplaceableSpaceResourcesNEW)
        {
            tiresourcesCost.AddCost(factionResource, tiresourcesCost2.GetSingleCostValue(factionResource), true);
        }
        float num3 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
        float num4 = this.buildTime_Days * num * num2 + num3 + TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num3);
        if (tihabState != null && tihabState.coreModule.underConstruction && tihabState.tier <= this.tier)
        {
            num4 = Mathf.Max(num4, -(float)TITimeState.Now().DifferenceInDays(new TIDateTime(tihabState.coreModule.completionDate)));
        }
        tiresourcesCost.SetCompletionTime_Days(num4);
        return tiresourcesCost;
    }



    public TIResourcesCost CostFromSpace(TIFactionState faction, TIGameState destinationState, bool isUpgrade, bool substituteBoost, int maxDaysToSave = 0, bool dontRecalculateIncome = false)
    {
        //  int canFoundLocally = faction.MaxTierCanFoundAtLocation(destinationState, false, false);
        //  if (canFoundLocally >= 1)
        //  {
        float irradiatedValue = TIUtilities.IrradiatedMultiplier(destinationState);
        float num = isUpgrade ? this.upgradeDiscount : 1f;
        TISpaceBodyState ref_spaceBody = destinationState.ref_spaceBody;
        float num2 = 1f;
        float pass = 99f;
        TIHabState tihabState = null;
        if (destinationState.isHabSiteState)
        {
            ref_spaceBody = destinationState.ref_habSite.ref_spaceBody;
            pass = 1f;
        }
        else if (destinationState.isHabState)
        {
            tihabState = destinationState.ref_hab;
            num2 = tihabState.GetModuleConstructionTimeModifier(false);
            if (tihabState.IsBase)
            {
                ref_spaceBody = tihabState.habSite.ref_spaceBody;
            }
        }

        if (num2 < 1 || pass == 1)
        {
            TIResourcesCost tiresourcesCost = this.BuildMaterials(irradiatedValue, ref_spaceBody, faction, num).ToResourcesCost(1f);
            float num3 = 0f;
            if (substituteBoost && !tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity) && (faction.IsActiveHumanFaction || GameStateManager.AlienNation().extant))
            {
                tiresourcesCost = tiresourcesCost.GetBoostSubstitutedCost(faction, destinationState);
                float num4 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
                num4 += TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num4);
                if (num4 > num3)
                {
                    num3 = num4;
                }
            }
            float num5 = this.buildTime_Days * num * num2 + num3;
            if (tihabState != null && tihabState.coreModule.underConstruction && tihabState.tier <= this.tier)
            {
                num5 = Mathf.Max(num5, -(float)TITimeState.Now().DifferenceInDays(new TIDateTime(tihabState.coreModule.completionDate)));
            }
            tiresourcesCost.SetCompletionTime_Days(num5);
            return tiresourcesCost;
        }
        else
        {
            TISpaceBodyState spaceBody = destinationState.ref_spaceBody;
            TIResourcesCost tiresourcesCost = new TIResourcesCost();
            tiresourcesCost.AddCost(FactionResource.Boost, this.BoostCostFromEarth(irradiatedValue, spaceBody, faction, destinationState, num, null), true);
            tiresourcesCost.AddCost(FactionResource.Money, this.MoneyCost(irradiatedValue, spaceBody, faction, num, null), true);
            TIResourcesCost tiresourcesCost2 = this.BuildMaterials(irradiatedValue, spaceBody, faction, num).ToResourcesCost(1f);
            foreach (FactionResource factionResource in patch_TIResourcesCost.irreplaceableSpaceResourcesNEW)
            {
                tiresourcesCost.AddCost(factionResource, tiresourcesCost2.GetSingleCostValue(factionResource), true);
            }
            float num3 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
            float num4 = this.buildTime_Days * num * num2 + num3 + TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num3);
            if (tihabState != null && tihabState.coreModule.underConstruction && tihabState.tier <= this.tier)
            {
                num4 = Mathf.Max(num4, -(float)TITimeState.Now().DifferenceInDays(new TIDateTime(tihabState.coreModule.completionDate)));
            }
            tiresourcesCost.SetCompletionTime_Days(num4);
            return tiresourcesCost;
        }
    }
    //  }      
}

    public class patch_StartMenuController : StartMenuController
    {
        private extern void orig_Initialize();
        private void Initialize()
        {
            orig_Initialize();

            AssetBundleManager.Initialize();
            var GO2 = GameObject.Find("EarthObject");
            var GO3 = GO2.GetComponent<StagitMaterialChanger>();
            var GO4 = GO3.GetComponent<Renderer>();
            var mats3 = GO4.sharedMaterials;
            foreach (var mat in mats3)
            {
                mat.SetTexture("_MainTex", AssetBundleManager.LoadAsset<Texture2D>($"earthbundle_d.earth/{mat.mainTexture.name}"));

                mat.SetTexture("_Normals", AssetBundleManager.LoadAsset<Texture2D>($"earthnormalbundle_d/{mat.GetTexture("_Normals").name}"));

                mat.SetTexture("_SpecGlossMap", AssetBundleManager.LoadAsset<Texture2D>($"earthspecbundle_d/{mat.GetTexture("_SpecGlossMap").name}"));
        }
        var GO5 = GameObject.Find("TILogo");
        var GO6 = GO5.GetComponent<Image>();
        GO6.sprite = AssetBundleManager.LoadAsset<Sprite>("misc/MenuTitle");
    }
    }

    public class TICouncilorCondition_iStatWithoutOrgsTotal : TICouncilorCondition
    {

        public override List<string> descriptionParams
        {
            get
            {
                return new List<string>(2)
            {
                TIUtilities.GetAttributeString(this.strIdx.ToEnum(CouncilorAttribute.None)),
                base.GetNumericComparisonString(false)
            };
            }
        }

        public override bool PassesCondition(TIGameState state)
        {
            TICouncilorState ref_councilor = state.ref_councilor;
            CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
            int attribute = ref_councilor.GetAttribute(CouncilorAttribute.Persuasion, false, false, true, false);
            int attribute2 = ref_councilor.GetAttribute(CouncilorAttribute.Investigation, false, false, true, false);
            int attribute3 = ref_councilor.GetAttribute(CouncilorAttribute.Espionage, false, false, true, false);
            int attribute4 = ref_councilor.GetAttribute(CouncilorAttribute.Command, false, false, true, false);
            int attribute5 = ref_councilor.GetAttribute(CouncilorAttribute.Administration, false, false, true, false);
            int attribute6 = ref_councilor.GetAttribute(CouncilorAttribute.Science, false, false, true, false);
            int attribute7 = ref_councilor.GetAttribute(CouncilorAttribute.Security, false, false, true, false);
            int value = attribute7 + attribute6 + attribute5 + attribute4 + attribute3 + attribute2 + attribute;
            return ref_councilor != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, value, TIUtilities.GetIntValue(this.strValue));
        }
    }

    public class TIMissionCondition_FederationMember : TIMissionCondition
    {

        public override List<string> feedback
        {
            get
            {
                return new List<string>
            {
                "TIMissionCondition_EQSMember",
                "TIMissionCondition_EQSwhat"
            };
            }
        }

        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            TINationState ref_nation = possibleTarget.ref_nation;
            bool flag = ref_nation.displayNameWithArticle != "Equestria";
            string result;
            if (flag)
            {
                result = "_Pass";
            }
            else
            {
                bool flag2 = !ref_nation.inSameFederation(GameStateManager.NationLookup()["EQS"]);
                if (flag2)
                {
                    result = "TIMissionCondition_EQSMember";
                }
                else
                {
                    bool flag3 = ref_nation.inSameFederation(GameStateManager.NationLookup()["EQS"]);
                    if (flag3)
                    {
                        result = "_Pass";
                    }
                    else
                    {
                        result = "TIMissionCondition_EQSwhat";
                    }
                }
            }
            return result;
        }
    }

    public class TIMissionCondition_NotDrained : TIMissionCondition
    {

        public override List<string> feedback
        {
            get
            {
                return new List<string>
            {
                "TIMissionCondition_Dry"
            };
            }
        }

        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            bool flag = !possibleTarget.isCouncilorState || !(possibleTarget != councilor);
            string result;
            if (flag)
            {
                result = "TIMissionCondition_GenericFail";
            }
            else
            {
                TICouncilorState ref_councilor = possibleTarget.ref_councilor;
                bool flag2 = !ref_councilor.HasTraitWithTag("Drained");
                if (flag2)
                {
                    result = "_Pass";
                }
                else
                {
                    result = "TIMissionCondition_Dry";
                }
            }
            return result;
        }
    }

public class TIMissionEffect_Advise_Scientist : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TICouncilorState councilor = mission.councilor;
        if (target.isNationState)
        {
            float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
            target.ref_nation.AddAdvisingCouncilor(councilor);
            target.ref_nation.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise_Scientist.Special1", new object[]
            {
                (target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
            });
        }
        if (target.isHabState)
        {
            TIHabState ref_hab = target.ref_hab;
            float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) ;
            ref_hab.AddAdvisingCouncilor(councilor);
            ref_hab.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise_Scientist.Special2", new object[]
            {
                (ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) - advisingAttribute).ToPercent("P0"),
            });
        }
        TISpaceFleetState ref_fleet = target.ref_fleet;
        return Loc.T("TIMissionEffect_Advise_Scientist.Special3", new object[]
        {
            councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0"),
            councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
        });
    }
}

public class TIMissionEffect_Advise_Statesman : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TICouncilorState councilor = mission.councilor;
        if (target.isNationState)
        {
            float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;
            target.ref_nation.AddAdvisingCouncilor(councilor);
            target.ref_nation.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise_Statesman.Special1", new object[]
            {
                (target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
            });
        }
        if (target.isHabState)
        {
            TIHabState ref_hab = target.ref_hab;
            float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);
            ref_hab.AddAdvisingCouncilor(councilor);
            ref_hab.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise_Statesman.Special2", new object[]
            {
                (ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration) - advisingAttribute3).ToPercent("P0"),
            });
        }
        TISpaceFleetState ref_fleet = target.ref_fleet;
        return Loc.T("TIMissionEffect_Advise_Statesman.Special3", new object[]
        {
        });
    }
}

public class TIMissionEffect_Advise_Super : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        {
            TICouncilorState councilor = mission.councilor;
            if (target.isNationState)
            {
                float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;
                float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
                target.ref_nation.AddAdvisingCouncilor(councilor);
                target.ref_nation.AddAdvisingCouncilor(councilor);
                return Loc.T("TIMissionEffect_Advise_Super.Special1", new object[]
                {
                (target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
                (target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
                });
            }
            if (target.isHabState)
            {
                TIHabState ref_hab = target.ref_hab;
                float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);
                float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) ;
                ref_hab.AddAdvisingCouncilor(councilor);
                ref_hab.AddAdvisingCouncilor(councilor);
                return Loc.T("TIMissionEffect_Advise_Super.Special2", new object[]
                {
                (ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration) - advisingAttribute3).ToPercent("P0"),
                (ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) - advisingAttribute).ToPercent("P0"),
                });
            }
            TISpaceFleetState ref_fleet = target.ref_fleet;
            return Loc.T("TIMissionEffect_Advise_Super.Special3", new object[]
            {
            councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0"),
            councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
            });
        }
    }
}

public class TIMissionCondition_MaxArmyLevel : TIMissionCondition
{
    public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
    {
        TINationState ref_nation = possibleTarget.ref_nation;
        if ( ref_nation.militaryTechLevel < ref_nation.maxMilitaryTechLevel)
        {
            return "_Pass";
        }
        return "TIMissionCondition_MaxArmy";
    }
}
public class TIMissionEffect_TrainArmy : TIMissionEffect
{

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TINationState ref_nation = target.ref_nation;
        if (base.MissionSuccess(outcome))
        {
            float strength = ((outcome == TIMissionOutcome.CriticalSuccess) ? 0.1f : 0.05f);
            ref_nation.AddToMilitaryTechLevel(strength);
            return strength.ToString();
        }
        if (outcome == TIMissionOutcome.CriticalFailure)
        {
            float num = -0.1f;
            ref_nation.AddToMilitaryTechLevel(num);
            return (num).ToString();
        }
        return string.Empty;
    }
}


public class TIMissionEffect_GiveMagic : TIMissionEffect
{
    
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TINationState ref_nation = target.ref_nation;
        float strength = 0.1f;
        ref_nation.AddToMilitaryTechLevel(strength);
        return string.Empty;
    }
}

public class TIMissionEffect_Healthspell : TIMissionEffect
{

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TIRegionState ref_region = target.ref_region;
        float strength = 1000f;
            ref_region.ChangeAnnualPopulationGrowthModifier(strength);
            return string.Empty;
    }
}

public class TIMissionEffectHarmonyPrinciple : TIMissionEffect
{

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {

        TICouncilorState councilor = mission.councilor;
        TINationState ref_nation = target.ref_nation;
        if (base.MissionSuccess(outcome))
        {
            float num = 0.25f * ((outcome == TIMissionOutcome.CriticalSuccess) ? 2f : 1f);
            ref_nation.AddToCohesion(num);
            return string.Empty;
        }
        else
        {
            if (outcome == TIMissionOutcome.CriticalFailure)
            {
                float inputFloat = ref_nation.PropagandaOnPop(councilor.faction.ideology, (float)Mathf.Max(-1, councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false) - 11));
                return Loc.T("TIMissionEffect_Propaganda.Special", new object[]
                {
                    inputFloat.ToPercent("P0"),
                    ref_nation.GetPublicOpinionOfFaction(councilor.faction).ToPercent("P0")
                });
            }
            return string.Empty;
        }
    }
}

public class TIMissionWealthofMagic : TIMissionEffect
{

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TINationState ref_nation = target.ref_nation;
        float strength = -0.1f;
        ref_nation.AddToInequality(strength, TINationState.InequalityChangeReason.WelfarePriority);
        return string.Empty;
    }
}

public class TIMissionEffectProtectionfield : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TICouncilorState councilor = mission.councilor;
        patch_TIRegionState ref_region = (patch_TIRegionState)target.ref_region;
        ref_region.ChangeEnvType(EnvironmentType.Beneficiary);
        Log.Debug($"A {ref_region.environment}");
        return string.Empty;
    }
}

public class TIMissionEffect_Siphon : TIMissionEffect
    {
        public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
        {
            TICouncilorState ref_councilor = target.ref_councilor;
            StringBuilder stringBuilder = new StringBuilder();
           if (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.Failure || outcome == TIMissionOutcome.CriticalFailure)
			{
				if (ref_councilor.GetTraitWithSpecialTraitRule(SpecialTraitRule.AtrocityIfAssassinationAttemptUpon) != null)
				{
					mission.councilor.faction.CommitAtrocity(1, TIFactionState.AtrocityCause.AssassinatedBeloved);
				}
				if (ref_councilor.GrantsMarkedToAssassin() && !mission.councilor.traitTemplateNames.Contains("Marked"))
				{
					mission.councilor.AddTrait("Marked");
					stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString()));
				}
			}
        if (outcome == TIMissionOutcome.CriticalFailure && !ref_councilor.detained && mission.councilor.GetProtectors().Count == 0 && !mission.councilor.isAlien)
        {
            if (ref_councilor.traits.None((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.HardTarget))
            {
                TIFactionState faction = ref_councilor.faction;
                if (faction != mission.councilor.faction)
                {
                    mission.councilor.DetainCouncilor(faction, 2f, 1f, true);
                    stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[]
                    {
                            faction.displayNameCapitalized
                    }));
                }
            }
        }
        bool flag7 = outcome == TIMissionOutcome.CriticalSuccess;
            if (flag7)
            {
                ref_councilor.AddTrait("Drained");
                mission.councilor.AddTrait("Satiated");
            }
            bool flag8 = outcome == TIMissionOutcome.Success;
            if (flag8)
            {
                ref_councilor.AddTrait("Drained");
            }
            return string.Empty;
        }
        public override bool HasDelayedEffect()
        {
            return true;
        }
        public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
        {
            TICouncilorState ref_councilor = target.ref_councilor;
            bool flag = base.MissionSuccess(outcome);
            if (flag)
            {
                bool flag2 = mission.councilor.faction != ref_councilor.faction;
                if (flag2)
                {
                    ref_councilor.faction.AddSuspicionForMajorReversal(25f, ref_councilor);
                    TINotificationQueueState.LogMyCouncilorAssassinated(ref_councilor, mission.councilor, mission.missionTemplate.hate[(int)outcome]);
                }
                bool flag3 = !mission.councilor.assassinations.ContainsKey(ref_councilor.faction);
                if (flag3)
                {
                    mission.councilor.assassinations.Add(ref_councilor.faction, 0);
                }
                Dictionary<TIFactionState, int> dictionary = mission.councilor.assassinations;
                TIFactionState faction = ref_councilor.faction;
                Dictionary<TIFactionState, int> dictionary2 = dictionary;
                TIFactionState key = faction;
                int num = dictionary2[key];
                dictionary2[key] = num + 1;
                bool flag4 = !mission.councilor.faction.factionAssassinations.ContainsKey(ref_councilor.faction);
                if (flag4)
                {
                    mission.councilor.faction.factionAssassinations.Add(ref_councilor.faction, 0);
                }
                dictionary = mission.councilor.faction.factionAssassinations;
                faction = ref_councilor.faction;
                Dictionary<TIFactionState, int> dictionary3 = dictionary;
                key = faction;
                num = dictionary3[key];
                dictionary3[key] = num + 1;
                bool flag5 = outcome == TIMissionOutcome.CriticalSuccess;
                if (flag5)
                {
                    ref_councilor.KillCouncilor(true, (outcome == TIMissionOutcome.Success) ? mission.councilor.faction : null);
                }
            }
            else
            {
                bool flag6 = outcome == TIMissionOutcome.CriticalFailure && !ref_councilor.detained && mission.councilor.GetProtectors().Count == 0;
                if (flag6)
                {
                    bool flag7 = ref_councilor.traits.Any((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.HardTarget);
                    if (flag7)
                    {
                        mission.councilor.KillCouncilorOnMission(mission);
                        return;
                    }
                }
                bool flag8 = mission.councilor.faction == ref_councilor.faction;
                if (flag8)
                {
                    IEnumerable<TIFactionState> enumerable = from x in GameStateManager.AllHumanFactions().Except(new List<TIFactionState>
                {
                    ref_councilor.faction
                })
                                                             where x.turnedCouncilors.Count < 2
                                                             select x;
                    bool flag9 = enumerable.Count<TIFactionState>() > 0;
                    if (flag9)
                    {
                        TIFactionState tifactionState = enumerable.SelectRandomItem<TIFactionState>();
                        ref_councilor.TurnCouncilor(tifactionState);
                        mission.councilor.faction.DismissCouncilor(ref_councilor, tifactionState);
                        ref_councilor.AddTrait("Vengeful");
                    }
                    else
                    {
                        List<TransferOrgToFactionPoolAction> list = new List<TransferOrgToFactionPoolAction>();
                        foreach (TIOrgState org in ref_councilor.orgs)
                        {
                            list.Add(new TransferOrgToFactionPoolAction(org, ref_councilor));
                        }
                        foreach (TransferOrgToFactionPoolAction action in list)
                        {
                            mission.councilor.faction.playerControl.StartAction(action);
                        }
                        mission.councilor.faction.DismissCouncilor(ref_councilor, mission.councilor.faction);
                        mission.councilor.faction.availableCouncilors.Remove(ref_councilor);
                    }
                }
            }
        }
    }


public class patch_TITraitTemplate : TITraitTemplate
{

    public patch_TechBonus[] techBonuses;
    public int magicCost;
    public bool costsmagic()
    {
        return (this.magicCost > 0);
       }


    public bool CouncilorCanAdd(TICouncilorState councilor)
    {
        return (this.magicCost > 0 || this.XPCost > 0 || this.moneyCost > 0 || this.influenceCost > 0 || this.opsCost > 0 || this.boostCost > 0 || this.requiredProject != null) && !councilor.traits.Contains(this);
    }

    public bool CouncilorCanRemove(TICouncilorState councilor)
    {
        return (this.magicCost < 0 || this.XPCost < 0 || this.moneyCost < 0 || this.influenceCost < 0 || this.opsCost < 0 || this.boostCost < 0) && councilor.traits.Contains(this);
    }
}

public class TIMissionModifier_Miltech : TIMissionModifier
{
    
    public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
    {
        float num = 0f;
        TIFactionState faction = attackingCouncilor.faction;
        TIRegionState ref_region = target.ref_region;
        if (ref_region != null)
        { 
                num += ref_region.nation.militaryTechLevel * 3f;
            }
        
        return Math.Max(num, 0f);
    }
}
public class patch_TIMissionModifier_ResourceSpent : TIMissionModifier_ResourceSpent
    {
        public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
        {
            this.heldResource = resource;
            float result = 0f;
            if (resourcesSpent > 0f)
            {
                if (resourcesSpent < 1f)
                {
                    result = resourcesSpent;
                }
                else if (resource == FactionResource.Money)
                {
                    result = 1f + (Mathf.Log(resourcesSpent / TemplateManager.global.missionMoneyMultiplier) * 2) / Mathf.Log(2f);
                }
                else
                {
                    result = 1f + (Mathf.Log(resourcesSpent) * 2) / Mathf.Log(2f);
                }
            }
            return result;
        }
        public override string displayName
        {
            get
            {
                return TIUtilities.GetResourceString(this.heldResource);
            }
        }
        private FactionResource heldResource;
    }



    public enum patch_OrgType : ushort
    {
        Military = 42
    }

//public enum patch_PriorityType : ushort
//{
//    ExploitMagic = 420,
//}

public enum patch_TechCategory : ushort
{
     Materials = 0,
        SpaceScience = 1,
        Energy = 2,
        LifeScience = 3,
        MilitaryScience = 4,
        InformationScience = 5,
        SocialScience = 6,
        Xenology = 7,
        MagicScience = 8,
}
public enum patch_Context : ushort
{
    ExploitMagicPriority = 420,
    MegaspellLevel = 421,
}

public enum patch_WorldOceanType : ushort
{
    Teleport = 4,
}

public enum patch_DeploymentType : ushort
{
    teleport = 3
}