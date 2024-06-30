using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PavonisInteractive.TerraInvicta
{
    public class patch_TINotificationQueueState : TINotificationQueueState
    {
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
            NotificationSummaryItem notificationSummaryItem = new NotificationSummaryItem(item.itemSummary, item.icon, item.iconBackgroundResource, item.backgroundColor, item.gotoGameState, addToAlienQueue, item.dateTime, item.templateName, item.timerFactions, item.newsFeedFactions, item.summaryLogFactions);
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
                    if (tifactionState.checkNotificationOverrides)
                    {
                        if (TINotificationQueueState.FirstNotificationOfType(tifactionState, item.templateName) && item.template.alertAudience == NotificationAudience.None && (!tifactionState.notificationOverrides.ContainsKey(item.templateName) || tifactionState.notificationOverrides[item.templateName].alert == NotificationOverrideBehavior.Remove))
                        {
                            item.itemDetail = new StringBuilder(item.itemDetail).AppendLine().AppendLine(Loc.T("UI.Notifications.OneTimeOnly")).ToString();
                        }
                        TINotificationQueueState.SetFirstNotificationofType(tifactionState, item.templateName);
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

        private static List<TIFactionState> AllFactions
        {
            get
            {
                return GameStateManager.AllFactions().ToList<TIFactionState>();
            }
        }

        public static void LogTechComplete(TIFactionState winningFaction, TITechTemplate techTemplate, int slot, bool cheat = false)
        {
            NotificationQueueItem notificationQueueItem = patch_TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
            notificationQueueItem.alertBlockFaction = winningFaction;
            notificationQueueItem.relevantFactions.Add(winningFaction);
            notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ResearchCompleteHed", new object[]
            {
                techTemplate.displayName
            });
            notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ResearchCompleteSummary", new object[]
            {
                TIUtilities.HighlightLine(techTemplate.displayName)
            });
            notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ResearchCompleteDetailPrompt", new object[]
        {
                TIUtilities.HighlightLine(techTemplate.displayName),
                winningFaction.displayNameCapitalizedWithColor,
                TemplateManager.global.researchInlineSpritePath,
                techTemplate.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.JustCompleted, null, false),
                //winningFaction.techContributionHistory[techTemplate],
        });
            notificationQueueItem.icon = techTemplate.IconResource;
            notificationQueueItem.popupResource1 = techTemplate.IconResource;
            if (!cheat || TIGlobalResearchState.CurrentResearchingTechs.Contains(techTemplate))
            {
                notificationQueueItem.alertBlockEventName = "PromptSelectTech";
                notificationQueueItem.utilityValue = slot;
            }
            notificationQueueItem.illustrationResource = techTemplate.GetCompletedIllustrationPath();
            notificationQueueItem.promptingGameState = GameStateManager.GlobalResearch();
            notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
            notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/TechQuote_").Append(techTemplate.dataName).ToString();
            notificationQueueItem.customButtonTemplateName = techTemplate.dataName;
            if (techTemplate.SpaceExplorationTech())
            {
                notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.LaunchAllProbes);
            }
            patch_TINotificationQueueState.AddItem(notificationQueueItem, false);
        }
        public static void LogTechCompleteAndNewTechSelected(TIFactionState winningFaction, TITechTemplate oldTechTemplate, TITechTemplate newTechTemplate)
        {
            NotificationQueueItem notificationQueueItem = patch_TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
            notificationQueueItem.primaryFactions = patch_TINotificationQueueState.AllFactions;
            notificationQueueItem.primaryFactions.Remove(winningFaction);
            notificationQueueItem.relevantFactions = new List<TIFactionState>(notificationQueueItem.primaryFactions);
            notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ResearchCompleteHed", new object[]
            {
                TIUtilities.HighlightLine(oldTechTemplate.displayName)
            });
            notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ResearchCompleteSummary", new object[]
            {
                TIUtilities.HighlightLine(oldTechTemplate.displayName)
            });
            notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ResearchCompleteDetail", new object[]
            {
                TIUtilities.HighlightLine(oldTechTemplate.displayName),
                winningFaction.displayNameCapitalizedWithColor,
                TemplateManager.global.researchInlineSpritePath,
                TIUtilities.HighlightLine(newTechTemplate.displayName),
                oldTechTemplate.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.JustCompleted, null, false),
                //winningFaction.techContributionHistory[oldTechTemplate],
            });
            notificationQueueItem.icon = oldTechTemplate.IconResource;
            notificationQueueItem.popupResource1 = winningFaction.factionIcon256path;
            notificationQueueItem.popupResource2 = oldTechTemplate.IconResource;
            notificationQueueItem.illustrationResource = oldTechTemplate.GetCompletedIllustrationPath();
            notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
            notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/TechQuote_").Append(oldTechTemplate.dataName).ToString();
            notificationQueueItem.customButtonTemplateName = oldTechTemplate.dataName;
            if (oldTechTemplate.SpaceExplorationTech())
            {
                notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.LaunchAllProbes);
            }
            patch_TINotificationQueueState.AddItem(notificationQueueItem, false);
        }
    }
}
