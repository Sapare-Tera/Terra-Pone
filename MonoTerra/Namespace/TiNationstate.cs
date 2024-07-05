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

namespace PavonisInteractive.TerraInvicta
{
    public class patch_MarkerController : MarkerController, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public extern void orig_Initialize(MarkerType mType, TIGameState location);
        public void Initialize(MarkerType mType, TIGameState location)
        {
            orig_Initialize(mType, location);

            switch (mType)
            {
                case MarkerType.Army:
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
            foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
            {
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
                foreach (TICouncilorState ticouncilorState2 in from x in array[i].councilors
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
                        double Attribute = ticouncilorState2.GetAttribute(CouncilorAttribute.Administration);
                        //Log.Debug($"Initial Attribute {Attribute}");
                        Attribute = Attribute * 0.333;
                        //Log.Debug($"The Deductions {Attribute}");
                        Attribute = Math.Round(Attribute);
                        //Log.Debug($"The rounded {Attribute}");
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Administration, (int)-Attribute);
                        //Log.Debug($"Final Attribute {ticouncilorState2.GetAttribute(CouncilorAttribute.Administration)}");
                    }
                    if (completedMission != null && completedMission.missionTemplate.dataName == "Advise_Scientist")
                    {
                        double Attribute = ticouncilorState2.GetAttribute(CouncilorAttribute.Science);
                        Attribute = Attribute * 0.333;
                        Attribute = Math.Round(Attribute);
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Science, -(int)Attribute);
                    }

                    if (completedMission != null && completedMission.missionTemplate.dataName == "LivingComputing")
                    {
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Security, -25);
                    }

                    if (completedMission != null && completedMission.missionTemplate.dataName == "Advise_Super")
                    {
                        double Attribute2 = ticouncilorState2.GetAttribute(CouncilorAttribute.Administration);
                        Attribute2 = Attribute2 * 0.333;
                        Attribute2 = Math.Round(Attribute2);
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Administration, -(int)Attribute2);
                        double Attribute = ticouncilorState2.GetAttribute(CouncilorAttribute.Science);
                        Attribute = Attribute * 0.333;
                        Attribute = Math.Round(Attribute);
                        ticouncilorState2.ModifyAttribute(CouncilorAttribute.Science, -(int)Attribute);
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
        public static float MeanAnnualGDPDamage(float tempAnomaly_C, float inequality)
        {
            float num = 0f;
            if (tempAnomaly_C > 0f)
            {
                float num2 = tempAnomaly_C;
                num = 0.001f * num2;
                num *= Mathf.Pow(1.14f, inequality);
                //if (num2 >= 5f)
                //{
                //    float num3 = Mathf.Clamp((num2 + inequality) / 10f, 1f, 1.5f);
                //    num *= num3;
                //}
                //num /= 100f;
                num *= -1f;
            }
            else if (tempAnomaly_C < 0f)
            {
                float num4 = Mathf.Abs(tempAnomaly_C);
                num = num4 * -0.04032f;
                if (tempAnomaly_C < -7f)
                {
                    num += (num4 - 7f) * -0.04032f;
                    if ((double)tempAnomaly_C < -10.5)
                    {
                        num += (num4 - 10.5f) * -0.04032f * 10f;
                    }
                }
            }
            return Mathf.Clamp(num, -0.99f, 0f);
        }
        //public void ActivateBuildSpaceDefenses()
        //{
        //    if (!this.canBuildSpaceDefenses)
        //    {
        //        this.canBuildSpaceDefenses = true;
        //        foreach (TIControlPoint ticontrolPoint in this.controlPoints)
        //        {
        //            ticontrolPoint.SetControlPointPriority(PriorityType.BuildSpaceDefenses, ticontrolPoint.GetControlPointPriority(PriorityType.BuildSpaceDefenses, true), false, false);
        //        }
        //    }
        //}

