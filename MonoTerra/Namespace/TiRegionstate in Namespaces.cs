using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using static MonoMod.InlineRT.MonoModRule;

namespace PavonisInteractive.TerraInvicta
{
    public class patch_GovMarkerController : GovMarkerController
    {
        public void UpdateRegionStatusMarker()
        {
            patch_TIRegionState region_VLC = region as patch_TIRegionState;
            bool flag = base.region.coreEconomicRegion || base.region.resourceRegion || region_VLC.MagicResource || region_VLC.TeleportRegion;
            this.regionStatusMarker = base.container.ManageMarkerStack(this.regionStatusMarker, !flag, MarkerType.RegionalStatusIcon, base.region, "regionStatusMarker", -1);
            if (flag)
            {
                this.regionStatusMarker.associatedState = base.region;
                if (region_VLC.MagicResource && !region_VLC.TeleportRegion)
                {
                    this.regionStatusMarker.SetCentralIcon(patch_AssetCacheManager.GeoscapeMagicResource1);
                    base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dEarthmodels/geoscape_core_resources");
                    this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.MagicResourceRegion"));
                    return;
                }
                if (region_VLC.TeleportRegion)
                {
                    this.regionStatusMarker.SetCentralIcon(patch_AssetCacheManager.GeoscapeTeleportRegion1);
                    base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dEarthmodels/geoscape_core_resources");
                    this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.RegionTeleporterRegion"));
                    return;
                }
                if (base.region.coreEconomicRegion)
                {
                    this.regionStatusMarker.SetCentralIcon(AssetCacheManager.coreEconomicRegionIcon);
                    this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.CoreEconomicRegion"));
                    base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dearthmodels/geoscape_core_eco");
                    return;
                }
                if (base.region.resourceRegion)
                {
                    if (base.region.template.oilResource)
                    {
                        this.regionStatusMarker.SetCentralIcon(AssetCacheManager.coreResourceRegionOilIcon);
                        base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dearthmodels/geoscape_core_resources");
                    }
                    else
                    {
                        this.regionStatusMarker.SetCentralIcon(AssetCacheManager.coreResourceRegionMiningIcon);
                        base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dEarthmodels/geoscape_core_resources_mining");
                    }
                    this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.CoreResourceRegion"));
                }

            }
        }
    }

    public static class patch_AssetCacheManager
    {

        public static readonly Sprite GeoscapeMagicResource1 = GameControl.assetLoader.LoadAsset<Sprite>(patch_TemplateManager.global_PVC.pathGeoscapeMagicResource1);
        public static readonly Sprite GeoscapeTeleportRegion1 = GameControl.assetLoader.LoadAsset<Sprite>(patch_TemplateManager.global_PVC.pathTeleportRegion1);
    }

    public class patch_TIRegionState : TIRegionState
    {

        public bool MagicResource;

        public bool TeleportRegion;

