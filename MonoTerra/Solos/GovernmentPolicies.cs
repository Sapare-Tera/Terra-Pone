using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MonoMod.InlineRT.MonoModRule;

public class patch_WarOption : WarOption
{
    public static readonly patch_TIResourcesCost Warcost = new patch_TIResourcesCost(TIFactionState.setPolicyMission.cost.resourceType, 100);
    public override bool Allowed(TINationState nationState)
    {
        if (nationState.executiveFaction != null)
        {
            patch_TIFactionState Faction = (patch_TIFactionState)nationState.executiveFaction;
            if (Faction.InternationalTreatyType == 1)
            { return false;
            }
            return nationState.WarCapable && this.GetPossibleTargets(nationState).Count > 0 && Warcost.CanAffordWarOption(nationState.executiveFaction);
        }
        return nationState.WarCapable && this.GetPossibleTargets(nationState).Count > 0;
    }
    public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
    {
        if (policyTarget.isNationState)
        {
              Warcost.PayCostWarOption(enactingNation.executiveFaction);
            patch_TIGlobalValuesState.GlobalValues.ReduceUnity(5);
            enactingNation.DeclareFullWar(enactingNation.executiveFaction, policyTarget.ref_nation);
            if (enactingNation.executiveFaction != null && enactingNation.executiveFaction.isActivePlayer)
            {
                enactingNation.executiveFaction.UnlockAchievement("declareWar");
                return;
            }
        }
        else if (policyTarget.isWarState)
        {
            TIWarState ref_War = policyTarget.ref_War;
            if (enactingNation.CanJoinExistingWarAsAttacker(ref_War))
            {
                enactingNation.JoinWar(enactingNation.executiveFaction, ref_War.attacker, ref_War);
                return;
            }
            if (enactingNation.CanJoinExistingWarAsDefender(ref_War))
            {
                enactingNation.JoinWar(enactingNation.executiveFaction, ref_War.defender, ref_War);
            }
        }
    }
    public override string GetConfirmPrompt(TINationState enactingNation, TIGameState target)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (target.isNationState)
        {
            TINationState ref_nation = target.ref_nation;
            List<TIGameState> list = new List<TIGameState>();
            list.Add(ref_nation);
            list.AddRange(ref_nation.WarCapableAllies);
            string text = TIUtilities.ConstructTextList(list, false, false);
            stringBuilder.Append(Loc.T(new StringBuilder(base.GetType().Name).Append(".confirmText").ToString(), new object[]
            {
                 text + ". This will cost " + TIUtilities.InlineResourceStr(TIFactionState.setPolicyMission.cost.resourceType)+ ((Warcost.GetSingleCostValue(FactionResource.Influence) * TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm)/100 ),
            }));;
            List<TINationState> list2 = enactingNation.ProspectiveOffensiveAlliance(ref_nation, false);
            if (list2.Count > 0)
            {
                string text2 = TIUtilities.ConstructTextList(list2.ConvertAll<TIGameState>((TINationState x) => x), false, false);
                list2.Add(enactingNation);
                TINationState tinationState = (from x in list2
                                               orderby x.militaryStrength descending
                                               select x).First<TINationState>();
                stringBuilder.Append(Loc.T("WarOption.confirmTextAlliance", new object[]
                {
                    text2,
                    tinationState.displayNameWithArticle
                }));
            }
            float num = enactingNation.CohesionLossFromDeclaringWar(ref_nation);
            if (num > 0f)
            {
                stringBuilder.Append(Loc.T("WarOption.cohesionEffect", new object[]
                {
                    num.ToString("N2")
                }));
            }
        }
        else
        {
            TIWarState ref_War = target.ref_War;
            TINationState tinationState2;
            if (ref_War.attackingAlliance.SelectMany((TINationState x) => x.allies).Contains(enactingNation))
            {
                tinationState2 = ref_War.attacker;
            }
            else
            {
                tinationState2 = ref_War.defender;
            }
            stringBuilder.Append(Loc.T("WarOption.joinWarConfirmText", new object[]
            {
                tinationState2.displayNameWithArticle,
                ref_War.displayNameWithArticle
            }));
        }
        return stringBuilder.ToString();
    }
    public override string GetDescription()
    {
        return Loc.T(new StringBuilder(base.dataName).Append(".description").ToString(), new object[]
        {
            TIUtilities.InlineResourceStr(TIFactionState.setPolicyMission.cost.resourceType),
            ((Warcost.GetSingleCostValue(FactionResource.Influence) * TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm) / 100)
        });;
    }
}
public class patch_JoinFederationOption : JoinFederationOption
{
    public static readonly patch_TIResourcesCost Fedcost = new patch_TIResourcesCost(TIFactionState.setPolicyMission.cost.resourceType, 100);
    public override bool Allowed(TINationState nationState)
    {
        return nationState.extant && (!nationState.inFederation || nationState.federation.leadNation == nationState) && !nationState.breakaway && this.GetPossibleTargets(nationState).Count > 0 && Fedcost.CanAffordFederationOption(nationState.executiveFaction);
    }

