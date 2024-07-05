using Microsoft.CSharp.RuntimeBinder;
using MonoMod;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using ModelShark;


// patch_TINationState nation_VLC = nation as patch_TINationState;
public class patch_TIGlobalConfig : TIGlobalConfig
{
    public bool skipIntro;
    public float TIMissionModifier_ControlPointUnder_Multiplier;
    public string pathGeoscapeMagicResource1;
    public string pathTeleportRegion1;
    public string MagicResourceInlineSpritePath;
    public string TeleportRegionInlineSpritePath;
    public string TeleportRegionSpritePath;
    public float priority_MAG;
    public string pathMagicScienceIcon;
    public string MagicScienceInlineSpritePath;
    public string pathMoneyIcon;
    public string pathMagicIcon;
    public string MagicInlineSpritePath;
    public string DEF_IconPath;

    public Color32[] techColor = new Color32[]
{
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
        new Color32(14, 14, 14, byte.MaxValue),
};

    [MonoModIgnore] public patch_TIGlobalConfig() : base() { }
    [MonoModOriginal] public extern void orig_TIGlobalConfig();
    [MonoModConstructor]
    public void TIGlobalConfig()
    {
        orig_TIGlobalConfig();

        pathGeoscapeMagicResource1 = "c_mapicons/ICO_geoscape_Magical_resource";

        pathTeleportRegion1 = "c_mapicons/ICO_geoscape_Magical_resource";

        TeleportRegionSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        MagicResourceInlineSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        MagicScienceInlineSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        MagicInlineSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";


        pathMagicScienceIcon = "c_icons_2d/magic_tech_icon";

        pathMoneyIcon = "icons_2d/ICO_currency";
        
        pathMagicIcon = "c_icons_2d/magic_tech_icon";

        DEF_IconPath = "c_icons_2d/magic_tech_icon";


    TIMissionModifier_ControlPointUnder_Multiplier = -0.333f;
        priority_MAG = 1f;



        techColor = techColor.Concat(new UnityEngine.Color32[] { new UnityEngine.Color32(101, 101, 101, 255) }).ToArray();

        illus_techCompletePath = new Dictionary<TechCategory, string>(9)
            {
                {TechCategory.Energy,"illustrations/ProjectComplete_Energy"
                },
                {TechCategory.InformationScience,"illustrations/ProjectComplete_InformationScience"
                },
                {TechCategory.LifeScience,"illustrations/ProjectComplete_LifeScience"
                },
                {TechCategory.Materials,"illustrations/ProjectComplete_Materials"
                },
                {TechCategory.MilitaryScience,"illustrations/ProjectComplete_MilitaryScience"
                },
                {TechCategory.SocialScience,"illustrations/ProjectComplete_SocialScience"
                },
                {TechCategory.SpaceScience,"illustrations/ProjectComplete_SpaceScience"
                },
                {TechCategory.Xenology,"illustrations/ProjectComplete_Xenology"
                },
             {(TechCategory)patch_TechCategory.MagicScience,"illustrations/ProjectComplete_Energy"
                }
            };
        illus_projectCompletePath = new Dictionary<TechCategory, string>(9)
            {
                {TechCategory.Energy,"illustrations/ProjectComplete_Energy"
                },
                {TechCategory.InformationScience,"illustrations/ProjectComplete_InformationScience"
                },
                {TechCategory.LifeScience,"illustrations/ProjectComplete_LifeScience"
                },
                {TechCategory.Materials,"illustrations/ProjectComplete_Materials"
                },
                {TechCategory.MilitaryScience,"illustrations/ProjectComplete_MilitaryScience"
                },
                {TechCategory.SocialScience,"illustrations/ProjectComplete_SocialScience"
                },
                {TechCategory.SpaceScience,"illustrations/ProjectComplete_SpaceScience"
                },
                {TechCategory.Xenology,"illustrations/ProjectComplete_Xenology"
                },
             {(TechCategory)patch_TechCategory.MagicScience,"illustrations/ProjectComplete_Energy"
                }
            };

        techColor = new UnityEngine.Color32[]
        {
        new Color32(226, 110, 52, byte.MaxValue),
        new Color32(0, 146, 223, byte.MaxValue),
        new Color32(235, 175, 55, byte.MaxValue),
        new Color32(60, 194, 116, byte.MaxValue),
        new Color32(105, 140, 116, byte.MaxValue),
        new Color32(232, 90, 116, byte.MaxValue),
        new Color32(141, 246, 243, byte.MaxValue),
        new Color32(131, 86, byte.MaxValue, byte.MaxValue),
        new Color32(207, 159, byte.MaxValue, byte.MaxValue)
        };

    }

