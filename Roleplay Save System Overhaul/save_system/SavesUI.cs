using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HarmonyLib;
using Qud.API;
using Qud.UI;
using UnityEngine;
using UnityEngine.UI;
using XRL.UI;
using XRL.UI.Framework;
using System.Linq;


namespace zbrek_RoleplaySaveSystemOverhaul.HarmonyPatches
{
    internal static class SavesListPopup
    {
        public static async Task Show(string dir)
        {
            List<SaveGameInfo> infos = new List<SaveGameInfo>();
            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.EnumerateFiles(dir, "zbrek_RoleplaySaveSystemOverhaul_*.json"))
                {
                    SaveGameInfo info = await SavesAPI.ReadSaveJson(dir, file);
                    if (info != null)
                    {
                        infos.Add(info);
                    }
                }
            }
            infos.Sort((a, b) => b.SaveTime.CompareTo(a.SaveTime));

            string[] options = new string[infos.Count];
            for (int i = 0; i < infos.Count; i++)
            {
                SaveGameInfo info = infos[i];
                string modsDiffer = info.DifferentMods() ? "\n{{r|mods differ}}" : "";
                options[i] = "{{W|" + info.Name + " :: " + info.Description + " }}" + "\n{{C|Location:}} " + info.json.Location + "\n{{C|Turn:}} " + info.json.Turn + "\n{{C|Saved:}} " + info.SaveTime + modsDiffer;
            }
            string intro = "{{k|" + infos.Count + " backup save" + (infos.Count == 1 ? "" : "s") + "}}";

            Popup.PickOption("Saves", intro, "", "Sounds/UI/ui_notification", options, OnResult: delegate (int selected)
            {
                if (selected < 0 || selected >= infos.Count)
                {
                    return;
                }
                SaveGameInfo info = infos[selected];
                SingletonWindowBase<SaveManagement>.instance.SelectedInfo(new SaveInfoData { SaveGame = info });
            }, AllowEscape: true, RespectOptionNewlines: true);
        }
    }

    class RowInfo : MonoBehaviour
    {
        public SaveInfoData info;

        public GameObject button;

        public RowInfo Init(SaveInfoData info, GameObject button)
        {
            this.info = info;
            this.button = button;
            return this;
        }
    }

    [HarmonyPatch(typeof(SaveManagementRow), "setData")]
    internal class AddButton
    {
        private static void Postfix(SaveManagementRow __instance, FrameworkDataElement data)
        {
            if (data is not SaveInfoData saveInfoData)
            {
                return;
            }
            if (__instance.gameObject.TryGetComponent<RowInfo>(out var component))
            {
                UnityEngine.Object.Destroy(component.button);
                UnityEngine.Object.Destroy(component);
            }
            string dir = saveInfoData.SaveGame.Directory;
            if (!Directory.Exists(dir) || !Directory.EnumerateFiles(dir, "zbrek_RoleplaySaveSystemOverhaul_*.json").Any())
            {
                return;
            }

            GameObject source = __instance.deleteButton.gameObject;
            GameObject clone = UnityEngine.Object.Instantiate(source, source.transform.parent);
            clone.name = "zbrek_RoleplaySaveSystemOverhaul_SavesListButton";
            clone.GetComponentInChildren<UITextSkin>()?.SetText("save list");

            Button button = clone.GetComponent<Button>();
            __instance.gameObject.AddComponent<RowInfo>().Init(saveInfoData, clone);
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                button.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
            }
            button.onClick.AddListener(async () => await SavesListPopup.Show(dir));

            NavigationContext navContext = new NavigationContext
            {
                parentContext = __instance.context.context,
                buttonHandlers = new Dictionary<InputButtonTypes, System.Action>
                {
                    { InputButtonTypes.AcceptButton, XRL.UI.Framework.Event.Helpers.Handle(async () => await SavesListPopup.Show(dir)) }
                }
            };

            clone.GetComponent<FrameworkContext>().context = navContext;
            clone.GetComponent<RectTransform>().anchoredPosition += new Vector2(80f, 0f);
        }
    }

    [HarmonyPatch(typeof(SaveManagementRow), "Update")]
    internal class ButtonSetActive
    {
        private static void Postfix(SaveManagementRow __instance)
        {
            var component = __instance.gameObject.GetComponent<RowInfo>();
            if (component != null && component.button != null)
            {
                component.button.SetActive(__instance.deleteButton?.gameObject.activeSelf ?? false);
            }

        }
    }

    [HarmonyPatch(typeof(SaveManagement), "Show")]
    internal class SaveManagementCommandHandlers
    {
        private static void Postfix(SaveManagement __instance)
        {
            foreach (SaveManagementRow row in __instance.savesScroller.selectionClones.Select((FrameworkUnityScrollChild s) => s.GetComponent<SaveManagementRow>()))
            {
                if (row != null)
                {
                    var component = row.gameObject.GetComponent<RowInfo>();
                    if (component != null)
                    {
                        string dir = component.info?.SaveGame?.Directory;
                        if (Directory.Exists(dir) && !Directory.EnumerateFiles(dir, "zbrek_RoleplaySaveSystemOverhaul_*.json").Any())
                        {
                            NavigationContext rowContext = row?.context?.context;
                            if (rowContext != null)
                            {
                                rowContext.commandHandlers ??= new Dictionary<string, System.Action>();
                                rowContext.commandHandlers["zbrek_RoleplaySaveSystemOverhaul_SavesList"] = XRL.UI.Framework.Event.Helpers.Handle(async () => await SavesListPopup.Show(dir));
                            }
                        }
                    }
                }
            }
        }
    }
}
