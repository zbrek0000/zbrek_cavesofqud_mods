using HarmonyLib;
using System;
using System.Collections.Generic;
using Genkit;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.WorldBuilders;

namespace zbrek_CustomWorldGeneration.HarmonyPatches
{
    [HarmonyPatch(typeof(JoppaWorldBuilder))]
    [HarmonyPatch(nameof(JoppaWorldBuilder.BuildLairs))]
    class BuildLairsPatch
    {
        static bool Prefix(JoppaWorldBuilder __instance, ref string WorldID)
        {
            if (WorldID != "JoppaWorld")
            {
                return false;
            }
            MetricsManager.rngCheckpoint("%LAIRS 1");
            __instance.Lairs = new int[240, 75];
            List<Location2D> list = new List<Location2D>();
            for (int i = 0; i < 240; i++)
            {
                for (int j = 0; j < 75; j++)
                {
                    if (__instance.mutableMap.GetMutable(i, j) > 0 && ((__instance.RoadSystem[i, j] & JoppaWorldBuilder.ROAD_START) != 0 || (__instance.RiverSystem[i, j] & __instance.RIVER_START) != 0))
                    {
                        list.Add(Location2D.Get(i, j));
                    }
                }
            }
            int lairsMultiplier = The.Game.GetIntGameState("zbrek_CustomWorldGeneration_Lair", 1);
            int totalLairs = lairsMultiplier * 125;
            if (totalLairs == 0)
            {
                totalLairs = 1;
            }
            int fallbackNumber = 300;
            int potentialLocations = totalLairs - list.Count + fallbackNumber;
            MetricsManager.rngCheckpoint("%LAIRS 2");
            for (int i = 0; i < potentialLocations; i++)
            {
                list.Add(__instance.mutableMap.popMutableLocation());
            }
            MetricsManager.rngCheckpoint("%LAIRS 3");
            Coach.StartSection("Generate Lairs");
            list.ShuffleInPlace();
            MetricsManager.rngCheckpoint("%LAIRS 4");
            Event.PinCurrentPool();
            for (int i = 0; i < totalLairs; i++)
            {
                Event.ResetPool();
                try
                {
                    if (__instance.AddLairAt(list[i], i))
                    {
                        __instance.mutableMap.SetMutable(list[i], 0);
                    }
                }
                catch (Exception x)
                {
                    MetricsManager.LogException($"AddLairAt::{list[i]}", x);
                }
                Event.ResetToPin();
            }
            Coach.EndSection();
            MetricsManager.rngCheckpoint("%LAIRS 5");
            return false;
        }
    }
}