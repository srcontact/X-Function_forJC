namespace clrev01.ClAction.Bullets
{
    [System.Serializable]
    public class BulletMoveCommonPar
    {
        /// <summary>
        /// 空気抵抗の加速度：nvreak＊速度
        /// </summary>
        public float nvreak;
        /// <summary>
        /// 重力を使用するか
        /// </summary>
        public bool useGraviry;
    }
}