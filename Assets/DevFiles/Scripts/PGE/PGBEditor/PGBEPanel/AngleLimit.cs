using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class AngleLimit
    {
        public float MINAngle;
        public float MAXAngle;

        public AngleLimit(float minAngle, float maxAngle)
        {
            this.MINAngle = minAngle;
            this.MAXAngle = maxAngle;
        }

        public float CalcLimitedAngle(float angle)
        {
            return Mathf.Max(Mathf.Min(angle, MAXAngle), MINAngle);
        }
    }
}