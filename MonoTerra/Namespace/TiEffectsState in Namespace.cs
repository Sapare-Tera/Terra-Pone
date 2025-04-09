using MonoMod;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using System.Reflection;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Systems.UI;

namespace PavonisInteractive.TerraInvicta
{

    public class patch_TIEffectsState : TIEffectsState
    {
        //public static List<TIEffectTemplate> GetFactionEffectsForContext(patch_Context context, patch_TIFactionState faction)
        //{
        //    patch_TIEffectsState tieffectsState = (patch_TIEffectsState)GameStateManager.Effects();
        //    if (tieffectsState.factionEffects[faction].ContainsKey(context))
        //    {
        //        return tieffectsState.factionEffects[faction][context];
        //    }
        //    return new List<TIEffectTemplate>();
        //}

        //private Dictionary<patch_TIFactionState, Dictionary<patch_Context, List<TIEffectTemplate>>> factionEffects;

        public static extern void orig_ProcessInstantEffect(TIFactionState sourceFaction, EffectTargetType effectTargetType, EffectSecondaryStateType secondaryStateType, InstantEffect instantEffect, float value, float randomizer, string strValue, TIGameState inputState = null, TIGameState secondaryinputState = null);

        public static void ProcessInstantEffect(patch_TIFactionState sourceFaction, EffectTargetType effectTargetType, EffectSecondaryStateType secondaryStateType, InstantEffect instantEffect, float value, float randomizer, string strValue, TIGameState inputState = null, TIGameState secondaryinputState = null)
        {
            orig_ProcessInstantEffect(sourceFaction, effectTargetType, secondaryStateType, instantEffect, value, randomizer, strValue, inputState, secondaryinputState);
            switch (instantEffect)
            {
                case (InstantEffect)patch_InstantEffect.GrantControlPoint:
                    {
                        if (!(sourceFaction != null))
                        {
                            return;
                        }
                        String[] args = new String[] { strValue };

                        TINationState tinationState = GameStateManager.IterateByClass<TINationState>(false).FirstOrDefault((TINationState x) => x.templateName == args[0]);

                        TIControlPoint ticontrolPoint2 = tinationState.ref_nation.GetControlPoint((int)value);
                        if (ticontrolPoint2 == null && tinationState.ref_nation.NumNativeControlPoints > 1)
                        {
                            ticontrolPoint2 = tinationState.ref_nation.FirstNativeControlPoint();
                        }
                        if (ticontrolPoint2 != null)
                        {
                            tinationState.ref_nation.ChangeControlPointOwner(ticontrolPoint2.positionInNation, ControlPointChangeCause.Event, sourceFaction);
                            return;
                        }
                        return;
                    }
                case (InstantEffect)patch_InstantEffect.ActivateNAP:
                    {
                        foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
                        {
                            if ( sourceFaction.ideology.dataName == "destroy" )
                            {
                                if (tifactionState.ideology.dataName == "cooperate" || tifactionState.ideology.dataName == "resist")
                                {
                                    sourceFaction.AddGoal(new FactionGoal_NonAggressionPact(sourceFaction, 4, tifactionState), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
                                }
                                   // sourceFaction.BeginIntelSharingWith(tifactionState);
                                    //tifactionState.BeginIntelSharingWith(sourceFaction);
                                }
                            if (sourceFaction.ideology.dataName == "cooperate")
                            {
                                if (tifactionState.ideology.dataName == "destroy" || tifactionState.ideology.dataName == "resist")
                                {
                                    sourceFaction.AddGoal(new FactionGoal_NonAggressionPact(sourceFaction, 4, tifactionState), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
                                }
                                // sourceFaction.BeginIntelSharingWith(tifactionState);
                                //tifactionState.BeginIntelSharingWith(sourceFaction);
                            }
                            if (sourceFaction.ideology.dataName == "resist")
                            {
                                if (tifactionState.ideology.dataName == "destroy" || tifactionState.ideology.dataName == "cooperate")
                                {
                                    sourceFaction.AddGoal(new FactionGoal_NonAggressionPact(sourceFaction, 4, tifactionState), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
                                }
                                // sourceFaction.BeginIntelSharingWith(tifactionState);
                                //tifactionState.BeginIntelSharingWith(sourceFaction);
                            }
                        }
                            return;
                    }
                case (InstantEffect)patch_InstantEffect.SetTimerforCouncil:
                    {
                        sourceFaction.CouncilTimer = TITimeState.Now();
                        Log.Debug($"TEST {sourceFaction.CouncilTimer}");
                        return;
                    }
            }
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

        public bool SufficientCapacityForOrg(TIOrgState org)//Not used but will be relevant for org limits?
        {
            return this.orgs.Count < (TemplateManager.global.councilorMaxOrgs + 5) && this.availableAdministration >= org.tier - org.administration && this.orgsWeight + org.tier <= this.maxCouncilorAttribute;
        }

        public int SpareCapacityForOrgs()//Not used but will be relevant for org limits?
        {
            return Mathf.Min((TemplateManager.global.councilorMaxOrgs + 5) - this.orgs.Count, this.availableAdministration);
        }

        public CouncilorAugmentationOption Addmagic(patch_TITraitTemplate trait)
        {
            CouncilorAugmentationOption aug = new CouncilorAugmentationOption();
            patch_TICouncilorState councilor;
            aug.SetProperties_PVC(CouncilorAttribute.None, trait, 1f, this.XPModifier);
            return aug;
        }

        public int Adminholder = 0;
        public int ScienceHolder = 0;
        private int maxCouncilorAttribute
        {
            get
            {
                return TemplateManager.global.maxCouncilorAttribute;
            }
        }
        public extern List<CouncilorAugmentationOption> orig_GetCandidateAugmentations();



        public List<CouncilorAugmentationOption> GetCandidateAugmentations()
        {
            List<CouncilorAugmentationOption> list = new List<CouncilorAugmentationOption>();

            if (isAlien)
                foreach (CouncilorAttribute councilorAttribute in Enums.CouncilorAttributes)
                {
                    if (councilorAttribute != CouncilorAttribute.Loyalty && councilorAttribute != CouncilorAttribute.ApparentLoyalty && this.GetAttribute(councilorAttribute, false, true, true, false) < this.maxCouncilorAttribute)
                    {
                        list.Add(new CouncilorAugmentationOption(councilorAttribute, null, 1f, this.XPModifier));
                    }
                }


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


}
