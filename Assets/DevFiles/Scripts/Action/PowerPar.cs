using UnityEngine;

namespace clrev01.ClAction
{
    [System.Serializable]
    public class PowerPar
    {
        public int penetrationPower;
        [SerializeField]
        private AnimationCurve penetrationDecayCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public int impactPower;
        [SerializeField]
        private AnimationCurve impactDecayCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public int heatPower;
        [SerializeField]
        private AnimationCurve heatDecayCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        public enum PowerDecayType
        {
            speed,
            frame,
        }

        [SerializeField]
        private PowerDecayType powerDecayType;

        public (int penetrationPower, int impactPower, int heatPower) GetPower(float baseSpeed, float speed, int maxFrame, int exeFrame)
        {
            float decay = 0;
            switch (powerDecayType)
            {
                case PowerDecayType.speed:
                    decay = speed / baseSpeed;
                    break;
                case PowerDecayType.frame:
                    decay = (float)(maxFrame - exeFrame) / (float)maxFrame;
                    break;
            }
            return (
                (int)(penetrationPower * penetrationDecayCurve.Evaluate(decay)),
                (int)(impactPower * impactDecayCurve.Evaluate(decay)),
                (int)(heatPower * heatDecayCurve.Evaluate(decay))
            );
        }
    }
}