    // public string illus_MagicResourceSmallPath = "illustrations/Location_MediumMissionControlFacility";

    // public string illus_MagicResourceMediumPath = "illustrations/Location_MediumMissionControlFacility";

    //   public string illus_MagicResourceLargePath = "illustrations/Location_MediumMissionControlFacility";

    // public string pathGeoscapeMagicResource2 = "mapicons/ICO_geoscape_mission_ctrl";

    //public string pathGeoscapeMagicResource3 = "mapicons/ICO_geoscape_mission_ctrl";

    // public string MR_IconPath = "icons_2d/ICO_missionControl_priority";

    //public string pathMagicResource = "icons_2d/ICO_mission_control";
}

public class patch_TIRegionTemplate : TIRegionTemplate
{
    public bool? magic;

    public bool? Teleport;
}




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
                if (region_VLC.MagicResource)
                {
                    this.regionStatusMarker.SetCentralIcon(patch_AssetCacheManager.GeoscapeMagicResource1);
                    base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dEarthmodels/geoscape_core_resources");
                    this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.MagicResourceRegion"));
                    return;
                }
                if (region_VLC.TeleportRegion)
                {
                    this.regionStatusMarker.SetCentralIcon(patch_AssetCacheManager.GeoscapeMagicResource1);
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

        public extern void orig_InitWithTemplate(TIDataTemplate template);
        public override void InitWithTemplate(TIDataTemplate template)
        {

            patch_TIRegionTemplate template_PVC = template as patch_TIRegionTemplate;

            this.MagicResource = template_PVC.magic.GetValueOrDefault();
            this.TeleportRegion = template_PVC.Teleport.GetValueOrDefault();

            orig_InitWithTemplate(template);
        }

       

        public void OnNuclearAttackArrives(TIFactionState applyingFaction, TINationState applyingNation = null)
        {

            float num3 =TIEffectsState.SumEffectsModifiers((Context)patch_Context.MegaspellLevel, applyingFaction, 0f);

            //Log.Debug($"A {TIEffectsState.SumEffectsModifiers((Context)patch_Context.MegaspellLevel, applyingFaction, 0f)}");
            //Log.Debug($"A {(Context)patch_Context.MegaspellLevel}");
            Mood.TriggerEvent(Mood.Event.SDKL_MushroomCloud);
            GameControl.eventManager.TriggerEvent(new NuclearStrike(applyingNation, this), null, new object[]
            {
                this
            });
            if (num3 >= 3)
            {
                this.ApplyDamageToRegion(2.00f, applyingFaction, applyingNation, true, true, true, true);
            }
            else if (num3 >= 2)
            {
                this.ApplyDamageToRegion(1.00f, applyingFaction, applyingNation, true, true, true, true);   
            }
            else
            {
                this.ApplyDamageToRegion(0.50f, applyingFaction, applyingNation, true, true, true, true);
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
                stringBuilder.Append(TemplateManager.global.coreResourceRegionInlineSpritePath);
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
    }
        public class patch_TemplateManager : TemplateManager
    {
        public static patch_TIGlobalConfig global_PVC => global as patch_TIGlobalConfig;
    }


    public class patch_NationInfoController : NationInfoController
    {

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
            else if (region.IsOccupied())
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
                stringBuilder.Append(patch_TemplateManager.global_PVC.MagicResourceInlineSpritePath).Append(Loc.T("UI.Nation.RegionMagicResource")).AppendLine().AppendLine();
            }
            if (region_VLC.TeleportRegion)
            {
                stringBuilder.Append(patch_TemplateManager.global_PVC.TeleportRegionSpritePath).Append(Loc.T("UI.Nation.RegionTeleporter")).AppendLine().AppendLine();
            }
            if (region.resourceRegion)
            {
                stringBuilder.Append(TemplateManager.global.coreResourceRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionResource")).AppendLine().AppendLine();
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