﻿using HarmonyLib;
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
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using TMPro;

public abstract class patch_TIOperationTemplate : TIOperationTemplate, IOperation
{
    public string operationIconImagePath
    {
        get
        {
            string starter;
            string name = GetType().Name;
            switch (name)
            {
                case "TeleportArmyOperation":
                    starter = "c_operations/ICO_";
                    break;
                default:
                    starter = "operations/ICO_";
                    break;
            }
            return new StringBuilder(starter).Append(name).ToString();
        }
    }
}

    public static class OperationsManager
{
        public static void Initalize()
        {
            OperationsManager.armyOperations.Clear();
            OperationsManager.fleetOperations.Clear();
            OperationsManager.spaceOperations.Clear();
            OperationsManager.nationOperations.Clear();
            OperationsManager.operationsLookup.Clear();
            OperationsManager.armyOperations.Add(new TeleportArmyOperation());
            OperationsManager.armyOperations.Add(new DeployArmyOperation_OpenTarget(false));
            OperationsManager.armyOperations.Add(new DeployArmiesOperation(false));
            OperationsManager.armyOperations.Add(new ArmyGoHomeOperation());
            OperationsManager.armyOperations.Add(new AllArmiesGoHomeOperation());
            OperationsManager.armyOperations.Add(new DeployArmyOperation_TargetHome());
            OperationsManager.armyOperations.Add(new AllArmiesPathHomeOperation());
            OperationsManager.armyOperations.Add(new AssaultAlienAssetOperation());
            OperationsManager.armyOperations.Add(new AssaultSpaceFacilityOperation());
            OperationsManager.armyOperations.Add(new AnnexRegionOperation());
            OperationsManager.armyOperations.Add(new RazeRegionOperation());
            OperationsManager.armyOperations.Add(new CancelArmyOperation());
            OperationsManager.fleetOperations.Add(new TransferOperation());
            OperationsManager.fleetOperations.Add(new BombardOperation_Low());
            OperationsManager.fleetOperations.Add(new BombardOperation_Med());
            OperationsManager.fleetOperations.Add(new BombardOperation_High());
            OperationsManager.fleetOperations.Add(new AssaultHabOperation());
            OperationsManager.fleetOperations.Add(new DestroyHabOperation());
            OperationsManager.fleetOperations.Add(new MergeFleetOperation());
            OperationsManager.fleetOperations.Add(new MergeAllFleetOperation());
            OperationsManager.fleetOperations.Add(new SplitFleetOperation());
            OperationsManager.fleetOperations.Add(new ResupplyAndRepairOperation());
            OperationsManager.fleetOperations.Add(new ResupplyOperation());
            OperationsManager.fleetOperations.Add(new RepairFleetOperation());
            OperationsManager.fleetOperations.Add(new InterfleetRefuelOperation());
            OperationsManager.fleetOperations.Add(new LandOnSurfaceOperation());
            OperationsManager.fleetOperations.Add(new LaunchFromSurfaceOperation());
            OperationsManager.fleetOperations.Add(new UndockFromStationOperation());
            OperationsManager.fleetOperations.Add(new SurveyPlanetFromFleetOperation());
            OperationsManager.fleetOperations.Add(new FoundSolarPlatformOperation());
            OperationsManager.fleetOperations.Add(new FoundFissionPlatformOperation());
            OperationsManager.fleetOperations.Add(new FoundFusionPlatformOperation());
            OperationsManager.fleetOperations.Add(new FoundSolarOutpostOperation());
            OperationsManager.fleetOperations.Add(new FoundFissionOutpostOperation());
            OperationsManager.fleetOperations.Add(new FoundFusionOutpostOperation());
            OperationsManager.fleetOperations.Add(new FoundAutomatedSolarPlatformOperation());
            OperationsManager.fleetOperations.Add(new FoundAutomatedFissionPlatformOperation());
            OperationsManager.fleetOperations.Add(new FoundAutomatedSolarOutpostOperation());
            OperationsManager.fleetOperations.Add(new FoundAutomatedFissionOutpostOperation());
            OperationsManager.fleetOperations.Add(new ScuttleShipsOperation());
            OperationsManager.fleetOperations.Add(new SetHomeportOperation());
            OperationsManager.fleetOperations.Add(new ClearHomeportOperation());
            OperationsManager.fleetOperations.Add(new AlienEarthSurveillanceOperation());
            OperationsManager.fleetOperations.Add(new AlienCrashdownOperation());
            OperationsManager.fleetOperations.Add(new AlienLandArmyOperation());
            OperationsManager.fleetOperations.Add(new CancelFleetOperation());
            OperationsManager.spaceOperations.Add(new LaunchProbeOperation());
            OperationsManager.spaceOperations.Add(new FoundPlatformOperation());
            OperationsManager.spaceOperations.Add(new FoundOrbitalOperation());
            OperationsManager.spaceOperations.Add(new FoundRingOperation());
            OperationsManager.spaceOperations.Add(new FoundOutpostOperation());
            OperationsManager.spaceOperations.Add(new FoundSettlementOperation());
            OperationsManager.spaceOperations.Add(new FoundColonyOperation());
            OperationsManager.spaceOperations.Add(new FoundAutomatedPlatformOperation());
            OperationsManager.spaceOperations.Add(new FoundAutomatedOutpostOperation());
            OperationsManager.nationOperations.Add(new NuclearWeaponsStrike());
            foreach (IOperation operation in OperationsManager.armyOperations)
            {
                OperationsManager.operationsLookup.Add(operation.GetType(), operation);
            }
            foreach (IOperation operation2 in OperationsManager.fleetOperations)
            {
                OperationsManager.operationsLookup.Add(operation2.GetType(), operation2);
            }
            foreach (IOperation operation3 in OperationsManager.spaceOperations)
            {
                OperationsManager.operationsLookup.Add(operation3.GetType(), operation3);
            }
            foreach (IOperation operation4 in OperationsManager.nationOperations)
            {
                OperationsManager.operationsLookup.Add(operation4.GetType(), operation4);
            }
        }

