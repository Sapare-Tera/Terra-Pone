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
using Unity.Entities;
using static UnityEngine.GraphicsBuffer;
using static PavonisInteractive.TerraInvicta.TINationState;

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
public class TIMissionCondition_LowEnoughUnity: TIMissionCondition///This is a placeholder test for now, needs more versions/varity and needs to be implemented for missions.
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
        float test = TIGlobalValuesState.GlobalValues.earthAtmosphericN2O_ppm;
        if (test <= 75)
        {
            result = "_Pass";
        }
        else
        {
            result = "TIMissionCondition_GenericFail";  
            }

        return result;
    }
}
public class TIMissionTarget_OwnControlPoint : MissionTarget<TIControlPoint>
{
    public override TIFactionState GetRelevantFaction(TIGameState target)
    {
        return target.ref_faction;
    }

    public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
    {
        TIControlPoint ref_controlPoint = target.ref_controlPoint;
        List<string> list = new List<string>();
        bool? flag;
        if (ref_controlPoint == null)
        {
            flag = null;
        }
        else
        {
            TINationState nation = ref_controlPoint.nation;
            flag = ((nation != null) ? new bool?(nation.extant) : null);
        }
        bool? flag2 = flag;
        if (flag2.GetValueOrDefault() && !ref_controlPoint.EnemyFactionControlPoint(councilor.faction))
        {
            using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    TIMissionCondition timissionCondition = enumerator.Current;
                    list.Add(timissionCondition.CanTarget(councilor, ref_controlPoint));
                }
                return list;
            }
        }
        list.Add("_Fail");
        return list;
    }

    public override IEnumerable<TIControlPoint> GetAllPotentialTargets(TIFactionState faction = null)
    {
        return GameStateManager.IterateByClass<TIControlPoint>(false);
    }
    public override IList<TIControlPoint> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
    {
        List<TIControlPoint> list = new List<TIControlPoint>();
        foreach (TIControlPoint ticontrolPoint in this.GetAllPotentialTargets(null))
        {
            if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, ticontrolPoint)))
            {
                list.Add(ticontrolPoint);
            }
        }
        return list;
    }
}

public class TIMissionCondition_FixableAsset : TIMissionCondition
{
    public override List<string> feedback
    {
        get
        {
            return new List<string>
            {
                "TIMissionCondition_DefendableAsset",
            };
        }
    }

    public TIMissionCondition_FixableAsset()
    {
        if (Application.isPlaying)
        {
            this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
        }
    }
    public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
    {

        DateTime now = this.gameTime.Now;
        TIDateTime crackdownExpiration = possibleTarget.ref_controlPoint.crackdownExpiration;
        if (crackdownExpiration != null)
        {
        if (possibleTarget.isControlPointState && !possibleTarget.ref_controlPoint.EnemyFactionControlPoint(councilor.faction))
        {
                return "_Pass";
        }
       }
        return "TIMissionCondition_FixableAsset";
    }
    private GameTimeManager gameTime;
}

public class TIMissionEffect_CrackdownEnd : TIMissionEffect
{

    public TIMissionEffect_CrackdownEnd()
    {
        if (Application.isPlaying)
        {
            this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
        }
    }

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        float inputFloat = 0f;
        TINationState ref_nation = target.ref_nation;
        TIFactionState faction = mission.councilor.faction;
        if (target.isControlPointState)
        {
            DateTime now = this.gameTime.Now;
            TIControlPoint ref_controlPoint = target.ref_controlPoint;
            if (outcome == TIMissionOutcome.Success )
            {
                ref_controlPoint.EnableBenefits();
            }
            if (outcome == TIMissionOutcome.CriticalSuccess)
            {
                ref_controlPoint.EnableBenefits();
                inputFloat = ref_nation.PropagandaOnPop(faction.ideology, 5f);
            }
            if (outcome == TIMissionOutcome.CriticalFailure)
            {
                ref_controlPoint.nation.PropagandaOnPop(mission.councilor.faction.ideology, (float)Mathf.Max(-1, mission.councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false) - 11));
            }
        }
        return inputFloat.ToPercent("P0");
    }

    private GameTimeManager gameTime;
}

