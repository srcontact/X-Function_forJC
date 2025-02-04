namespace clrev01.ClAction.Level
{
    [System.Serializable]
    public class ValuePerlinNoize : ExPerlinNoize
    {
        public float heightBasis = 10;

        public override float Value(float x, float y)
        {
            return base.Value(x, y) * heightBasis;
        }
    }
}