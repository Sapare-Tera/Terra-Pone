using FullSerializer;
using MonoMod;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;


namespace FullSerializer.Internal
{
    public class patch_fsEnumConverter : fsEnumConverter
    {
        // This handles non-vanilla enum values that get handled as ints instead of strings when saving.
        public extern fsResult orig_TrySerialize(object instance, out fsData serialized, Type storageType);
        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            if (!Serializer.Config.SerializeEnumsAsInteger)
            {
                if (Enum.GetName(storageType, instance) == null)
                {
                    //Log.Debug($"{instance} is not a valid enum value for {storageType}!");
                    serialized = new fsData(Convert.ToInt64(instance));
                    //Log.Debug($"We have converted it to an int {serialized}");
                    return fsResult.Success;
                }
                else
                {
                    return orig_TrySerialize(instance, out serialized, storageType);
                }
            }
            else
            {
                return orig_TrySerialize(instance, out serialized, storageType);
            }
        }
    }
}

namespace PavonisInteractive.TerraInvicta
{
    public class patch_TIHabState : TIHabState
    {
        [fsIgnore]
        private Dictionary<TIFactionState, Dictionary<patch_FactionResource, float>> netAnnualIncomes;
        public float GetAnnualResourceIncome(TIFactionState faction, patch_FactionResource resource)
        {
            if (this.netAnnualIncomes.Keys.Contains(faction))
            {
                return this.netAnnualIncomes[faction][resource];
            }
            return 0f;
        }
    }


    public class patch_TIFactionState : TIFactionState
    {
        [fsIgnore]
        private Dictionary<TIObjectiveTemplate, ObjectiveStatus> objectives;
        [fsProperty]
        private TIDateTime lastTechRaceDate;
        [fsProperty]
       public TIDateTime LastObjectiveProjectCompletionDate { get; private set; }

        private Dictionary<patch_FactionResource, bool> resourceIncomeDataDirty = new Dictionary<patch_FactionResource, bool>();
        private Dictionary<patch_FactionResource, float> annualResourceIncomes = new Dictionary<patch_FactionResource, float>();

        [SerializeField]
        public Dictionary<patch_FactionResource, float> resources;

        [SerializeField]
        public Dictionary<patch_FactionResource, float> resourcesnew;

        public List<patch_FactionResource> resourceIncomeDeficiencies;
        public Dictionary<patch_FactionResource, float> copyResources
        {
            get
            {
                return this.resources.ToDictionary((KeyValuePair<patch_FactionResource, float> x) => x.Key, (KeyValuePair<patch_FactionResource, float> x) => x.Value);
            }
        }
        [SerializeField]
        private List<string> availableProjectNames;
        [SerializeField]
        private List<ProjectTrigger> activeProjectTriggers;
        public List<string> finishedProjectNames { get; private set; }

        [SerializeField]
        private Dictionary<string, ObjectiveStatus> objectiveNames;

        [SerializeField]
        private Dictionary<TIGameState, float> highestIntel;

        [SerializeField]
        private Dictionary<TIGameState, float> intel;

        [SerializeField]
        private Dictionary<TIFactionState, float> factionHate;

        public List<CampaignMilestone> milestones { get; private set; }

        public Dictionary<TIHabModuleState, List<ShipConstructionQueueItem>> nShipyardQueues { get; private set; } = new Dictionary<TIHabModuleState, List<ShipConstructionQueueItem>>();

        public List<PolicyOptionWithTarget> plannedPolicies { get; private set; }

        public Dictionary<string, float> techNameContributionHistory { get; private set; }

        //[MonoModOriginal] public extern bool orig_Initialize();
        //public override bool Initialize()
        //{
        //    bool returnVal = orig_Initialize();
        //    this.resources = new Dictionary<patch_FactionResource, float>(15);
        //    //this.baseIncomes_year = new Dictionary<patch_FactionResource, float>();
        //    return returnVal;
        //}

