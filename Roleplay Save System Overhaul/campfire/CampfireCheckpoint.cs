using HarmonyLib;
using XRL;
using XRL.Messages;
using XRL.World.Parts;


namespace zbrek_RoleplaySaveSystemOverhaul.HarmonyPatches
{
    [HarmonyPatch(typeof(Campfire), "Cook")]
    internal class CheckpointOnCooking
    {
        private static void Postfix(Campfire __instance)
        {
            if (!CheckpointingSystem.IsCheckpointingEnabled())
            {
                return;
            }
            if (!The.Player.CheckFrozen())
            {
                return;
            }
            if (The.Player.AreHostilesNearby())
            {
                return;
            }
            ActivePartStatus activePartStatus = __instance.GetActivePartStatus(UseCharge: true, IgnoreCharge: false, IgnoreLiquid: false, IgnoreBootSequence: false, IgnoreBreakage: false, IgnoreRust: false, IgnoreEMP: false, IgnoreRealityStabilization: false, IgnoreSubject: false, IgnoreLocallyDefinedFailure: false, 1, null, UseChargeIfUnpowered: false, 0L);
            if (activePartStatus != ActivePartStatus.Operational)
            {
                return;
            }
            if (The.Player.HasPart("MentalShield"))
            {
                MessageQueue.AddPlayerMessage("Warm firelight penetrates your body's surface and relaxes your insides. Your progress has been checkpointed.", "y");
            }
            else
            {
                MessageQueue.AddPlayerMessage("You relax by the warmth of the fire. Your progress has been checkpointed.", "y");
            }
            CheckpointingSystem.ManualCheckpoint();
        }
    }
}