    public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
    {
        TINationState ref_nation = policyTarget.ref_nation;
        Fedcost.PayCostFederationOption(enactingNation.executiveFaction);
        patch_TIGlobalValuesState.GlobalValues.ReduceUnity(-5);
        if (enactingNation.inFederation)
        {
            enactingNation.federation.AddNation(enactingNation.executiveFaction, ref_nation, false);
            return;
        }
        if (ref_nation.inFederation)
        {
            ref_nation.federation.AddNation(enactingNation.executiveFaction, enactingNation, false);
            return;
        }
        enactingNation.FormFederation(ref_nation);
    }

    public override string GetDescription()
    {
        return Loc.T(new StringBuilder(base.dataName).Append(".description").ToString(), new object[]
        {
            TIUtilities.InlineResourceStr(TIFactionState.setPolicyMission.cost.resourceType),
            ((Fedcost.GetSingleCostValue(FactionResource.Influence) * (1 - (TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm / 100))))
        }); ;
    }
}
public class CancelOption2 : TIPolicyOption
{
    public override PolicyType GetPolicyType()
    {
        return (PolicyType)patch_PolicyType.CancelOption2;
    }

    public override bool Allowed(TINationState nationState)
    {
        return true;
    }
    public bool Faction = true;
    public override string GetDescription()
    {
        return Loc.T(new StringBuilder(base.dataName).Append(".description").ToString(), new object[]
        {
            TIUtilities.InlineResourceStr(TIFactionState.setPolicyMission.cost.resourceType),
            TIFactionState.setPolicyMission.cost.value
        });
    }
    public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
    {
        return null;
    }

    public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
    {
        TIFactionState executiveFaction = enactingNation.executiveFaction;
        if (executiveFaction == null)
        {
            return;
        }
        //executiveFaction.AddToCurrentResource(TIFactionState.setPolicyMission.cost.value, TIFactionState.setPolicyMission.cost.resourceType, false);
        patch_TINationState PatchNation = (patch_TINationState)enactingNation;
        PatchNation.Harmony = !PatchNation.Harmony;
    }

    public override bool RequiresTargets()
    {
        return false;
    }

    public override int Importance(TINationState policyNation, TIGameState target)
    {
        return -10;
    }
}



public class LeaveCouncil : TIPolicyOption
{
    public override PolicyType GetPolicyType()
    {
        return (PolicyType)patch_PolicyType.LeaveCouncil;
    }

    public override bool Allowed(TINationState nationState)
    {
        if (nationState.executiveFaction != null)
        {
            patch_TIFactionState Faction = (patch_TIFactionState)nationState.executiveFaction;
            return Faction.CouncilMember > 0 && Faction.CouncilMember < 2;
        }
        return false;
    }
    public bool Faction = true;
    public override string GetDescription()
    {
        return Loc.T(new StringBuilder(base.dataName).Append(".description").ToString(), new object[]
        {
            TIUtilities.InlineResourceStr(TIFactionState.setPolicyMission.cost.resourceType),
            TIFactionState.setPolicyMission.cost.value
        });
    }
    public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
    {
        return null;
    }

    public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
    {
        patch_TIFactionState executiveFaction = (patch_TIFactionState)enactingNation.executiveFaction;
        TIEffectsState.ProcessInstantEffect(executiveFaction, EffectTargetType.SourceFaction, EffectSecondaryStateType.none, InstantEffect.RemoveEffectFromFaction, 0f, 0f, "Effect_CouncilMemberSetter", null, null);
    }

    public override bool RequiresTargets()
    {
        return false;
    }

    public override int Importance(TINationState policyNation, TIGameState target)
    {
        return -10;
    }
}

