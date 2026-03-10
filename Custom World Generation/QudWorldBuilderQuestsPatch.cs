using HarmonyLib;
using System;
using System.Collections.Generic;
using Genkit;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.WorldBuilders;
using HistoryKit;

namespace zbrek_CustomWorldGeneration.HarmonyPatches
{
    [HarmonyPatch(typeof(JoppaWorldBuilder))]
    [HarmonyPatch(nameof(JoppaWorldBuilder.BuildDynamicQuests))]
    class BuildDynamicQuestsPatch
    {
        static bool Prefix(ref string WorldID)
        {
            int questsMultiplier = The.Game.GetIntGameState("zbrek_CustomWorldGeneration_Quest", 1);
            foreach (HistoricEntity entitiesWherePropertyEqual in The.Game.sultanHistory.GetEntitiesWherePropertyEquals("type", "village"))
            {
                if (!entitiesWherePropertyEqual.GetCurrentSnapshot().hasProperty("zoneID"))
                {
                    MetricsManager.LogEditorError("Village " + (entitiesWherePropertyEqual?.Name ?? "(null)") + " didn't have a zoneId");
                    continue;
                }
                int questsNumber = 1 * questsMultiplier;
                if (entitiesWherePropertyEqual.HasEntityProperty("isVillageZero", -1L))
                {
                    questsNumber = 2 * questsMultiplier;
                }
                for (int i = 0; i < questsNumber; i++)
                {
                    VillageDynamicQuestContext villageDynamicQuestContext = new VillageDynamicQuestContext(entitiesWherePropertyEqual)
                    {
                        questNumber = i
                    };
                    string populationName = "Dynamic Village Quests";
                    try
                    {
                        DynamicQuestFactory.fabricateQuestTemplate(PopulationManager.RollOneFrom(populationName).Blueprint, villageDynamicQuestContext);
                    }
                    catch (Exception x)
                    {
                        MetricsManager.LogException("DynamicVillageQuestFab", x);
                    }
                }
            }
            return false;
        }
    }
}