        public bool ValidPriority(PriorityType priority)
        {
            switch (priority)
            {
                case PriorityType.Economy:
                case PriorityType.Welfare:
                case PriorityType.Knowledge:
                case PriorityType.Unity:
                case PriorityType.Spoils:
                case PriorityType.SpaceDevelopment:      
                    return true;
                case PriorityType.Military:
                    return this.unrest > 0f || this.militaryTechLevel < this.maxMilitaryTechLevel;
                case PriorityType.SpaceflightProgram:
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
                case PriorityType.BuildArmy:
                    return this.canBuildArmy;
                case PriorityType.UpgradeArmy:
                    return this.canBuildNavy;
                case PriorityType.InitiateNuclearProgram:
                    return  this.canBuildSpaceDefenses && !this.nuclearProgram && currentMagicRegions > 0;
                case PriorityType.BuildNuclearWeapons:
                    return  this.canBuildSpaceDefenses && this.nuclearProgram && currentMagicRegions > 0;
                case PriorityType.BuildSpaceDefenses :
                    return this.canBuildSpaceDefenses && currentMagicRegions > 0;
                default:
                    return false;
            }
        }
        public bool nuclearProgram { get; private set; }
        private void InitiateNuclearProgramComplete()
        {
            this.nuclearProgram = true;
            TIGlobalValuesState.GlobalValues.TriggerNuclearDetonationEffect(false, null, null, null);
            this.ChangeNumNuclearWeapons(1);
            foreach (TIControlPoint ticontrolPoint in this.controlPoints)
            {
                int controlPointPriority = ticontrolPoint.GetControlPointPriority(PriorityType.InitiateNuclearProgram, true);
                ticontrolPoint.SetControlPointPriority(PriorityType.InitiateNuclearProgram, 0, true, true);
                ticontrolPoint.SetControlPointPriority(PriorityType.BuildNuclearWeapons, controlPointPriority, false, false);
            }
            TINotificationQueueState.LogNationGainsNukes(this);
            if (this.executiveFaction != null && this.executiveFaction.isActivePlayer)
            {
                this.executiveFaction.UnlockAchievement("completeNukeProgram");
            }
            TIGlobalValuesState.GlobalValues.ModifyMarketValuesForNuclearWeaponsPriority();
        }

        public int currentMagicRegions //would be better if it used its own instead of oilresource
		{
			get
			{
				return this.regions.Count((TIRegionState region) => region.template.oilResource && !region.IsOccupied());
			}
		}
        public void BuildAntiSpaceDefensesPriorityComplete()
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
                float num = 0f + (float)this.currentMagicRegions;
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
            this.AddToSustainability(this.spoilsSustainabilityChange * this.currentMagicRegions);
            float num2 = -0.0050f * this.currentMagicRegions;
            TIGlobalValuesState.GlobalValues.AddCO2_ppm(num2, GHGSources.SpoilsPriority);
            //TIGlobalValuesState.GlobalValues.AddMagicPriorityEnvEffect(this, this.priorityEffectPopScaling * this.sustainability);
        }

        public Tuple<double, double, double> GHGsFromEconomy_tons(bool monthly, float proposedSustainabilityChange = 0f)
        {
            float num = Mathf.Min(this.perCapitaGDP / 15000f, 1f);
            float num2 = Mathf.Min(this.perCapitaGDP / 7500f, 1f);
            double num3 = this.GDP / 1000000000.0;
            double num4 = num3 * 275000.0;
            double num5 = num3 * 275000.0;
            num5 += (double)this.population * 2.41 * (double)num * (double)num * (double)num2 * (double)num2;
            num5 += Mathd.Min((double)this.oilRegions * 250000000.0, num4 / 10.0);
            num5 *= (double)((this.sustainability *-1f) + proposedSustainabilityChange);
            if (monthly)
            {
                num5 /= 12.0;
            }
            double item = num5 * 0.8230000138282776 * 0.4000000059604645;
            double item2 = num5 * 0.11500000208616257 * 1.0 / 21.0;
            double item3 = num5 * 0.06199999898672104 * 1.0 / 289.0;
            return new Tuple<double, double, double>(item, item2, item3);
        }
        public static double CO2toPPM(double input_tons)
        {
            return input_tons * 0;
        }
        public static double CH4toPPM(double input_tons)
        {
            return input_tons / 2850308000.0;
        }

        public static double N2OtoPPM(double input_tons)
        {
            return input_tons * 0;
        }
        public float get_spaceFundingPriorityIncomeChange()
        {
            return TemplateManager.global.fundingPriorityBaseIncomeIncrease * (float)this.numControlPoints_unclamped + this.currentCoreEconomicRegions * 5;
        }

    }
}
