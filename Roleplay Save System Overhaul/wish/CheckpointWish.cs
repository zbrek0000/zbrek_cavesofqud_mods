using XRL;
using XRL.Wish;


namespace zbrek_RoleplaySaveSystemOverhaul
{
    [HasWishCommand]
    public class CheckpointWishHandler
    {
        [WishCommand("checkpoint")]
        public static void RunCheckpointWish()
        {
            CheckpointingSystem.ManualCheckpoint();
        }
    }
}