using ModelShark;
using MonoMod;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
    //[MonoModIgnore]
    //class EffectContextListItemController : MonoBehaviour
    //{
    //    public void SetListItem(patch_Context context, ResearchScreenController controller)
    //    {
    //        this.effectContext = context;
    //        this.controller = controller;
    //        this.selectContextButtonText.SetText(ResearchScreenController.EffectContextToString((Context)this.effectContext));
    //    }
    //    public void OnContextButtonPressed()
    //    {
    //        this.controller.OnEffectContextButtonPressed((Context)this.effectContext);
    //    }
    //    private ResearchScreenController controller;
    //    public Button selectContextButton;
    //    public TMP_Text selectContextButtonText;
    //    private patch_Context effectContext;
    //}

    public class patch_NotificationScreenController : NotificationScreenController
    {
        private void OnSetPolicyMission(TICouncilorState councilor)
        {
            if (councilor.completedMission.missionTemplate.dataName == "GoToGround")
            {
                this.masterPolicyPanelObject.SetActive(true);
                this.currentNation = councilor.currentNation;
                this.currentPolicyCouncilor = councilor;
                this.masterPolicyHeader.SetText(Loc.T("UI.Notifications.SelectPolicy", new object[]
                {
                this.currentNation.displayNameWithArticle
                }));
                this.masterPolicyFlag.sprite = this.currentNation.flag;
                GameControl.eventManager.AddListener<NationRelationsChange>(new EventManager.EventDelegate<NationRelationsChange>(this.UpdateFactionOptions), null, this.currentNation, true, false);
                this.PopulateFactionOptions(currentNation);
            }
            else
            {
                this.masterPolicyPanelObject.SetActive(true);
                this.currentNation = councilor.currentNation;
                this.currentPolicyCouncilor = councilor;
                this.masterPolicyHeader.SetText(Loc.T("UI.Notifications.SelectPolicy", new object[]
                {
                this.currentNation.displayNameWithArticle
                }));
                this.masterPolicyFlag.sprite = this.currentNation.flag;
                GameControl.eventManager.AddListener<NationRelationsChange>(new EventManager.EventDelegate<NationRelationsChange>(this.UpdatePolicyOptions), null, this.currentNation, true, false);
                this.PopulatePolicyOptions(currentNation);
            }
        }

        private TICouncilorState currentPolicyCouncilor;

        private void UpdateFactionOptions(NationRelationsChange e)
        {
            this.PopulatePolicyOptions(currentNation);
        }
        private void PopulateFactionOptions(TINationState currentNation)
        {
            this.selectPolicyPanelObject.SetActive(true);
            this.selectPolicyTargetPanelObject.SetActive(false);
            this.backButtonObject.SetActive(false);
            List<IPolicyOption> list = (from x in PolicyManager.policies.Values
                                        where !x.HandledAtFactionLevel()
                                        where x.Importance(currentNation,currentNation) == -10
                                        select x).ToList<IPolicyOption>();
            policyOptionsList.SetListSize<PolicyListItemController>(list.Count);
            int k = 0;
            foreach (PolicyListItemController listItem in policyOptionsList)
            {

                if (list[k].Allowed(currentNation) == true)
                {
                    listItem.SetListItem(this, list[k++] as TIPolicyOption, this.currentNation);
                }
                else { k++; }
            }
        }


        private void UpdatePolicyOptions(NationRelationsChange e)
        {
            this.PopulatePolicyOptions(currentNation);
        }
        private void PopulatePolicyOptions(TINationState currentNation)
        {
            this.selectPolicyPanelObject.SetActive(true);
            this.selectPolicyTargetPanelObject.SetActive(false);
            this.backButtonObject.SetActive(false);
            List<IPolicyOption> list = (from x in PolicyManager.policies.Values
                                        where !x.HandledAtFactionLevel()
                                        select x).ToList<IPolicyOption>();
            policyOptionsList.SetListSize<PolicyListItemController>(list.Count);
            int k = 0;
            foreach (PolicyListItemController listItem in policyOptionsList)
            {
                    listItem.SetListItem(this, list[k++] as TIPolicyOption, this.currentNation);
            }
        }

        private TINationState currentNation;
    }
   
    //    public class patch_ResearchScreenController : ResearchScreenController 
    //{
    //    private void UpdateEffectsBreakdownScreen()
    //    {
    //        List<patch_Context> list = new List<patch_Context>();
    //        foreach (object obj in Enum.GetValues(typeof(patch_Context)))
    //        {
    //            patch_Context context = (patch_Context)obj;
    //            if (patch_TIEffectsState.GetFactionEffectsForContext(context, (patch_TIFactionState)base.activePlayer).Any((TIEffectTemplate x) => x.description(base.activePlayer, null) != string.Empty) && patch_ResearchScreenController.EffectContextToString(context) != string.Empty)
    //            {
    //                list.Add(context);
    //            }
    //        }
    //        effectsContextList.SetListSize<EffectContextListItemController>(list.Count);
    //        int k = 0;
    //        foreach (EffectContextListItemController listItem in effectsContextList)
    //        {
    //            listItem.SetListItem(list[k++], this);
    //        }
    //        if (this.selectedContext == patch_Context.None)
    //        {
    //            this.selectedContextNameText.SetText(string.Empty);
    //            this.primarySelectedEffectListingText.SetText(string.Empty);
    //        }
    //    }
    //    public static string EffectContextToString(patch_Context context)
    //    {
    //        return Loc.T(new StringBuilder("Context.displayName.").Append(context.ToString()).ToString());
    //    }

    //    private patch_Context selectedContext;
    //}



    //Unrelated
    public class ResearchPanelController : MonoBehaviour
    {
        public static extern string orig_TechCategoryTooltip(TIFactionState faction, TIGenericTechTemplate currentGenericTemplate);
        public static string TechCategoryTooltip(TIFactionState faction, TIGenericTechTemplate currentGenericTemplate)
        {

            if (currentGenericTemplate.techCategory == (TechCategory)patch_TechCategory.MagicScience)
            {
                float num = faction.SumCategoryModifiers(currentGenericTemplate.techCategory);
                float num2 = faction.DistributedCategoryModifierValue(currentGenericTemplate.techCategory);
                string text = Loc.T("UI.Science.Panel.PositiveBonus", new object[]
                {
                num2.ToPercent("P0")
                });
                StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Science.Panel.TechCategoryTooltip_Bonus", new object[]
                {
                text,
                currentGenericTemplate.categoryString
                })).AppendLine();
                float num3 = 0;
                float num4 = faction.FleetsModifier(TechCategory.LifeScience);
                float num5 = faction.FleetsModifier(TechCategory.Xenology);
                if (num5 > 0f)
                {
                    stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Councilors", new object[]
                    {
                    num5.ToPercent("P0")
                    }));
                }
                if (num4 > 0f)
                {
                    stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Orgs", new object[]
                    {
                    num4.ToPercent("P0")
                    }));
                }
                if (num3 > 0f)
                {
                    stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Habs", new object[]
                    {
                    num3.ToPercent("P0")
                    }));
                }
                stringBuilder.AppendLine(Loc.T("UI.Science.Panel.DiminishingReturns"));
                if (num2 != num)
                {
                    stringBuilder.AppendLine().AppendLine(Loc.T("UI.Science.Panel.BonusDistribution", new object[]
                    {
                    num.ToPercent("P0"),
                    num2.ToPercent("P0")
                    }));
                }
                return stringBuilder.ToString();
            }
            else
            {
                return orig_TechCategoryTooltip(faction, currentGenericTemplate);
            }
        }
    }
}