//public class TIMissionEffect_BuySuperOrgs : TIMissionEffect ///Nothing ATM
//{
//    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
//    {
//        if (base.MissionSuccess(outcome))
//        {
//            TIPromptQueueState.AddPromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptStealProject", 0);
//            float num = Mathf.Max(0f, target.ref_faction.GetDailyIncome(FactionResource.Research, false, false) - mission.councilor.faction.GetDailyIncome(FactionResource.Research, false, false));
//            if (num > 0f)
//            {
//                if (outcome == TIMissionOutcome.CriticalSuccess)
//                {
//                    num += 25f;
//                    num *= 2f;
//                }
//                num *= 10f;
//                mission.councilor.faction.AddToCurrentResource(num, FactionResource.Research, false);
//                return Loc.T(new StringBuilder(base.GetType().Name).Append(".Steal").ToString(), new object[]
//                {
//                    new StringBuilder(TemplateManager.global.researchInlineSpritePath).Append(num.ToString("N0")).ToString()
//                });
//            }
//        }
//        else if (outcome == TIMissionOutcome.CriticalFailure)
//        {
//            TIFactionState ref_faction = target.ref_faction;
//            mission.councilor.DetainCouncilor(ref_faction, 2f, 1f, true);
//            return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special").ToString(), new object[]
//            {
//                ref_faction.displayNameCapitalized
//            });
//        }
//        return string.Empty;
//    }
//}


public class TIMissionEffect_Advise_Scientist : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        patch_TICouncilorState councilor = (patch_TICouncilorState)mission.councilor;

        double SCI = councilor.GetAttribute(CouncilorAttribute.Science, false, true, false,false); ; ;
        SCI = SCI * 0.5;
        SCI = Math.Round(SCI);
        councilor.ScienceHolder = (int)SCI;
        councilor.ModifyAttribute(CouncilorAttribute.Science, councilor.ScienceHolder);
        if (target.isNationState)
        {
            float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
            float adviserCommandBonus = target.ref_nation.adviserCommandBonus;
            float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;
            target.ref_nation.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise_Scientist.Special1", new object[]
            {
                (target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
                (target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
                (target.ref_nation.adviserCommandBonus - adviserCommandBonus).ToString("N2")
            });
        }
        if (target.isHabState)
        {
            TIHabState ref_hab = target.ref_hab;
            float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science);
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
            councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0"),
    });
    }
}

public class TIMissionEffect_Advise_Statesman : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {


        patch_TICouncilorState councilor = (patch_TICouncilorState)mission.councilor;
        double Admin = councilor.GetAttribute(CouncilorAttribute.Administration, false, true, false, false);
        Admin  = Admin * 0.5;

        Admin = Math.Round(Admin);
        councilor.Adminholder = (int)Admin;
        councilor.ModifyAttribute(CouncilorAttribute.Administration, councilor.Adminholder);

        if (target.isNationState)
        {
            float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
            float adviserCommandBonus = target.ref_nation.adviserCommandBonus;
            float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;

            target.ref_nation.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise_Statesman.Special1", new object[]
            {
                (target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
                (target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
                (target.ref_nation.adviserCommandBonus - adviserCommandBonus).ToString("N2"),
        });
        }
        if (target.isHabState)
        {
            TIHabState ref_hab = target.ref_hab;
			float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science);
			float advisingAttribute2 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command);
			float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);
			ref_hab.AddAdvisingCouncilor(councilor);
			return Loc.T("TIMissionEffect_Advise.Special2", new object[]
			{
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) - advisingAttribute).ToPercent("P0"),
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration) - advisingAttribute3).ToPercent("P0"),
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command) - advisingAttribute2).ToPercent("P0"),
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
		patch_TICouncilorState councilor = (patch_TICouncilorState)mission.councilor;

        double Admin = councilor.GetAttribute(CouncilorAttribute.Administration, false, true, false, false);
        Admin = Admin * 0.5;
        Admin = Math.Round(Admin);
        councilor.Adminholder = (int)Admin;
        councilor.ModifyAttribute(CouncilorAttribute.Administration, councilor.Adminholder);

        double SCI = councilor.GetAttribute(CouncilorAttribute.Science, false, true, false, false);
        SCI = SCI * 0.5;
        SCI = Math.Round(SCI);
        councilor.ScienceHolder = (int)SCI;
        councilor.ModifyAttribute(CouncilorAttribute.Science, councilor.ScienceHolder);



        if (target.isNationState)
		{
			float adviserScienceBonus = target.ref_nation.adviserScienceBonus;
			float adviserCommandBonus = target.ref_nation.adviserCommandBonus;
			float adviserAdministrationBonus = target.ref_nation.adviserAdministrationBonus;
			target.ref_nation.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise.Special1", new object[]
			{
				(target.ref_nation.adviserScienceBonus - adviserScienceBonus).ToPercent("P0"),
				(target.ref_nation.adviserAdministrationBonus - adviserAdministrationBonus).ToPercent("P0"),
				(target.ref_nation.adviserCommandBonus - adviserCommandBonus).ToString("N2")
			});
		}
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			float advisingAttribute = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science);
			float advisingAttribute2 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command);
			float advisingAttribute3 = ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration);
			ref_hab.AddAdvisingCouncilor(councilor);
            return Loc.T("TIMissionEffect_Advise.Special2", new object[]
			{
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Science) - advisingAttribute).ToPercent("P0"),
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Administration) - advisingAttribute3).ToPercent("P0"),
				(ref_hab.GetAdvisingAttribute(CouncilorAttribute.Command) - advisingAttribute2).ToPercent("P0")
			});
		}
		TISpaceFleetState ref_fleet = target.ref_fleet;
        return Loc.T("TIMissionEffect_Advise.Special3", new object[]
		{
			councilor.AdvisingBonus(CouncilorAttribute.Command).ToPercent("P0"),
			councilor.AdvisingBonus(CouncilorAttribute.Science).ToPercent("P0")
		});
	}
}