    // Token: 0x04000CE4 RID: 3300
    public static List<IOperation> armyOperations = new List<IOperation>();

    // Token: 0x04000CE5 RID: 3301
    public static List<IOperation> fleetOperations = new List<IOperation>();

    // Token: 0x04000CE6 RID: 3302
    public static List<IOperation> spaceOperations = new List<IOperation>();

    // Token: 0x04000CE7 RID: 3303
    public static List<IOperation> nationOperations = new List<IOperation>();

    // Token: 0x04000CE8 RID: 3304
    public static Dictionary<Type, IOperation> operationsLookup = new Dictionary<Type, IOperation>();
}

//public class ArmyGoHomeOperation : TIArmyOperationTemplate
//{
//    // Token: 0x06000AA8 RID: 2728 RVA: 0x000365BA File Offset: 0x000347BA
//    public override OperationTiming GetOperationTiming()
//    {
//        return OperationTiming.DelayedExecutionOfInstantEffect;
//    }

//    // Token: 0x06000AA9 RID: 2729 RVA: 0x000365BD File Offset: 0x000347BD
//    public override int SortOrder()
//    {
//        return 1;
//    }

//    // Token: 0x06000AAA RID: 2730 RVA: 0x000365C0 File Offset: 0x000347C0
//    public override Type GetTargetingMethod()
//    {
//        return typeof(TIOperationTargeting_Region);
//    }

//    // Token: 0x06000AAB RID: 2731 RVA: 0x000365CC File Offset: 0x000347CC
//    public override bool IsCombatOperation()
//    {
//        return false;
//    }

//    // Token: 0x06000AAC RID: 2732 RVA: 0x000365CF File Offset: 0x000347CF
//    public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
//    {
//        return 45f;
//    }

//    // Token: 0x06000AAD RID: 2733 RVA: 0x000365D6 File Offset: 0x000347D6
//    public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
//    {
//        return actorState.ref_army.armyType == ArmyType.Human && actorState.ref_army.currentRegion != actorState.ref_army.homeRegion && !actorState.ref_army.atSea;
//    }

//    // Token: 0x06000AAE RID: 2734 RVA: 0x00036612 File Offset: 0x00034812
//    public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
//    {
//        if (this.OpVisibleToActor(actorState, target) && actorState.ref_army.CurrentOperations().Count == 0)
//        {
//            TINationState homeNation = actorState.ref_army.homeNation;
//            return homeNation != null && homeNation.wars.Count == 0;
//        }
//        return false;
//    }

//    // Token: 0x06000AAF RID: 2735 RVA: 0x00036650 File Offset: 0x00034850
//    public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
//    {
//        if (actorState.isArmyState && actorState.ref_army.homeRegion != null)
//        {
//            TIArmyState ref_army = actorState.ref_army;
//            return new List<TIGameState>
//            {
//                ref_army.homeRegion.ref_gameState
//            };
//        }
//        return new List<TIGameState>();
//    }

