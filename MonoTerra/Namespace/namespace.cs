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
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using System.IO;
using PavonisInteractive.TerraInvicta.Systems.UI;

namespace PavonisInteractive.TerraInvicta
{
    //    public class patch_TIRegionAlienFacilityState : TIRegionAlienFacilityState
    //{

    //    public bool built { get; private set; }
    //    public void BuildFacility()
    //    {
    //        this.built = true;
    //        this.currentHP = 80f;
    //        foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
    //        {
    //            if (tifactionState.IsAlienProxy || tifactionState.IsAlienFaction)
    //            {
    //                tifactionState.SetIntel(this, 1f, null);
    //            }
    //            else
    //            {
    //                tifactionState.SetIntel(this, 0f, null);
    //            }
    //        }
    //        base.region.ChangeOceanType((WorldOceanType)patch_WorldOceanType.Teleport);

    //        GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
    //        {
    //            base.region
    //        });
    //    }
    //}

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
    public class patch_TISpaceFleetState : TISpaceFleetState, IOperationCapableState, IMobileAsset, ITransferTarget
    {
        public extern void orig_PostCombat(TISpaceCombatState combat, double combatDuration_s, bool relocate);
        public void PostCombat(TISpaceCombatState combat, double combatDuration_s, bool relocate)
        {
            foreach (TISpaceShipState tispaceShipState in this.ships.ToList<TISpaceShipState>())
            {
                if (tispaceShipState.ShipDestroyed() && !tispaceShipState.faction.IsAlienFaction)
                {
                        int Crew = tispaceShipState.hull.crew;
                        patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(Crew, false);
                }
                orig_PostCombat(combat, combatDuration_s, relocate);
            }
        }
        public bool AllowUseBoostForRepairsResupply
        {
            get
            {
                return false;
            }
        }
    }

