using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FullSerializer;
using Unity.Entities;
using System.Security.AccessControl;
using Mono.Cecil;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Entities;
using UnityEngine.EventSystems;
using MonoMod;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PavonisInteractive.TerraInvicta
{
    //Decativating 3d models
    public class patch_MarkerController : MarkerController, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public extern void orig_Initialize(MarkerType mType, TIGameState location);
        public void Initialize(MarkerType mType, TIGameState location)
        {
            orig_Initialize(mType, location);

            switch (mType)
            {
                //case MarkerType.Army:
                case MarkerType.HumanLaserFacility:
                case MarkerType.HumanMissionControlFacility:
                case MarkerType.HumanLaunchFacility:
                case MarkerType.RegionalStatusIcon:
                    hasModel = false;
                    break;
            }
        }
    }

        public class patch_TIMissionPhaseState : TIMissionPhaseState
    {
        //Run checks at start of missionphase
        private void StartofTurnBookkeeping()
        {
                foreach (TINationState tinationState in GameStateManager.AllExtantNations())
            {
                tinationState.UpdateControlPointStatus();
                tinationState.UpdateNativeControlPointsCount();
                tinationState.UpdateArmiesControllingFactions();
                tinationState.ClearAdvisingCouncilors();
            }
            foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
            {
                tihabState.UpdateDefendHabStatus();
                tihabState.ClearAdvisingCouncilors();
            }
            foreach (patch_TIFactionState tifactionState in GameStateManager.AllFactions())
            {

                if (TIEffectsState.SumEffectsModifiers((Context)patch_Context.InfluenceIncomeModifier, tifactionState, 0f) == 0)
                {
                    if (tifactionState.ideology.dataName == "cooperate")
                    {
                        //tifactionState.SetCouncilTimer(TITimeState.Now());
                        //Log.Debug($"HELLO ! {tifactionState.CouncilTimer}");

                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_Cooperatestart", this, null);
                        foreach (TIFactionState enemyCouncil in GameStateManager.AllFactions())
                        {
                            if (enemyCouncil.ideology.dataName == "destroy" || (enemyCouncil.ideology.dataName == "resist"))
                                {
                                tifactionState.GainFactionHate(enemyCouncil, -100f, false);
                                enemyCouncil.GainFactionHate(tifactionState, -100f, false);
                            }
                        }
                    }
                    if (tifactionState.ideology.dataName == "destroy")
                    {
                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_Destroystart", this, null);
                        foreach (TIFactionState enemyCouncil in GameStateManager.AllFactions())
                        {
                            if (enemyCouncil.ideology.dataName == "cooperate" || (enemyCouncil.ideology.dataName == "resist"))
                            {
                                tifactionState.GainFactionHate(enemyCouncil, -100f, false);
                                enemyCouncil.GainFactionHate(tifactionState, -100f, false);
                            }
                        }
                    }
                    if (tifactionState.ideology.dataName == "resist")
                    {
                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_Resiststart", this, null);
                        foreach (TIFactionState enemyCouncil in GameStateManager.AllFactions())
                        {
                            if (enemyCouncil.ideology.dataName == "destroy" || (enemyCouncil.ideology.dataName == "cooperate"))
                            {
                                tifactionState.GainFactionHate(enemyCouncil, -100f, false);
                                enemyCouncil.GainFactionHate(tifactionState, -100f, false);
                            }
                        }
                    }
                    if (tifactionState.ideology.dataName == "exploit")
                    {
                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_Exploitstart", this, null);
                    }
                    if (tifactionState.ideology.dataName == "submit")
                    {

                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_SubmitStart", this, null);
                    }
                    if (tifactionState.ideology.dataName == "appease")
                    {
                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_Appeasestart", this, null);
                    }
                    if (tifactionState.ideology.dataName == "escape")
                    {
   
                        TIEffectsState.ProcessInstantEffect(tifactionState, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.TriggerNarrativeEvent_StrValue, 0f, 0f, "event_Escapestart", this, null);
                    }
                    if (tifactionState.ideology.dataName == "alien")//without this start aliens make 0?
                    {
                        }
                }

                tifactionState.ActivateCouncilorOrgs();
                foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
                {
                    ticouncilorState.RecordLocation();
                    ticouncilorState.EndProtectionOfTarget();
                }
            }
            TIFactionState[] array = GameStateManager.AllFactions();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (patch_TICouncilorState ticouncilorState2 in from x in array[i].councilors
                                                               orderby x.SumMissionRelevantAttributes() descending
                                                               select x)
                {
                    if (ticouncilorState2.repeatOrder)
                    {
                        if (ticouncilorState2.CanRepeatMission(ticouncilorState2.completedMission))
                        {
                            ticouncilorState2.faction.playerControl.StartAction(new AssignCouncilorToMission(ticouncilorState2, ticouncilorState2.completedMission.missionTemplate, ticouncilorState2.completedMission.target, ticouncilorState2.completedMission.resources, false));
                        }
                        else
                        {
                            ticouncilorState2.SetPermanentAssignment(false);
                        }
                    }
                    else if (ticouncilorState2.permanentDefenseMode)
                    {
                        ticouncilorState2.SelectPermanentDefenseModeMission();
                    }
                    bool flag = false;
                    TIMissionState completedMission = ticouncilorState2.completedMission;

                    if (completedMission != null && completedMission.missionTemplate.dataName == "Advise_Statesman")
                    {
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Administration, -ticouncilorState2.Adminholder);
                    }
                    if (completedMission != null && completedMission.missionTemplate.dataName == "Advise_Scientist")
                    {
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Science, -ticouncilorState2.ScienceHolder);
                    }

                    if (completedMission != null && completedMission.missionTemplate.dataName == "LivingComputing")
                    {
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Security, -25);
                    }

                    if (completedMission != null && completedMission.missionTemplate.dataName == "Advise_Super")
                    {

                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Administration, -ticouncilorState2.Adminholder);
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Science, -ticouncilorState2.ScienceHolder);
                    }

                    if (completedMission != null && completedMission.missionTemplate.persistentEffect)
                    {
                        flag = true;
                    }
                    ticouncilorState2.ClearCompletedMission();
                    ticouncilorState2.SetCompletedMission(null);
                    if (flag)
                    {
                        GameControl.eventManager.TriggerEvent(new CouncilorMissionUpdated(ticouncilorState2, null), null, new object[]
                        {
                            this,
                            ticouncilorState2.faction,
                            ticouncilorState2.location,
                            ticouncilorState2.ref_nation
                        }.Where((object x) => x != null).ToArray<object>());
                    }
                }
            }
            GameStateManager.NotificationQueue().CleanSummaryQueue(false);
        }
    }










    
    public class patch_TINationState : TINationState
    {
        //Impacts of Harmony on nation
        public float ControlPointPriorityBonuses(TIControlPoint controlPoint, PriorityType priority)
        {
            TIFactionState faction = controlPoint.faction;
            return ((faction != null) ? faction.cachedPriorityBonuses[priority] : 0f) + controlPoint.diversityBonus[priority] + this.NationalPriorityBonuses(priority) + this.HarmonyPriorityBonus(priority);
        }

        public float HarmonyPriorityBonus(PriorityType priority)
        {
            if (priority == PriorityType.Military_BuildArmy || priority == PriorityType.Military || priority == PriorityType.Military_BuildNavy)
            {
                return ((float)((-this.sustainability + 5) * 0.05));
            }
            if (priority == PriorityType.Oppression)
            {
                return ((float)((-TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm + 50) * 0.025));
            }
            if (priority == PriorityType.Welfare)
            {
                return ((float)((TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm - 50) * 0.025));
            }
            return 0f;
        }
        public float HarmonyImpactOnCohesion
        {
            get
            {
                return ((float)((this.sustainability - 5) * 0.5));
            }
        }
        public float cohesionRestState
        {
            get
            {
                if (this.extant)
                {
                    float num = 16f + this.inequalityImpactOnCohesion + this.perCapitaGDPImpactOnCohesion + this.populationImpactOnCohesion + this.regionsImpactOnCohesion + this.rivalsImpactOnCohesion + this.warsImpactOnCohesion + this.publicEliteDivideImpactOnCohesion + this.publicOpinionImpactOnCohesion + this.autocracyImpactOnCohesion + this.anocracyImpactOnCohesion + HarmonyImpactOnCohesion;
                    num += this.DemocracyImpactOnCohesion(num);
                    return Mathf.Clamp(num, 0f, 10f);
                }
                return this.cohesion;
            }
        }
        //Nation impact on Harmony and Unity
        public Tuple<double, double, double> GHGsFromEconomy_tons(bool monthly, float proposedSustainabilityChange = 0f)
        {
            double Allies = this.allies.Count;
            Allies -= this.rivals.Count;
            if (monthly)
            {
                Allies /= 12.0;
            }

            double FederationBonus = 0;
            if (this.inFederation)
            {
                FederationBonus = this.federation.members.Count;
            }
            if (monthly)
            {
                FederationBonus /= 12.0;
            }

            double Harmony = (double)this.population / 1000000;
            float CurrentHarmony = TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm / 10;
            Harmony *= (-CurrentHarmony + Mathd.Max(0f, (double)(this.sustainability + proposedSustainabilityChange)));
            Harmony /= 100;
            if (monthly)
            {
                Harmony /= 12.0;
            }
            double item = 0;
            double item2 = Harmony;
            double item3 = FederationBonus + Allies;
            return new Tuple<double, double, double>(item, item2, item3);
        }

        public static double CO2toPPM(double input_tons)
        {
            return input_tons;
        }
        public static double CH4toPPM(double input_tons)
        {
            return input_tons;
        }

        public static double N2OtoPPM(double input_tons)
        {
            return input_tons;
        }

        //Coup Casualty addition
        public void Coup(TICouncilorState councilor = null, int strength = 0)
        {
            List<TIGameState> controlPointOwnersByPoint = this.controlPointOwnersByPoint;
            TIFactionState executiveFaction = this.executiveFaction;
            TIFactionState tifactionState = null;

            int Casualties = UnityEngine.Random.Range(0, 101);
            patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(Casualties, false);///Coup casualties
            
            if (councilor != null)
            {
                tifactionState = (councilor.faction.IsAlienFaction ? GameStateManager.AlienProxy() : councilor.faction);
            }
            List<int> list = this.NewGovernment(ControlPointChangeCause.Coup, tifactionState);
            this.AddToDemocracy(UnityEngine.Random.Range(-2f, 1f), TINationState.DemocracyChangeReason.DemReason_Coup);
            this.AddToUnrest(UnityEngine.Random.Range(-3f, 0f), TINationState.UnrestChangeReason.UnrestReason_Coup, 10f);
            this.AddToCohesion(UnityEngine.Random.Range(-1f, 1f), TINationState.CohesionChangeReason.CohesionReason_Coup);
            this.GDPPctChange(UnityEngine.Random.Range(0f, -0.1f), TINationState.GDPChangeReason.GDPReason_Coup);
            int maxToGrant = (list.Count > 0) ? Mathf.Clamp(list.Max() + 1, 2, this.numControlPoints) : Mathf.Min(2, this.numControlPoints);
            this.GrantControlPointsToUnrestingFactions(maxToGrant, ControlPointChangeCause.Coup);
            if (councilor != null)
            {
                strength = Math.Min(strength, this.numControlPoints);
                int num = 0;
                TIControlPoint ticontrolPoint = this.FirstNativeControlPoint();
                if (ticontrolPoint != null)
                {
                    if (this.NumNativeControlPoints < strength)
                    {
                        num = this.numControlPoints - strength;
                    }
                    else
                    {
                        num = ticontrolPoint.positionInNation;
                    }
                }
                for (int i = 0; i < strength; i++)
                {
                    if (this.controlPoints[num + i].faction != tifactionState)
                    {
                        this.ChangeControlPointOwner(num + i, ControlPointChangeCause.Coup, tifactionState);
                    }
                }
            }
            TIControlPoint ticontrolPoint2 = this.FirstNativeControlPoint();
            if (ticontrolPoint2 != null)
            {
                foreach (TIControlPoint ticontrolPoint3 in this.controlPoints)
                {
                    if (ticontrolPoint3.positionInNation > ticontrolPoint2.positionInNation && ticontrolPoint3.faction != null)
                    {
                        this.ChangeControlPointOwner(ticontrolPoint3.positionInNation, ControlPointChangeCause.Coup, null);
                    }
                }
            }
            if (councilor != null)
            {
                TINotificationQueueState.LogCoup(this, controlPointOwnersByPoint, tifactionState);
            }
            else
            {
                TINotificationQueueState.LogCoup(this, controlPointOwnersByPoint, null);
            }
            if (executiveFaction != null && this.executiveFaction != null && this.executiveFaction != executiveFaction)
            {
                foreach (TINationState tinationState in new List<TINationState>(this.allies))
                {
                    if (this.CanEndAlliance(tinationState))
                    {
                        this.EndAlliance(null, tinationState);
                        this.improveRelationsCooldowns.Remove(tinationState);
                        tinationState.improveRelationsCooldowns.Remove(this);
                    }
                }
            }
            TIFactionState[] array = GameStateManager.AllFactions();
            for (int j = 0; j < array.Length; j++)
            {
                foreach (TICouncilorState ticouncilorState in array[j].activeCouncilors)
                {
                    if (councilor != ticouncilorState && ticouncilorState.HasMission && ticouncilorState.activeMission.target == this && ticouncilorState.activeMission.missionTemplate == TIFactionState.coupMission)
                    {
                        ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.NationAlreadyCouped, "");
                    }
                }
            }
            this.factionUnrestAttempts.Clear();
        }
        [SerializeField]
        private Dictionary<TIFactionState, int> factionUnrestAttempts = new Dictionary<TIFactionState, int>();


        //Deactivating base code
        private float BestCurrentSustainabilityValue()//always 10
        {
            TIGlobalValuesState globalValues = TIGlobalValuesState.GlobalValues;
            float num = (globalValues != null) ? globalValues.initialSustainabilityMin : 0f;
            float num2 = TIEffectsState.SumEffectsModifiers(Context.Environment_BestSustainabilityValue, this, num);
            return 0;
        }

        public static float MeanAnnualGDPDamage(float tempAnomaly_C, float inequality)
        {
            //float num = 0f;
            //if (tempAnomaly_C > 0f)
            //{
            //    float num2 = tempAnomaly_C;
            //    num = 0.001f * num2;
            //    num *= Mathf.Pow(1.14f, inequality);
            //    //if (num2 >= 5f)
            //    //{
            //    //    float num3 = Mathf.Clamp((num2 + inequality) / 10f, 1f, 1.5f);
            //    //    num *= num3;
            //    //}
            //    //num /= 100f;
            //    num *= -1f;
            //}
            //else if (tempAnomaly_C < 0f)
            //{
            //    float num4 = Mathf.Abs(tempAnomaly_C);
            //    num = num4 * -0.04032f;
            //    if (tempAnomaly_C < -7f)
            //    {
            //        num += (num4 - 7f) * -0.04032f;
            //        if ((double)tempAnomaly_C < -10.5)
            //        {
            //            num += (num4 - 10.5f) * -0.04032f * 10f;
            //        }
            //    }
            //}
            //return Mathf.Clamp(num, -0.99f, 0f);
            return 0;//Disabled
        }
        public void OnWelfarePriorityComplete()
        {
            this.AddToInequality(this.welfarePriorityInequalityChange, TINationState.InequalityChangeReason.InqReason_WelfarePriority);
            if (this.canAccumulateDecolonizeTriggers)
            {
                this.accumulatedDecolonizeTriggers++;
                if (this.accumulatedDecolonizeTriggers >= 1000 && this.CandidateDecolonizeRegions().Count > 0)
                {
                    this.OnDecolonizeRegionPriorityComplete();
                    this.accumulatedDecolonizeTriggers = 0;
                }
            }
            this.AddToSustainability(this.spoilsSustainabilityChange * -1);
        }
        public void OnFoundMilitaryPriorityComplete()
        {
            //TIFactionState controlPointTypeOwner = this.GetControlPointTypeOwner(ControlPointType.Aristocracy);
            //TIFactionState controlPointTypeOwner2 = this.GetControlPointTypeOwner(ControlPointType.ExtractiveSector);
            float control = 0;

            foreach (patch_TIControlPoint ticontrolPoint in this.controlPoints)
            {
                control += 1;
            }
            foreach (patch_TIControlPoint ticontrolPoint in this.controlPoints)
            {
                   float num = 0f + (float)this.currentMagicRegions * (patch_TIGlobalValuesState.GlobalValues.globalSeaLevelAnomaly_cm /100);
                // Log.Debug($"control1 {control}");
                if (ticontrolPoint.faction != null && !ticontrolPoint.benefitsDisabled)
                {
                    // Log.Debug($"Num1 {num}");
                    // Log.Debug($"control2 {control}");
                    num /= control;
                    // Log.Debug($"Num2 {num}");
                    ticontrolPoint.faction.AddToCurrentResource(num, patch_FactionResource.Magic, false);
                    //ticontrolPoint.faction.thisWeeksCumulativeSpoils += num;
                }
            }
            //this.AddToSustainability(this.spoilsSustainabilityChange * this.currentMagicRegions);
            float num2 = -0.010f * this.currentMagicRegions;
            TIGlobalValuesState.GlobalValues.AddCO2_ppm(num2, GHGSources.SpoilsPriority);
            //TIGlobalValuesState.GlobalValues.AddMagicPriorityEnvEffect(this, this.priorityEffectPopScaling * this.sustainability);
        }
        public void OnEnvironmentPriorityComplete()
        {
            if (this.Harmony == false)
            {
                this.AddToSustainability((this.environmentPrioritySustainabilityChange) * 10);
            }
            else
            {
                this.AddToSustainability(this.environmentPrioritySustainabilityChange * -10);
            }
            if (this.canAccumulateDecontaminateTriggers)
            {
                this.accmulatedDecontaminateTriggers++;
                if (this.accmulatedDecontaminateTriggers > 100 && this.CandidateDecontaminateRegions().Count > 0)
                {
                    this.OnDecontaminateRegionPriorityComplete();
                    this.accmulatedDecontaminateTriggers = 0;
                }
            }
        }

        public float environmentPrioritySustainabilityChange
        {
            get
            {
                float num = this.priorityEffectPopScaling * (TemplateManager.global.environmentPrioritySustainabilityChange + TIEffectsState.SumEffectsModifiers(Context.Environment_SustainabilityChange, this, TemplateManager.global.environmentPrioritySustainabilityChange));
                float sustainability = this.sustainability;
                //int num3 = this.regions.Sum((TIRegionState x) => x.nuclearDetonations);
                //if (num3 > 0)
                //{
                //    num /= (float)num3;
                //}
                return num;
            }
        }

        public float spoilsSustainabilityChange
        {
            get
            {
                return (this.priorityEffectPopScaling * (TemplateManager.global.spoilsPrioritySustainabilityChange)) * -100;
            }
        }



        public string SustainabilityChangeForDisplay(float proposedChange)
        {
            float num = this.sustainability + proposedChange;
            float num2 = proposedChange;
            return TIUtilities.FormatSmallNumber(num2, 7, 0, true, false);
            if (this.sustainability > 0f)
            {
                if (this.sustainability <= 0.1f)
                {
                    return Loc.T("UI.Nation.ASmallAmount");
                }
                return TIUtilities.FormatSmallNumber(1f / num - 1f / this.sustainability, 7, 0, true, false);
            }
            else
            {
                if (proposedChange != 0f)
                {
                    return TIUtilities.FormatSmallNumber(-num, 7, 0, true, false);
                }
                return 0.ToString("N0");
            }
        }
        //Adds requirment for harness magic and making Megabombs
        public bool ValidPriority(PriorityType priority)
        {
            float Magic = TIEffectsState.SumEffectsModifiers((Context)patch_Context.ExploitMagicPriority, this.executiveControlPoint.faction, 0f);

            patch_TIFactionState Faction = (patch_TIFactionState)this.executiveFaction;

            switch (priority)
            {
                case PriorityType.Economy:
                case PriorityType.Welfare:
                case PriorityType.Knowledge:
                case PriorityType.Unity:
                case PriorityType.Spoils:
                    return true;
                case PriorityType.Environment:
                    return this.sustainability <= 0f || this.sustainability > this.BestCurrentSustainabilityValue();
                case PriorityType.Government:
                    return this.democracy < 10f;
                case PriorityType.Oppression:
                    return this.military;
                case PriorityType.Funding:
                    return this.spaceFunding_year <= this.maxFunding_year;
                case PriorityType.Civilian_InitiateSpaceflightProgram:
                    return !this.spaceFlightProgram;
                case PriorityType.LaunchFacilities:
                    return this.spaceFlightProgram;
                case PriorityType.MissionControl:
                    if (!this.spaceFlightProgram)
                    {
                        TIFederationState tifederationState = this.federation;
                        if (tifederationState == null || !tifederationState.spaceProgram)
                        {
                            return false;
                        }
                    }
                    return this.regions.Any((TIRegionState x) => x.missionControl < x.maxMissionControl);
                case PriorityType.Military_FoundMilitary:
                    return this.currentMagicRegions > 0 && Magic > 0; 
                case PriorityType.Military:
                    return this.military && this.militaryTechLevel < this.maxMilitaryTechLevel;
                case PriorityType.Military_BuildArmy:
                    return this.canBuildArmy;
                case PriorityType.Military_BuildNavy:
                    return this.canBuildNavy;
                case PriorityType.Military_InitiateNuclearProgram:
                    if (!this.executiveControlPoint.owned || Faction.InternationalTreatyType == 1)
                    {
                        return false;
                    }
                    return this.military && !this.nuclearProgram && !this.policy_noNukes && this.currentMagicRegions > 0;
                case PriorityType.Military_BuildNuclearWeapons:
                    if (!this.executiveControlPoint.owned || Faction.InternationalTreatyType == 1)
                    {
                        return false;
                    }
                        return this.nuclearProgram && !this.policy_noNukes && this.currentMagicRegions > 0;
                case PriorityType.Military_BuildSpaceDefenses:
                    return this.military && this.canBuildSpaceDefenses && !this.completeAntiSpaceDefenses;
                case PriorityType.Military_BuildSTOSquadron:
                    if (this.military && this.canBuildSTOSquadrons && this.boostPerYear_dekatons > 0f)
                    {
                        return this.regions.Any((TIRegionState x) => x.numSTOFighters < x.maxSTOFighters);
                    }
                    return false;
                default:
                    return false;
            }
        }
		private void OnInitiateNuclearProgramComplete()//Remove Atrocity.
		{
			this.nuclearProgram = true;
			TIGlobalValuesState.GlobalValues.TriggerNuclearDetonationEffect(false, null, null, null);
			this.ChangeNumNuclearWeapons(1);
			TINotificationQueueState.LogNationGainsNukes(this);
			if (this.executiveFaction != null && this.executiveFaction.isActivePlayer)
			{
				this.executiveFaction.UnlockAchievement("completeNukeProgram");
			}
			TIGlobalValuesState.GlobalValues.ModifyMarketValuesForNuclearWeaponsPriority();
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				int controlPointPriority = ticontrolPoint.GetControlPointPriority(PriorityType.Military_InitiateNuclearProgram, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.Military_InitiateNuclearProgram, 0, true, true);
				ticontrolPoint.SetControlPointPriority(PriorityType.Military_BuildNuclearWeapons, controlPointPriority, false, false);
			}
		}

        public int currentMagicRegions //would be better if it used its own instead of oilresource
		{
			get
			{
				return this.regions.Count((TIRegionState region) => region.template.oilResource && !region.IsFullyOccupied());
			}
		}

        //Showing Sustainbility/Harmony as exact number.
        public static string SustainabilityValueForDisplay(float sustainability, int extendValue = 2)
        {

            return TIUtilities.FormatSmallNumber(Mathf.Max(0,sustainability), 7, extendValue, true, false);
        }

    

        //CHanges to Funding and Spoils Calculation:
        public float get_spaceFundingPriorityIncomeChange()
        {
            return ((TemplateManager.global.fundingPriorityBaseIncomeIncrease * this.BaseInvestmentPoints_month() + this.numCoreEconomicRegions_dailyCache * 10) * (0.5f + (this.sustainability/5)));
        }

        public float spoilsPriorityMoney
        {
            get
            {
                return (TemplateManager.global.spoilsPriorityMoneyPerInvestmentPoint * this.BaseInvestmentPoints_month() + TemplateManager.global.spoilsPriorityMoneyPerResourceRegion * (float)this.currentResourceRegions) * (2.5f - (this.sustainability/5));
            }
        }
        //Making Oil regions not conflict with the region evolution:
      public List<TIRegionState> CandidateCoreMiningRegions()
		{
			return (from x in this.regions
			where !x.coreEconomicRegion && !x.resourceRegion && x.nuclearDetonations == 0 && x.template.mineCapable
			select x).ToList<TIRegionState>();
		}


        //Adding Harmony to template
        public extern void orig_InitWithTemplate(TIDataTemplate template);
        public override void InitWithTemplate(TIDataTemplate template)
        {
            orig_InitWithTemplate(template);
            patch_TINationTemplate tinationTemplate = template as patch_TINationTemplate;
            this.Harmony = tinationTemplate.Harmony.GetValueOrDefault();
        }













        //Dependencies and new variables.
        public float sustainability { get; private set; }
        public int accumulatedDecolonizeTriggers { get; private set; }
        public int accmulatedDecontaminateTriggers { get; private set; }
        public bool Harmony;
        public bool nuclearProgram { get; private set; }
        private float boostPerYear_dekatons
        {
            get
            {
                return this.regions.Sum((TIRegionState region) => region.boostPerYear_dekatons);
            }
        }



        //Change what image to use for Sustainbility(HARMONY)
        public string SustainabilityIcon()
        {
            float sustainability = this.sustainability;
            if (sustainability <= 1.5f)
            {
                return "icons_2d/ICO_GHG_emission_1";
            }
            if (sustainability < 4.0f)
            {
                return "icons_2d/ICO_GHG_emission_2";
            }
            if (sustainability >= 4.0f && sustainability <= 6.0f)
            {
                return "icons_2d/ICO_GHG_emission_3";
            }
            if (sustainability <= 8.5f)
            {
                return "icons_2d/ICO_GHG_emission_4";
            }
            return "icons_2d/ICO_GHG_emission_5";
        }

        public string SustainabilityIconInlinePath()
        {
            float sustainability = this.sustainability;
            if (sustainability <= 0f)
            {
                return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Red;
            }
            if (sustainability < 0.33333334f)
            {
                return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Orange;
            }
            if (sustainability < 0.6666667f)
            {
                return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Yellow;
            }
            if (sustainability <= 1f)
            {
                return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Blue;
            }
            return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Green;
        }
    }


    


    public class patch_NationInfoController : NationInfoController
    {
        public extern void orig_Initialize();
        public override void Initialize()//Want to change LOC based on flip but its locked behind broken decompiled code.
        {
            orig_Initialize();
            //base.Initialize();
        }

        private static string requiredIPSummaryText(TINationState nation, PriorityType priority)
        {
            return Loc.T("UI.Nation.RequiredInvestmentPoints", new object[]
            {
                TIUtilities.FormatSmallNumber(nation.GetRequiredInvestmentPointsForPriority(priority), 1, 0, true, false)
            });
        }
        private static string ChangeString_Sustainability(TINationState nation, float changeValue, bool useColor)
        {
            if (useColor)
            {
                if (changeValue < 0f)
                {
                    return Loc.T("UI.Nation.RecentChange", new object[]
                    {
                        Loc.T("UI.Nation.Sustainability"),
                        TIUtilities.GreenLine(nation.SustainabilityChangeForDisplay(changeValue))
                    });
                }
                if (changeValue > 0f)
                {
                    return Loc.T("UI.Nation.RecentChange", new object[]
                    {
                        Loc.T("UI.Nation.Sustainability"),
                        TIUtilities.RedLine(nation.SustainabilityChangeForDisplay(changeValue))
                    });
                }
            }
            return Loc.T("UI.Nation.RecentChange", new object[]
            {
                Loc.T("UI.Nation.Sustainability"),
                nation.SustainabilityChangeForDisplay(changeValue)
            });
        }
		public static string BuildSustainabilityTooltip(TINationState nation)
		{
			Tuple<double, double, double> tuple = nation.GHGsFromEconomy_tons(false, 0f);
			return new StringBuilder(nation.SustainabilityIconInlinePath()).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Sustainability"),
				TINationState.SustainabilityValueForDisplay(nation.sustainability)
			})).AppendLine().AppendLine(patch_NationInfoController.ChangeString_Sustainability(nation, nation.sustainability - nation.historySustainability[31], true)).AppendLine().AppendLine(Loc.T("UI.Nation.SustainabilityHelp2", new object[]
			{
				nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange),
                patch_NationInfoController.requiredIPSummaryText(nation, PriorityType.Environment),
				nation.BestCurrentSustainabilityValueForDisplay(),
				nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange),
                patch_NationInfoController.requiredIPSummaryText(nation, PriorityType.Spoils)
			})).AppendLine().AppendLine(Loc.T("UI.Nation.SustainabilityHelp3", new object[]
			{
				TIUtilities.FormatBigNumber(tuple.Item1, 2, false),
				TIUtilities.FormatSmallNumber(TINationState.CO2toPPM(tuple.Item1), 7, 0, true, false),
				TIUtilities.FormatBigNumber(tuple.Item2, 2, false),
				TIUtilities.FormatSmallNumber(TINationState.CH4toPPM(tuple.Item2), 7, 0, true, false),
				TIUtilities.FormatBigNumber(tuple.Item3, 2, false),
				TIUtilities.FormatSmallNumber(TINationState.N2OtoPPM(tuple.Item3), 7, 0, true, false)
			})).ToString();
		}

        public static string PrioritySummaryString(PriorityType priority, patch_TINationState nation, bool includeIPSymbol = true)
        {
            TIGlobalConfig global = TemplateManager.global;
            StringBuilder stringBuilder = new StringBuilder();
            if (includeIPSymbol)
            {
                stringBuilder.Append(global.investmentInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.GetRequiredInvestmentPointsForPriority(priority), 7, 0, true, false)).Append(": ");
            }
            switch (priority)
            {
                case PriorityType.Economy:
                    stringBuilder.Append(global.perCapitaGDPInlineSpritePath).Append(nation.economyPriorityPerCapitaIncomeChange.ToString("N2")).Append(" ").Append(global.inequalityInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.economyPriorityInequalityChange, 7, 0, true, false));
                    break;
                case PriorityType.Welfare:
                    stringBuilder.Append(global.inequalityInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.welfarePriorityInequalityChange, 7, 1, true, false)).Append(global.sustainabilityInlineSpritePath_Green).Append(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange * -1));
                    break;
                case PriorityType.Environment:
                    stringBuilder.Append(nation.SustainabilityIconInlinePath());
                    if (nation.sustainability <= 0f)
                    {
                        stringBuilder.Append(Loc.T("UI.Nation.WelfareGHGReductionShort", new object[]
                        {
                        nation.EnvPriorityCO2Removed(),
                        nation.EnvPriorityCH4Removed(),
                        nation.EnvPriorityN2ORemoved()
                        }));
                    }
                    else if (nation.Harmony == false)
                    {
                        stringBuilder.Append(nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange * 10));
                    }
                    else
                    {
                        stringBuilder.Append(nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange * -10));
                    }
                    break;
                case PriorityType.Knowledge:
                    stringBuilder.Append(global.educationInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.knowledgePriorityEducationChange, 7, 1, true, false)).Append(" ").Append(global.cohesionInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false));
                    break;
                case PriorityType.Government:
                    stringBuilder.Append(global.democracyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.governmentPriorityDemocracyChange, 7, 1, true, false));
                    break;
                case PriorityType.Unity:
                    stringBuilder.Append(global.cohesionInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false)).Append(" ").Append(global.educationInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.unityPriorityEducationChange, 7, 1, true, false));
                    break;
                case PriorityType.Oppression:
                    stringBuilder.Append(global.unrestInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.OppressionPriorityUnrestChange, 7, 1, true, false)).Append(" ").Append(global.democracyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.OppressionPriorityDemocracyChange, 7, 1, true, false));
                    if (nation.OppressionPriorityCohesionChange != 0f)
                    {
                        stringBuilder.Append(" ").Append(global.cohesionInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.OppressionPriorityCohesionChange, 7, 1, true, false));
                    }
                    break;
                case PriorityType.Funding:
                    stringBuilder.Append(global.moneyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.spaceFundingPriorityIncomeChange, 7, 0, true, false)).Append(Loc.T("UI.Nation.Yearly"));
                    break;
                case PriorityType.Spoils:
                    stringBuilder.Append(global.moneyInlineSpritePath).Append(nation.spoilsPriorityMoney.ToString("N1")).Append(" ").Append(global.inequalityInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.spoilsPriorityInequalityChange, 7, 1, true, false)).Append(" ").Append(global.democracyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.spoilsPriorityDemocracyChange, 7, 1, true, false)).Append(" ").Append(global.sustainabilityInlineSpritePath_Red).Append(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange));
                    break;
                case PriorityType.Civilian_InitiateSpaceflightProgram:
                    stringBuilder.Append(global.boostInlineSpritePath).Append(nation.spaceflightInitialBoost);
                    break;
                case PriorityType.LaunchFacilities:
                    {
                        float num = nation.BoostGainLow();
                        float num2 = nation.BoostGainHigh();
                        if (num != num2)
                        {
                            stringBuilder.Append(global.boostInlineSpritePath).Append(Loc.T("UI.Nation.BoostRange", new object[]
                            {
                        TIUtilities.FormatSmallNumber(num, 7, 0, true, false),
                        TIUtilities.FormatSmallNumber(num2, 7, 0, true, false)
                            })).Append(Loc.T("UI.Nation.Yearly"));
                        }
                        else
                        {
                            stringBuilder.Append(global.boostInlineSpritePath).Append(TIUtilities.FormatSmallNumber(num, 7, 0, true, false)).Append(Loc.T("UI.Nation.Yearly"));
                        }
                        break;
                    }
                case PriorityType.MissionControl:
                    stringBuilder.Append(global.missionControlInlineSpritePath).Append("1");
                    break;
                case PriorityType.Military_FoundMilitary:
                    stringBuilder.Append(" ").Append(global.sustainabilityInlineSpritePath_Red).Append(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange));
                    break;
                case PriorityType.Military:
                    stringBuilder.Append(global.miltechInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.militaryPriorityTechLevelChange, 7, 1, true, false));
                    break;
                case PriorityType.Military_BuildArmy:
                    {
                        StringBuilder stringBuilder2 = stringBuilder;
                        string key = "UI.Nation.NationalStatTooltipHeader";
                        object[] array = new object[2];
                        array[0] = global.armyInlineSpritePath;
                        int num3 = 1;
                        TIRegionState nextArmyRegion = nation.GetNextArmyRegion();
                        array[num3] = (((nextArmyRegion != null) ? nextArmyRegion.displayName : null) ?? "Error");
                        stringBuilder2.Append(Loc.T(key, array));
                        break;
                    }
                case PriorityType.Military_BuildNavy:
                    {
                        StringBuilder stringBuilder3 = stringBuilder;
                        string key2 = "UI.Nation.NationalStatTooltipHeader";
                        object[] array2 = new object[2];
                        array2[0] = global.navyInlineSpritePath;
                        int num4 = 1;
                        TIArmyState nextNavy = nation.GetNextNavy();
                        array2[num4] = (((nextNavy != null) ? nextNavy.displayName : null) ?? "Error");
                        stringBuilder3.Append(Loc.T(key2, array2));
                        break;
                    }
                case PriorityType.Military_InitiateNuclearProgram:
                    stringBuilder.Append(global.nukesInlineSpritePath).Append("1");
                    break;
                case PriorityType.Military_BuildNuclearWeapons:
                    stringBuilder.Append(global.nukesInlineSpritePath).Append("1");
                    break;
                case PriorityType.Military_BuildSpaceDefenses:
                    {
                        StringBuilder stringBuilder4 = stringBuilder;
                        string key3 = "UI.Nation.NationalStatTooltipHeader";
                        object[] array3 = new object[2];
                        array3[0] = global.antiSpaceDefensesInlineSpritePath;
                        int num5 = 1;
                        TIRegionState nextSpaceDefensesRegion = nation.GetNextSpaceDefensesRegion();
                        array3[num5] = (((nextSpaceDefensesRegion != null) ? nextSpaceDefensesRegion.displayName : null) ?? "Error");
                        stringBuilder4.Append(Loc.T(key3, array3));
                        break;
                    }
                case PriorityType.Military_BuildSTOSquadron:
                    {
                        StringBuilder stringBuilder5 = stringBuilder;
                        string key4 = "UI.Nation.NationalStatTooltipHeader";
                        object[] array4 = new object[2];
                        array4[0] = global.STO_InlineSpritePath;
                        int num6 = 1;
                        TILaunchFacilityState nextSTOSquadronLocation = nation.GetNextSTOSquadronLocation();
                        array4[num6] = (((nextSTOSquadronLocation != null) ? nextSTOSquadronLocation.displayName : null) ?? "Error");
                        stringBuilder5.Append(Loc.T(key4, array4));
                        break;
                    }
            }
            return stringBuilder.ToString();
        }




        //    public static string BuildCohesionTooltip(TINationState nation)//figure out
        //{
        //    return new StringBuilder(TIGlobalConfig.globalConfig.cohesionInlineSpritePath).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
        //    {
        //        Loc.T("UI.Nation.Cohesion"),
        //        nation.GetCohesionDescriptiveStringAndValue(3)
        //    })).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Cohesion"), nation.cohesion - nation.historyCohesion[31], true, NationInfoController.WhatIsGood.upOrMiddleIsGood, false, false, nation.cohesion)).AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.CohesionRestState"), nation.cohesionRestState - nation.historyCohesionRestState[31], true, NationInfoController.WhatIsGood.upOrMiddleIsGood, false, false, nation.cohesionRestState)).AppendLine(NationInfoController.RestStateString(Loc.T("UI.Nation.Cohesion"), nation.cohesion, nation.cohesionRestState, Mathf.Abs(nation.GetMonthlyCohesionMovement()))).AppendLine().AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.CohesionHelp1Short") : string.Empty).AppendLine().AppendLine(Loc.T("UI.Nation.CohesionHelp2", new object[]
        //    {
        //        TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false),
        //        NationInfoController.requiredIPSummaryText(nation, PriorityType.Unity),
        //        TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false),
        //        NationInfoController.requiredIPSummaryText(nation, PriorityType.Knowledge)
        //    })).AppendLine().AppendLine(nation.CohesionRestStateDetail).ToString();
        //}


        //add tooltip for harmony influence on priorities. For Harmony on Spoils/Funding













        public static string BuildRegionDataTooltip(TIRegionState region, TIFactionState faction)
        {
            StringBuilder stringBuilder = new StringBuilder(region.displayName).AppendLine();
            patch_TIRegionState region_VLC = region as patch_TIRegionState;
            if (region.isBeingAnnexed)
            {
                stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.IsBeingAnnexed", new object[]
                {
                    region.annexingArmy.homeNation.displayNameWithArticleCapitalized,
                    region.annexationEndDate.ToCustomDateString()
                }))).AppendLine();
            }
            else if (region.IsFullyOccupied())
            {
                stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.IsOccupied"))).AppendLine();
            }
            else if (region.OccupationUnderwayButNotComplete())
            {
                stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.IsBeingOccupied"))).AppendLine();
            }
            if (region.nation.capital == region)
            {
                stringBuilder.Append(TemplateManager.global.capitalRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionCapital", new object[]
                {
                    region.nation.displayNameWithArticle
                })).AppendLine().AppendLine();
            }
            if (region.coreEconomicRegion)
            {
                stringBuilder.Append(TemplateManager.global.coreEconomicRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionCoreEco")).AppendLine().AppendLine();
            }
            if (region_VLC.MagicResource)
            {
                stringBuilder.Append(patch_TemplateManager.global_PVC.MagicResourceInlineSpritePath).Append(Loc.T("UI.Nation.RegionMagicResource")).AppendLine().AppendLine();//Notes magic region
            }
            if (region_VLC.TeleportRegion)
            {
                stringBuilder.Append(patch_TemplateManager.global_PVC.TeleportRegionSpritePath).Append(Loc.T("UI.Nation.RegionTeleporter")).AppendLine().AppendLine();//notes Teleport Region
            }
            if (region.resourceRegion)
            {
                stringBuilder.Append(TemplateManager.global.miningRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionResource")).AppendLine().AppendLine();
            }
            if (region.colonyRegion)
            {
                stringBuilder.Append(TemplateManager.global.colonyRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionColony")).AppendLine().AppendLine();
            }
            if (region.template.environment == EnvironmentType.Vulnerable)
            {
                stringBuilder.Append(TemplateManager.global.ecologicallyVulnerableRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionEcoVulnerable")).AppendLine().AppendLine();
            }
            else if (region.template.environment == EnvironmentType.Beneficiary)
            {
                stringBuilder.Append(TemplateManager.global.ecologicallySafeRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionEcoBeneficiary")).AppendLine().AppendLine();
            }
            if (region.terrain == TerrainType.Rugged)
            {
                stringBuilder.Append(TemplateManager.global.ruggedRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionRugged")).AppendLine().AppendLine();
            }
            if (region.nuclearDetonations > 0)
            {
                stringBuilder.Append(TemplateManager.global.nukedRegionInlineSpritePath).Append(Loc.T("UI.Nation.NukedRegion", new object[]
                {
                    region.nuclearDetonations.ToString()
                })).AppendLine().AppendLine();
            }
            if (region.antiSpaceDefenses)
            {
                stringBuilder.Append(TemplateManager.global.antiSpaceDefensesInlineSpritePath).Append(Loc.T("UI.Nation.SpaceDefenses")).AppendLine().AppendLine();
            }
            if (faction.KnownAlienEntities.Any((TIRegionAlienEntityState x) => x.region == region))
            {
                stringBuilder.Append(TemplateManager.global.alienEntityInlineSpritePath).Append(Loc.T("UI.Nation.AlienEntityPresent")).AppendLine().AppendLine();
            }
            List<TIRegionState> list = region.AdjacentRegions(true);
            List<TIRegionState> list2 = region.AdjacentRegions(false).Except(list).ToList<TIRegionState>();
            List<TIBilateralTemplate> list3 = (from x in TemplateManager.IterateByClass<TIBilateralTemplate>(true)
                                               where x.relationType == BilateralRelationType.PhysicalAdjacency && x.BilateralIsInScenario() && x.projectUnlock != null && (x.regionState1 == region || x.regionState2 == region) && !GameControl.control.activePlayer.completedProjects.Contains(x.projectUnlock)
                                               select x).ToList<TIBilateralTemplate>();
            if (list.Count > 0)
            {
                StringBuilder stringBuilder2 = stringBuilder;
                string key = "UI.Nation.RegionAdjacencies";
                object[] array = new object[2];
                array[0] = region.displayName;
                array[1] = TIUtilities.ConstructTextList(list.ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState), false, false);
                stringBuilder2.AppendLine(Loc.T(key, array)).AppendLine();
            }
            if (list2.Count > 0)
            {
                StringBuilder stringBuilder3 = stringBuilder;
                string key2 = "UI.Nation.RegionAdjacenciesFriendlyOnly";
                object[] array2 = new object[2];
                array2[0] = region.displayName;
                array2[1] = TIUtilities.ConstructTextList(list2.ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState), false, false);
                stringBuilder3.AppendLine(Loc.T(key2, array2)).AppendLine();
            }
            if (list3.Count > 0)
            {
                List<TIRegionState> list4 = list3.Select(delegate (TIBilateralTemplate x)
                {
                    if (!(x.regionState1 == region))
                    {
                        return x.regionState1;
                    }
                    return x.regionState2;
                }).ToList<TIRegionState>();
                StringBuilder stringBuilder4 = stringBuilder;
                string key3 = "UI.Nation.RegionAdjanciesAddable";
                object[] array3 = new object[1];
                array3[0] = TIUtilities.ConstructTextList(list4.ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState), false, false);
                stringBuilder4.AppendLine(Loc.T(key3, array3)).AppendLine();
            }
            return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
        }
    }
}
