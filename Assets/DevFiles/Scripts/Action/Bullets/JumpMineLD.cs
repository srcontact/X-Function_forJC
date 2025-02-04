using clrev01.ClAction.ObjectSearch;

namespace clrev01.ClAction.Bullets
{
    public class JumpMineLD : MineLD<JumpMineCD, JumpMineLD, JumpMineHD>
    {
        public ObjectSearchTgt nowTarget;
        public int latestJumpFrame = int.MinValue;
    }
}