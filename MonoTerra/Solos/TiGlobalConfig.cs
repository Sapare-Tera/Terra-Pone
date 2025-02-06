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
    public int councilorMaxOrgs;

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


    public List<string> illus_loadingScreens = new List<string>
    {
    };

    [MonoModIgnore] public patch_TIGlobalConfig() : base() { }
    [MonoModOriginal] public extern void orig_TIGlobalConfig();
    [MonoModConstructor]
    public void TIGlobalConfig()
    {
        orig_TIGlobalConfig();

        pathGeoscapeMagicResource1 = "c_mapicons/ICO_geoscape_Magical_resource";

        pathTeleportRegion1 = "c_mapicons/ICO_geoscape_Teleporter";

        TeleportRegionSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        MagicResourceInlineSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        MagicScienceInlineSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        MagicInlineSpritePath = "<color=#9F2B68FF><sprite tint=1 name=\"education\"></color>";

        pathMagicScienceIcon = "c_icons_2d/magic_tech_icon";

        pathMoneyIcon = "icons_2d/ICO_currency";
        
        pathMagicIcon = "c_icons_2d/magic_tech_icon";

        FMI_IconPath = "c_icons_2d/magic_tech_icon";
        councilorMaxOrgs = 15;

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
