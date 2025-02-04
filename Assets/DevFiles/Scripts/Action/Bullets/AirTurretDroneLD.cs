namespace clrev01.ClAction.Bullets
{
    public class AirTurretDroneLD : MineLD<AirTurretDroneCD, AirTurretDroneLD, AirTurretDroneHD>
    {
        /// <summary>
        /// 回避時など左右フラグ
        /// </summary>
        public bool moveLeftOrRight => spawnFrame % 2 == 0;
        public float randomize => spawnFrame % 24f / 24f;
        public int latestFireFrame = int.MinValue;
        public int firedCount = 0;
        public int maxLiveFrame = 8 * 60;
    }
}