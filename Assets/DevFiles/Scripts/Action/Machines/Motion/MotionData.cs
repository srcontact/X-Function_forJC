using clrev01.Bases;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.Serialization;

namespace clrev01.ClAction.Machines.Motion
{
    [CreateAssetMenu(menuName = "CommonData/FightMotionData")]
    public class MotionData : SOBaseOfCL
    {
        [SerializeField]
        private string name;
        public string Name => name;
        [SerializeField]
        private MachineHD tgtMachineHd;

        [BoxGroup("MoveType")]
        public MotionMoveType motionMoveType;
        private bool isJump => motionMoveType is MotionMoveType.jump;
        [BoxGroup("MoveType"), ShowIf("isJump")]
        public float jumpMinVerticalAngle = 25;
        public int landingRigidityFrame = 20;

        [BoxGroup("Homing")]
        public bool enableHoming;
        [BoxGroup("Homing"), ShowIf("enableHoming")]
        public FightHomingData homingData;
        [BoxGroup("Animation")]
        public ClipTransition animationClip;
        [BoxGroup("Animation")]
        public bool overrideAnim;
        [BoxGroup("Animation")]
        public int animLayer = 1;
        public List<MotionSegment> motionSegments = new();

        private void OnValidate()
        {
            foreach (var segment in motionSegments)
            foreach (var unit in segment.ikMotionUnits)
            {
                unit.SetName(null);
            }
            if (tgtMachineHd != null)
            {
                foreach (var segment in motionSegments)
                foreach (var unit in segment.ikMotionUnits)
                {
                    if (unit.fightIkNum < 0 || unit.fightIkNum >= tgtMachineHd.fightMover.ikMotionMoverUnits.Count) continue;
                    var ik = tgtMachineHd.fightMover.ikMotionMoverUnits[unit.fightIkNum];
                    unit.SetName(ik.fightIk);
                    if (segment.endMotion) unit.tgtPos = ik.fightIk.solver.IKPosition;
                }
            }
            for (var i = motionSegments.Count - 1; i >= 0; i--)
            {
                motionSegments[i].isEndSegment = i == motionSegments.Count - 1;
            }
        }
    }
}