public class TIMissionCondition_MaxArmyLevel : TIMissionCondition
{
    public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
    {
        TINationState ref_nation = possibleTarget.ref_nation;
        if (ref_nation.militaryTechLevel < ref_nation.maxMilitaryTechLevel)
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
            float strength = ((outcome == TIMissionOutcome.CriticalSuccess) ? 0.02f : 0.01f);
            ref_nation.AddToMilitaryTechLevel(strength);
            return strength.ToString();
        }
        if (outcome == TIMissionOutcome.CriticalFailure)
        {
            float num = -0.01f;
            ref_nation.AddToMilitaryTechLevel(num);
            return (num).ToString();
        }
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
            ref_nation.AddToCohesion(num, CohesionChangeReason.CohesionReason_UnityPriority);
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

public class TIMissionWealthofMagic : TIMissionEffect
{

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TINationState ref_nation = target.ref_nation;
        float strength = -0.1f;
        ref_nation.AddToInequality(strength, TINationState.InequalityChangeReason.InqReason_WelfarePriority);
        return string.Empty;
    }
}

public class TIMissionCondition_IsMagic : TIMissionCondition
{
    public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
    {
        patch_TIRegionState ref_region = (patch_TIRegionState)possibleTarget.ref_region;
        if (ref_region.MagicResource && !ref_region.TeleportRegion)
        {
            return "_Pass";
        }
        return "TIMissionCondition_IsMagic";
    }
}

public class TIMissionEffect_BuildTeleporter: TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        patch_TIRegionState ref_region = (patch_TIRegionState)target.ref_region;
        ref_region.TeleportRegion = true;

        GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(ref_region), null, new object[]
            {
                        ref_region
            });
     return string.Empty;
    }
   }


public class TIMissionCondition_ExecutiveControlEnemy : TIMissionCondition
{
    public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
    {
        TINationState ref_nation = possibleTarget.ref_nation;
        if (councilor.faction != ref_nation.executiveFaction && ref_nation.executiveControlPoint.owned)
        {
            return "_Pass";
        }
        return base.GetType().Name;
    }
}


public class TIMissionEffect_EstablishIntelAgency : TIMissionEffect
{

    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TICouncilorState councilor = mission.councilor;
        TINationState ref_nation = target.ref_nation;
        TIFactionState ref_faction = ref_nation.executiveFaction;
        StringBuilder stringBuilder = new StringBuilder();
        TIFactionState faction = councilor.faction;

        if (base.MissionSuccess(outcome))
        {
            faction.SetIntel(ref_faction, 2f, null);
            foreach (TICouncilorState intelTarget in ref_faction.councilors)
            {
                faction.SetIntel(intelTarget, 2f, null);
            }
        }
        else
        {
            if (outcome == TIMissionOutcome.CriticalFailure)
            {
                mission.councilor.DetainCouncilor(faction, 2f, 1f, true);
                stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[]
                {
                            faction.displayNameCapitalized
                }));
            }
        }
        return string.Empty;
    }
}

