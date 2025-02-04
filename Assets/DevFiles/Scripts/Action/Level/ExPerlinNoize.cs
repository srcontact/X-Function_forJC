using UnityEngine;

namespace clrev01.ClAction.Level
{
    public abstract class ExPerlinNoize
    {
        public Vector2 noizeScale = Vector2.one * 10;
        public float offsetSize = 10;
        float offset;

        public void Initialize()
        {
            offset = offsetSize * Random.value;
        }
        public virtual float Value(float x, float y)
        {
            return Mathf.PerlinNoise((x + offset) * noizeScale.x, (y + offset) * noizeScale.y);
        }
    }
}