        //[MonoModIgnore] private Dictionary<TechCategory, float> cachedBaseHabsMultipliers = new Dictionary<TechCategory, float>();
        //[MonoModIgnore] private Dictionary<TechCategory, int> baseHabsMultiplierCachedFrames;
        //[MonoModIgnore] private Dictionary<TechCategory, float> cachedTraitsMultiplier = new Dictionary<TechCategory, float>();
        //[MonoModIgnore] private Dictionary<TechCategory, int> traitsMultiplierCachedFrame = Enums.TechCategories.ToDictionary((TechCategory x) => x, (TechCategory x) => -1);
        //[MonoModIgnore] private Dictionary<patch_TechCategory, float> cachedOrgsMultiplier = new Dictionary<patch_TechCategory, float>();
        //[MonoModIgnore] private Dictionary<TechCategory, int> orgsMultiplierCachedFrame = Enums.TechCategories.ToDictionary((TechCategory x) => x, (TechCategory x) => -1);
        //[MonoModIgnore] private Dictionary<TechCategory, float> cachedFleetsModifier = new Dictionary<TechCategory, float>();
        //[MonoModIgnore] private Dictionary<TechCategory, int> fleetsModifierCachedFrame = Enums.TechCategories.ToDictionary((TechCategory x) => x, (TechCategory x) => -1);
        //[SerializeField]


