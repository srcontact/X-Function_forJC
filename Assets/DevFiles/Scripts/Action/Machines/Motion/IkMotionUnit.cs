using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace clrev01.ClAction.Machines.Motion
{
    [Serializable]
    public class IkMotionUnit
    {
        [ReadOnly, SerializeField]
        private string name;
        public int fightIkNum;
        /// <summary>
        /// IKのTgt位置の目標地点（ローカル座標）
        /// </summary>
        public Vector3 tgtPos;
        public MotionMode motionMode;
        public AnimationCurve lerpCurve;

        [BoxGroup("hitSettings")]
        public bool enableHit;
        [BoxGroup("hitSettings"), ShowIf("enableHit")]
        public MotionHitSetting hitSetting = new();


        public void SetName(CCDIK fightIk)
        {
            if (fightIk == null)
            {
                name = null;
            }
            else
            {
                name = fightIk != null ? fightIk.GetIKSolver().GetPoints()[^1].transform.name : "";
            }
        }
    }
}