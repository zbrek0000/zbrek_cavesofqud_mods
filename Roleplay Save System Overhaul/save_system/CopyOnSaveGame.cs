using HarmonyLib;
using XRL;

namespace zbrek_RoleplaySaveSystemOverhaul.HarmonyPatches
{
    [HarmonyPatch(typeof(XRLGame), "SaveGame")]
    internal class SaveCopyOnSaveGame
    {
        private const string SavePrefix = "zbrek_RoleplaySaveSystemOverhaul_";

        private static void Postfix(XRLGame __instance, string GameName)
        {
            if (!CheckpointingSystem.IsCheckpointingEnabled())
            {
                return;
            }
            // Only save copies on Checkpoint to avoid spam every 100 turns
            if (GameName != "Checkpoint" /*&& GameName != "Primary"*/)
            {
                return;
            }
            __instance.SaveGame(SavePrefix + __instance.Turns);
        }
    }
}