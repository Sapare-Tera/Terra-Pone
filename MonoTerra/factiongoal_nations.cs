using PavonisInteractive.TerraInvicta;
using System.Collections.Generic;
using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
    
    public class patch_FactionGoal_ExpandNation : FactionGoal_ExpandNation
    {
  
    public patch_FactionGoal_ExpandNation()
    {
    }
    public patch_FactionGoal_ExpandNation(TIFactionState faction, int importance, TINationState nation)
    {
        this.faction = faction;
        base.SetImportance(importance);
        base.nation = nation;
    }

    public static FactionGoal_ExpandNation CreateGoal(FactionGoal_ExpandNation prospectiveGoal)
    {
        patch_FactionGoal_ExpandNation factionGoal_ExpandNation = GameStateManager.CreateNewGameState<patch_FactionGoal_ExpandNation>();
        factionGoal_ExpandNation.nation = prospectiveGoal.nation;
        return factionGoal_ExpandNation;
    }
    public override void RemoveState()
    {
        GameStateManager.RemoveGameState<FactionGoal_ExpandNation>(base.ID, false);
    }
    public override bool NationMissionModifyingGoal()
    {
        return true;
    }
    public override bool NationPrioritiesGoal()
    {
        return true;
    }
    public override GoalType GetGoalType()
    {
        return GoalType.ExpandNation;
    }
    public override TIGameState actor()
    {
        if (!(base.nation.executiveFaction == this.faction))
        {
            return this.faction.ref_gameState;
        }
        return base.nation.ref_gameState;
    }
    public override TIGameState target()
    {
        return base.nation;
    }
    public override TIGameState location()
    {
        return base.nation.capital;
    }
    public override TIGameState goalProduct()
    {
        return base.nation;
    }
    public override bool ValidNewGoal()
    {
        return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
    }
    public override bool IsDuplicate(TIFactionGoalState test)
    {
        return test.target() == base.nation;
    }
    public override bool InProgress()
    {
        return true;
    }
    public override bool ShouldDiscardGoal()
    {
        return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
    }
    public override bool GoalFulfilled()
    {
        return false;
    }

    public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
    {
        get
        {
            return patch_FactionGoal_ExpandNation.missionModifiers;
        }
    }

    public override List<Type> armyOperations
    {
        get
        {
            return null;
        }
    }

    public override List<PolicyType> policiesAsNation
    {
        get
        {
            return PolicyManager.RegularPolicyNames_SetPolicy;
        }
    }

    public override List<PolicyType> factionLevelPoliciesAsNation
    {
        get
        {
            return PolicyManager.AllPolicyNames_Faction;
        }
    }

    public override List<PolicyType> policiesAtTarget
    {
        get
        {
            return PolicyManager.RegularPolicyNames_SetPolicy;
        }
    }

    public override List<PolicyType> factionLevelPoliciesAtTarget
    {
        get
        {
            return PolicyManager.AllPolicyNames_Faction;
        }
    }

    public override Dictionary<PriorityType, int> prioritiesAsNation
    {
        get
        {
            return patch_FactionGoal_ExpandNation.prioritySettings;
        }
    }

    public override List<GoalType> incompatibleGoals
    {
        get
        {
            return patch_FactionGoal_ExpandNation.incompatibleGoalsForNation;
        }
    }

    public override List<TIFactionGoalState> BuildSubsequentGoals()
    {
        return null;
    }

    private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
        {
            {
                "Coup",
                0f
            },
            {
                "Advise",
                5f
            },
            {
                "Crackdown",
                10f
            },
            {
                "DefendInterests",
                100f
            },
            {
                "GainInfluence",
                10f
            },
            {
                "EnthrallElites",
                10f
            },
            {
                "EnthrallPublic",
                10f
            },
            {
                "EnthrallUnalignedElites",
                10f
            },
            {
                "Propaganda",
                5f
            },
            {
                "Purge",
                10f
            },
            {
                "Unrest",
                0f
            },
            {
                "SabotageFacilities",
                0f
            },
            {
                "TerrorizeRegion",
                0f
            },
            {
                "Stabilize",
                5f
            }
        };

    private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
        {
            {
                PriorityType.BuildArmy,
                3
            },
            {
                PriorityType.BuildSpaceDefenses,
                0
            },
            {
                PriorityType.UpgradeArmy,
                3
            },
            {
                PriorityType.Military,
                3
            }
        };

    private static readonly List<GoalType> incompatibleGoalsForNation = new List<GoalType>
        {
            GoalType.NeutralizeNation,
            GoalType.PillageNation,
            GoalType.DevelopNation,
            GoalType.MilitarizeNation,
            GoalType.SpaceifyNation
        };
}
    public class patch_FactionGoal_MilitarizeNation : FactionGoal_MilitarizeNation
    {
        public patch_FactionGoal_MilitarizeNation()
        {
        }

        public patch_FactionGoal_MilitarizeNation(TIFactionState faction, int importance, TINationState nation)
        {
            this.faction = faction;
            base.SetImportance(importance);
            base.nation = nation;
        }

        public static FactionGoal_MilitarizeNation CreateGoal(FactionGoal_MilitarizeNation p)
        {
            patch_FactionGoal_MilitarizeNation factionGoal_MilitarizeNation = GameStateManager.CreateNewGameState<patch_FactionGoal_MilitarizeNation>();
            factionGoal_MilitarizeNation.nation = p.nation;
            return factionGoal_MilitarizeNation;
        }

        public override void RemoveState()
        {
            GameStateManager.RemoveGameState<FactionGoal_MilitarizeNation>(base.ID, false);
        }

        public override bool NationMissionModifyingGoal()
        {
            return true;
        }

        public override bool NationPrioritiesGoal()
        {
            return true;
        }

        public override GoalType GetGoalType()
        {
            return GoalType.MilitarizeNation;
        }

        public override TIGameState actor()
        {
            if (!(base.nation.executiveFaction == this.faction))
            {
                return this.faction.ref_gameState;
            }
            return base.nation.ref_gameState;
        }

        public override TIGameState target()
        {
            return base.nation;
        }

        public override TIGameState location()
        {
            return base.nation.capital;
        }

        public override TIGameState goalProduct()
        {
            return base.nation;
        }

        public override bool ValidNewGoal()
        {
            return !this.ShouldDiscardGoal();
        }

        public override bool IsDuplicate(TIFactionGoalState test)
        {
            return test.target() == base.nation;
        }

        public override bool InProgress()
        {
            return true;
        }

        public override bool ShouldDiscardGoal()
        {
            return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
        }

        public override bool GoalFulfilled()
        {
            return false;
        }

        public override List<Type> armyOperations
        {
            get
            {
                return null;
            }
        }

        public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
        {
            get
            {
                return patch_FactionGoal_MilitarizeNation.missionModifiers;
            }
        }

        public override List<PolicyType> policiesAsNation
        {
            get
            {
                return PolicyManager.StabilizeNationPolicyNames_SetPolicy;
            }
        }

        public override List<PolicyType> factionLevelPoliciesAtTarget
        {
            get
            {
                return PolicyManager.ImproveRelationsPolicyNames_Faction;
            }
        }

        public override List<PolicyType> policiesAtTarget
        {
            get
            {
                return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
            }
        }

        public override List<PolicyType> factionLevelPoliciesAsNation
        {
            get
            {
                return PolicyManager.ImproveRelationsPolicyNames_Faction;
            }
        }

        public override Dictionary<PriorityType, int> prioritiesAsNation
        {
            get
            {
                return patch_FactionGoal_MilitarizeNation.prioritySettings;
            }
        }

        public override List<GoalType> incompatibleGoals
        {
            get
            {
                return patch_FactionGoal_MilitarizeNation.incompatibleGoalsForTarget;
            }
        }

        public override List<TIFactionGoalState> BuildSubsequentGoals()
        {
            return null;
        }

        private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
        {
            {
                "Coup",
                0f
            },
            {
                "Advise",
                10f
            },
            {
                "Crackdown",
                10f
            },
            {
                "DefendInterests",
                50f
            },
            {
                "GainInfluence",
                10f
            },
            {
                "EnthrallElites",
                10f
            },
            {
                "EnthrallPublic",
                5f
            },
            {
                "EnthrallUnalignedElites",
                10f
            },
            {
                "Propaganda",
                5f
            },
            {
                "Purge",
                10f
            },
            {
                "Unrest",
                0f
            },
            {
                "SabotageFacilities",
                0f
            },
            {
                "TerrorizeRegion",
                0f
            },
            {
                "Stabilize",
                10f
            }
        };

        private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
        {
            {
                PriorityType.BuildArmy,
                3
            },
            {
                PriorityType.BuildNuclearWeapons,
                3
            },
            {
                PriorityType.BuildSpaceDefenses,
                0
            },
            {
                PriorityType.InitiateNuclearProgram,
                3
            },
            {
                PriorityType.UpgradeArmy,
                3
            }
        };

        private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
        {
            GoalType.NeutralizeNation,
            GoalType.PillageNation,
            GoalType.ExpandNation,
            GoalType.DevelopNation,
            GoalType.SpaceifyNation
        };
    }

    public class patch_FactionGoal_SpaceifyNation : FactionGoal_SpaceifyNation
    {
        public patch_FactionGoal_SpaceifyNation()
        {
        }

        public patch_FactionGoal_SpaceifyNation(TIFactionState faction, int importance, TINationState nation)
        {
            this.faction = faction;
            base.SetImportance(importance);
            base.nation = nation;
        }


        public static FactionGoal_SpaceifyNation CreateGoal(FactionGoal_SpaceifyNation p)
        {
            patch_FactionGoal_SpaceifyNation factionGoal_SpaceifyNation = GameStateManager.CreateNewGameState<patch_FactionGoal_SpaceifyNation>();
            factionGoal_SpaceifyNation.nation = p.nation;
            return factionGoal_SpaceifyNation;
        }

        public override void RemoveState()
        {
            GameStateManager.RemoveGameState<FactionGoal_SpaceifyNation>(base.ID, false);
        }

        public override bool NationMissionModifyingGoal()
        {
            return true;
        }

        public override bool NationPrioritiesGoal()
        {
            return true;
        }

        public override GoalType GetGoalType()
        {
            return GoalType.SpaceifyNation;
        }

        public override TIGameState actor()
        {
            if (!(base.nation.executiveFaction == this.faction))
            {
                return this.faction.ref_gameState;
            }
            return base.nation.ref_gameState;
        }

        public override TIGameState target()
        {
            return base.nation;
        }

        public override TIGameState location()
        {
            return base.nation.capital;
        }

        public override TIGameState goalProduct()
        {
            return base.nation;
        }

        public override bool ValidNewGoal()
        {
            return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
        }

        public override bool IsDuplicate(TIFactionGoalState test)
        {
            return test.target() == base.nation;
        }

        public override bool InProgress()
        {
            return true;
        }

        public override bool ShouldDiscardGoal()
        {
            return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
        }

        public override bool GoalFulfilled()
        {
            return false;
        }

        public override List<Type> armyOperations
        {
            get
            {
                return null;
            }
        }

        public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
        {
            get
            {
                return patch_FactionGoal_SpaceifyNation.missionModifiers;
            }
        }

        public override List<PolicyType> policiesAsNation
        {
            get
            {
                return PolicyManager.StabilizeNationPolicyNames_SetPolicy;
            }
        }

        public override List<PolicyType> factionLevelPoliciesAtTarget
        {
            get
            {
                return PolicyManager.ImproveRelationsPolicyNames_Faction;
            }
        }

        public override List<PolicyType> policiesAtTarget
        {
            get
            {
                return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
            }
        }

        public override List<PolicyType> factionLevelPoliciesAsNation
        {
            get
            {
                return PolicyManager.ImproveRelationsPolicyNames_Faction;
            }
        }

        public override Dictionary<PriorityType, int> prioritiesAsNation
        {
            get
            {
                return patch_FactionGoal_SpaceifyNation.prioritySettings;
            }
        }

        public override List<GoalType> incompatibleGoals
        {
            get
            {
                return patch_FactionGoal_SpaceifyNation.incompatibleGoalsForTarget;
            }
        }

        public override List<TIFactionGoalState> BuildSubsequentGoals()
        {
            return null;
        }

        private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
        {
            {
                "Coup",
                0f
            },
            {
                "Advise",
                5f
            },
            {
                "Crackdown",
                10f
            },
            {
                "DefendInterests",
                50f
            },
            {
                "GainInfluence",
                10f
            },
            {
                "Propaganda",
                5f
            },
            {
                "Purge",
                10f
            },
            {
                "Unrest",
                0f
            },
            {
                "SabotageFacilities",
                0f
            },
            {
                "Protect",
                2f
            },
            {
                "Stabilize",
                10f
            }
        };

        private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
        {
            {
                PriorityType.Economy,
                2
            },
            {
                PriorityType.SpaceflightProgram,
                3
            },
            {
                PriorityType.LaunchFacilities,
                3
            },
            {
                PriorityType.MissionControl,
                3
            },
            {
                PriorityType.BuildSpaceDefenses,
                0
            }
        };

        private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
        {
            GoalType.NeutralizeNation,
            GoalType.PillageNation,
            GoalType.ExpandNation,
            GoalType.MilitarizeNation,
            GoalType.DevelopNation
        };
    }
}