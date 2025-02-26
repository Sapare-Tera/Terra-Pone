using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MonoMod.InlineRT.MonoModRule;

namespace PavonisInteractive.TerraInvicta
{

    public class patch_TINotificationQueueState : TINotificationQueueState//use this?
    {

        public static void LogMissionOutcome(TIMissionState mission, MissionResult result, TIFactionState heldTargetFaction, List<TIGameState> newControlPoints = null, List<TIGameState> oldControlPoints = null, bool spy = false, string abortedReason = "")
        {
            string text = MethodBase.GetCurrentMethod().Name;
            if (mission.councilor.permanentAssignment || mission.councilor.permanentDefenseMode)
            {
                text = new StringBuilder(text).Append("_Permanent").ToString();
            }
            else if (spy)
            {
                text = new StringBuilder(text).Append("_Spy").ToString();
            }
            else if (!mission.missionTemplate.ContestedMission)
            {
                text = new StringBuilder(text).Append("_Uncontested").ToString();
            }
            NotificationQueueItem notificationQueueItem = patch_TINotificationQueueState.InitItem(text);
            TICouncilorState councilor = mission.councilor;
            if (spy)
            {
                notificationQueueItem.primaryFactions.Add(councilor.agentForFaction);
                notificationQueueItem.relevantFactions.Add(councilor.agentForFaction);
            }
            else
            {
                notificationQueueItem.primaryFactions.Add(councilor.faction);
                notificationQueueItem.relevantFactions.Add(councilor.faction);
            }
            TIFactionState faction = councilor.faction;
            bool flag = result.missionOutcome == TIMissionOutcome.CriticalSuccess || result.missionOutcome == TIMissionOutcome.Success;
            string displayName = councilor.displayName;
            string displayName2 = mission.displayName;
            string locationString = TIUtilities.GetLocationString(councilor.location, true, true);
            string locationString2 = TIUtilities.GetLocationString(councilor.location, true, false);
            string stateDisplayName = TIUtilities.GetStateDisplayName(mission.target, faction, false, false, false, false, false);
            string text2 = Loc.T(new StringBuilder("UI.Notifications.").Append(result.missionOutcome.ToString()).ToString());
            string text3 = flag ? "<color=#85B260>" : "<color=#B26A60>";
            notificationQueueItem.outcome = result.missionOutcome;
            if (councilor.faction == GameControl.control.activePlayer)
            {
                councilor.PlayMissionVoice(mission.missionTemplate, result.missionOutcome, councilor.OnEarth);
                if (result.missionOutcome == TIMissionOutcome.Success || result.missionOutcome == TIMissionOutcome.CriticalSuccess)
                {
                    TIFactionState ref_faction = mission.target.ref_faction;
                    if (ref_faction != null && ref_faction.IsAlienFaction && mission.missionTemplate.successSFXAlienSpecial != string.Empty)
                    {
                        notificationQueueItem.soundToPlay = mission.missionTemplate.successSFXAlienSpecial;
                    }
                    else if (mission.missionTemplate.successSFX != string.Empty)
                    {
                        notificationQueueItem.soundToPlay = mission.missionTemplate.successSFX;
                    }
                }
            }
            if (result.missionOutcome == TIMissionOutcome.Aborted)
            {
                notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MissionHeadline", new object[]
                {
                    displayName2,
                    text2
                });
                notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AbortedSummary", new object[]
                {
                    displayName,
                    displayName2
                });
                notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AbortedDetail", new object[]
                {
                    displayName,
                    displayName2,
                    Loc.T(abortedReason)
                });
            }
            else
            {
                string value;
                if (mission.missionTemplate.ContestedMission)
                {
                    bool flag2 = false;
                    int num = 0;
                    string text4 = string.Empty;
                    string text5 = string.Empty;
                    while (!flag2 && num < 40)
                    {
                        string args = new StringBuilder("P").Append(num.ToString()).ToString();
                        text4 = result.successChance.ToPercent(args);
                        text5 = result.roll.ToPercent(args);
                        if (text5 != text4)
                        {
                            flag2 = true;
                        }
                        num++;
                    }
                    if (!flag2)
                    {
                        text4 = result.successChance.ToPercent("P0");
                        text5 = result.roll.ToPercent("P0");
                    }
                    value = Loc.T("UI.Notifications.ContestedResult", new object[]
                    {
                        displayName,
                        displayName2,
                        stateDisplayName,
                        locationString2,
                        text4,
                        text5,
                        text3,
                        text2
                    });
                    notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MissionHeadline", new object[]
                    {
                        displayName2,
                        text2
                    });
                }
                else
                {
                    value = Loc.T("UI.Notifications.UncontestedResult", new object[]
                    {
                        displayName,
                        displayName2,
                        locationString2
                    });
                    notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MissionCompleteHed", new object[]
                    {
                        displayName2
                    });
                }
                TINationState ref_nation = mission.target.ref_nation;
                TIRegionState ref_region = mission.target.ref_region;
                TICouncilorState ref_councilor = mission.target.ref_councilor;
                TIControlPoint ticontrolPoint = mission.target.ref_controlPoint;
                TIHabState ref_hab = mission.target.ref_hab;
                notificationQueueItem.controlPointsRelevant = (oldControlPoints.Count > 0 && newControlPoints.Count > 0 && !oldControlPoints.SequenceEqual(newControlPoints));
                if (notificationQueueItem.controlPointsRelevant && ticontrolPoint == null)
                {
                    foreach (TIControlPoint ticontrolPoint2 in ref_nation.controlPoints)
                    {
                        if (oldControlPoints.Count > ticontrolPoint2.positionInNation && ticontrolPoint2.ref_faction != oldControlPoints[ticontrolPoint2.positionInNation])
                        {
                            ticontrolPoint = ticontrolPoint2;
                            break;
                        }
                    }
                }
                StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder().Append("TIMissionTemplate.").Append(result.missionOutcome.ToString()).Append(".").Append(mission.templateName).ToString()));
                if (result.valueChange != null && result.valueChange.Contains("|"))
                {
                    string newValue = result.valueChange.Substring(result.valueChange.LastIndexOf("|") + 1);
                    result.valueChange = result.valueChange.Substring(0, result.valueChange.IndexOf("|"));
                    stringBuilder.Replace("{returnedValue2}", newValue);
                }
                else
                {
                    stringBuilder.Replace("{returnedValue2}", string.Empty);
                }
                stringBuilder.Replace("{returnedValue}", (result.valueChange == "0%") ? Loc.T("UI.Notifications.ASmallAmount") : result.valueChange);
                stringBuilder.Replace("{myFactionName}", faction.displayNameWithColor);
                stringBuilder.Replace("{myFactionNameCapitalized}", faction.displayNameCapitalizedWithColor);
                stringBuilder.Replace("{targetNationNameWithArticle}", (ref_nation != null) ? ref_nation.displayNameWithArticle : null);
                stringBuilder.Replace("{targetNationNameWithPrep}", (ref_nation != null) ? ref_nation.displayNameWithArticleAndPlacePrep : null);
                stringBuilder.Replace("{myTargetNationControlPoints}", (ref_nation != null) ? ref_nation.CountFactionControlPoints(faction, true, false, true).ToString() : null);
                stringBuilder.Replace("{totalTargetNationControlPoints}", (ref_nation != null) ? ref_nation.numControlPoints.ToString() : null);
                stringBuilder.Replace("{missionName}", mission.displayName);
                stringBuilder.Replace("{targetFactionName}", (heldTargetFaction != null) ? heldTargetFaction.displayNameWithColor : null);
                stringBuilder.Replace("{targetFactionAdjective}", (heldTargetFaction != null) ? heldTargetFaction.adjective : null);
                stringBuilder.Replace("{targetRegionName}", (ref_region != null) ? ref_region.displayName : null);
                stringBuilder.Replace("{targetHabName}", (ref_hab != null) ? ref_hab.GetDisplayName(mission.councilor.faction) : null);
                stringBuilder.Replace("{targetNationUnrestWithString}", (ref_nation != null) ? ref_nation.GetUnrestDescriptiveStringAndValue(1) : null);
                stringBuilder.Replace("{targetDisplayName}", TIUtilities.GetStateDisplayName(mission.target, faction, false, false, false, false, false));
                stringBuilder.Replace("{targetDisplayNameSent}", TIUtilities.GetStateDisplayName(mission.target, faction, true, false, false, false, false));
                stringBuilder.Replace("{targetDisplayNameSentArticle}", TIUtilities.GetStateDisplayName(mission.target, faction, false, true, false, false, false));
                stringBuilder.Replace("{controlPointTypeDisplayName}", (ticontrolPoint != null) ? ticontrolPoint.controlPointTypeDisplayName : null);
                stringBuilder.Replace("{targetLocationStrSentence}", TIUtilities.GetLocationString(mission.targetLocation, false, true));
                StringBuilder stringBuilder2 = stringBuilder;
                string oldValue = "{targetOrgDetails}";
                TIOrgState ref_org = mission.target.ref_org;
                stringBuilder2.Replace(oldValue, (ref_org != null) ? ref_org.description(true, councilor.faction, false) : null);
                if (ref_councilor != null)
                {
                    stringBuilder.Replace("{targetCouncilorName}", councilor.faction.GetViewofCouncilor(ref_councilor).displayNameCurrentSentence);
                }
                if (notificationQueueItem.controlPointsRelevant)
                {
                    TIGameState left = oldControlPoints.Last<TIGameState>();
                    TIFactionState ref_faction2 = newControlPoints.Last<TIGameState>().ref_faction;
                    if (left != ref_faction2 && ref_faction2 != null)
                    {
                        stringBuilder.AppendLine().AppendLine().Append(Loc.T("TIMissionResult_ExecutiveControlChange", new object[]
                        {
                            ref_faction2.displayNameWithColor
                        }));
                    }
                }
                string value2 = stringBuilder.ToString();
                notificationQueueItem.itemDetail = new StringBuilder(256).AppendLine(value2).AppendLine().AppendLine(value).ToString();
                if (mission.missionTemplate.ContestedMission)
                {
                    notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ContestedMissionSummary", new object[]
                    {
                        displayName,
                        text3,
                        text2,
                        displayName2,
                        locationString
                    });
                }
                else
                {
                    notificationQueueItem.itemSummary = Loc.T("UI.Notifications.UncontestedMissionSummary", new object[]
                    {
                        displayName,
                        displayName2,
                        locationString
                    });
                }
                notificationQueueItem.illustrationResource = (flag ? mission.missionTemplate.GetCompletedIllustrationResource(mission.target, ticontrolPoint) : string.Empty);
            }
            if (mission.templateName == TIFactionState.setPolicyMission.dataName && flag)
            {
                notificationQueueItem.alertBlockFaction = faction;
                notificationQueueItem.promptingGameState = mission.target.ref_nation;
                notificationQueueItem.alertBlockEventName = "PromptSelectPolicy";
                notificationQueueItem.alertRelatedState = councilor;
            }
            if (mission.templateName == TIFactionState.goToGroundMission.dataName && flag)
            {
                notificationQueueItem.alertBlockFaction = faction;
                notificationQueueItem.promptingGameState = mission.target.ref_nation;
                notificationQueueItem.alertBlockEventName = "PromptSelectPolicy";
                notificationQueueItem.alertRelatedState = councilor;
            }
            if (spy)
            {
                notificationQueueItem.itemHeadline = new StringBuilder(Loc.T("UI.Notifications.SpyReport")).AppendLine().AppendLine(notificationQueueItem.itemHeadline).ToString();
            }
            notificationQueueItem.icon = mission.missionTemplate.missionIconImagePath_Off;
            notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
            if (councilor.faction != null)
            {
                notificationQueueItem.backgroundColor = councilor.faction.template.color;
            }
            notificationQueueItem.popupResource2 = mission.missionTemplate.missionIconImagePath_Off;
            notificationQueueItem.animationSpriteSheetPath = mission.missionTemplate.resolvingAnimation;
            notificationQueueItem.gotoGameState = councilor;
            notificationQueueItem.mission = mission;
            if (!spy)
            {
                notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatMission);
                notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatMissionContinue);
                notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.PermanentAssignment);
            }
            patch_TINotificationQueueState.AddItem(notificationQueueItem, false);
        }
        private static void AddItem(NotificationQueueItem item, bool addToAlienQueue = false)
        {
            if (item.template == null)
            {
                Log.Error("Null notification template for " + item.templateName + ". No notification pushed.", Array.Empty<object>());
                return;
            }
            item.dateTime = TITimeState.Now();
            item.dateTimeString = item.dateTime.ToCustomDateString();
            item.primaryFactions = (from x in item.primaryFactions
                                    where x != null
                                    select x).Distinct<TIFactionState>().ToList<TIFactionState>();
            item.relevantFactions = (from x in item.relevantFactions
                                     where x != null
                                     select x).Distinct<TIFactionState>().ToList<TIFactionState>();
            if (string.IsNullOrEmpty(item.itemDetail))
            {
                item.itemDetail = item.itemSummary;
            }
            else if (string.IsNullOrEmpty(item.itemSummary))
            {
                item.itemSummary = item.itemDetail;
            }
            TINotificationTemplate template = item.template;
            item.itemSummary = Loc.T("UI.Notifications.DateLog", new object[]
            {
                item.dateTimeString,
                item.itemSummary
            });
            patch_TINotificationQueueState tinotificationQueueState = (patch_TINotificationQueueState)GameStateManager.NotificationQueue();
            tinotificationQueueState.notificationQueue.Insert(0, item);
            if (tinotificationQueueState.notificationQueue.Count > 60)
            {
                tinotificationQueueState.notificationQueue.RemoveRange(60, tinotificationQueueState.notificationQueue.Count - 60);
            }
            if (addToAlienQueue)
            {
                tinotificationQueueState.alienEvents++;
            }
            if (!string.IsNullOrEmpty(item.alertBlockEventName))
            {
                if (item.promptingGameState.isNationState)
                {
                    tinotificationQueueState.promptQueue.AddPrompt(item.promptingGameState.ref_nation, item.alertBlockFaction, item.alertRelatedState, item.alertBlockEventName, item.utilityValue);
                }
                else
                {
                    tinotificationQueueState.promptQueue.AddPrompt(item.alertBlockFaction, item.promptingGameState, item.alertRelatedState, item.alertBlockEventName, item.utilityValue);
                }
            }
            NotificationSummaryItem notificationSummaryItem = new NotificationSummaryItem(item.itemSummary, item.icon, item.iconBackgroundResource, item.backgroundColor, item.gotoGameState, addToAlienQueue, item.dateTime, item.templateName, item.timerFactions, item.newsFeedFactions, item.summaryLogFactions, item.outcome);
            List<TIFactionState> list = new List<TIFactionState>();
            List<TIFactionState> list2 = new List<TIFactionState>(item.alertFactions);
            if (list2.Count > 0)
            {
                list.AddRangeUnique(list2);
            }
            if (item.putInNewsFeed)
            {
                tinotificationQueueState.notificationSummaryQueue.Insert(0, notificationSummaryItem);
                list.AddRangeUnique(item.newsFeedFactions);
            }
            if (item.putInTimerQueue)
            {
                tinotificationQueueState.timerNotificationQueue.Insert(0, notificationSummaryItem);
                list.AddRangeUnique(item.timerFactions);
                if (tinotificationQueueState.timerNotificationQueue.Count > 60)
                {
                    tinotificationQueueState.timerNotificationQueue.RemoveRange(60, tinotificationQueueState.timerNotificationQueue.Count - 60);
                }
            }
            if (item.putInSummaryLog)
            {
                SummaryCategory category = item.template.summaryAudience.category;
                tinotificationQueueState.panelSummaryQueue[category].Insert(0, notificationSummaryItem);
                list.AddRangeUnique(item.summaryLogFactions);
                if (tinotificationQueueState.panelSummaryQueue[category].Count > patch_TINotificationQueueState.maxSummaryQueueSize[category])
                {
                    tinotificationQueueState.panelSummaryQueue[category].RemoveRange(patch_TINotificationQueueState.maxSummaryQueueSize[category], tinotificationQueueState.panelSummaryQueue[category].Count - patch_TINotificationQueueState.maxSummaryQueueSize[category]);
                }
            }
            if (item.template.firstAlertOverride)
            {
                foreach (TIFactionState tifactionState in list2)
                {
                    if (tifactionState.checkNotificationOverrides && TINotificationQueueState.FirstNotificationOfType(tifactionState, item.templateName) && item.template.alertAudience == NotificationAudience.None && (!tifactionState.notificationOverrides.ContainsKey(item.templateName) || tifactionState.notificationOverrides[item.templateName].alert == NotificationOverrideBehavior.Remove))
                    {
                        item.itemDetail = new StringBuilder(item.itemDetail).AppendLine().AppendLine(Loc.T("UI.Notifications.OneTimeOnly")).ToString();
                    }
                }
            }
            if (list.Count > 0)
            {
                EventManager eventManager = GameControl.eventManager;
                GameEvent evt = new NewsItemCreated(item, notificationSummaryItem);
                string eventName = null;
                object[] sourceObjects = list.ToArray();
                eventManager.TriggerEvent(evt, eventName, sourceObjects);
            }
        }
        private TIPromptQueueState promptQueue;
        private static readonly Dictionary<SummaryCategory, int> maxSummaryQueueSize = new Dictionary<SummaryCategory, int>
        {
            {
                SummaryCategory.CouncilorSightings,
                60
            },
            {
                SummaryCategory.EarthEvents,
                60
            },
            {
                SummaryCategory.Missions,
                60
            },
            {
                SummaryCategory.SpaceEvents,
                60
            },
            {
                SummaryCategory.Bombardment,
                120
            },
            {
                SummaryCategory.None,
                0
            }
        };

        private static NotificationQueueItem InitItem(string templateName)
        {
            return new NotificationQueueItem
            {
                relevantFactions = new List<TIFactionState>(),
                primaryFactions = new List<TIFactionState>(),
                alertBlockFaction = null,
                templateName = templateName
            };
        }
    }
}