        public static float SeaTravelMultiplier(TINationState movingNation, TIRegionState region1, TIRegionState region2)
        {
            if (region1.IsAdjacent(region2, false))
            {
                return 1f;
            }
            bool suezAccess = movingNation == null || TIRegionState.SuezAccess(movingNation);
            bool panamaAccess = movingNation == null || TIRegionState.PanamaAccess(movingNation);
            suezAccess = false;
            panamaAccess = false;
            //Log.Debug($"A {region1}");
            //Log.Debug($"A {region2}");
            //bool flag = false;
            //bool flag2 = false;
            //CoastRegion region3 = CoastRegion.none;
            //CoastRegion region4 = CoastRegion.none;
            //CoastRegion region5 = CoastRegion.none;
            //CoastRegion region6 = CoastRegion.none;
            //switch (region1.mapRegionTemplate.coast)
            //{
            //    case CoastRegion.IndianMed:
            //        region3 = CoastRegion.Indian;
            //        region4 = CoastRegion.Mediterranean;
            //        flag = true;
            //        break;
            //    case CoastRegion.PacificCarib:
            //        region3 = CoastRegion.NortheastPacific;
            //        region4 = CoastRegion.Caribbean;
            //        flag = true;
            //        break;
            //    case CoastRegion.MedNorthAtlantic:
            //        region3 = CoastRegion.Mediterranean;
            //        region4 = CoastRegion.NortheastAtlantic;
            //        flag = true;
            //        break;
            //    case CoastRegion.BlackMed:
            //        region3 = CoastRegion.BlackSea;
            //        region4 = CoastRegion.Mediterranean;
            //        flag = true;
            //        break;
            //}
            //switch (region2.mapRegionTemplate.coast)
            //{
            //    case CoastRegion.IndianMed:
            //        region5 = CoastRegion.Indian;
            //        region6 = CoastRegion.Mediterranean;
            //        flag2 = true;
            //        break;
            //    case CoastRegion.PacificCarib:
            //        region5 = CoastRegion.NortheastPacific;
            //        region6 = CoastRegion.Caribbean;
            //        flag2 = true;
            //        break;
            //    case CoastRegion.MedNorthAtlantic:
            //        region5 = CoastRegion.Mediterranean;
            //        region6 = CoastRegion.NortheastAtlantic;
            //        flag2 = true;
            //        break;
            //    case CoastRegion.BlackMed:
            //        region5 = CoastRegion.BlackSea;
            //        region6 = CoastRegion.Mediterranean;
            //        flag2 = true;
            //        break;
            //}
            //if (flag)
            //{
            //    if (flag2)
            //    {
            //        float seaTravelMultiplier = TIMapRegionTemplate.GetSeaTravelMultiplier(region3, region5, suezAccess, panamaAccess, false);
            //        float seaTravelMultiplier2 = TIMapRegionTemplate.GetSeaTravelMultiplier(region4, region5, suezAccess, panamaAccess, false);
            //        float seaTravelMultiplier3 = TIMapRegionTemplate.GetSeaTravelMultiplier(region3, region6, suezAccess, panamaAccess, false);
            //        float seaTravelMultiplier4 = TIMapRegionTemplate.GetSeaTravelMultiplier(region4, region6, suezAccess, panamaAccess, false);
            //        return Mathf.Min(new float[]
            //        {
            //            seaTravelMultiplier,
            //            seaTravelMultiplier2,
            //            seaTravelMultiplier3,
            //            seaTravelMultiplier4
            //        });
            //    }
            //    float seaTravelMultiplier5 = TIMapRegionTemplate.GetSeaTravelMultiplier(region3, region2.mapRegionTemplate.coast, suezAccess, panamaAccess, false);
            //    float seaTravelMultiplier6 = TIMapRegionTemplate.GetSeaTravelMultiplier(region4, region2.mapRegionTemplate.coast, suezAccess, panamaAccess, false);
            //    return Mathf.Min(seaTravelMultiplier5, seaTravelMultiplier6);
            //}
            //else
            //{
            //    if (flag2)
            //    {
            //        float seaTravelMultiplier7 = TIMapRegionTemplate.GetSeaTravelMultiplier(region1.mapRegionTemplate.coast, region5, suezAccess, panamaAccess, false);
            //        float seaTravelMultiplier8 = TIMapRegionTemplate.GetSeaTravelMultiplier(region1.mapRegionTemplate.coast, region6, suezAccess, panamaAccess, false);
            //        return Mathf.Min(seaTravelMultiplier7, seaTravelMultiplier8);
            //    }
            return TIMapRegionTemplate.GetSeaTravelMultiplier(region1.mapRegionTemplate.coast, region2.mapRegionTemplate.coast, suezAccess, panamaAccess, false);
            //}
        }

        public bool isMagic => this.MagicResource;

        public bool isTeleport => this.TeleportRegion;
        public double annualPopulationGrowth
        {
            get
            {
                double num = 4.49788037409348;
                return Mathd.Max(Mathd.Clamp(num + Mathd.Max(-num, -0.418190741 * (double)this.nation.education) + -0.0624798523403752 * (double)this.nation.cohesion + 9.80843732089162E-06 * (double)Mathf.Min(180000f, this.nation.perCapitaGDP) + -0.115739931206548 * (double)Mathf.Sqrt(Mathf.Abs(this.latitude)) + (double)((this.annualPopGrowthModifier + this.nation.template.popGrowthModifier) * Mathf.Max(0f, (25f - TITimeState.CampaignDuration_years_Exact()) / 25f)) - (double)(this.xenoforming.xenoformingLevel / 200f) - (double)(this.nuclearDetonations * 4), -10.0, 10.0), -100.0) * 0.01;//- (double)(Math.Max(0f, Mathf.Abs(GameStateManager.GlobalValues().temperatureAnomaly_C) - 8f) * ((this.template.environment == EnvironmentType.Beneficiary) ? 0.5f : ((this.template.environment == EnvironmentType.Vulnerable) ? 2f : 1f)))
            }
        }

