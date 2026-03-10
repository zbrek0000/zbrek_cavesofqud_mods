using HarmonyLib;
using System;
using System.Collections.Generic;
using HistoryKit;
using XRL;
using XRL.Rules;
using XRL.Annals;
using XRL.Language;
using XRL.World.Parts;

namespace zbrek_CustomWorldGeneration.HarmonyPatches
{
	[HarmonyPatch(typeof(QudHistoryFactory))]
	[HarmonyPatch(nameof(QudHistoryFactory.GenerateVillageEraHistory))]
	class GenerateVillageEraHistoryPatch
	{
		static bool Prefix(ref History __result, ref History history)
		{
			const double DesertCanyonVillagesMultiplier = 2.352;
			const double SaltdunesVillagesMultiplier = 2.688;
			const double SaltmarshVillagesMultiplier = 0.784;
			const double HillsVillagesVillagesMultiplier = 2.520;
			const double WaterVillagesVillagesMultiplier = 1.792;
			const double BananaGroveVillagesMultiplier = 0.280;
			const double FungalVillagesMultiplier = 0.280;
			const double LakeHinnomVillagesMultiplier = 1.008;
			const double PalladiumReefVillagesMultiplier = 0.672;
			const double MountainsVillagesMultiplier = 2.240;
			const double FlowerfieldsVillagesMultiplier = 1.008;
			const double JungleVillagesMultiplier = 6.720;
			const double DeepJungleVillagesMultiplier = 2.800;
			const double RuinsVillagesMultiplier = 0.840;
			const double BaroqueRuinsVillagesMultiplier = 0.840;
			const double MoonStairVillagesMultiplier = 0.840;

			int num = int.Parse(history.GetEntitiesWithProperty("Resheph").GetRandomElement().GetCurrentSnapshot()
				.GetProperty("flipYear"));
			for (int i = 0; i < history.events.Count; i++)
			{
				if (history.events[i].HasEventProperty("gospel"))
				{
					string sentence = QudHistoryHelpers.ConvertGospelToSultanateCalendarEra(history.events[i].GetEventProperty("gospel"), num);
					history.events[i].SetEventProperty("gospel", Grammar.ConvertAtoAn(sentence));
				}
				if (history.events[i].HasEventProperty("tombInscription"))
				{
					history.events[i].SetEventProperty("tombInscription", Grammar.ConvertAtoAn(history.events[i].GetEventProperty("tombInscription")));
				}
			}

			// The four starting villages:
			var method = AccessTools.Method(typeof(QudHistoryFactory), "GenerateNewVillage");
			method.Invoke(null, new object[] { history, num, "DesertCanyon", null, 400, 900, 2, true, false });
			method.Invoke(null, new object[] { history, num, "Saltdunes", null, 400, 900, 2, true, false });
			method.Invoke(null, new object[] { history, num, "Saltmarsh", null, 400, 900, 2, true, false });
			method.Invoke(null, new object[] { history, num, "Hills", null, 400, 900, 2, true, false });

			int villagesMultiplier = The.Game.GetIntGameState("zbrek_CustomWorldGeneration_Village", 1);
			
			// Village generation based on the decompiled Qud code, multipliers rounded for readibility
			int desertCanyonVillages = villagesMultiplier * (int)Math.Round(DesertCanyonVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int saltdunesVillages = villagesMultiplier * (int)Math.Round(SaltdunesVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int saltmarshVillages = villagesMultiplier * (int)Math.Round(SaltmarshVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int hillsVillages = villagesMultiplier * (int)Math.Round(HillsVillagesVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int waterVillages = villagesMultiplier * (int)Math.Round(WaterVillagesVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int bananaGroveVillages = villagesMultiplier * (int)Math.Round(BananaGroveVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int fungalVillages = villagesMultiplier * (int)Math.Round(FungalVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int lakeHinnomVillages = villagesMultiplier * (int)Math.Round(LakeHinnomVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int palladiumReefVillages = villagesMultiplier * (int)Math.Round(PalladiumReefVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int mountainsVillages = villagesMultiplier * (int)Math.Round(MountainsVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int flowerfieldsVillages = villagesMultiplier * (int)Math.Round(FlowerfieldsVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int jungleVillages = villagesMultiplier * (int)Math.Round(JungleVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int deepJungleVillages = villagesMultiplier * (int)Math.Round(DeepJungleVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int ruinsVillages = villagesMultiplier * (int)Math.Round(RuinsVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int baroqueRuinsVillages = villagesMultiplier * (int)Math.Round(BaroqueRuinsVillagesMultiplier * Stat.Random(80, 120) / 100.0);
			int moonStairVillages = villagesMultiplier * (int)Math.Round(MoonStairVillagesMultiplier * Stat.Random(80, 120) / 100.0);

			for (int i = 1; i <= desertCanyonVillages; i++)
				method.Invoke(null, new object[] { history, num, "DesertCanyon", null, 400, 900, 2, false, false });

			for (int i = 1; i <= saltdunesVillages; i++)
				method.Invoke(null, new object[] { history, num, "Saltdunes", null, 400, 900, 2, false, false });

			for (int i = 1; i <= saltmarshVillages; i++)
				method.Invoke(null, new object[] { history, num, "Saltmarsh", null, 400, 900, 2, false, false });

			for (int i = 1; i <= hillsVillages; i++)
				method.Invoke(null, new object[] { history, num, "Hills", null, 400, 900, 2, false, false });

			for (int i = 1; i <= waterVillages; i++)
				method.Invoke(null, new object[] { history, num, "Water", null, 400, 900, 2, false, false });

			for (int i = 1; i <= bananaGroveVillages; i++)
				method.Invoke(null, new object[] { history, num, "BananaGrove", null, 400, 900, 2, false, false });

			for (int i = 1; i <= fungalVillages; i++)
				method.Invoke(null, new object[] { history, num, "Fungal", null, 400, 900, 2, false, false });

			for (int i = 1; i <= lakeHinnomVillages; i++)
				method.Invoke(null, new object[] { history, num, "LakeHinnom", null, 400, 900, 2, false, false });

			for (int i = 1; i <= palladiumReefVillages; i++)
				method.Invoke(null, new object[] { history, num, "PalladiumReef", null, 400, 900, 2, false, false });

			for (int i = 1; i <= mountainsVillages; i++)
				method.Invoke(null, new object[] { history, num, "Mountains", null, 400, 900, 2, false, false });

			for (int i = 1; i <= flowerfieldsVillages; i++)
				method.Invoke(null, new object[] { history, num, "Flowerfields", null, 400, 900, 2, false, false });

			for (int i = 1; i <= jungleVillages; i++)
				method.Invoke(null, new object[] { history, num, "Jungle", null, 400, 900, 2, false, false });

			for (int i = 1; i <= deepJungleVillages; i++)
				method.Invoke(null, new object[] { history, num, "DeepJungle", null, 400, 900, 2, false, false });

			for (int i = 1; i <= ruinsVillages; i++)
				method.Invoke(null, new object[] { history, num, "Ruins", null, 400, 900, 2, false, false });

			for (int i = 1; i <= baroqueRuinsVillages; i++)
				method.Invoke(null, new object[] { history, num, "BaroqueRuins", null, 400, 900, 2, false, false });

			for (int i = 1; i <= moonStairVillages; i++)
				method.Invoke(null, new object[] { history, num, "MoonStair", null, 400, 900, 2, false, false });
			
			history.currentYear = num + 1000;
			__result = history;
			return false;
		}
	}
}