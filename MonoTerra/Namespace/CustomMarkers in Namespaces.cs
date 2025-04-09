using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
    public enum patch_MarkerType
    {
        ConduitMarker = 420,
        FactionCapital = 421
    }
    public class patch_AlienMarkerController : AlienMarkerController
    {
        public extern void orig_InitializeWithRegion(RegionController regionController, MarkerContainerController container);
        public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
        {
            orig_InitializeWithRegion(regionController, container);
            //base.InitializeWithRegion(regionController, container);
            patch_TIRegionState region = (patch_TIRegionState)base.region;
            this.EtruscanFacility = region.Facility;
            //this.alienLanding = base.region.alienLanding;
            //this.alienCrashdown = base.region.alienCrashdown;
            //this.alienActivity = base.region.alienActivity;
            //this.xenoforming = base.region.xenoforming;
            //this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
            GameControl.eventManager.AddListener<Etruscan_RegionEntityUpdated>(new EventManager.EventDelegate<Etruscan_RegionEntityUpdated>(this.AlternativeUpdateAllMarkersForActivityEvent), null, base.region, true, false);
            //GameControl.eventManager.AddListener<AlienCrashdownInRegion>(new EventManager.EventDelegate<AlienCrashdownInRegion>(this.UpdateForCrashdown), null, base.region, true, false);
            GameControl.eventManager.AddListener<ArmyTargetAlienAsset>(new EventManager.EventDelegate<ArmyTargetAlienAsset>(this.ActivateAssetTargetsForArmy), null, null, true, false);
            //GameControl.eventManager.AddListener<CouncilorTargetAlienActivity>(new EventManager.EventDelegate<CouncilorTargetAlienActivity>(this.ActivateActivityTargetsForCouncilor), null, null, true, false);
            //GameControl.eventManager.AddListener<CouncilorTargetAlienAsset>(new EventManager.EventDelegate<CouncilorTargetAlienAsset>(this.ActivateAssetTargetsForCouncilor), null, null, true, false);
            //GameControl.eventManager.AddListener<DeTargetAlienActivity>(new EventManager.EventDelegate<DeTargetAlienActivity>(this.DeactivateActivityTargets), null, null, true, false);
            //GameControl.eventManager.AddListener<DeTargetAlienAssets>(new EventManager.EventDelegate<DeTargetAlienAssets>(this.DeactivateAssetTargets), null, null, true, false);
            //GameControl.eventManager.AddListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewTargetSelected), null, base.region, true, false);
            //GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateAllMarkersForMapActivation), null, null, true, false);
            //GameControl.eventManager.AddListener<RegionXenoformingIntelUpdate>(new EventManager.EventDelegate<RegionXenoformingIntelUpdate>(this.UpdateXenoformingMarker), null, base.region, true, false);
            //GameControl.eventManager.AddListener<AlienFacilityDamaged>(new EventManager.EventDelegate<AlienFacilityDamaged>(this.OnAlienFacilityDamaged), null, this.alienFacility, false, false);
            //GameControl.eventManager.AddListener<AlienFacilityDamaged>(new EventManager.EventDelegate<AlienFacilityDamaged>(this.OnAlienFacilityDamaged), null, this.EtruscanFacility, false, false);
            //GameControl.eventManager.AddListener<AlienLandingDamaged>(new EventManager.EventDelegate<AlienLandingDamaged>(this.OnAlienLandingDamaged), null, this.alienLanding, false, false);
            //GameControl.eventManager.AddListener<XenoformingDamaged>(new EventManager.EventDelegate<XenoformingDamaged>(this.OnXenoformingDamaged), null, this.xenoforming, false, false);
            //GameControl.eventManager.AddListener<XenoformingDestroyed>(new EventManager.EventDelegate<XenoformingDestroyed>(this.OnXenoformingDestroyed), null, this.xenoforming, false, false);
            //GameControl.eventManager.AddListener<TIGameStateAttacking>(new EventManager.EventDelegate<TIGameStateAttacking>(this.OnXenoformingAttacking), null, this.xenoforming, false, false);
            this.UpdateAllMarkers();
            this.currentTargetList = new List<TIGameState>();
        }
        public void AlternativeUpdateAllMarkersForActivityEvent(Etruscan_RegionEntityUpdated e)
        {
            this.AlternativeUpdateAllMarkers();
        }
        public void AlternativeUpdateAllMarkers()
        {
            this.AlternativeUpdateAlienFacilityMarker();
        }

        public void AlterantiveUpdateAlienMarker(MarkerController marker, TIRegionEtruscanEntityState alienEntity)
        {
            marker.associatedState = alienEntity;
            marker.SetCentralIcon(alienEntity.GetIcon(base.activePlayer));
            marker.centralIcon.raycastTarget = true;
            //this.SetAnimationOnMarker(marker);
            marker.centralButton.enabled = true;
            marker.SetTooltip(() => marker.BuildTooltipText(alienEntity.isRegionXenoformingState ? new StringBuilder(alienEntity.displayName).AppendLine().AppendLine(alienEntity.ref_xenoforming.severityDescription).ToString() : alienEntity.displayName, this.activePlayer, TIMissionPhaseState.InMissionPhase(), alienEntity));
            marker.SetHoverSprite(base.activePlayer.shouldNeverAttackAliens ? 2 : 1);
        }

        private void OnEtruscanFacilityClicked(MarkerController controller)
        {
            this.TriggerUIEffects(controller);
            AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Activity_Earth", false);
            GameControl.eventManager.TriggerEvent(new Etruscan_AssetTargetSelected(this.EtruscanFacility), null, Array.Empty<object>());
            GameControl.eventManager.TriggerEvent(new Etruscan_RegionMapEntitySelected(this.EtruscanFacility), null, Array.Empty<object>());
        }
        private void AlternativeUpdateAlienFacilityMarker()
        {
            bool flag2 = this.EtruscanFacility != null && this.EtruscanFacility.built && this.EtruscanFacility.VisibleToFaction(base.activePlayer);
            this.alienFacilityMarker2 = base.container.ManageMarkerStack(this.alienFacilityMarker2, !flag2,(MarkerType)patch_MarkerType.ConduitMarker, base.region, "conduitFacility", -1, false);
            if (flag2)
            {
                this.AlterantiveUpdateAlienMarker(this.alienFacilityMarker2, this.EtruscanFacility);
                this.alienFacilityMarker2.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnEtruscanFacilityClicked));
                this.alienFacilityMarker2.TriggerAlienLights(7);
                if (!this.SetCouncilorTargeting(this.alienFacilityMarker2) && !this.SetArmyTargeting(this.alienFacilityMarker2))
                {
                    this.alienFacilityMarker2.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
                }
                base.container.InitializeGeoscapeModel(this.alienFacilityMarker2, "3dearthmodels/geoscape_alien_facilities");
            }
        }
        private ConduitFacilityState EtruscanFacility;
        public MarkerController alienFacilityMarker2;

        //Dependencies
        private bool councilorTargetingAlienSurfaceAssetMode;
        private bool councilorTargetingAlienActivity;
        private TIMissionTemplate missionTemplate;
        private TICouncilorState targetingCouncilor;
        private List<TIGameState> currentTargetList;
        private GameTimeManager gameTime;



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
        private bool Targeting => this.armyTargetingAlienSurfaceAssetMode || this.councilorTargetingAlienActivity || this.councilorTargetingAlienSurfaceAssetMode;
        private void TriggerUIEffects(MarkerController controller)
        {
            if (this.Targeting)
            {
                if (this.TargetingButInvalidTarget(controller))
                {
                    Log.Debug($"trig_SFX_BadUI");
                    AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false);
                }
                else
                {
                    Log.Debug($"trig_SFX_AlienEarthObjectSelect");
                    AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false);
                }
            }
            else
            {
                Log.Debug($"trig_SFX_AlienEarthObjectSelect");
                AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false);
            }
            GeneralControlsController.SetSelectedState(controller.associatedState, true);
        }
    }


    //Facilities
    public class ConduitFacilityState : TIRegionEtruscanAssetState
    {
        public bool built { get; private set; }
        public override bool Extant()
        {
            return this.built;
        }
        public override string GetIconResourcePath(TIFactionState faction)
        {
            return TemplateManager.global.pathGeoscapeTerrorize;
        }
        public override string GetIllustrationPath(TIFactionState faction)
        {
            return TemplateManager.global.illus_enthrallPublic;
        }


        public override bool isRegionEtruscanFacility => true;
        public override ConduitFacilityState ref_EtruscanFacility => this;
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
            foreach (patch_TIFactionState tifactionState in GameStateManager.AllFactions())
            {
                if (tifactionState.IsAlienProxy || tifactionState.IsAlienFaction)
                {
                    tifactionState.AlternativeSetIntel(this, 1f, null);
                }
                else
                {
                    tifactionState.AlternativeSetIntel(this, 1f, null);
                }
            }
            GameControl.eventManager.TriggerEvent(new Etruscan_RegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
        }
        public void SightedByFaction(TIFactionState council)
        {
            GameControl.eventManager.TriggerEvent(new Etruscan_RegionEntityUpdated(this, base.region), null, new object[]
            {
                base.region
            });
            patch_TIRegionState region = (patch_TIRegionState)base.region;
            patch_TINotificationQueueState.LogEtruscanFacilityDetected(council, region.Facility);
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
            GameControl.eventManager.TriggerEvent(new Etruscan_FacilityDamaged(this), null, new object[]
            {
                this
            });
            if (outcome >= TIMissionOutcome.Success)
            {
                this.built = false;
                int num = (int)((float)base.region.abductions * TemplateManager.global.abductionsCancelledFactorOnFacilityAssault);
                float num2 = TemplateManager.global.exoticsFromAlienFacilityRaid * UnityEngine.Random.Range(0.75f, 1.25f);
                //TINotificationQueueState.LogAlienFacilityAssaulted(assaultingState, assaultingFaction, this, num2, num);
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
                GameControl.eventManager.TriggerEvent(new Etruscan_FacilityDamaged(this), null, new object[]
                {
                    this
                });
            }
            if (this.currentHP <= 0f)
            {
                this.built = false;
                //TINotificationQueueState.LogAlienFacilityBombed(fleet, this);
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
            GameControl.eventManager.TriggerEvent(new Etruscan_RegionEntityUpdated(this, base.region), null, new object[]
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

    public class Etruscan_FacilityDamaged : GameEvent
    {
        public Etruscan_FacilityDamaged(ConduitFacilityState facility)
        {
            this.facility = facility;
        }

        public ConduitFacilityState facility;
    }

    //Dependencies
    public abstract class TIRegionEtruscanAssetState : TIRegionEtruscanEntityState
    {
        public abstract string ResolveAssault(TIGameState assaultingState, TIFactionState assaultingFaction, TIMissionOutcome outcome);

        public abstract List<CampaignMilestone> CampaignMilestonesGrantedOnCapture(TIFactionState capturingFaction, TIMissionOutcome outcome);
        public override bool isRegionEtruscanAsset => true;
        public override TIRegionEtruscanAssetState ref_regionEtruscanAsset => this;
        public abstract float GetArmyAssaultDefenseScore();
        public virtual string GetDestroyedIllustrationPath()
        {
            return "illustrations/Mission_AssaultAlienAsset";
        }
        public bool UnderArmyAssault()
        {
            return base.region.armies.Any((TIArmyState x) => x.CurrentOperations().Count > 0 && x.CurrentOperations()[0].target == this && x.CurrentOperations()[0].operation is AssaultAlienAssetOperation);
        }
    }
    public class Etruscan_AssetTargetSelected : GameEvent
    {
        public Etruscan_AssetTargetSelected(TIRegionEtruscanAssetState EtruscanAsset)
        {
            this.EtruscanAsset = EtruscanAsset;
        }
        public TIRegionEtruscanAssetState EtruscanAsset;
    }

    //
    public abstract class TIRegionEtruscanEntityState : Alternative_TIRegionEntityState
    {
        public override string descriptor => Loc.T("TIRegionAlienEntityState.BasicDescriptor");

        public override string description => Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString());
        public virtual bool VisibleToFaction(TIFactionState faction)
        {
            return this.Extant() && faction.GetIntel(this) > 0f;
        }
        public override string GetDisplayName(TIFactionState faction)
        {
            return Loc.T("TIRegionAlienEntityState.displayNameWithLocation", new object[]
            {
                this.displayName,
                base.region.displayName
            });
        }
        public override bool isRegionEtruscanEntity => true;
        public override TIFactionState ref_faction => GameStateManager.AlienFaction();
        public override TIRegionEtruscanEntityState ref_regionEtruscanEntity => this;
        public override bool Initialize()
        {
            this.displayName = Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName").ToString());
            return base.Initialize();
        }
    }
    public class Etruscan_RegionEntityUpdated : GameEvent
    {
        public Etruscan_RegionEntityUpdated(TIRegionEtruscanEntityState alienEntityState, TIRegionState region)
        {
            this.alienEntityState = alienEntityState;
            this.region = region;
        }
        public TIRegionEtruscanEntityState alienEntityState;
        public TIRegionState region;
    }

    public class Etruscan_RegionMapEntitySelected : GameEvent
    {
        public Etruscan_RegionMapEntitySelected(TIRegionEtruscanEntityState alienEntity)
        {
            this.alienEntity = alienEntity;
        }
        public TIRegionEtruscanEntityState alienEntity;
    }

    //
    public abstract class Alternative_TIRegionEntityState : patch_TIGameState
    {
        public TIRegionState region { get; protected set; }
        public override TIRegionState ref_region => this.region;

        public override TINationState ref_nation => this.region.nation;
        public override bool hasMapObject => true;
        public override TISpaceBodyState ref_spaceBody => this.region.spaceBody;
        public override TINaturalSpaceObjectState ref_naturalSpaceObject => this.region.spaceBody;
        public override TISpaceObjectState ref_spaceObject => this.region.spaceBody;
        public override bool hasEarthMapObject => true;

        public abstract bool Extant();
        public abstract string descriptor { get; }

        public abstract string description { get; }
        public abstract string GetIllustrationPath(TIFactionState faction);
        public virtual Sprite GetIcon(TIFactionState faction)
        {
            return GameControl.assetLoader.LoadAsset<Sprite>(this.GetIconResourcePath(faction));
        }
        public abstract string GetIconResourcePath(TIFactionState faction);
        public void SetRegionEntityDataDirty()
        {
            GameControl.eventManager.TriggerEvent(new Alternative_RegionEntityUpdated(this), null, new object[]
            {
                this,
                this.region
            });
        }
        [SerializeField]
        protected bool gameStateSubjectCreated;
    }
    public class Alternative_RegionEntityUpdated : GameEvent
    {
        public Alternative_RegionEntityUpdated(Alternative_TIRegionEntityState facility)
        {
            this.facility = facility;
        }
        public Alternative_TIRegionEntityState facility;
    }

    //

    public abstract class patch_TIGameState : TIGameState
    {
        public virtual ConduitFacilityState ref_EtruscanFacility => null;
        public virtual TIRegionEtruscanEntityState ref_regionEtruscanEntity => null;
        public virtual TIRegionEtruscanAssetState ref_regionEtruscanAsset => null;
        public virtual bool isRegionEtruscanEntity => false;
        public virtual bool isRegionEtruscanFacility => false;
        public virtual bool isRegionEtruscanAsset => false;
    }

   

























    

}