        public void OnNuclearAttackArrives(TIFactionState applyingFaction, TINationState applyingNation = null)
        {
            float num3 = TIEffectsState.SumEffectsModifiers((Context)patch_Context.MegaspellLevel, applyingFaction, 0f);
            Mood.TriggerEvent(Mood.Event.SDKL_MushroomCloud);
            patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm(-5, GHGSources.Effect);
            GameControl.eventManager.TriggerEvent(new NuclearStrike(applyingNation, this), null, new object[]
            {
                this
            });
            if (num3 == 3)
            {
                this.ApplyDamageToRegion(2.00f, applyingFaction, applyingNation, true, true, true, true);
            }
            else if (num3 == 2)
            {
                this.ApplyDamageToRegion(1.00f, applyingFaction, applyingNation, true, true, true, true);
            }
            else if (num3 == 1)
            {
                this.ApplyDamageToRegion(0.50f, applyingFaction, applyingNation, true, true, true, true);
            }
            else
            {
                this.ApplyDamageToRegion(0.25f, applyingFaction, applyingNation, true, true, true, true);
            }
        }


        public void ApplyDamageToRegion(float strength, TIFactionState applyingCouncilState = null, TINationState applyingNation = null, bool includeArmies = true, bool includeCouncilors = false, bool forceAttackSpaceAssets = false, bool nuclear = false)
        {
            if (strength > 0f)
            {
                bool flag = nuclear && (applyingNation == null || applyingNation.enemies.Contains(this.nation));
                double num;
                float num2;
                if (nuclear)
                {
                    num = -1.0 * this.nationalGDPShareValue * (double)strength * (double)(0.75f + UnityEngine.Random.Range(0f, 0.5f)) * (flag ? 0.7 : 0.20000000298023224);
                    num += (double)TIEffectsState.SumEffectsModifiers(Context.NuclearStrikeDamageReduction, this, (float)num);
                    num2 = -1f * this.populationInMillions * strength * ((0.75f + UnityEngine.Random.Range(0f, 0.5f)) * (flag ? 0.25f : 0.025f));
                    num2 += TIEffectsState.SumEffectsModifiers(Context.NuclearStrikeDamageReduction, this, num2);
                    this.nation.AddToSustainability(this.NationalGDPProportion() * strength * (0.075f + UnityEngine.Random.Range(0f, 0.05f)) * (flag ? 1f : 0.05f));
                }
                else
                {
                    num = -1.0 * this.nationalGDPShareValue * (double)strength * (double)(0.75f + UnityEngine.Random.Range(0f, 0.5f)) * 0.10000000149011612;
                    num2 = -1f * this.populationInMillions * strength * ((0.75f + UnityEngine.Random.Range(0f, 0.5f)) * 0.001f);
                    this.nation.AddToSustainability(this.NationalGDPProportion() * strength * (7.5E-06f + UnityEngine.Random.Range(0f, 5E-06f)));
                }
                this.nation.ModifyGDP(num, TINationState.GDPChangeReason.GDPReason_RegionDamage);
                this.ChangePopulation_Millions(num2);
                patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(-num2, true);/////ADDITION, feeds the deaths to globalstate.
                if ((nuclear || -num2 > 0.1f) && applyingCouncilState != null)
                {
                    applyingCouncilState.CommitAtrocity((int)Mathf.Clamp(-num2 * 10f, 1f, 20f), TIFactionState.AtrocityCause.MassCasualtiesfromRegionDamage);
                }
                if (strength >= 0.9f)
                {
                    if (nuclear && this.populationInMillions >= 1f)
                    {
                        float num3 = (applyingNation != this.nation) ? 0.005f : 0.001f;
                        foreach (TINationState tinationState in GameStateManager.AllExtantHumanNations())
                        {
                            tinationState.GDPPctChange(-1f * (num3 + (UnityEngine.Random.value + UnityEngine.Random.value) / 100f), TINationState.GDPChangeReason.GDPReason_RegionDamage);
                        }
                    }
                    if (nuclear)
                    {
                        foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
                        {
                            foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
                            {
                                if (ticouncilorState.homeRegion == this)
                                {
                                    TITraitTemplate.ProcessLoyaltyChangeFromTraits(ticouncilorState, SpecialTraitRule.LoyaltyLossOnHomeRegionNuked, (applyingCouncilState == tifactionState) ? 2 : 1);
                                }
                            }
                        }
                    }
                    if (this.coreEconomicRegion && applyingNation != this.nation)
                    {
                        this.coreEconomicRegion = false;
                        GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(this), null, new object[]
                        {
                            this
                        });
                        foreach (TINationState tinationState2 in GameStateManager.AllExtantHumanNations())
                        {
                            tinationState2.GDPPctChange(-1f * (0.025f + (UnityEngine.Random.value + UnityEngine.Random.value) / 100f), TINationState.GDPChangeReason.GDPReason_GlobalCoreEconomicRegionDestroyed);
                        }
                    }
                    if (this.coreResourceRegion && applyingNation != this.nation)
                    {
                        this.resourceRegion = false;
                        this.oilRegion = false;
                        GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(this), null, new object[]
                        {
                            this
                        });
                        foreach (TINationState tinationState3 in GameStateManager.AllExtantHumanNations())
                        {
                            tinationState3.GDPPctChange(-1f * (0.015f + (UnityEngine.Random.value + UnityEngine.Random.value) / 100f), TINationState.GDPChangeReason.GDPReason_GlobalCoreResourceRegionDestroyed);
                        }
                    }
                    foreach (PriorityType priorityType in Enums.PriorityTypes)
                    {
                        if (priorityType - PriorityType.Unity > 1 && priorityType != PriorityType.Spoils)
                        {
                            this.nation.ModifyAccumulatedInvestment(priorityType, 1f - strength, true, false);
                        }
                    }
                    this.nation.SetDataDirty();
                }
                else if (UnityEngine.Random.value < strength * 5f)
                {
                    this.nation.ModifyAccumulatedInvestment(this.nation.GetRandomPriorityToDamage(), this.colonyRegion ? (1f - strength * 0.5f) : (1f - strength), true, true);
                }
                if (strength >= 0.75f && applyingNation != this.nation)
                {
                    this.DestroySpaceAssets(true);
                }
                else
                {
                    this.nation.ChangeAnnualSpaceFundingValue(-1f * (this.NationalGDPProportion() * this.nation.spaceFunding_year * strength * (nuclear ? 0.5f : 0.1f)));
                    if (this.boostPerMonth_dekatons > 0f && (UnityEngine.Random.value < strength || forceAttackSpaceAssets))
                    {
                        this.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -(this.boostPerYear_dekatons * strength), false, true);
                    }
                    if (this.missionControl > 0 && (UnityEngine.Random.value < strength || forceAttackSpaceAssets))
                    {
                        this.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, -1f, false, true);
                    }
                    if (this.antiSpaceDefenses && (UnityEngine.Random.value < strength || forceAttackSpaceAssets))
                    {
                        this.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, true);
                    }
                }
                if (includeArmies)
                {
                    List<TIArmyState> list = this.armies.Where(delegate (TIArmyState army)
                    {
                        if (army.homeNation != applyingNation)
                        {
                            TINationState applyingNation2 = applyingNation;
                            if (applyingNation2 == null || !applyingNation2.allies.Contains(army.homeNation))
                            {
                                return army.faction != applyingCouncilState || applyingCouncilState == null;
                            }
                        }
                        return false;
                    }).ToList<TIArmyState>();
                    TIFactionState applyingCouncilState2 = applyingCouncilState;
                    if (applyingCouncilState2 == null || !applyingCouncilState2.IsAlienFaction)
                    {
                        list.AddRange(this.MegafaunaArmiesPresent());
                    }
                    list = (from x in list
                            orderby x.strength * x.techLevel descending
                            select x).ToList<TIArmyState>();
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        if (nuclear && j > 0)
                        {
                            float num4 = strength;
                            if (list[j].AlienRegularArmy || (Mathd.d100() < 50 && list[j].techLevel >= 3.8f))
                            {
                                float num5 = Mathf.Max(list[j].techLevel - 3.79f, 0f) * UnityEngine.Random.Range(1f, 5f);
                                num4 -= num5 / 100f;
                            }
                            num4 = Mathf.Max(num4, 0f);
                            num4 += TIEffectsState.SumEffectsModifiers(Context.ArmyNuclearHardening, list[j].faction, num4);
                            list[j].TakeDamage(num4, applyingCouncilState, applyingNation);
                        }
                        else
                        {
                            list[j].TakeDamage(strength, applyingCouncilState, applyingNation);
                        }
                    }
                    if (nuclear)
                    {
                        TIArmyState[] array2 = this.armies.Except(list).ToArray<TIArmyState>();
                        for (int k = array2.Length - 1; k >= 0; k--)
                        {
                            array2[k].TakeDamage(strength / (48f + UnityEngine.Random.Range(0f, 4f)), applyingCouncilState, applyingNation);
                        }
                    }
                }
                if (includeCouncilors)
                {
                    foreach (TICouncilorState ticouncilorState2 in this.GetCouncilorsInRegion())
                    {
                        if (ticouncilorState2.traits.None((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.Survivor) && UnityEngine.Random.Range(0f, 2f) < strength)
                        {
                            TINotificationQueueState.LogCouncilorKilledInAttack(ticouncilorState2, ticouncilorState2.location);
                            ticouncilorState2.KillCouncilor(true, applyingCouncilState);
                        }
                    }
                }
                if (nuclear)
                {
                    this.xenoforming.SetXenoformingLevel(0f);
                    TIGlobalValuesState.GlobalValues.TriggerNuclearDetonationEffect(true, applyingNation, this, this.nation);
                }
                else if (applyingCouncilState == null || (!applyingCouncilState.IsAlienFaction && !applyingCouncilState.IsAlienProxy))
                {
                    this.xenoforming.ChangeXenoformingLevel(-(this.xenoforming.xenoformingLevel * strength));
                }
                GameControl.eventManager.TriggerEvent(new RegionDamaged(this), null, new object[]
                {
                    this
                });
                GameControl.eventManager.TriggerEvent(new RegionDataUpdated(this), null, new object[]
                {
                    this
                });
            }
        }

        public string IconString(TIFactionState faction)
        {

            StringBuilder stringBuilder = new StringBuilder(8);
            if (this.nation.capital == this)
            {
                stringBuilder.Append(TemplateManager.global.capitalRegionInlineSpritePath);
            }
            if (this.coreEconomicRegion)
            {
                stringBuilder.Append(TemplateManager.global.coreEconomicRegionInlineSpritePath);
            }
            if (this.resourceRegion)
            {
                stringBuilder.Append(TemplateManager.global.miningRegionInlineSpritePath);
            }
            if (this.MagicResource)
            {
                stringBuilder.Append(patch_TemplateManager.global_PVC.MagicResourceInlineSpritePath);
            }
            if (this.TeleportRegion)
            {
                stringBuilder.Append(patch_TemplateManager.global_PVC.TeleportRegionSpritePath);
            }
            if (this.colonyRegion)
            {
                stringBuilder.Append(TemplateManager.global.colonyRegionInlineSpritePath);
            }
            if (this.template.environment == EnvironmentType.Vulnerable)
            {
                stringBuilder.Append(TemplateManager.global.ecologicallyVulnerableRegionInlineSpritePath);
            }
            if (this.template.environment == EnvironmentType.Beneficiary)
            {
                stringBuilder.Append(TemplateManager.global.ecologicallySafeRegionInlineSpritePath);
            }
            if (this.mapRegionTemplate.terrain == TerrainType.Rugged)
            {
                stringBuilder.Append(TemplateManager.global.ruggedRegionInlineSpritePath);
            }
            if (this.nuclearDetonations > 0)
            {
                stringBuilder.Append(TemplateManager.global.nukedRegionInlineSpritePath);
            }
            if (this.antiSpaceDefenses)
            {
                stringBuilder.Append(TemplateManager.global.antiSpaceDefensesInlineSpritePath);
            }
            if (faction.KnownAlienEntities.Any((TIRegionAlienEntityState x) => x.region == this))
            {
                stringBuilder.Append(TemplateManager.global.alienEntityInlineSpritePath);
            }
            return stringBuilder.ToString();
        }

        public extern void orig_InitWithTemplate(TIDataTemplate template);
        public override void InitWithTemplate(TIDataTemplate template)
        {
            orig_InitWithTemplate(template);
            patch_TIRegionTemplate template_PVC = template as patch_TIRegionTemplate;

            this.MagicResource = template_PVC.magic.GetValueOrDefault();
            this.TeleportRegion = template_PVC.Teleport.GetValueOrDefault();

            ConduitFacilityState tiregionFacilityState = GameStateManager.CreateNewGameState<ConduitFacilityState>();
            tiregionFacilityState.InitWithRegionState(this);
            this.Facility = tiregionFacilityState;
        }
        public ConduitFacilityState Facility { get; private set; }

        public float populationInMillions { get; private set; }
    }

    public class patch_TemplateManager : TemplateManager
    {
        public static patch_TIGlobalConfig global_PVC => global as patch_TIGlobalConfig;
    }
}
