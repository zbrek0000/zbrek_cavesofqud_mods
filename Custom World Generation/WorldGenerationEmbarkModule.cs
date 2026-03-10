using System;
using System.Collections.Generic;
using UnityEngine;
using ConsoleLib.Console;
using XRL;
using XRL.CharacterBuilds;
using XRL.CharacterBuilds.Qud;
using XRL.CharacterBuilds.UI;
using XRL.UI;
using XRL.UI.Framework;
using XRL.World.Parts;

namespace zbrek_CustomWorldGeneration
{
    public class CustomWorldGenerationEmbarkModule : QudEmbarkBuilderModule<WorldGenerationData>
    {

        public override bool shouldBeEnabled()
        {
            return builder?.GetModule<QudSubtypeModule>()?.data?.Subtype != null;
        }

        public bool IsSelected(string Id)
        {
            string[] parts = Id.Split('_');

            if (parts.Length != 4)
            {
                return false;
            }

            if (base.data == null) return false;

            if (!int.TryParse(parts[3], out int z)) return false;

            if (parts[2] == "Village")
            {
                return base.data.villageMultiplier == z;
            }

            if (parts[2] == "Lair")
            {
                return base.data.lairsMultiplier == z;
            }

            if (parts[2] == "Sultan")
            {
                return base.data.sultanHistoryMultiplier == z;
            }

            if (parts[2] == "Quest")
            {
                return base.data.questMultiplier == z;
            }

            return false;
        }

        public override object handleBootEvent(string id, XRLGame game, EmbarkInfo info, object element = null)
        {
            if (id == QudGameBootModule.BOOTEVENT_BEFOREINITIALIZEHISTORY)
            {
                GameObject gameObject = element as GameObject;
                if (base.data == null)
                {
                    MetricsManager.LogWarning("zbrek_CustomWorldGeneration.CustomWorldGenerationEmbarkModule module was active but data or selections was null");
                    return element;
                }
                WorldGenerationData data = base.data ?? new WorldGenerationData();
                game.SetIntGameState("zbrek_CustomWorldGeneration_Village", data.villageMultiplier);
                game.SetIntGameState("zbrek_CustomWorldGeneration_Lair", data.lairsMultiplier);
                game.SetIntGameState("zbrek_CustomWorldGeneration_Sultan", data.sultanHistoryMultiplier);
                game.SetIntGameState("zbrek_CustomWorldGeneration_Quest", data.questMultiplier);
            }
            return base.handleBootEvent(id, game, info, element);
        }
    }

    [UIView("zbrek_CustomWorldGeneration:Selection", NavCategory = "Chargen", UICanvas = "Chargen/zbrek_CustomWorldGeneration", UICanvasHost = 1)]
    public class CustomWorldGenerationEmbarkWindow : EmbarkBuilderModuleWindowPrefabBase<CustomWorldGenerationEmbarkModule, CategoryMenusScroller>
    {
        public EmbarkBuilderModuleWindowDescriptor windowDescriptor;
        protected const string EMPTY_CHECK = "[ ]";
        protected const string CHECKED = "[■]";
        private List<CategoryMenuData> worldGenMenuState = new List<CategoryMenuData>();

        private void SelectCustomWorldGenerationOption(FrameworkDataElement dataElement)
        {
            string[] parts = dataElement.Id.Split('_');

            if (parts.Length != 4)
            {
                return;
            }

            if (!int.TryParse(parts[3], out int z)) return;

            if (parts[2] == "Village")
            {
                base.module.data.villageMultiplier = z;
            }

            if (parts[2] == "Lair")
            {
                base.module.data.lairsMultiplier = z;
            }

            if (parts[2] == "Sultan")
            {
                base.module.data.sultanHistoryMultiplier = z;
            }

            if (parts[2] == "Quest")
            {
                base.module.data.questMultiplier = z;
            }

            UpdateControls();
        }

        public override IEnumerable<MenuOption> GetKeyMenuBar()
        {
            if (HasSelection())
            {
                yield return new MenuOption
                {
                    Id = RESET,
                    InputCommand = "CmdChargenReset",
                    Description = "Reset Selection"
                };
            }
        }

        PrefixMenuOption MakeOption(string id, string desc, string longDesc, Renderable renderable)
        {
            return new PrefixMenuOption
            {
                Id = id,
                Prefix = base.module.IsSelected(id) ? CHECKED : EMPTY_CHECK,
                Description = desc,
                LongDescription = longDesc,
                Renderable = renderable
            };
        }

