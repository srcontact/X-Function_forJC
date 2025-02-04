using clrev01.Bases;
using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.ClAction.Machines.Motion
{
    public class LegMoverTest : BaseOfCL
    {
        [System.Serializable]
        public class LegPar
        {
            public IK ik;
            public Vector3 nowIkPos;
            public Vector3 defaultPos;
            public Vector3 tgtPos, startPos;
        }

        [System.Serializable]
        public class LegPair
        {
            public List<LegPar> legPair = new List<LegPar>();
        }

        public Transform armatureCenter;
        public List<LegPair> legPairList = new List<LegPair>();
        public AnimationCurve hSpeed, vSpeed;
        public int walkFrame, walkCount, walkCycle = 0;

        public Vector3 walkSpeed;

        public void Update()
        {
#if UNITY_EDITOR
            onExe++;
#endif
            TestMove();
            LegMove();
        }

        private void TestMove()
        {
            armatureCenter.Translate(walkSpeed / 60);
        }

        public void LegMove()
        {
            foreach (var lp in legPairList[walkCycle].legPair)
            {
                lp.tgtPos = GetNextLegPos(lp.defaultPos);
            }
            if (walkCount == 0)
            {
                foreach (var lp in legPairList[walkCycle].legPair)
                {
                    lp.startPos = armatureCenter.InverseTransformPoint(lp.ik.GetIKSolver().GetIKPosition());
                }
            }
            else
            {
                foreach (var lp in legPairList[walkCycle].legPair)
                {
                    float nowHs = GetCurvePar(hSpeed, walkFrame, walkCount);
                    float nowVs = GetCurvePar(vSpeed, walkFrame, walkCount);
                    lp.nowIkPos = Vector3.Lerp(lp.startPos, lp.tgtPos, nowHs);
                    lp.nowIkPos.y += nowVs;
                    lp.ik.GetIKSolver().SetIKPosition(armatureCenter.TransformPoint(lp.nowIkPos));
                }
            }
            walkCount++;
            if (walkCount > walkFrame)
            {
                walkCount = 0;
                walkCycle++;
                if (walkCycle >= legPairList.Count) walkCycle = 0;
            }
        }

        private float GetCurvePar(AnimationCurve ac, int allFrame, int nowFrame)
        {
            float cp = (ac.keys[ac.length - 1].time) / allFrame * nowFrame;
            return ac.Evaluate(cp);
        }

        private Vector3 GetNextLegPos(Vector3 defaultPos)
        {
            Vector3 strideLength = walkSpeed / 60 / 2 * walkFrame;
            defaultPos += strideLength;
            return defaultPos;
        }
#if UNITY_EDITOR
        public int onExe;
        private void OnValidate()
        {
            if (onExe > 0) return;
            IKSolver iKSolver;
            foreach (var lp in legPairList)
            {
                foreach (var l in lp.legPair)
                {
                    iKSolver = l.ik.GetIKSolver();
                    l.defaultPos = l.startPos = l.nowIkPos = armatureCenter.InverseTransformPoint(iKSolver.GetIKPosition());
                }
            }
        }
#endif
    }
}