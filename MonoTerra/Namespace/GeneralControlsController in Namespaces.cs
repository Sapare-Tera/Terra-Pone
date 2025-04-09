using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using AssetBundles;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta

{ 

    public class patch_GeneralControlsController : GeneralControlsController, IHud, ICanvas
    {
        private Dictionary<patch_FactionResource, int> proposedResourceSales;


        [Header("Resources Data")]

        public TMP_Text waterInfoText;

        public Transform MagicPanel;
        public TMP_Text magicInfoText;

    public extern void orig_Initialize();
    public override void Initialize()
    {

        var referenceGO = GameObject.Find("Exotics");
        GameObject newGO = Instantiate(referenceGO, referenceGO.transform.parent);
        //var referenceGO1 = GameObject.Find("ExoticsIcon");
        //GameObject newGO1 = Instantiate(referenceGO1, newGO.transform, true);

        // newGO.BroadcastMessage("ApplyDamage",Destroy(this));

        //Destroy(newGO.Child);

        var childtest = newGO.transform.GetChild(2);// exotics text
        Log.Debug($"What child am I 2 {childtest}");


        //var childtest2 = newGO.transform.GetChild(0);// resource background
        //Log.Debug($"What child am I 0 {childtest2}");

        var childtest3 = newGO.transform.GetChild(1); // exotics icon
        Log.Debug($"What child am I 1 {childtest3}");
        //Destroy(childtest3);

        //var childtest4 = newGO.transform.GetChild(3); //exoticsicon  clone
        //Log.Debug($"What child am I 4 {childtest4}");
        //var GO7 = newGO.GetComponentOnChild<Image>("ExoticsIcon");
        // GO7.sprite = AssetBundleManager.LoadAsset<Sprite>("misc/MenuTitle");

        //var GO5 = GameObject.Find("ExoticsIcon(Clone)");

        newGO.transform.name = "magicPanel";
        childtest.name = "magicInfoText";
        childtest3.name = "MagicIcon";
        //childtest.GetType;
        var GO6 = childtest3.GetComponent<Image>();
        GO6.sprite = AssetBundleManager.LoadAsset<Sprite>("misc/Magic");
        // newGO.transform.gameObject.SetActive(unlockedExotics);

        var test = childtest3.name;
        Log.Debug($" What am I down here {test}");

        Log.Debug($" What am I down here2 {newGO}");

        magicInfoText = childtest.GetComponent<TMP_Text>();

        MagicPanel = newGO.GetComponent<Transform>();


        orig_Initialize();
    }
    private void UpdateResourceData(patch_TIFactionState faction)
        {
            this.incomeInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Money));
            this.influenceInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Influence));
            this.operationInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Operations));
            this.boostInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Boost));
            this.researchInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Research));
            this.missionControlInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.MissionControl));
            this.controlPointMaintenanceText.SetText(GeneralControlsController.ControlPointMaintenanceString(faction));
            bool unlockedSpaceResources = faction.UnlockedSpaceResources;
            bool unlockedAntimatter = faction.UnlockedAntimatter;
            bool unlockedExotics = faction.UnlockedExotics;
            this.waterPanel.gameObject.SetActive(unlockedSpaceResources);
            this.volatilesPanel.gameObject.SetActive(unlockedSpaceResources);
            this.baseMetalsPanel.gameObject.SetActive(unlockedSpaceResources);
            this.nobleMetalsPanel.gameObject.SetActive(unlockedSpaceResources);
            this.fissilesPanel.gameObject.SetActive(unlockedSpaceResources);
            this.antimatterPanel.gameObject.SetActive(unlockedAntimatter);
            this.exoticsPanel.gameObject.SetActive(unlockedExotics);
            this.MagicPanel.gameObject.SetActive(unlockedSpaceResources);

            if (unlockedSpaceResources)
            {
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Water, this.waterInfoText, this.waterPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Volatiles, this.volatilesInfoText, this.volatilesPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Metals, this.baseMetalsInfoText, this.baseMetalsPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.NobleMetals, this.nobleMetalsInfoText, this.nobleMetalsPanel);
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Fissiles, this.fissilesInfoText, this.fissilesPanel);
                this.SetSpaceResourceValuesInBar(faction, (FactionResource)patch_FactionResource.Magic, this.magicInfoText, this.MagicPanel);
            }
            if (unlockedAntimatter)
            {
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Antimatter, this.antimatterInfoText, this.antimatterPanel);
            }
            if (unlockedExotics)
            {
                this.SetSpaceResourceValuesInBar(faction, FactionResource.Exotics, this.exoticsInfoText, this.exoticsPanel);//figure out where and how hover over text is generated
               //this.SetSpaceResourceValuesInBar(faction, (FactionResource)patch_FactionResource.Magic, this.magicInfoText, this.MagicPanel);
            }
        }
        private void SetSpaceResourceValuesInBar(TIFactionState faction, FactionResource resourceType, TMP_Text reportText, Transform panel)
        {
            if ((float)Screen.width / (float)Screen.height >= 1.5f)
            {
                panel.gameObject.GetComponent<LayoutElement>().preferredWidth = 75f;
                reportText.SetText(GeneralControlsController.ResourceReportString(faction, resourceType));
                return;
            }
            panel.gameObject.GetComponent<LayoutElement>().preferredWidth = 65f;
            float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
            if (resourceType == FactionResource.Antimatter)
            {
                reportText.SetText(TIUtilities.FormatBigOrSmallNumber(currentResourceAmount, 1, 7, 0, false, false));
                return;
            }
            reportText.SetText(TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false));
        }

        private static bool showMonthlyIncomes => GameControl.control.activePlayer.showMonthlyIncomesInTopBarAndIntel;

        public static string ResourceReportString(patch_TIFactionState faction, FactionResource resourceType)
        {
            string result = string.Empty;
            switch (resourceType)
            {
                case (FactionResource)FactionResource.Money:
                case (FactionResource)FactionResource.Influence:
                case (FactionResource)FactionResource.Operations:
                case (FactionResource)FactionResource.Boost:
                case (FactionResource)FactionResource.Water:
                case (FactionResource)FactionResource.Volatiles:
                case (FactionResource)FactionResource.Metals:
                case (FactionResource)FactionResource.NobleMetals:
                case (FactionResource)FactionResource.Fissiles:
                case (FactionResource)FactionResource.Exotics:
                case (FactionResource)patch_FactionResource.Magic:
                    {
                        float num;
                        if (patch_GeneralControlsController.showMonthlyIncomes)
                        {
                            num = faction.GetMonthlyIncome((FactionResource)resourceType, false, false);
                        }
                        else
                        {
                            num = faction.GetDailyIncome(resourceType, false, false);
                        }
                        float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
                        if (num == 0f)
                        {
                            result = TIUtilities.FormatBigOrSmallNumber(currentResourceAmount, 1, 7, 0, false, false);
                        }
                        else if (num > 0f)
                        {
                            int smallCap = (num >= 100f) ? 0 : 2;
                            result = Loc.T("UI.GeneralControls.ResourcesGain", new object[]
                            {
                        TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false),
                        TIUtilities.FormatBigOrSmallNumber(num, 1, smallCap, 0, false, false)
                            });
                        }
                        else if (num <= -0.01f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesLoss", new object[]
                            {
                        TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false),
                        TIUtilities.FormatBigOrSmallNumber(num, 0, 2, 0, false, false)
                            });
                        }
                        else
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesSmallLoss", new object[]
                            {
                        TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false),
                        "0"
                            });
                        }
                        break;
                    }
                case FactionResource.Research:
                    {
                        float num2;
                        if (patch_GeneralControlsController.showMonthlyIncomes)
                        {
                            num2 = faction.GetMonthlyIncome((FactionResource)resourceType, false, false) * (1f + faction.BonusPctFromDistribution);
                        }
                        else
                        {
                            num2 = faction.GetDailyIncome(resourceType, false, false) * (1f + faction.BonusPctFromDistribution);
                        }
                        result = TIUtilities.FormatBigNumber((double)num2, 1, false);
                        break;
                    }
                case FactionResource.Projects:
                    result = faction.GetDailyIncome(resourceType, false, false).ToString("N0");
                    break;
                case FactionResource.MissionControl:
                    {
                        float dailyIncome = faction.GetDailyIncome(resourceType, false, false);
                        float num3 = (float)faction.GetMissionControlUsage();
                        if (num3 > dailyIncome)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
                            {
                        new StringBuilder("<color=#B26A60>").Append(num3.ToString()).Append("</color>"),
                        dailyIncome.ToString("N0")
                            });
                        }
                        else if (faction.MineNetworkSize > faction.SafeMineNextworkSize)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
                            {
                        new StringBuilder("<color=#EC9933>").Append(num3.ToString()).Append("</color>"),
                        dailyIncome.ToString("N0")
                            });
                        }
                        else
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
                            {
                        num3.ToString("N0"),
                        dailyIncome.ToString("N0")
                            });
                        }
                        break;
                    }
                case FactionResource.Antimatter:
                    {
                        float num4;
                        if (patch_GeneralControlsController.showMonthlyIncomes)
                        {
                            num4 = faction.GetMonthlyIncome((FactionResource)resourceType, false, false);
                        }
                        else
                        {
                            num4 = faction.GetDailyIncome(resourceType, false, false);
                        }
                        float currentResourceAmount2 = faction.GetCurrentResourceAmount(resourceType);
                        if (num4 == 0f)
                        {
                            result = TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false);
                        }
                        else if (num4 > 0f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesGain", new object[]
                            {
                        TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false),
                        TIUtilities.FormatBigOrSmallNumber(num4, 1, 7, 0, true, false)
                            });
                        }
                        else if (num4 <= -1f)
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesLoss", new object[]
                            {
                        TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false),
                        Math.Truncate((double)num4).ToString("N0")
                            });
                        }
                        else
                        {
                            result = Loc.T("UI.GeneralControls.ResourcesSmallLoss", new object[]
                            {
                        TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false),
                        TIUtilities.FormatBigOrSmallNumber(num4, 1, 7, 0, true, false)
                            });
                        }
                        break;
                    }
            }
            return result;
        }
    }
}