        public TradeOffer GetAIGiveOffer(patch_TIFactionState offerReciever, patch_TIFactionState offerSender, TradeOffer decidedWantOffer)
		{
			bool flag = !offerReciever.isActivePlayer && !offerSender.isActivePlayer;
			TradeOffer tradeOffer = offerSender.InitializeTradingOptions(offerReciever, false);
			offerSender.InitializeTradingOptions(offerReciever, true);
			tradeOffer.habs.Clear();
			tradeOffer.projects.Clear();
			tradeOffer.resourceValues.Clear();
			tradeOffer.habSectors.Clear();
			tradeOffer.controlPoints.Clear();
			float num2;
			float num3;
			float num = TIFactionState.NetTradeScore(decidedWantOffer, tradeOffer, out num2, out num3);
			List<TIOrgState> list = new List<TIOrgState>();
			float num4 = (float)UnityEngine.Random.Range(0, 100);
			num4 += (float)tradeOffer.orgs.Count - offerSender.GetFactionHate(offerReciever);
			if (tradeOffer.orgs.Count > 0 && num4 > 50f)
			{
				TIOrgState tiorgState = tradeOffer.orgs[0];
				foreach (TIOrgState tiorgState2 in tradeOffer.orgs)
				{
					if (AIEvaluators.EvaluateOrgForTrade(tiorgState2, offerReciever, offerSender) < num && AIEvaluators.EvaluateOrgForTrade(tiorgState2, offerReciever, offerSender) < AIEvaluators.EvaluateOrgForTrade(tiorgState, offerReciever, offerSender))
					{
						tiorgState = tiorgState2;
					}
				}
				if (AIEvaluators.EvaluateOrgForTrade(tiorgState, offerReciever, offerSender) > 0f || !flag)
				{
					list.Add(tiorgState);
				}
			}
			int num5 = 0;
			using (List<ResourceValue>.Enumerator enumerator2 = decidedWantOffer.resourceValues.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.value > 0f)
					{
						num5++;
					}
				}
			}
			if (num5 > 0 && tradeOffer.orgs.Count > 0)
			{
				TIOrgState tiorgState3 = tradeOffer.orgs[0];
				foreach (TIOrgState tiorgState4 in tradeOffer.orgs)
				{
					if (!list.Contains(tiorgState4) && AIEvaluators.EvaluateOrgForTrade(tiorgState4, offerReciever, offerSender) < num && AIEvaluators.EvaluateOrgForTrade(tiorgState4, offerReciever, offerSender) < AIEvaluators.EvaluateOrgForTrade(tiorgState3, offerReciever, offerSender))
					{
						tiorgState3 = tiorgState4;
					}
				}
				if (!list.Contains(tiorgState3) && tiorgState3 != null)
				{
					list.Add(tiorgState3);
				}
			}
			tradeOffer.orgs.Clear();
			foreach (TIOrgState item in list)
			{
				tradeOffer.orgs.Add(item);
			}
			num = patch_TIFactionState.NetTradeScore(decidedWantOffer, tradeOffer, out num3, out num2);
			foreach (patch_FactionResource resource in Enums.FactionResources.Except(patch_TIResourcesCost.unTradeableResourcesNEW))
			{
                if (offerSender.EvaluateTradeOffer(tradeOffer, false, tradeOffer) - num < 0f)
				{
                    Log.Debug($"Step F {resource}");
                    float num6 = Mathf.Min(num / patch_AIEvaluators.FixedResourceValue(offerSender,resource, 1f, true), this.AI_MaxWillingToTradeAway((FactionResource)resource, offerSender));
					if (num6 > 0f)
					{
						tradeOffer.resourceValues.Add(new ResourceValue((FactionResource)resource, num6));
					}
				}
			}
			return tradeOffer;
		}



        public List<ResourceValue> AvailableSpaceResources(float fraction = 1f)
        {
            List<ResourceValue> list = new List<ResourceValue>();
            foreach (FactionResource factionResource in patch_TIResourcesCost.spaceResourcesNEW)
            {
                list.Add(new ResourceValue
                {
                    resource = factionResource,
                    value = this.GetCurrentResourceAmount(factionResource) * fraction
                });
            }
            return list;
        }

        //public bool HasAnySpaceResources
        //{
        //    get
        //    {
        //        return this.resources.Any((KeyValuePair<patch_FactionResource, float> x) => TIResourcesCost.spaceResources.Contains(x.Key) && x.Value > 0f);
        //    }
        //}

        public static bool IsASpaceResource(FactionResource resource)
        {
            return patch_TIResourcesCost.spaceResourcesNEW.Contains(resource);
        }

        private bool gameStateSubjectCreated;

        [MonoModOriginal] public extern void orig_PostGameStateCreateInit_OnCreationOnly_1();
        public override void PostGameStateCreateInit_OnCreationOnly_1()
        {
            orig_PostGameStateCreateInit_OnCreationOnly_1();
            if (!this.gameStateSubjectCreated)
            {
                foreach (patch_FactionResource factionResource in Enums.FactionResources)
                {
                    if (factionResource == patch_FactionResource.Magic)
                    {
                        this.resources.Add(patch_FactionResource.Magic, 0f);
                    }
                    
                }
            }
           }

        [SerializeField]
        private Dictionary<patch_FactionResource, float> baseIncomes_year;

        public float GetYearlyIncomeFromHQ(patch_FactionResource resourceType)
        {
            return this.baseIncomes_year[resourceType];
        }

        public extern float orig_GetYearlyIncome(FactionResource resourceType, bool dontRecalculate = false, bool suppressFactionResourcesUpdatedEvent = false);
        public float GetYearlyIncome(FactionResource resourceType, bool dontRecalculate = false, bool suppressFactionResourcesUpdatedEvent = false)
        {
            switch ((patch_FactionResource)resourceType)
            {
                case patch_FactionResource.Magic:
                    return 0f;

                default:
                    return orig_GetYearlyIncome(resourceType);
            }
        }
        public bool UnlockedExotics
        {
            get
            {
                return true;
            }
        }
        public bool UnlockedResource(FactionResource resource)
        {
            switch (resource)
            {
                case FactionResource.Money:
                case FactionResource.Influence:
                case FactionResource.Operations:
                case FactionResource.Research:
                case FactionResource.Boost:
                case FactionResource.MissionControl:
                case (FactionResource)patch_FactionResource.Magic:
                    return true;
                case FactionResource.Water:
                case FactionResource.Volatiles:
                case FactionResource.Metals:
                case FactionResource.NobleMetals:
                case FactionResource.Fissiles:
                    return this.UnlockedSpaceResources;
                case FactionResource.Antimatter:
                    return this.UnlockedAntimatter;
                case FactionResource.Exotics:
                    return this.UnlockedExotics;
            }
            return false;
        }

        public float GetCurrentResourceAmount(patch_FactionResource resourceType)
        {
                if (!this.resources.ContainsKey(resourceType))
            {
                return 0f;
            }
            return this.resources[resourceType];
        }

        public float AddToCurrentResource(float amountToAdd, patch_FactionResource resourceType, bool suppressFactionResourcesUpdatedEvent = false)
        {
            if (amountToAdd == 0f)
            {
                return this.GetCurrentResourceAmount(resourceType);
            }
            if (resourceType == (patch_FactionResource)FactionResource.Research)
            {
                if (amountToAdd < 0f)
                {
                    List<ProjectProgress> list = this.currentProjectProgress.ToList<ProjectProgress>();
                    float num = -amountToAdd;
                    float num3;
                    for (float num2 = 0f; num2 < num; num2 += num3)
                    {
                        if (list.Count<ProjectProgress>() <= 0)
                        {
                            break;
                        }
                        ProjectProgress projectProgress = list.SelectRandomItem<ProjectProgress>();
                        list.Remove(projectProgress);
                        num3 = Mathf.Min(num - num2, projectProgress.accumulatedResearch);
                        projectProgress.accumulatedResearch -= num3;
                    }
                }
                else
                {
                    this.DistributeResearchToSlots(amountToAdd);
                }
                return 0f;
            }
            if (patch_TIFactionState.DontAccumulateResource(resourceType))
            {
                return 0f;
            }
            Dictionary<patch_FactionResource, float> dictionary = this.resources;
            dictionary[resourceType] += amountToAdd;
            if (this.resources[resourceType] < 0f && !patch_TIFactionState.ResourceCanGoNegative(resourceType))
            {
                this.resources[resourceType] = 0f;
            }
            if (!suppressFactionResourcesUpdatedEvent)
            {
                GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(this), null, new object[]
                {
                    this
                });
            }
            return this.resources[resourceType];
        }

        public static bool DontAccumulateResource(patch_FactionResource resourceType)
        {
            return patch_TIResourcesCost.unAccumulatableResources.Contains(resourceType);
        }

        public static bool ResourceCanGoNegative(patch_FactionResource resourceType)
        {
            return patch_TIResourcesCost.resourcesAllowedToGoNegative.Contains(resourceType);
        }

        public float SumCategoryModifiers(TechCategory category)
        {
            switch ((patch_TechCategory)category)
            {
                case patch_TechCategory.MagicScience:
                    return 0f;
                default:
                    return this.HabsMultiplier(category) + this.OrgsMultiplier(category) + this.TraitsMultiplier(category) + this.InvestigationsModifier(category) + this.FleetsModifier(category);
            }
        }

        public extern float orig_TechCategoryValuation(TechCategory category);
        public float TechCategoryValuation(TechCategory category)
        {
            switch ((patch_TechCategory)category)
            {
                case patch_TechCategory.MagicScience:
                    return 0f;
                default:
                    return orig_TechCategoryValuation(category);
            }
        }

        public bool GenerateRecruitableCouncilors(bool campaignStart = false)
        {
            bool result = false;
            if (this.availableCouncilors.Count > 1 && !campaignStart && this.IsActiveHumanFaction)
            {
                for (int i = this.availableCouncilors.Count - 1; i >= 0; i--)
                {
                    if (UnityEngine.Random.value * 100f < (float)this.availableCouncilors[i].age)
                    {
                        TICouncilorState ticouncilorState = this.availableCouncilors[i];
                        this.availableCouncilors.Remove(ticouncilorState);
                        if (ticouncilorState.template.randomized)
                        {
                            ticouncilorState.ArchiveState();
                            GameStateManager.RemoveGameState<TICouncilorState>(ticouncilorState.ID, false);
                        }
                    }
                }
            }
            if (this.maxRecruitableCandidates > 0)
            {
                int num = this.IsActiveHumanFaction ? UnityEngine.Random.Range(-2, 2) : 0;
                for (int j = this.availableCouncilors.Count; j <= this.maxRecruitableCandidates + num; j++)
                {
                    List<TICouncilorState> list = new List<TICouncilorState>();
                    foreach (TICouncilorState ticouncilorState2 in GameStateManager.IterateByClass<TICouncilorState>(false))
                    {
                        if (!ticouncilorState2.everBeenAvailable && !ticouncilorState2.template.debugOnly && string.IsNullOrEmpty(ticouncilorState2.template.debugStartingCouncil) && !ticouncilorState2.template.randomized && ticouncilorState2.age >= 14 && ticouncilorState2.age <= 85 && ticouncilorState2.template.allowedIdeologies.Contains(this.ideology.ideology))
                        {
                            list.Add(ticouncilorState2);
                        }
                    }
                    if (UnityEngine.Random.value > TemplateManager.global.chanceCouncilorTemplate || list.Count == 0)
                    {
                        TICouncilorState ticouncilorState3 = GameStateManager.CreateNewGameState<TICouncilorState>();
                        if (this.IsAlienFaction)
                        {
                            ticouncilorState3.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>("randomizedAlienCouncilor2", false));
                        }
                        else
                        {
                            ticouncilorState3.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>("randomizedCouncilor1", false));
                        }
                        if (this.availableCouncilors.None((TICouncilorState x) => x.HireRecruitCost(this).CanAfford(this, 1f, null, float.PositiveInfinity)))
                        {
                            IEnumerable<TICouncilorTypeTemplate> enumerable = from x in TemplateManager.GetAllTemplates<TICouncilorTypeTemplate>(true)
                                                                              where x.affinities.Contains(this.ideology.ideology)
                                                                              select x;
                            if (enumerable.Count<TICouncilorTypeTemplate>() > 0)
                            {
                                ticouncilorState3.NewCharacterGeneration(enumerable.SelectRandomItem<TICouncilorTypeTemplate>(), null, (this.IsAlienFaction || campaignStart) ? null : this, false, false);
                            }
                        }
                        else
                        {
                            ticouncilorState3.NewCharacterGeneration(null, null, (this.IsAlienFaction || campaignStart) ? null : this, false, false);
                        }
                        this.availableCouncilors.Add(ticouncilorState3);
                        result = true;
                    }
                    else
                    {
                        int index = UnityEngine.Random.Range(0, list.Count);
                        this.availableCouncilors.Add(list[index]);
                        result = true;
                        list[index].everBeenAvailable = true;
                    }
                }
            }
            return result;
        }


        public List<TICouncilorState> advisingCouncilors { get; private set; }
        public void AddAdvisingCouncilor(TICouncilorState councilor)
        {
            this.advisingCouncilors.Add(councilor);
        }
        public float GetAdvisers()
        {
            if (this.advisingCouncilors.Count > 0)
            {
                float num = this.advisingCouncilors.Count;
                return num;
            }
            return 0f;
        }

        public float GetControlPointMaintenanceFreebieCap()
        {
            if (!this.IsAlienFaction)
            {
                return (float)(TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies + (this.isActivePlayer ? 0 : TIGlobalValuesState.GlobalValues.scenarioCustomizations.controlPointMaintenanceFreebieBonusAI) + this.activeCouncilors.Sum((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Security, true, true, true, false)*2) + this.habs.Sum((TIHabState x) => x.controlPointCapacityValue)) - TIEffectsState.SumEffectsModifiers(Context.ControlPointMaintenance, this, (float)TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies);
            }
            return 20000f;
        }
        public new float GetNegativeMonthlyIncomeFromUnassignedOrgs(FactionResource resource)
        {
            float num = 0f;
            foreach (TIOrgState tiorgState in this.unassignedOrgs)
            {
                switch (resource)
                {
                    case FactionResource.Money:
                        num += ((tiorgState.incomeMoney_month < 0f) ? tiorgState.incomeMoney_month : 0f);
                        break;
                    case FactionResource.Influence:
                        num += ((tiorgState.incomeInfluence_month < 0f) ? tiorgState.incomeInfluence_month : 0f);
                        break;
                    case FactionResource.Operations:
                        num += ((tiorgState.incomeOps_month < 0f) ? tiorgState.incomeOps_month : 0f);
                        break;
                    case FactionResource.Boost:
                        num += ((tiorgState.incomeBoost_month < 0f) ? tiorgState.incomeBoost_month : 0f);
                        break;
                    case FactionResource.MissionControl:
                        num += ((tiorgState.incomeMissionControl < 0f) ? tiorgState.incomeMissionControl : 0f);
                        break;
                }
            }
            num *= 0f;
            return num;
        }

        public static bool TradeableResource(FactionResource resourceType)
        {
            return !patch_TIResourcesCost.unTradeableResourcesNEW.Contains(resourceType);
        }
        public TradeOffer InitializeTradingOptions(TIFactionState otherFaction, bool empty = false)
        {
            TradeOffer tradeOffer = new TradeOffer
            {
                offeringFaction = this,
                resourceValues = new List<ResourceValue>(),
                orgs = new List<TIOrgState>(),
                controlPoints = new List<TIControlPoint>(),
                habSectors = new List<TISectorState>(),
                habs = new List<TIHabState>(),
                projects = new List<TIProjectTemplate>(),
                intelData = new List<TIGameState>(),
                treatyType = TradeOffer.TreatyType.None
            };
            foreach (FactionResource resource in Enums.FactionResources.Except(patch_TIResourcesCost.unTradeableResourcesNEW))
            {
                if (this.UnlockedResource(resource) && otherFaction.UnlockedResource(resource))
                {
                    tradeOffer.resourceValues.Add(new ResourceValue(resource, 0f));
                }
            }
            if (empty)
            {
                return tradeOffer;
            }
            foreach (TIOrgState tiorgState in this.GetAllOrgs())
            {
                if (tiorgState.IsEligibleForFaction(otherFaction))
                {
                    TICouncilorState assignedCouncilor = tiorgState.assignedCouncilor;
                    if (assignedCouncilor == null || assignedCouncilor.CanRemoveOrg(tiorgState))
                    {
                        tradeOffer.orgs.Add(tiorgState);
                    }
                }
            }
            if (!this.IsAlienFaction && !otherFaction.IsAlienFaction)
            {
                foreach (TIControlPoint item in this.controlPoints)
                {
                    tradeOffer.controlPoints.Add(item);
                }
                foreach (TISectorState tisectorState in this.habSectors)
                {
                    if (otherFaction.CanExplore(tisectorState.hab))
                    {
                        tradeOffer.habSectors.Add(tisectorState);
                    }
                }
                foreach (TIProjectTemplate tiprojectTemplate in this.completedProjects)
                {
                    if (tiprojectTemplate.PrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), this.completedProjects, otherFaction) && tiprojectTemplate.techCategory != TechCategory.Xenology && !otherFaction.availableProjects.Contains(tiprojectTemplate) && !otherFaction.completedProjects.Contains(tiprojectTemplate) && !tiprojectTemplate.repeatable)
                    {
                        tradeOffer.projects.Add(tiprojectTemplate);
                    }
                }
            }
            return tradeOffer;
        }
    }
}