        public void UpdateControls()
        {
            base.prefabComponent.onSelected.RemoveAllListeners();
            worldGenMenuState = new List<CategoryMenuData>();

            CategoryMenuData categoryMenuDataVillage = new CategoryMenuData();
            worldGenMenuState.Add(categoryMenuDataVillage);
            categoryMenuDataVillage.Title = "Village generation";
            categoryMenuDataVillage.menuOptions = new List<PrefixMenuOption>{
                MakeOption("zbrek_CustomWorldGeneration_Village_0", "Desolate", "No villages besides the villages created to give player starting locations", new Renderable("Terrain/sw_joppa.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Village_1", "Standard", "Standard Qud village coverage", new Renderable("Terrain/sw_joppa.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Village_2", "Populated", "Two times as many extra villages", new Renderable("Terrain/sw_joppa.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Village_4", "Urbanized", "Four times as many extra villages", new Renderable("Terrain/sw_joppa.bmp", "f", "&Y", "&y", 'w')),
            };

            CategoryMenuData categoryMenuDataLairs = new CategoryMenuData();
            worldGenMenuState.Add(categoryMenuDataLairs);
            categoryMenuDataLairs.Title = "Lairs generation";
            categoryMenuDataLairs.menuOptions = new List<PrefixMenuOption>{
                MakeOption("zbrek_CustomWorldGeneration_Lair_0", "Extinct", "No lairs besides static lairs and one lair to enable village generation", new Renderable("Items/sw_bed.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Lair_1", "Standard", "Standard Qud lair coverage", new Renderable("Items/sw_bed.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Lair_2", "Populated", "Two times as many lairs", new Renderable("Items/sw_bed.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Lair_4", "Infested", "Four times as many lairs", new Renderable("Items/sw_bed.bmp", "f", "&Y", "&y", 'w'))
            };

            CategoryMenuData categoryMenuDataSultats = new CategoryMenuData();
            worldGenMenuState.Add(categoryMenuDataSultats);
            categoryMenuDataSultats.Title = "Sultan histories";
            categoryMenuDataSultats.menuOptions = new List<PrefixMenuOption>{
                MakeOption("zbrek_CustomWorldGeneration_Sultan_1", "Standard", "Standard Qud Sultan histories", new Renderable("Items/sw_unfurled_scroll1.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Sultan_2", "Long", "Sultan histories contain up to two times as many events, historic sites and items", new Renderable("Items/sw_unfurled_scroll1.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Sultan_4", "Very long", "Sultan histories contain up to four times as many events, historic sites and items", new Renderable("Items/sw_unfurled_scroll1.bmp", "f", "&Y", "&y", 'w')),
            };

            CategoryMenuData categoryMenuDataQuests = new CategoryMenuData();
            worldGenMenuState.Add(categoryMenuDataQuests);
            categoryMenuDataQuests.Title = "Village quests";
            categoryMenuDataQuests.menuOptions = new List<PrefixMenuOption>{
                MakeOption("zbrek_CustomWorldGeneration_Quest_0", "None", "Villages do not generate quests", new Renderable("Items/sw_scroll1.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Quest_1", "Standard", "Standard Qud number of quests per village", new Renderable("Items/sw_scroll1.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Quest_2", "More quests", "Villages generate up to two times as many quests", new Renderable("Items/sw_scroll1.bmp", "f", "&Y", "&y", 'w')),
                MakeOption("zbrek_CustomWorldGeneration_Quest_4", "Even more quests", "Villages generate up to four times as many quests", new Renderable("Items/sw_scroll1.bmp", "f", "&Y", "&y", 'w')),
            };

            base.prefabComponent.onSelected.AddListener(SelectCustomWorldGenerationOption);
            base.prefabComponent.BeforeShow(windowDescriptor, worldGenMenuState);
        }


        public override void BeforeShow(EmbarkBuilderModuleWindowDescriptor descriptor)
        {
            if (descriptor != null)
            {
                windowDescriptor = descriptor;
            }
            if (base.module.data == null)
            {
                base.module.setData(new WorldGenerationData());
            }
            UpdateControls();

            base.BeforeShow(descriptor);
        }

        public override UnityEngine.GameObject InstantiatePrefab(UnityEngine.GameObject prefab)
        {
            prefab.GetComponentInChildren<CategoryMenusScroller>().allowVerticalLayout = false;
            return base.InstantiatePrefab(prefab);
        }

        public override UIBreadcrumb GetBreadcrumb()
        {
            return new UIBreadcrumb
            {
                Id = GetType().FullName,
                Title = "World",
                IconPath = "Items/sw_globe.bmp",
                IconDetailColor = The.Color.Blue,
                IconForegroundColor = The.Color.Green
            };
        }
    }
}