    public class patch_TIResourcesCost : TIResourcesCost
    {
        public patch_TIResourcesCost(FactionResource resource, float value)
        {
            this.resourceCosts = new List<ResourceValue>();
            this.resourceCosts.Add(new ResourceValue(resource, value));
        }
        public bool CanAffordWarOption(TIFactionState faction, float maxFractionCanSpend = 1f, List<FactionResource> resourcesToPreserve = null, float maxDays = float.PositiveInfinity)
        {
            maxFractionCanSpend = Mathf.Clamp(maxFractionCanSpend, 0f, 1f);
            foreach (ResourceValue resourceValue in this.resourceCosts)
            {
                float WarValue = resourceValue.value * TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm / 100;
                if (WarValue > 0f)
                {
                    if (resourcesToPreserve != null && resourcesToPreserve.Contains(resourceValue.resource))
                    {
                        if (faction.GetCurrentResourceAmount(resourceValue.resource) * maxFractionCanSpend < WarValue)
                        {
                            return false;
                        }
                    }
                    else if (faction.GetCurrentResourceAmount(resourceValue.resource) < WarValue)
                    {
                        return false;
                    }
                    if (this.completionTime_days > maxDays)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void PayCostWarOption(TIFactionState faction)
        {
            if (this.resourceCosts != null)
            {
                foreach (ResourceValue resourceValue in this.resourceCosts)
                {
                    float WarValue = resourceValue.value * TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm / 100;
                    faction.SubtractFromCurrentResource(WarValue, resourceValue.resource, true);
                    if (resourceValue.resource == FactionResource.Boost && resourceValue.value > 0f)
                    {
                        int num = (int)Mathf.Clamp(WarValue * TemplateManager.global.spaceResourceToTons, 1f, 3f);
                        for (int i = 0; i < num; i++)
                        {
                            TIRegionSpaceFacilityState tiregionSpaceFacilityState = faction.SelectRandomLaunchSite();
                            if (tiregionSpaceFacilityState != null)
                            {
                                TIDateTime tidateTime = TITimeState.Now();
                                tidateTime.AddDays(this.completionTime_days - UnityEngine.Random.Range(0.01f, 0.25f) * this.completionTime_days);
                                TITimeEvent.CreateNewTimeEvent(tidateTime, tiregionSpaceFacilityState, null, null, "Launch Rocket to Orbit", false, false, TITimeQueueRepeatType.None, 1, true, false);
                            }
                        }
                    }
                }
                GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(faction), null, new object[]
                {
                    faction
                });
            }
        }

        public bool CanAffordFederationOption(TIFactionState faction, float maxFractionCanSpend = 1f, List<FactionResource> resourcesToPreserve = null, float maxDays = float.PositiveInfinity)
        {
            maxFractionCanSpend = Mathf.Clamp(maxFractionCanSpend, 0f, 1f);
            foreach (ResourceValue resourceValue in this.resourceCosts)
            {
                float WarValue = resourceValue.value * (1 - (TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm / 100));
                if (WarValue > 0f)
                {
                    if (resourcesToPreserve != null && resourcesToPreserve.Contains(resourceValue.resource))
                    {
                        if (faction.GetCurrentResourceAmount(resourceValue.resource) * maxFractionCanSpend < WarValue)
                        {
                            return false;
                        }
                    }
                    else if (faction.GetCurrentResourceAmount(resourceValue.resource) < WarValue)
                    {
                        return false;
                    }
                    if (this.completionTime_days > maxDays)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void PayCostFederationOption(TIFactionState faction)
        {
            if (this.resourceCosts != null)
            {
                foreach (ResourceValue resourceValue in this.resourceCosts)
                {
                    float WarValue = resourceValue.value * (1 - (TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm / 100));
                    faction.SubtractFromCurrentResource(WarValue, resourceValue.resource, true);
                    if (resourceValue.resource == FactionResource.Boost && resourceValue.value > 0f)
                    {
                        int num = (int)Mathf.Clamp(WarValue * TemplateManager.global.spaceResourceToTons, 1f, 3f);
                        for (int i = 0; i < num; i++)
                        {
                            TIRegionSpaceFacilityState tiregionSpaceFacilityState = faction.SelectRandomLaunchSite();
                            if (tiregionSpaceFacilityState != null)
                            {
                                TIDateTime tidateTime = TITimeState.Now();
                                tidateTime.AddDays(this.completionTime_days - UnityEngine.Random.Range(0.01f, 0.25f) * this.completionTime_days);
                                TITimeEvent.CreateNewTimeEvent(tidateTime, tiregionSpaceFacilityState, null, null, "Launch Rocket to Orbit", false, false, TITimeQueueRepeatType.None, 1, true, false);
                            }
                        }
                    }
                }
                GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(faction), null, new object[]
                {
                    faction
                });
            }
        }


        public List<ResourceValue> resourceCosts { get; private set; }
        public float completionTime_days { get; private set; }

        public patch_TIResourcesCost()
        {
            this.resourceCosts = new List<ResourceValue>();
        }
        public patch_TIResourcesCost GetBoostSubstitutedCost(TIFactionState faction, TIGameState location, bool ignoreTime = false, List<ResourceValue> availableResources = null)
		{
            patch_TIResourcesCost tiresourcesCost = new patch_TIResourcesCost();
			foreach (ResourceValue resourceValue in this.resourceCosts)
			{
				FactionResource resource = resourceValue.resource;
				float value = resourceValue.value;
				float num = 0f;
				if (availableResources == null)
				{
					num = faction.GetCurrentResourceAmount(resource);
				}
				else
				{
					foreach (ResourceValue resourceValue2 in availableResources)
					{
						if (resourceValue2.resource == resource)
						{
							num = resourceValue2.value;
							break;
						}
					}
				}
				if (num >= value || patch_TIResourcesCost.irreplaceableSpaceResourcesNEW.Contains(resource))
				{
					tiresourcesCost.AddCost(resource, value, true);
				}
				else
				{
					tiresourcesCost.AddCost(resource, num, true);
					float num2 = value - num;
					float resourceAmount = (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, location, num2 / TemplateManager.global.spaceResourceToTons);
					tiresourcesCost.AddCost(FactionResource.Boost, resourceAmount, true);
					float resourceAmount2 = num2 * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(resource);
					tiresourcesCost.AddCost(FactionResource.Money, resourceAmount2, true);
				}
			}
			if (!ignoreTime)
			{
				float num3 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, location);
				num3 += TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num3);
				tiresourcesCost.completionTime_days = this.completionTime_days + num3;
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

        public static readonly HashSet<patch_FactionResource> unTradeableResources = new HashSet<patch_FactionResource>
        {
            (patch_FactionResource)FactionResource.MissionControl,
            (patch_FactionResource)FactionResource.Projects,
            (patch_FactionResource)FactionResource.None,
            (patch_FactionResource)FactionResource.Research,
            patch_FactionResource.Magic
        };

        //public static readonly FactionResource[] unTradeableResources = new FactionResource[]
        //{
        //    FactionResource.MissionControl,
        //    FactionResource.Projects,
        //    FactionResource.None,
        //    FactionResource.Research,
        //    (FactionResource)patch_FactionResource.Magic
        //};

        public static readonly HashSet<patch_FactionResource> unAccumulatableResources = new HashSet<patch_FactionResource>
        {
            (patch_FactionResource)FactionResource.MissionControl,
            (patch_FactionResource)FactionResource.Projects,
            (patch_FactionResource) FactionResource.None
        };

        public static readonly HashSet<patch_FactionResource> resourcesAllowedToGoNegative = new HashSet<patch_FactionResource>
        {
            (patch_FactionResource) FactionResource.Money
        };

        public static bool DontAccumulateResource(patch_FactionResource resourceType)
        {
            return patch_TIResourcesCost.unAccumulatableResources.Contains(resourceType);
        }
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

    public class TIGlobalCondition_fEarthAtmosphericN2O_ppm : TIGlobalCondition
    {
        
        public override bool PassesCondition(TIGameState state)
        {
            return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.earthAtmosphericN2O_ppm, TIUtilities.GetFloatValue(this.strValue));
        }
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
    