public class TIMissionEffectLivingComputing : TIMissionEffect
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        TICouncilorState councilor = mission.councilor;

       // double Admin = councilor.GetAttribute(CouncilorAttribute.Security);
       // Admin = Admin * 0.5;
       // Admin = Math.Round(Admin);
        councilor.ModifyAttribute(CouncilorAttribute.Security, 25);
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
public class patch_TIMissionEffect_Assassinate : TIMissionEffect_Assassinate
{
    public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
    {
        TICouncilorState ref_councilor = target.ref_councilor;
        if (base.MissionSuccess(outcome))
        {
            if (ref_councilor.isAlien)
            {
                mission.councilor.faction.CompleteMilestone(CampaignMilestone.AccessHydraCorpus);
                if (mission.councilor.faction.aliensRemoved > 0)
                {
                    int num = ref_councilor.orgs.Count((TIOrgState x) => x.templateName == TIGlobalConfig.globalConfig.alienShockTroopOrgDataName);
                    if (num > 0)
                    {
                        if (mission.councilor.faction.MilestoneCompleted(CampaignMilestone.AccessSalamanderCorpus))
                        {
                            if (UnityEngine.Random.value < (float)num * 0.1f)
                            {
                                mission.councilor.faction.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
                            }
                        }
                        else if (UnityEngine.Random.value < (float)num * 0.25f)
                        {
                            mission.councilor.faction.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
                        }
                    }
                }
                mission.councilor.faction.aliensRemoved++;
                mission.councilor.faction.alienInvestigations += 2;
                if (mission.missionTemplate.hate[(int)outcome] == 0f)
                {
                    (from x in GameStateManager.AllHumanFactions()
                     where x != GameStateManager.AlienProxy() && x != GameStateManager.AlienAppeaser()
                     select x).ToList<TIFactionState>().ForEach(delegate (TIFactionState x)
                     {
                         GameStateManager.AlienFaction().GainFactionHate(x, mission.missionTemplate.hate.Max() / (float)GameStateManager.AllHumanFactions().Length, false);
                     });
                }
                if (mission.councilor.faction.isActivePlayer)
                {
                    mission.councilor.faction.UnlockAchievement("killAlien");
                }
            }
            else
            {
                TIFactionState faction = mission.councilor.faction;
                patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)-1, GHGSources.Effect);///JUST FOR THIS
                patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(1, false);///Someone died
                if (faction != null && faction.isActivePlayer)
                {
                    mission.councilor.faction.UnlockAchievement("killCouncilor");
                    if (ref_councilor.inSpace)
                    {
                        mission.councilor.faction.UnlockAchievement("killCouncilorSpace");
                    }
                }
            }
            if (mission.councilor.faction != ref_councilor.faction)
            {
                ref_councilor.faction.AddSuspicionForMajorReversal(25f, ref_councilor);
                TINotificationQueueState.LogMyCouncilorAssassinated(ref_councilor, mission.councilor, mission.missionTemplate.hate[(int)outcome]);
            }
            if (!mission.councilor.assassinations.ContainsKey(ref_councilor.faction))
            {
                mission.councilor.assassinations.Add(ref_councilor.faction, 0);
            }
            Dictionary<TIFactionState, int> dictionary = mission.councilor.assassinations;
            TIFactionState faction2 = ref_councilor.faction;
            dictionary[faction2]++;
            if (!mission.councilor.faction.factionAssassinations.ContainsKey(ref_councilor.faction))
            {
                mission.councilor.faction.factionAssassinations.Add(ref_councilor.faction, 0);
            }
            dictionary = mission.councilor.faction.factionAssassinations;
            faction2 = ref_councilor.faction;
            dictionary[faction2]++;
            ref_councilor.KillCouncilor(true, (outcome == TIMissionOutcome.Success) ? mission.councilor.faction : null);
            return;
        }
        if (outcome == TIMissionOutcome.CriticalFailure && !ref_councilor.detained && mission.councilor.GetProtectors().Count == 0)
        {
            if (ref_councilor.traits.Any((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.HardTarget))
            {
                mission.councilor.KillCouncilorOnMission(mission);
                patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(1, false);///Someone died
                return;
            }
        }
        if (mission.councilor.faction == ref_councilor.faction)
        {
            IEnumerable<TIFactionState> enumerable = from x in GameStateManager.AllHumanFactions().Except(new List<TIFactionState>
            {
                ref_councilor.faction
            })
                                                     where x.turnedCouncilors.Count < 2
                                                     select x;
            if (enumerable.Count<TIFactionState>() > 0)
            {
                TIFactionState tifactionState = enumerable.SelectRandomItem<TIFactionState>();
                ref_councilor.TurnCouncilor(tifactionState);
                mission.councilor.faction.DismissCouncilor(ref_councilor, tifactionState);
                ref_councilor.AddTrait("Vengeful");
                return;
            }
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

public class patch_TIMissionEffect_Coup : TIMissionEffect_Coup
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)-1, GHGSources.Effect);///JUST FOR THIS
        TICouncilorState councilor = mission.councilor;
        TINationState ref_nation = target.ref_nation;
        if (ref_nation == null)
        {
            return string.Empty;
        }
        if (base.MissionSuccess(outcome))
        {
            int strength = (outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1;
            ref_nation.Coup(councilor, strength);
            return strength.ToString();
        }
        if (outcome != TIMissionOutcome.CriticalFailure)
        {
            return string.Empty;
        }
        TIFactionState tifactionState = ref_nation.WeightedRandomFactionByControlPoints();
        if (tifactionState == mission.councilor.faction)
        {
            return string.Empty;
        }
        if (tifactionState == null || councilor.isAlien)
        {
            float inputFloat = ref_nation.PropagandaOnPop(councilor.faction.ideology, -5f);
            return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[]
            {
                inputFloat.ToPercent("P0")
            });
        }
        councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
        return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[]
        {
            tifactionState.displayNameWithColor
        });
    }
}

