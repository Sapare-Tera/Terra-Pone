using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

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



        public bool isMagic
        {
            get
            {
                return this.MagicResource;
            }
        }

        public bool isTeleport
        {
            get
            {
                return this.TeleportRegion;
            }
        }
        //public TIConduitState ConduitFacility;
        public extern void orig_InitWithTemplate(TIDataTemplate template);
        public override void InitWithTemplate(TIDataTemplate template)
        {
            orig_InitWithTemplate(template);
            patch_TIRegionTemplate template_PVC = template as patch_TIRegionTemplate;

            this.MagicResource = template_PVC.magic.GetValueOrDefault();
            this.TeleportRegion = template_PVC.Teleport.GetValueOrDefault();


            patch_TIRegionAlienFacilityState tiregionAlienFacilityState = GameStateManager.CreateNewGameState<patch_TIRegionAlienFacilityState>();
            tiregionAlienFacilityState.InitWithRegionState(this);
            this.alienFacility2 = tiregionAlienFacilityState;
        }
        public bool hasAlienFacility
        {
            get
            {
                return this.alienFacility2.built || this.alienFacility.built;
            }
        }
        //public patch_TISpaceDefensesFacilityState EveriaFacilityTest { get; private set; }

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


        public patch_TIRegionAlienFacilityState alienFacility2 { get; private set; }
    }
    public class patch_TemplateManager : TemplateManager
    {
        public static patch_TIGlobalConfig global_PVC => global as patch_TIGlobalConfig;
    }

    public class patch_TIRegionAlienFacilityState : TIRegionAlienFacilityState
    {
        public void BuildFacility2()
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
                    tifactionState.SetIntel(this, 1f, null);
                }
            }
            GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
        }

        //OLD
        public bool built { get; private set; }
        public override bool Extant()
        {
            return this.built;
        }
        public override string GetIconResourcePath(TIFactionState faction)
        {
            return TemplateManager.global.pathGeoscapeAlienFacility;
        }
        public override string GetIllustrationPath(TIFactionState faction)
        {
            return TemplateManager.global.illus_alienFacility;
        }
        public override bool isRegionAlienFacility
        {
            get
            {
                return true;
            }
        }
        public override TIRegionAlienFacilityState ref_alienFacility
        {
            get
            {
                return this;
            }
        }
        public void InitWithRegionState(TIRegionState region)
        {
            if (!this.gameStateSubjectCreated)
            {
                if (region.template == null)
                {
                    return;
                }
                this.templateName = region.template.dataName;
                base.region = region;
                this.built = false;
                this.gameStateSubjectCreated = true;
                if (TemplateManager.global.debug_advancedFactionStart && region.templateName == "RockyMountains")
                {
                    this.built = true;
                    this.currentHP = 80f;
                }
            }
        }
        public override void PostInitializationInit_4()
        {
            if (TemplateManager.global.debug_advancedFactionStart && this.built)
            {
                GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate (TIFactionState x)
                {
                    x.SetIntel(this, 1f, null);
                });
            }
        }
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
            GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
        }
        public void SightedByFaction(TIFactionState council)
        {
            GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
            TINotificationQueueState.LogAlienFacilityDetected(council, base.region.alienFacility);
        }
        public override float GetArmyAssaultDefenseScore()
        {
            return 4.5f + ((base.region.terrain == TerrainType.Rugged) ? 1.5f : 0f);
        }
        public override string ResolveAssault(TIGameState assaultingState, TIFactionState assaultingFaction, TIMissionOutcome outcome)
        {
            string empty = string.Empty;
            GameControl.eventManager.TriggerEvent(new TIGameStateAttacking(this), null, new object[]
            {
                assaultingState
            });
            GameControl.eventManager.TriggerEvent(new AlienFacilityDamaged(this), null, new object[]
            {
                this
            });
            if (outcome >= TIMissionOutcome.Success)
            {
                this.built = false;
                int num = (int)((float)base.region.abductions * TemplateManager.global.abductionsCancelledFactorOnFacilityAssault);
                float num2 = TemplateManager.global.exoticsFromAlienFacilityRaid * UnityEngine.Random.Range(0.75f, 1.25f);
                TINotificationQueueState.LogAlienFacilityAssaulted(assaultingState, assaultingFaction, this, num2, num);
                if (assaultingFaction != null && (assaultingState.ref_councilor == assaultingState || assaultingState.ref_army == assaultingState))
                {
                    foreach (CampaignMilestone milestone in this.CampaignMilestonesGrantedOnCapture(assaultingFaction, outcome))
                    {
                        assaultingFaction.CompleteMilestone(milestone);
                    }
                    assaultingFaction.AddToCurrentResource(num2, FactionResource.Exotics, false);
                }
                if (!assaultingState.isCouncilorState)
                {
                    this.ref_faction.GainFactionHate(assaultingFaction, TemplateManager.global.factionHateForDestroyAlienFacility, false);
                    GameStateManager.AlienProxy().GainFactionHate(assaultingFaction, TemplateManager.global.factionHateForDestroyAlienFacility, false);
                }
                base.region.ConductAbductions(this.ref_faction, -num);
                this.OnDestruction();
            }
            else if (outcome == TIMissionOutcome.Failure)
            {
                this.currentHP = Mathf.Clamp(this.currentHP - UnityEngine.Random.value * 10f, 1f, 80f);
            }
            return empty;
        }
        public bool Bombed(TISpaceFleetState fleet, float damageValue)
        {
            this.currentHP -= damageValue;
            if (damageValue > 0f)
            {
                GameControl.eventManager.TriggerEvent(new AlienFacilityDamaged(this), null, new object[]
                {
                    this
                });
            }
            if (this.currentHP <= 0f)
            {
                this.built = false;
                TINotificationQueueState.LogAlienFacilityBombed(fleet, this);
                this.OnDestruction();
                this.ref_faction.GainFactionHate(fleet.faction, TemplateManager.global.factionHateForDestroyAlienFacility, false);
                GameStateManager.AlienProxy().GainFactionHate(fleet.faction, TemplateManager.global.factionHateForDestroyAlienFacility, false);
                return true;
            }
            return false;
        }
        public void OnDestruction()
        {
            TIFactionState[] array = GameStateManager.AllFactions();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].ExpireIntel(this, true);
            }
            foreach (TIArmyState tiarmyState in base.region.armies)
            {
                using (List<OperationData>.Enumerator enumerator2 = tiarmyState.currentOperations.ToList<OperationData>().GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current.target == this)
                        {
                            tiarmyState.ClearOperations();
                        }
                    }
                }
            }
            GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
        }
        public override List<CampaignMilestone> CampaignMilestonesGrantedOnCapture(TIFactionState capturingFaction, TIMissionOutcome outcome)
        {
            List<CampaignMilestone> list = new List<CampaignMilestone>();
            if (outcome >= TIMissionOutcome.Success)
            {
                list.Add(CampaignMilestone.AccessAlienTech);
                if (TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, this.ref_faction))
                {
                    list.Add(CampaignMilestone.AccessSalamanderCorpus);
                    if (capturingFaction.CanDetectAlien && UnityEngine.Random.value <= 0.5f)
                    {
                        list.Add(CampaignMilestone.AccessHydraCorpus);
                    }
                }
            }
            if (outcome >= TIMissionOutcome.CriticalSuccess && TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, this.ref_faction))
            {
                list.Add(CampaignMilestone.AccessLiveSalamander);
                if (capturingFaction.CanCaptureAlien && UnityEngine.Random.value <= 0.5f)
                {
                    list.Add(CampaignMilestone.AccessLiveHydra);
                }
                else if (capturingFaction.CanDetectAlien)
                {
                    list.Add(CampaignMilestone.AccessHydraCorpus);
                }
            }
            return list;
        }
        private const int maxHP = 80;
        public float currentHP;
    }
    public class patch_AlienMarkerController : AlienMarkerController
    {

        public extern void orig_InitializeWithRegion(RegionController regionController, MarkerContainerController container);
        public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
        {

            orig_InitializeWithRegion(regionController, container);
            //base.InitializeWithRegion(regionController, container);
            this.alienFacility = base.region.alienFacility;
            this.alienFacility2 = base.region.alienFacility;
            //this.alienLanding = base.region.alienLanding;
            //this.alienCrashdown = base.region.alienCrashdown;
            //this.alienActivity = base.region.alienActivity;
            //this.xenoforming = base.region.xenoforming;
            //this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
            GameControl.eventManager.AddListener<AlienRegionEntityUpdated>(new EventManager.EventDelegate<AlienRegionEntityUpdated>(this.UpdateAllMarkersForActivityEvent), null, base.region, true, false);
            GameControl.eventManager.AddListener<AlienCrashdownInRegion>(new EventManager.EventDelegate<AlienCrashdownInRegion>(this.UpdateForCrashdown), null, base.region, true, false);
            GameControl.eventManager.AddListener<ArmyTargetAlienAsset>(new EventManager.EventDelegate<ArmyTargetAlienAsset>(this.ActivateAssetTargetsForArmy), null, null, true, false);
            GameControl.eventManager.AddListener<CouncilorTargetAlienActivity>(new EventManager.EventDelegate<CouncilorTargetAlienActivity>(this.ActivateActivityTargetsForCouncilor), null, null, true, false);
            GameControl.eventManager.AddListener<CouncilorTargetAlienAsset>(new EventManager.EventDelegate<CouncilorTargetAlienAsset>(this.ActivateAssetTargetsForCouncilor), null, null, true, false);
            GameControl.eventManager.AddListener<DeTargetAlienActivity>(new EventManager.EventDelegate<DeTargetAlienActivity>(this.DeactivateActivityTargets), null, null, true, false);
            GameControl.eventManager.AddListener<DeTargetAlienAssets>(new EventManager.EventDelegate<DeTargetAlienAssets>(this.DeactivateAssetTargets), null, null, true, false);
            GameControl.eventManager.AddListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewTargetSelected), null, base.region, true, false);
            GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateAllMarkersForMapActivation), null, null, true, false);
            GameControl.eventManager.AddListener<RegionXenoformingIntelUpdate>(new EventManager.EventDelegate<RegionXenoformingIntelUpdate>(this.UpdateXenoformingMarker), null, base.region, true, false);
            GameControl.eventManager.AddListener<AlienFacilityDamaged>(new EventManager.EventDelegate<AlienFacilityDamaged>(this.OnAlienFacilityDamaged), null, this.alienFacility, false, false);
            GameControl.eventManager.AddListener<AlienFacilityDamaged>(new EventManager.EventDelegate<AlienFacilityDamaged>(this.OnAlienFacilityDamaged), null, this.alienFacility2, false, false);
            //GameControl.eventManager.AddListener<AlienLandingDamaged>(new EventManager.EventDelegate<AlienLandingDamaged>(this.OnAlienLandingDamaged), null, this.alienLanding, false, false);
            //GameControl.eventManager.AddListener<XenoformingDamaged>(new EventManager.EventDelegate<XenoformingDamaged>(this.OnXenoformingDamaged), null, this.xenoforming, false, false);
            //GameControl.eventManager.AddListener<XenoformingDestroyed>(new EventManager.EventDelegate<XenoformingDestroyed>(this.OnXenoformingDestroyed), null, this.xenoforming, false, false);
            //GameControl.eventManager.AddListener<TIGameStateAttacking>(new EventManager.EventDelegate<TIGameStateAttacking>(this.OnXenoformingAttacking), null, this.xenoforming, false, false);
            this.UpdateAllMarkers();
            this.currentTargetList = new List<TIGameState>();
           
        }
        private TIRegionAlienFacilityState alienFacility;
        private TIRegionAlienFacilityState alienFacility2;
        //private TIRegionUFOCrashdownState alienCrashdown;
        //private TIRegionUFOLandingState alienLanding;
        //private TIRegionAlienActivityState alienActivity;
        //private TIRegionXenoformingState xenoforming;
        //private GameTimeManager gameTime;
        private List<TIGameState> currentTargetList;

        private void OnAlienFacilityDamaged(AlienFacilityDamaged e)
        {
            if (this.alienFacilityMarker != null && this.alienFacility.VisibleToFaction(base.activePlayer))
            {
                this.alienFacilityMarker.TriggerExplosion();
            }
        }
        private void UpdateAlienFacilityMarker()
        {
            bool flag = (this.alienFacility != null && this.alienFacility.built && this.alienFacility.VisibleToFaction(base.activePlayer)) || this.alienFacility2 != null && this.alienFacility2.built && this.alienFacility2.VisibleToFaction(base.activePlayer);
            this.alienFacilityMarker = base.container.ManageMarkerStack(this.alienFacilityMarker, !flag, MarkerType.AlienFacility, base.region, "alienFacility", -1, false);
            if (flag)
            {
                this.UpdateAlienMarker(this.alienFacilityMarker, this.alienFacility);
                this.UpdateAlienMarker(this.alienFacilityMarker, this.alienFacility2);
                this.alienFacilityMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnAlienFacilityClicked));
                this.alienFacilityMarker.TriggerAlienLights(7);
                if (!this.SetCouncilorTargeting(this.alienFacilityMarker) && !this.SetArmyTargeting(this.alienFacilityMarker))
                {
                    this.alienFacilityMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
                }
                base.container.InitializeGeoscapeModel(this.alienFacilityMarker, "3dearthmodels/geoscape_alien_facilities");
            }
        }
        private void OnAlienFacilityClicked(MarkerController controller)
        {
            this.TriggerUIEffects(controller);
            AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Activity_Earth", false);
            GameControl.eventManager.TriggerEvent(new AlienAssetTargetSelected(this.alienFacility), null, Array.Empty<object>());
            GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.alienFacility), null, Array.Empty<object>());
        }
        private bool armyTargetingAlienSurfaceAssetMode;
        private bool SetArmyTargeting(MarkerController marker)
        {
            if (marker != null && this.armyTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker))
            {
                if (!this.currentTargetList.Contains(marker.associatedState))
                {
                    marker.SetTooltip(() => Loc.T("TIMissionTargeting_InvalidTarget", new object[]
                    {
                        marker.associatedState.displayName
                    }));
                }
                return true;
            }
            return false;
        }
        private bool councilorTargetingAlienSurfaceAssetMode;
        private bool councilorTargetingAlienActivity;
        private TIMissionTemplate missionTemplate;
        private TICouncilorState targetingCouncilor;
        private bool SetCouncilorTargeting(MarkerController marker)
        {
            if (marker != null && ((this.councilorTargetingAlienActivity && (marker == this.alienActivityMarker || marker == this.alienCrashdownMarker)) || (this.councilorTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker))))
            {
                TIGameState state = marker.associatedState;
                if (this.currentTargetList.Contains(state))
                {
                    marker.SetToHitNumber(this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, state, 0f, false, 2), this.missionTemplate.resolutionMethod.automaticSuccess, (base.globalCurrentTarget == state) ? ClearFlag.TurnOff : ClearFlag.TurnOn, 0);
                }
                else
                {
                    marker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
                    marker.SetTooltip(() => MarkerController.BuildInvalidTargetTooltip(this.missionTemplate.target.ValidateSingleTarget(this.missionTemplate, this.targetingCouncilor, state)));
                }
                return true;
            }
            return false;
        }
        private bool TargetingButInvalidTarget(MarkerController marker)
        {
            return (!this.currentTargetList.Contains(marker.associatedState) && this.armyTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker)) || (this.councilorTargetingAlienActivity && (marker == this.alienActivityMarker || marker == this.alienCrashdownMarker)) || (this.councilorTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker));
        }
        private bool Targeting
        {
            get
            {
                return this.armyTargetingAlienSurfaceAssetMode || this.councilorTargetingAlienActivity || this.councilorTargetingAlienSurfaceAssetMode;
            }
        }
        private void TriggerUIEffects(MarkerController controller)
        {
            if (this.Targeting)
            {
                if (this.TargetingButInvalidTarget(controller))
                {
                    AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false);
                }
                else
                {
                    AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false);
                }
            }
            else
            {
                AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false);
            }
            GeneralControlsController.SetSelectedState(controller.associatedState, true);
        }




        //private void OnAlienLandingDamaged(AlienLandingDamaged e)
        //{
        //    if (this.alienLandingMarker != null)
        //    {
        //        this.alienLandingMarker.TriggerExplosion();
        //    }
        //}
        //private void OnXenoformingDamaged(XenoformingDamaged e)
        //{
        //    if (this.xenoformingMarker != null)
        //    {
        //        this.xenoformingMarker.TriggerExplosion();
        //        this.UpdateXenoformingMarker();
        //    }
        //}
        //private void OnXenoformingDestroyed(XenoformingDestroyed e)
        //{
        //    if (this.xenoformingMarker != null)
        //    {
        //        this.xenoformingMarker.TriggerExplosion();
        //        this.xenoformingMarker.TriggerDestruction();
        //        this.UpdateXenoformingMarker();
        //    }
        //}
        //private void OnXenoformingAttacking(TIGameStateAttacking e)
        //{
        //    if (this.xenoformingMarker != null)
        //    {
        //        this.xenoformingMarker.TriggerAttacking();
        //    }
        //}
        //private void UpdateXenoformingMarker()
        //{
        //    bool flag = this.xenoforming != null && this.xenoforming.xenoformingLevel > 0f && this.xenoforming.VisibleToFaction(base.activePlayer);
        //    this.xenoformingMarker = base.container.ManageMarkerStack(this.xenoformingMarker, !flag, MarkerType.Xenoforming, base.region, "xenoforming", -1, false);
        //    if (flag)
        //    {
        //        this.UpdateAlienMarker(this.xenoformingMarker, this.xenoforming);
        //        this.xenoformingMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnXenoformingMarkerClicked));
        //        if (!this.SetCouncilorTargeting(this.xenoformingMarker) && !this.SetArmyTargeting(this.xenoformingMarker))
        //        {
        //            this.xenoformingMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
        //        }
        //    }
        //}
    }

    //public class patch_AlienMarkerController : AlienMarkerController
    //{
    //    public void UpdateAlienActivityMarker()
    //    {
    //        bool flag = this.alienActivity != null && this.alienActivity.VisibleToFaction(base.activePlayer);
    //        this.alienActivityMarker = base.container.ManageMarkerStack(this.alienActivityMarker, !flag, MarkerType.AlienActivity, base.region, "alienActivity", -1, false);
    //        if (flag)
    //        {
    //            this.UpdateAlienMarker(this.alienActivityMarker, this.alienActivity);
    //            this.alienActivityMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnAlienActivityClicked));
    //            this.alienActivityMarker.TriggerAlienLights(5);
    //            if (!this.SetCouncilorTargeting(this.alienActivityMarker))
    //            {
    //                this.alienActivityMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
    //            }
    //        }
    //    }
    //    public void UpdateAlienMarker(MarkerController marker, TIRegionAlienEntityState alienEntity)
    //    {
    //        marker.associatedState = alienEntity;
    //        marker.SetCentralIcon(alienEntity.GetIcon(base.activePlayer));
    //        marker.centralIcon.raycastTarget = true;
    //        this.SetAnimationOnMarker(marker);
    //        marker.centralButton.enabled = true;
    //        marker.SetTooltip(() => marker.BuildTooltipText(alienEntity.isRegionXenoformingState ? new StringBuilder(alienEntity.displayName).AppendLine().AppendLine(alienEntity.ref_xenoforming.severityDescription).ToString() : alienEntity.displayName, this.activePlayer, TIMissionPhaseState.InMissionPhase(), alienEntity));
    //        marker.SetHoverSprite(base.activePlayer.shouldNeverAttackAliens ? 2 : 1);
    //    }
    //    public MarkerController alienActivityMarker;

    //    private void OnAlienActivityClicked(MarkerController controller)
    //    {
    //        this.TriggerUIEffects(controller);
    //        AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Activity_Earth", false);
    //        GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.alienActivity), null, Array.Empty<object>());
    //    }
    //    private void SetAnimationOnMarker(MarkerController marker)
    //    {
    //        if (marker != null)
    //        {
    //            if (marker.associatedState == base.globalCurrentTarget)
    //            {
    //                this.TargetingAnimation(marker);
    //                return;
    //            }
    //            marker.StopSelectionAnimation();
    //            if (marker == this.xenoformingMarker)
    //            {
    //                if (this.xenoformingMarker.centralIcon.gameObject.activeInHierarchy && this.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming)
    //                {
    //                    this.AlertAnimation(this.xenoformingMarker);
    //                    return;
    //                }
    //                this.xenoformingMarker.StopCentralIconAnimation();
    //                return;
    //            }
    //            else
    //            {
    //                this.AlertAnimation(marker);
    //            }
    //        }
    //    }
    //    private void TriggerUIEffects(MarkerController controller)
    //    {
    //        if (this.Targeting)
    //        {
    //            if (this.TargetingButInvalidTarget(controller))
    //            {
    //                AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false);
    //            }
    //            else
    //            {
    //                AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false);
    //            }
    //        }
    //        else
    //        {
    //            AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false);
    //        }
    //        GeneralControlsController.SetSelectedState(controller.associatedState, true);
    //    }
    //    private TIRegionAlienActivityState alienActivity;

    //}


    //public class RegionEntityUpdated2 : GameEvent
    //{
    //    public RegionEntityUpdated2(TIRegionSpaceFacilityState alienEntityState, TIRegionState region)
    //    {
    //        this.alienEntityState = alienEntityState;
    //        this.region = region;
    //    }
    //    public TIRegionSpaceFacilityState alienEntityState;

    //    public TIRegionState region;
    //}


    //public class patch_TISpaceDefensesFacilityState : TISpaceDefensesFacilityState, CombatWeaponCarrierState
    //{
    //    public bool built { get; private set; }

    //    public override void PostInitializationInit_4()
    //    {
    //        base.PostInitializationInit_4();
    //        this.built = true;
    //        if (this.Extant())
    //        {
    //            if (!base.region.underBombardment || string.IsNullOrEmpty(this.weaponTemplateName))
    //            {
    //                this.SetLaserDefenseWeaponTemplate();
    //                return;
    //            }
    //            this.weaponTemplate = TemplateManager.Find<TILaserWeaponTemplate>(this.weaponTemplateName, false);
    //            if (this.weaponTemplate == null)
    //            {
    //                this.SetLaserDefenseWeaponTemplate();
    //            }
    //        }
    //    }

    //    public void BuildFacility()
    //    {
    //        this.built = true;
    //        this.currentHP = 80f;
    //        foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
    //        {
    //            if (true)
    //            {
    //                tifactionState.SetIntel(this, 1f, null);
    //            }
    //            else
    //            {
    //                tifactionState.SetIntel(this, 0f, null);
    //            }
    //        }
    //        GameControl.eventManager.TriggerEvent(new RegionEntityUpdated2(this, base.region), null, new object[]
    //        {
    //            base.region
    //        });
    //    }

    //    public float currentHP;
    //    public override bool Extant()
    //    {
    //        return !this.built;
    //    }
    //}

    //public class patch_TIRegionAlienFacilityState : TIRegionAlienFacilityState
    //{
    //    public bool built { get; private set; }
    //    public void BuildFacility()
    //    {
    //        this.built = true;
    //        this.currentHP = 80f;
    //        foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
    //        {
    //            if (true)
    //            {
    //                tifactionState.SetIntel(this, 1f, null);
    //            }
    //            else
    //            {
    //                tifactionState.SetIntel(this, 0f, null);
    //            }
    //        }
    //        GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[]
    //        {
    //            base.region
    //        });
    //    }
    //    public float currentHP;

    //}











    //    public class patch_FacilityMarkerController : FacilityMarkerController //disable
    //{

    //    private bool launchDataDirty;

    //    private bool missionControlDataDirty;

    //    private bool spaceDefensesDataDirty;

    //    private void Update()
    //    {
    //        if (this.launchDataDirty)
    //        {
    //            this.UpdateBoostMarker();
    //           //this.UpdateConduitMarker();
    //            this.launchDataDirty = false;
    //            base.container.Refresh();
    //        }
    //        if (this.missionControlDataDirty)
    //        {
    //            this.UpdateMissionControlMarker();
    //            base.container.Refresh();
    //            this.missionControlDataDirty = false;
    //        }
    //        if (this.spaceDefensesDataDirty)
    //        {
    //            this.UpdateLaserMarker();
    //            base.container.Refresh();
    //            this.spaceDefensesDataDirty = false;
    //        }
    //    }

    //    public override void UpdateMarker()
    //    {
    //        this.UpdateBoostMarker();
    //        //this.UpdateConduitMarker();
    //        this.UpdateMissionControlMarker();
    //        this.UpdateLaserMarker();
    //        base.container.Refresh();
    //    }
    //    private void UpdateMarker(RegionDataUpdated e)
    //    {
    //        this.AttemptUpdateMarker();
    //    }
    //    private void UpdateMarker(MapActivationChangedEvent e)
    //    {
    //        if (e.active)
    //        {
    //            this.AttemptUpdateMarker();
    //        }
    //    }
    //    public extern void orig_InitializeWithRegion(RegionController regionController, MarkerContainerController container);
    //    private TIRegionAlienFacilityState EveriaFacilityTest;
    //    public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
    //    {
    //        orig_InitializeWithRegion(regionController, container);
    //        //base.InitializeWithRegion(regionController, container);
    //        this.EveriaFacilityTest = base.region.alienFacility;
    //        this.Conduit = base.region.boostFacility;
    //        TIRegionSpaceFacilityState tiregionSpaceFacilityState = this.Conduit;
    //        tiregionSpaceFacilityState.FacilityMarkerController = this;
    //        GameControl.eventManager.AddListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnLaunchDataUpdated), null, this.Conduit, true, false);
    //        this.UpdateMarker();
    //    }
    //    public void UpdateConduitMarker()
    //    {
    //        bool flag = this.Conduit.Extant();
    //        this.boostMarker = base.container.ManageMarkerStack(this.boostMarker, !flag, MarkerType.HumanLaunchFacility, base.region, "Test", -1, false);
    //        if (flag)
    //        {
    //            string text = TIUtilities.FormatSmallNumber(base.region.boostPerYear_dekatons, 1, 0, true, false);
    //            this.boostMarker.SetTooltip(() => Loc.T("UI.Markers.Boost", new object[]
    //            {
    //                this.launchState.displayName,
    //                TemplateManager.global.boostInlineSpritePath,
    //                TIUtilities.FormatSmallNumber(base.region.boostPerYear_dekatons, 7, 0, true, false)
    //            }));
    //            if (text == "0")
    //            {
    //                this.boostMarker.SetNumber("Min", ClearFlag.TurnOff, false);
    //            }
    //            else
    //            {
    //                this.boostMarker.SetNumber(text, ClearFlag.TurnOn, false);
    //            }
    //            this.ShowSpaceFacilityMarker(this.boostMarker, this.launchState);
    //            string value;
    //            switch (this.launchState.GetSize())
    //            {
    //                case 1:
    //                    value = "small";
    //                    break;
    //                case 2:
    //                    value = "medium";
    //                    break;
    //                case 3:
    //                    value = "large";
    //                    break;
    //                default:
    //                    Debug.Log("<color=yellow>Launch facility size not found.</color>");
    //                    value = "small";
    //                    break;
    //            }
    //            string assetPath = new StringBuilder("3dearthmodels/geoscape_space_launch_").Append(value).ToString();
    //            base.container.InitializeGeoscapeModel(this.boostMarker, assetPath);
    //        }
    //    }
    //    public TIRegionSpaceFacilityState Conduit;
    //}


    //public class TIConduitState : TIRegionSpaceFacilityState
    //{
    //    public override float GetValue()
    //    {
    //        return base.region.boostPerYear_dekatons + (float)base.region.numSTOFighters;
    //    }
    //    public override string GetDisplayName(TIFactionState faction)
    //    {
    //        string text = new StringBuilder("TIRegionTemplate.BoostFacilityName.").Append(base.region.template.dataName).ToString();
    //        string text2 = Loc.T(text);
    //        if (text2 == string.Empty || text2 == text)
    //        {
    //            text2 = Loc.T("TIRegionTemplate.BoostFacilityName.Generic", new object[]
    //            {
    //                base.region.displayName
    //            });
    //        }
    //        return text2;
    //    }
    //    public override string descriptor
    //    {
    //        get
    //        {
    //            return Loc.T("UI.Nation.LaunchFacility");
    //        }
    //    }
    //    public override string description
    //    {
    //        get
    //        {
    //            return Loc.T("UI.Nation.LaunchDescription");
    //        }
    //    }
    //    public override bool Extant()
    //    {
    //        return base.region.boostPerYear_dekatons >= 0f;
    //    }
    //    public override int GetSize()
    //    {
    //        if (base.region.boostPerYear_dekatons >= 500f)
    //        {
    //            return 3;
    //        }
    //        if (base.region.boostPerYear_dekatons < 100f)
    //        {
    //            return 1;
    //        }
    //        return 2;
    //    }
    //    public override Sprite GetIcon(TIFactionState faction)
    //    {
    //        switch (this.GetSize())
    //        {
    //            case 1:
    //                return AssetCacheManager.launchFacilitySmallIcon;
    //            case 3:
    //                return AssetCacheManager.launchFacilityLargeIcon;
    //        }
    //        return AssetCacheManager.launchFacilityMediumIcon;
    //    }

    //    public override string GetIconResourcePath(TIFactionState faction)
    //    {
    //        switch (this.GetSize())
    //        {
    //            case 1:
    //                return TemplateManager.global.pathGeoscapeLaunchSite1;
    //            case 3:
    //                return TemplateManager.global.pathGeoscapeLaunchSite3;
    //        }
    //        return TemplateManager.global.pathGeoscapeLaunchSite2;
    //    }
    //    public override string GetIllustrationPath(TIFactionState faction)
    //    {
    //        switch (this.GetSize())
    //        {
    //            default:
    //                return TemplateManager.global.illus_launchFacilitySmallPath;
    //            case 2:
    //                return TemplateManager.global.illus_launchFacilityMediumPath;
    //            case 3:
    //                return TemplateManager.global.illus_launchFacilityLargePath;
    //        }
    //    }
    //}
}
