using HarmonyLib;
using Qud.API;
using System.IO;
using System.Threading.Tasks;
using XRL;


namespace zbrek_RoleplaySaveSystemOverhaul.HarmonyPatches
{
    [HarmonyPatch(typeof(SaveGameInfo), "TryRestoreModsAndLoadAsync")]
    internal class SaveGameInfoPrefix
    {

        private static bool Prefix(SaveGameInfo __instance, ref Task<bool> __result)
        {
            string path = Path.Combine(__instance.Directory, "zbrek_RoleplaySaveSystemOverhaul_" + __instance.json.Turn.ToString());
            if (!File.Exists(path + ".sav.gz") && !File.Exists(path + ".sav"))
            {
                UnityEngine.Debug.Log("markerInfo: " + path + " " + __instance.json.Turn.ToString() + " " + __instance.Directory);
                return true;
            }
            __result = Run(__instance, path);
            return false;
        }

        private static async Task<bool> Run(SaveGameInfo saveGameInfo, string path)
        {
            await The.UiContext;

            if (await The.Core.RestoreModsLoadedAsync(saveGameInfo.ModsEnabled))
            {
                await XRLGame.LoadGame(path, Session: true, ShowPopup: true);
                return true;
            }

            return false;
        }
    }
}