public class patch_TIMissionEffect_DestroyHabModule : TIMissionEffect_DestroyHabModule
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)-0.5, GHGSources.Effect);///JUST FOR THIS
        TIHabModuleState tihabModuleState = target as TIHabModuleState;
        string displayName = tihabModuleState.displayName;
        int Crew = tihabModuleState.crew;
        patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(Crew, false);///The Crew is dead
        switch (outcome)
        {
            case TIMissionOutcome.CriticalFailure:
                mission.councilor.DetainCouncilor(tihabModuleState.ref_faction, 3f, 2f, true);
                break;
            case TIMissionOutcome.Success:
            case TIMissionOutcome.CriticalSuccess:
                tihabModuleState.hab.DestroyModule(mission.ref_faction, tihabModuleState, false, true, true, mission.missionTemplate.hate[(int)outcome], false, true);
                break;
        }
        return displayName;
    }
}
public class patch_TIMissionEffect_SabotageProject : TIMissionEffect_SabotageProject
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)-0.25, GHGSources.Effect);///JUST FOR THIS
        if (base.MissionSuccess(outcome))
        {
            patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(25, false);///Casualties is dead
            TIPromptQueueState.AddPromptStatic(mission.councilor.faction, mission.councilor, mission, "PromptSabotageProject", 0);
            if (outcome == TIMissionOutcome.CriticalSuccess)
            {
                patch_TIGlobalValuesState.GlobalValues.AddtoCasualties(50, false);///Casualties is dead
                float num = target.ref_faction.TransferResourceToFaction(50f, FactionResource.Research, mission.councilor.faction);
                return new StringBuilder(TemplateManager.global.researchInlineSpritePath).Append(num.ToString("N0")).ToString();
            }
        }
        else if (outcome == TIMissionOutcome.CriticalFailure)
        {
            TIFactionState ref_faction = target.ref_faction;
            mission.councilor.DetainCouncilor(ref_faction, 2f, 1f, true);
            return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special").ToString(), new object[]
            {
                ref_faction.displayNameCapitalized
            });
        }
        return string.Empty;
    }
}
public class patch_TIMissionEffect_TurnCouncilor : TIMissionEffect_TurnCouncilor
{
    public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
    {
        patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)-1, GHGSources.Effect);///JUST FOR THIS
        if (outcome == TIMissionOutcome.CriticalSuccess)
        {
            float amountToAdd = target.ref_faction.GetDailyIncome(FactionResource.Research, false, false) * 30f;
            mission.councilor.faction.AddToCurrentResource(amountToAdd, FactionResource.Research, false);
            return amountToAdd.ToString("N0");
        }
        return string.Empty;
    }
}

    public class patch_TIMissionEffect_DamageSpaceFacilities : TIMissionEffect_DamageSpaceFacilities
{
        public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
        {
        patch_TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)-0.5, GHGSources.Effect);///JUST FOR THIS
        if (target.isRegionSpaceFacility)
            {
                TICouncilorState councilor = mission.councilor;
                TIRegionSpaceFacilityState ref_regionSpaceFacility = target.ref_regionSpaceFacility;
                TIRegionState region = ref_regionSpaceFacility.region;
                switch (ref_regionSpaceFacility.spaceFacilityType)
                {
                    case SpaceFacilityType.launchFacility:
                        switch (outcome)
                        {
                            case TIMissionOutcome.CriticalFailure:
                                {
                                    TIFactionState tifactionState = region.nation.WeightedRandomFactionByControlPoints();
                                    if (tifactionState == mission.councilor.faction)
                                    {
                                        return string.Empty;
                                    }
                                    if (tifactionState == null)
                                    {
                                        float inputFloat = region.nation.PropagandaOnPop(councilor.faction.ideology, -5f);
                                        return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[]
                                        {
                            inputFloat.ToPercent("P0")
                                        });
                                    }
                                    councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
                                    return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[]
                                    {
                        tifactionState.displayName
                                    });
                                }
                            case TIMissionOutcome.Success:
                                {
                                    float num = Mathf.Clamp(10f - (region.boostPerYear_dekatons - 3f) * 1.5f, 0.2f, 1f);
                                    float num2 = Mathf.Min(1f, region.boostPerYear_dekatons * num);
                                    region.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -num2, false, true);
                                    TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, TIUtilities.FormatBigOrSmallNumber(region.boostPerYear_dekatons, 1, 7, 0, false, false), mission.missionTemplate.hate[(int)outcome]);
                                    return num2.ToString();
                                }
                            case TIMissionOutcome.CriticalSuccess:
                                {
                                    float num3 = Mathf.Clamp(10f - (region.boostPerYear_dekatons - 3f) * 0.5f, 0.5f, 1f);
                                    float num4 = Mathf.Min(2f, region.boostPerYear_dekatons * num3);
                                    region.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -num4, false, true);
                                    TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, TIUtilities.FormatBigOrSmallNumber(region.boostPerYear_dekatons, 1, 7, 0, false, false), mission.missionTemplate.hate[(int)outcome]);
                                    return num4.ToString();
                                }
                        }
                        break;
                    case SpaceFacilityType.missionControlFacility:
                        switch (outcome)
                        {
                            case TIMissionOutcome.CriticalFailure:
                                {
                                    TIFactionState tifactionState2 = region.nation.WeightedRandomFactionByControlPoints();
                                    if (tifactionState2 == mission.councilor.faction)
                                    {
                                        return string.Empty;
                                    }
                                    if (tifactionState2 == null)
                                    {
                                        float inputFloat2 = region.nation.PropagandaOnPop(councilor.faction.ideology, -5f);
                                        return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[]
                                        {
                            inputFloat2.ToPercent("P0")
                                        });
                                    }
                                    councilor.DetainCouncilor(tifactionState2, 2f, 1f, true);
                                    return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[]
                                    {
                        tifactionState2.displayName
                                    });
                                }
                            case TIMissionOutcome.Success:
                                {
                                    int num5 = 1;
                                    region.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)num5), false, true);
                                    TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, region.missionControl.ToString("N0"), mission.missionTemplate.hate[(int)outcome]);
                                    return num5.ToString();
                                }
                            case TIMissionOutcome.CriticalSuccess:
                                {
                                    int num6 = 2;
                                    region.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)num6), false, true);
                                    TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, region.missionControl.ToString("N0"), mission.missionTemplate.hate[(int)outcome]);
                                    return num6.ToString();
                                }
                        }
                        break;
                    case SpaceFacilityType.spaceDefenseFacility:
                        if (outcome != TIMissionOutcome.CriticalFailure)
                        {
                            if (outcome - TIMissionOutcome.Success <= 1)
                            {
                                region.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, true);
                                TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, string.Empty, mission.missionTemplate.hate[(int)outcome]);
                            }
                        }
                        else
                        {
                            TIFactionState tifactionState3 = region.nation.WeightedRandomFactionByControlPoints();
                            if (tifactionState3 == mission.councilor.faction)
                            {
                                return string.Empty;
                            }
                            if (tifactionState3 == null)
                            {
                                float inputFloat3 = region.nation.PropagandaOnPop(councilor.faction.ideology, -5f);
                                return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[]
                                {
                            inputFloat3.ToPercent("P0")
                                });
                            }
                            councilor.DetainCouncilor(tifactionState3, 2f, 1f, true);
                            return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[]
                            {
                        tifactionState3.displayName
                            });
                        }
                        break;
                }
            }
            return string.Empty;
        }
    }