//    // Token: 0x06000AB0 RID: 2736 RVA: 0x0003669C File Offset: 0x0003489C
//    public override void ExecuteOperation(TIGameState actorState, TIGameState target)
//    {
//        TIArmyState ref_army = actorState.ref_army;
//        TIRegionState ref_region = target.ref_region;
//        if (actorState.ref_army.homeNation.wars.Count == 0)
//        {
//            ref_army.MoveArmyToRegion(ref_region, false);
//            TINotificationQueueState.LogArmyArrivesInRegion(ref_army, ref_army.currentRegion);
//        }
//    }
//}



public class TeleportArmyOperation : TIArmyOperationTemplate
{    public override OperationTiming GetOperationTiming()
    {
        return OperationTiming.DelayedExecutionOfInstantEffect;
    }

    public override int SortOrder()
    {
        return 0;
    }

    public override Type GetTargetingMethod()
    {
        return typeof(TIOperationTargeting_Region);
    }
    public override bool IsCombatOperation()
    {
        return false;
    }

    public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
    {

        return 5;
    }

    public static List<TIGameState> GetPossibleTargets(TIGameState actorState, bool allowJournies)
    {
       TIArmyState ref_army = actorState.ref_army;
        return patch_TIArmyState.TeleportValid(ref_army, (patch_TIRegionState)ref_army.currentRegion).Cast<TIGameState>().ToList<TIGameState>();
    }

    public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
    {
        TIArmyState ref_army = actorState.ref_army;
        return patch_TIArmyState.TeleportValid(ref_army, (patch_TIRegionState)ref_army.currentRegion).Cast<TIGameState>().ToList<TIGameState>();
    }

    //public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
    //{
    //    TIArmyState ref_army = actorState.ref_army;
    //    TIRegionState ref_region = target.ref_region;
    //    if (this.JourneyMode && ref_army.currentRegion != ref_region)
    //    {
    //        float num;
    //        List<TIRegionState> journey = ref_army.GetJourney(ref_army.currentRegion, ref_region, out num);
    //        if (journey == null)
    //        {
    //            return false;
    //        }
    //        target = journey[1];
    //        if (ref_army.destinationQueue.Count == 0)
    //        {
    //            ref_army.destinationQueue.Add(ref_region);
    //        }
    //    }
    //    GameControl.eventManager.TriggerEvent(new ArmyPathChanged(ref_army), null, new object[]
    //    {
    //        ref_army.currentRegion
    //    });
    //    List<TIRegionState> destinationQueue = ref_army.destinationQueue.ToList<TIRegionState>();
    //    bool flag = base.OnOperationConfirm(actorState, target, resourcesCost, trajectory);
    //    ref_army.destinationQueue = destinationQueue;
    //    if (flag && ref_region != ref_army.currentRegion)
    //    {
    //        ref_army.SetIsMoving();
    //        if (ref_army.homeNation.wars.Contains(ref_region.nation) || (ref_army.AlienMegafaunaArmy && !ref_region.nation.alienNation))
    //        {
    //            TINotificationQueueState.LogArmyLaunchesTowardEnemyRegion(ref_army, ref_region);
    //        }
    //    }
    //    return flag;
    //}

    public override void ExecuteOperation(TIGameState actorState, TIGameState target)
    {
        TIArmyState ref_army = actorState.ref_army;
        TIRegionState ref_region = target.ref_region;
        if (ref_army.currentRegion == target)
        {
            ref_army.currentOperations.RemoveAt(0);
            ref_army.SetNotMoving();
            return;
        }
        ref_army.MoveArmyToRegion(ref_region, false);
        //if (ref_army.destinationQueue.Count > 0)
        //{
        //    if (ref_army.destinationQueue.First<TIRegionState>() == ref_army.currentRegion)
        //    {
        //        ref_army.destinationQueue.RemoveAt(0);
        //    }
        //    if (ref_army.destinationQueue.Count > 0)
        //    {
        //        if (ref_army.ref_faction != null)
        //        {
        //            ref_army.ref_faction.playerControl.StartAction(new ConfirmOperationAction(actorState, ref_army.destinationQueue.First<TIRegionState>(), new DeployArmyOperation(false), null, null));
        //        }
        //        else
        //        {
        //            new DeployArmyOperation(false).OnOperationConfirm(ref_army, ref_army.destinationQueue.First<TIRegionState>(), null, null);
        //        }
        //    }
        //}
        if (ref_army.destinationQueue.Count == 0 || !ref_army.InFriendlyRegion)
        {
            TINotificationQueueState.LogArmyArrivesInRegion(ref_army, ref_army.currentRegion);
        }
    }

    public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
    {
        base.OnOperationCancel(actorState, target, opCompleteDate);
        if (!actorState.isArmyState)
        {
            return;
        }
        TIArmyState ref_army = actorState.ref_army;
        ref_army.destinationQueue.Clear();
        GameTimeManager.Singleton.CancelTimeEvent(ref_army.armyOperationCompleteEventName, ref_army, target, this, opCompleteDate);
    }

    public override bool Equals(object obj)
    {
        return obj is TeleportArmyOperation;
    }
}

public class patch_TISpaceShipTemplate : TISpaceShipTemplate
{
    [MonoModIgnore]
    public patch_TISpaceShipTemplate(string dataNameToSet) : base(dataNameToSet)
    {
        dataName = dataNameToSet;
        SetHullTemplate(hullName);
    }
    public TIResourcesCost earthResourceConstructionCost(TIFactionState faction, TIHabModuleState shipyard)
    {
        TIResourcesCost tiresourcesCost = this.spaceResourceConstructionCost(false, shipyard, true, false, false);
        TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
        float num = 0f;
        float num2 = 0f;
        foreach (FactionResource resource in TIResourcesCost.replaceableSpaceResources)
        {
            num += tiresourcesCost.GetSingleCostValue(resource) * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(resource);
            num2 += tiresourcesCost.GetSingleCostValue(resource);
        }
        tiresourcesCost2.AddCost(FactionResource.Money, num, true);
        if (shipyard.hab.IsBase)
        {
            tiresourcesCost2.AddCost(FactionResource.Boost, (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, shipyard.ref_spaceBody, num2 / TemplateManager.global.spaceResourceToTons), true);
        }
        else
        {
            tiresourcesCost2.AddCost(FactionResource.Boost, (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, shipyard.ref_orbit, num2 / TemplateManager.global.spaceResourceToTons), true);
        }
        foreach (FactionResource factionResource in patch_TIResourcesCost.irreplaceableSpaceResourcesNEW)
        {
            tiresourcesCost2.AddCost(factionResource, tiresourcesCost.GetSingleCostValue(factionResource), true);
        }
        tiresourcesCost2.SetCompletionTime_Days((shipyard == null) ? this.hullTemplate.noShipyardConstructionTime_Days(faction) : (this.hullTemplate.constructionTime_Days(shipyard) + TISpaceObjectState.GenericTransferTime_d(shipyard.ref_faction, GameStateManager.Earth(), shipyard)));
        return tiresourcesCost2;
    }
}

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
        float irradiatedValue = TIUtilities.IrradiatedMultiplier(destinationState);
        float num = isUpgrade ? this.upgradeDiscount : 1f;
        TISpaceBodyState ref_spaceBody = destinationState.ref_spaceBody;
        float num2 = 1f;
        bool canFoundLocally = faction.CanFoundHabFromHabAtLocation(destinationState, false, false);
        float pass = 99f;
        TIHabState tihabState = null;
        bool ConstructionModule = (this.upgradesFromName == "ConstructionModule" || this.upgradesFromName == "Nanofactory") && isUpgrade;

        if ((destinationState.isHabSiteState ||  destinationState.isOrbitState) && canFoundLocally)
        {
            pass = 1f;
        }

       if (destinationState.isHabState)
        {
            tihabState = destinationState.ref_hab;
            num2 = tihabState.GetModuleConstructionTimeModifier(false);
            if (tihabState.IsBase)
            {
                ref_spaceBody = tihabState.habSite.ref_spaceBody;
            }
        }

        if ((canFoundLocally == true && (this.tier == 1 || this.dataName == "OrbitalCore" || this.dataName == "SettlementCore") || (num2 < 1  && this.tier == 1) || (num2 < 0.85 && this.tier == 2) ||  (num2 < 0.7 && this.tier == 3) || ConstructionModule) || pass == 1 || (this.dataName == "OrbitalCore" || this.dataName == "SettlementCore" && num2 < 1) || (this.dataName == "ColonyCore" || this.dataName == "RingCore" && num2 < 0.85))
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
}

    public class patch_StartMenuController : StartMenuController
    {
        private extern void orig_Initialize();
        private void Initialize()
        {
            orig_Initialize();
       //this.BuildCreditsStringsMOD();
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
   // public List<TMP_Text> TICreditsListmod = new List<TMP_Text>();
   // public List<string> ModCreditsStrings = new List<string>();
    //public void BuildCreditsStringsMOD()
    //{
    //    this.ModCreditsStrings.Clear();
    //    StringBuilder stringBuilder = new StringBuilder();
    //    int num = 0;
    //    for (int i = 0; i < TemplateManager.global.creditsEntries; i++)
    //    {
    //        string key = new StringBuilder("UI.StartScreen.ModCredits_").Append(i.ToString()).ToString();
    //        stringBuilder.Append(Loc.T(key));
    //        num++;
    //        if (num > 300)
    //        {
    //            this.ModCreditsStrings.Add(stringBuilder.ToString());
    //            stringBuilder.Clear();
    //            num = 0;
    //        }
    //    }
    //    this.ModCreditsStrings.Add(stringBuilder.ToString());
    //    for (int j = 0; j < this.ModCreditsStrings.Count; j++)
    //    {
    //        if (j > 0 && this.TICreditsListmod.Count - 1 < j)
    //        {
    //            TMP_Text component = UnityEngine.Object.Instantiate<GameObject>(this.TICreditsListmod[0].gameObject, this.TICreditsListmod[0].transform.parent).GetComponent<TMP_Text>();
    //            this.TICreditsListmod.Add(component);
    //        }
    //        this.TICreditsListmod[j].SetText(this.ModCreditsStrings[j]);
    //    }
    //    this.TICreditsListmod[0].transform.parent.gameObject.SetActive(true);
    //}

    public void BuildCreditsStrings()
    {
        this.TICreditsStrings.Clear();
        StringBuilder stringBuilder = new StringBuilder();
        int num = 0;
        for (int i = 0; i < TemplateManager.global.creditsEntries; i++)
        {
            string key = new StringBuilder("UI.StartScreen.TICredits_").Append(i.ToString()).ToString();
            stringBuilder.Append(Loc.T(key));
            num++;
            if (num > 300)
            {
                this.TICreditsStrings.Add(stringBuilder.ToString());
                stringBuilder.Clear();
                num = 0;
            }
        }
        this.TICreditsStrings.Add(stringBuilder.ToString());
        for (int j = 0; j < this.TICreditsStrings.Count; j++)
        {
            if (j > 0 && this.TICreditsList.Count - 1 < j)
            {
                TMP_Text component = UnityEngine.Object.Instantiate<GameObject>(this.TICreditsList[0].gameObject, this.TICreditsList[0].transform.parent).GetComponent<TMP_Text>();
                this.TICreditsList.Add(component);
            }
            this.TICreditsList[j].SetText(this.TICreditsStrings[j]);
        }
        this.TICreditsList[0].transform.parent.gameObject.SetActive(true);
    }
}


//Idk why this is here
    //public class TICouncilorCondition_iStatWithoutOrgsTotal : TICouncilorCondition
    //{

    //    public override List<string> descriptionParams
    //    {
    //        get
    //        {
    //            return new List<string>(2)
    //        {
    //            TIUtilities.GetAttributeString(this.strIdx.ToEnum(CouncilorAttribute.None)),
    //            base.GetNumericComparisonString(false)
    //        };
    //        }
    //    }

    //    public override bool PassesCondition(TIGameState state)
    //    {
    //        TICouncilorState ref_councilor = state.ref_councilor;
    //        CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
    //        int attribute = ref_councilor.GetAttribute(CouncilorAttribute.Persuasion, false, false, true, false);
    //        int attribute2 = ref_councilor.GetAttribute(CouncilorAttribute.Investigation, false, false, true, false);
    //        int attribute3 = ref_councilor.GetAttribute(CouncilorAttribute.Espionage, false, false, true, false);
    //        int attribute4 = ref_councilor.GetAttribute(CouncilorAttribute.Command, false, false, true, false);
    //        int attribute5 = ref_councilor.GetAttribute(CouncilorAttribute.Administration, false, false, true, false);
    //        int attribute6 = ref_councilor.GetAttribute(CouncilorAttribute.Science, false, false, true, false);
    //        int attribute7 = ref_councilor.GetAttribute(CouncilorAttribute.Security, false, false, true, false);
    //        int value = attribute7 + attribute6 + attribute5 + attribute4 + attribute3 + attribute2 + attribute;
    //        return ref_councilor != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, value, TIUtilities.GetIntValue(this.strValue));
    //    }
    //}

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






//public class Patch_ResupplyOperation : ResupplyOperation
//{

//    public TIResourcesCost PlanResupply(TISpaceFleetState fleet, bool prospectiveOnly, out bool freeRefueling, Dictionary<TISpaceShipState, int> pendingRepairedMagazines = null)
//    {
//        TIResourcesCost tiresourcesCost = new TIResourcesCost();
//        TIHabState ref_hab = fleet.ref_hab;
//        TIFactionState tifactionState = ((ref_hab != null) ? ref_hab.faction : null) ?? null;
//        float maxFractionCanSpend = (tifactionState == fleet.faction || tifactionState == null) ? 1f : (1f / (float)fleet.ref_hab.faction.habs.Count);
//        List<TISpaceShipState> list = (from x in fleet.ships
//                                       where x.NeedsRefuel()
//                                       select x into y
//                                       orderby y.currentDeltaV_kps
//                                       select y).ToList<TISpaceShipState>();
//        TIHabState ref_hab2 = fleet.ref_hab;
//        if (ref_hab2 != null && ref_hab2.AllowsResupply(fleet.faction, false, false))
//        {
//            Dictionary<TISpaceShipState, float> dictionary = fleet.ships.ToDictionary((TISpaceShipState x) => x, (TISpaceShipState y) => y.PropellantShortage_tons);
//            while (list.Count > 0)
//            {
//                List<TISpaceShipState> list2 = new List<TISpaceShipState>();
//                foreach (TISpaceShipState tispaceShipState in list)
//                {
//                    float num = this.RefuelTankAtHabDuration(tispaceShipState, tispaceShipState.ref_hab);
//                    TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
//                    float num2 = Mathf.Min(dictionary[tispaceShipState], 100f);
//                    if (num2 > 0f)
//                    {
//                        TIResourcesCost tiresourcesCost3 = tispaceShipState.GetPreferredPropellantTankCost(fleet.ref_hab.faction, num2, false);
//                        tiresourcesCost3.SetCompletionTime_Days(Mathf.Max(0.01f, num * num2 / 100f));
//                        TIResourcesCost tiresourcesCost4 = new TIResourcesCost(tiresourcesCost);
//                        tiresourcesCost4.SumCosts_NoDuration(tiresourcesCost3);
//                        bool flag = false;
//                        if (tiresourcesCost4.CanAfford(tifactionState, maxFractionCanSpend, null, float.PositiveInfinity))
//                        {
//                            flag = true;
//                        }
//                        else if (fleet.AllowUseBoostForRepairsResupply)
//                        {
//                            tiresourcesCost3 = TISpaceShipTemplate.MixedResourceConstructionCost(fleet.faction, fleet.ref_hab, tiresourcesCost3, fleet.faction.AvailableSpaceResourcesExcept(1f, tiresourcesCost), false);
//                            tiresourcesCost3.SetCompletionTime_Days(Mathf.Max(0.01f, num * num2 / 100f));
//                            TIResourcesCost tiresourcesCost5 = new TIResourcesCost(tiresourcesCost);
//                            tiresourcesCost5.SumCosts_NoDuration(tiresourcesCost3);
//                            if (tiresourcesCost5.CanAfford(tifactionState, maxFractionCanSpend, null, float.PositiveInfinity))
//                            {
//                                flag = true;
//                            }
//                        }
//                        if (flag)
//                        {
//                            if (!prospectiveOnly)
//                            {
//                                tispaceShipState.plannedResupplyAndRepair.AddPropellantToReload(num2);
//                            }
//                            Dictionary<TISpaceShipState, float> dictionary2 = dictionary;
//                            TISpaceShipState key = tispaceShipState;
//                            dictionary2[key] -= num2;
//                            tiresourcesCost2.SumCostsWithDuration(tiresourcesCost3);
//                            tiresourcesCost.SumCostsWithDuration(tiresourcesCost3);
//                        }
//                        else
//                        {
//                            list2.Add(tispaceShipState);
//                        }
//                    }
//                    else
//                    {
//                        list2.Add(tispaceShipState);
//                    }
//                    if (!prospectiveOnly)
//                    {
//                        tispaceShipState.plannedResupplyAndRepair.AddtoResupplyCost(tiresourcesCost2);
//                    }
//                }
//                foreach (TISpaceShipState item in list2)
//                {
//                    list.Remove(item);
//                }
//            }
//            if (fleet.ref_hab.faction == fleet.faction)
//            {
//                Dictionary<TISpaceShipState, Dictionary<ModuleDataEntry, int>> dictionary3 = fleet.ships.ToDictionary((TISpaceShipState sh) => sh, (TISpaceShipState weaps) => weaps.AllWeaponModuleData().ToDictionary((ModuleDataEntry mod) => mod, (ModuleDataEntry ammoNeeded) => 0));
//                using (List<TISpaceShipState>.Enumerator enumerator = fleet.ships.GetEnumerator())
//                {
//                    while (enumerator.MoveNext())
//                    {
//                        TISpaceShipState tispaceShipState2 = enumerator.Current;
//                        foreach (ModuleDataEntry moduleDataEntry in tispaceShipState2.AllWeaponModuleData())
//                        {
//                            if (pendingRepairedMagazines != null && pendingRepairedMagazines.ContainsKey(tispaceShipState2))
//                            {
//                                dictionary3[tispaceShipState2][moduleDataEntry] = ((moduleDataEntry.moduleTemplate.ref_projectileWeapon == null || !moduleDataEntry.moduleTemplate.ref_projectileWeapon.hasMagazine()) ? 0 : (moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_PendingRepairs(tispaceShipState2, pendingRepairedMagazines[tispaceShipState2]) - tispaceShipState2.ammo[moduleDataEntry]));
//                            }
//                            else
//                            {
//                                dictionary3[tispaceShipState2][moduleDataEntry] = ((moduleDataEntry.moduleTemplate.ref_projectileWeapon == null || !moduleDataEntry.moduleTemplate.ref_projectileWeapon.hasMagazine()) ? 0 : (moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_Current(tispaceShipState2) - tispaceShipState2.ammo[moduleDataEntry]));
//                            }
//                        }
//                    }
//                    goto IL_6E0;
//                }
//            IL_4FC:
//                foreach (TISpaceShipState tispaceShipState3 in fleet.ships)
//                {
//                    TIResourcesCost tiresourcesCost6 = new TIResourcesCost();
//                    float completionTime_Days = this.RearmWeaponAtHabDuration(tispaceShipState3, tispaceShipState3.ref_hab);
//                    foreach (ModuleDataEntry moduleDataEntry2 in tispaceShipState3.AllWeaponModuleData())
//                    {
//                        if (dictionary3[tispaceShipState3][moduleDataEntry2] > 0)
//                        {
//                            int num3 = Mathf.Min(50, dictionary3[tispaceShipState3][moduleDataEntry2]);
//                            TIResourcesCost tiresourcesCost7 = tispaceShipState3.CostToReloadPartialAmmo(moduleDataEntry2, num3, fleet.ref_hab, false);
//                            tiresourcesCost7.SetCompletionTime_Days(completionTime_Days);
//                            if (tiresourcesCost7 != null)
//                            {
//                                TIResourcesCost tiresourcesCost8 = new TIResourcesCost(tiresourcesCost);
//                                tiresourcesCost8.SumCosts_NoDuration(tiresourcesCost7);
//                                bool flag2 = false;
//                                if (tiresourcesCost8.CanAfford(tifactionState, 1f, null, float.PositiveInfinity))
//                                {
//                                    flag2 = true;
//                                }
//                                else if (fleet.AllowUseBoostForRepairsResupply)
//                                {
//                                    tiresourcesCost7 = TISpaceShipTemplate.MixedResourceConstructionCost(tifactionState, tispaceShipState3.ref_hab, tiresourcesCost7, tifactionState.AvailableSpaceResourcesExcept(1f, tiresourcesCost), false);
//                                    tiresourcesCost7.SetCompletionTime_Days(completionTime_Days);
//                                    TIResourcesCost tiresourcesCost9 = new TIResourcesCost(tiresourcesCost);
//                                    tiresourcesCost9.SumCosts_NoDuration(tiresourcesCost7);
//                                    if (tiresourcesCost9.CanAfford(tifactionState, maxFractionCanSpend, null, float.PositiveInfinity))
//                                    {
//                                        flag2 = true;
//                                    }
//                                }
//                                if (flag2)
//                                {
//                                    if (!prospectiveOnly)
//                                    {
//                                        tispaceShipState3.plannedResupplyAndRepair.AddAmmoOrder(moduleDataEntry2, num3);
//                                    }
//                                    tiresourcesCost.SumCostsWithDuration(tiresourcesCost7);
//                                    tiresourcesCost6.SumCostsWithDuration(tiresourcesCost7);
//                                    Dictionary<ModuleDataEntry, int> dictionary4 = dictionary3[tispaceShipState3];
//                                    ModuleDataEntry key2 = moduleDataEntry2;
//                                    dictionary4[key2] -= 50;
//                                }
//                                else
//                                {
//                                    dictionary3[tispaceShipState3][moduleDataEntry2] = 0;
//                                }
//                            }
//                            else
//                            {
//                                dictionary3[tispaceShipState3][moduleDataEntry2] = 0;
//                            }
//                        }
//                    }
//                    if (!prospectiveOnly)
//                    {
//                        tispaceShipState3.plannedResupplyAndRepair.AddtoResupplyCost(tiresourcesCost6);
//                    }
//                }
//            IL_6E0:
//                if (dictionary3.Values.Any((Dictionary<ModuleDataEntry, int> x) => x.Values.Any((int unHandledAmmo) => unHandledAmmo > 0)))
//                {
//                    goto IL_4FC;
//                }
//            }
//            float num4 = fleet.ref_hab.DaysUntilCanStartResupply();
//            if (!prospectiveOnly)
//            {
//                TIDateTime tidateTime = TITimeState.Now();
//                tidateTime.AddDays(num4);
//                foreach (TISpaceShipState tispaceShipState4 in fleet.ships)
//                {
//                    tispaceShipState4.plannedResupplyAndRepair.SetStartDate(tidateTime);
//                }
//            }
//            tiresourcesCost.AddToCompletionTime_Days(num4);
//        }
//        List<TISpaceShipState> list3 = (from x in list
//                                        where x.CanRefuelFromJovianAtmosphere() || x.CanRefuelFromHabSite(fleet.ref_habSite)
//                                        select x).ToList<TISpaceShipState>();
//        freeRefueling = (list3.Count > 0);
//        TIResourcesCost tiresourcesCost10 = new TIResourcesCost();
//        if (freeRefueling)
//        {
//            Func<KeyValuePair<FactionResource, float>, float> <> 9__11;
//            foreach (TISpaceShipState tispaceShipState5 in list3)
//            {
//                if (fleet.landed)
//                {
//                    IEnumerable<KeyValuePair<FactionResource, float>> en = tispaceShipState5.drive.GetPerTankPropellantMaterials(tispaceShipState5.faction).ToRVCollection(1f);
//                    Func<KeyValuePair<FactionResource, float>, float> evaluate;
//                    if ((evaluate = <> 9__11) == null)
//                    {
//                        evaluate = (<> 9__11 = ((KeyValuePair<FactionResource, float> x) => fleet.ref_habSite.GetDailyProduction(x.Key) / x.Value));
//                    }
//                    FactionResource key3 = en.MinBy(evaluate).Key;
//                    float num5 = fleet.ref_habSite.GetDailyProduction(key3) / TemplateManager.global.spaceResourceToTons;
//                    float num6 = tispaceShipState5.drive.GetPerTankPropellantMaterials(tispaceShipState5.faction).ToResourcesCost(tispaceShipState5.PropellantShortage_tons).GetSingleCostValue(key3) / num5;
//                    if (num6 < 90f)
//                    {
//                        if (!prospectiveOnly)
//                        {
//                            tispaceShipState5.plannedResupplyAndRepair.AddPropellantToReload(tispaceShipState5.PropellantShortage_tons);
//                            tispaceShipState5.plannedResupplyAndRepair.SetStartDate(TITimeState.Now());
//                        }
//                    }
//                    else
//                    {
//                        if (!prospectiveOnly)
//                        {
//                            tispaceShipState5.plannedResupplyAndRepair.AddPropellantToReload(num5 * 90f);
//                            tispaceShipState5.plannedResupplyAndRepair.SetStartDate(TITimeState.Now());
//                        }
//                        num6 = 90f;
//                    }
//                    tiresourcesCost10.SetCompletionTime_Days(Mathf.Max(tiresourcesCost10.completionTime_days, num6));
//                }
//                else
//                {
//                    tiresourcesCost10.SetCompletionTime_Days(Mathf.Max(tiresourcesCost10.completionTime_days, Mathf.Max(tispaceShipState5.PropellantShortage_tons / 1000f, 10f)));
//                    if (!prospectiveOnly)
//                    {
//                        tispaceShipState5.plannedResupplyAndRepair.AddPropellantToReload(tispaceShipState5.PropellantShortage_tons);
//                        tispaceShipState5.plannedResupplyAndRepair.SetStartDate(TITimeState.Now());
//                    }
//                }
//                list.Remove(tispaceShipState5);
//            }
//            tiresourcesCost.SetCompletionTime_Days(Mathf.Max(tiresourcesCost.completionTime_days, tiresourcesCost10.completionTime_days));
//        }
//        return tiresourcesCost;
//    }
//}