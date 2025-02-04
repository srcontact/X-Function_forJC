using clrev01.Bases;
using Den.Tools;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.Motion
{
    [System.Serializable]
    public class IkMotionMoverUnit
    {
        [SerializeField, ReadOnly]
        private string name;

        public CCDIK fightIk;
        [NonSerialized]
        public List<float> defaultWeights;
        [NonSerialized]
        public IkMotionUnit nowMotionUnit;

        private Vector3 _defaultLocalPos;
        public Vector3 GetDefaultWorldPos => fightIk.transform.TransformPoint(_defaultLocalPos);

        /// <summary>
        /// IKのTgt位置の開始地点（ローカル座標）
        /// </summary>
        private Vector3 _startIkPos;
        private Quaternion _startRot;
        private Quaternion _tgtRot;
        private float _startDist;
        private float _tgtDist;

        public void SetInfo()
        {
            if (fightIk == null) return;
            name = fightIk.solver.bones[^1].transform.name;
        }

        public void ExeOnMachineHdInit()
        {
            if (fightIk == null) return;
            _defaultLocalPos = fightIk.transform.InverseTransformPoint(fightIk.solver.bones[^1].transform.position);
            defaultWeights = fightIk.solver.GetPoints().ToList().ConvertAll(x => x.weight);
        }
        public void AdvanceFight(float leap)
        {
            var solver = fightIk.GetIKSolver();
            solver.SetIKPosition(fightIk.transform.TransformPoint(CalcNowIkLocalPos(leap)));
            solver.IKPositionWeight = 1;
        }

        private Collider[] _res = new Collider[10];
        public void ExeHitDetection(List<Collider> hdColliders, List<IHaveHitCollider> alreadyHitHardList, int hdUniqueId, Vector3 hdSpeed, int duration, int frameCount)
        {
            if (!nowMotionUnit.enableHit) return;

            var solver = fightIk.GetIKSolver();
            var hitDetectionCenter = solver.GetPoints()[^1].transform;
            _res.Fill(null);
            var hitNum = Physics.OverlapBoxNonAlloc(
                hitDetectionCenter.position,
                nowMotionUnit.hitSetting.hitBoxSize / 2,
                _res,
                hitDetectionCenter.rotation * Quaternion.Euler(nowMotionUnit.hitSetting.hitBoxRotate),
                layerOfMachine + layerOfMissile + layerOfMine + layerOfAerialSmallObject + layerOfShield);
            if (hitNum > 0)
            {
                for (var i = 0; i < hitNum; i++)
                {
                    var collider = _res[i];
                    if (hdColliders.Contains(collider)) continue;
                    var hardBase = ACM.GetHardFromCollider(collider);
                    if (hardBase is HardBase @base && @base.uniqueID == hdUniqueId) continue;
                    if (alreadyHitHardList.Contains(hardBase)) continue;
                    alreadyHitHardList.Add(hardBase);
                    hardBase.AddDamage(nowMotionUnit.hitSetting.hitPower, nowMotionUnit.hitSetting.speedOfPowerBase, Vector3.one, hitDetectionCenter.position, duration, frameCount);
                }
            }
        }

        public void OnSegmentStart(IkMotionUnit motionUnit, List<Transform> usedBoneList)
        {
            nowMotionUnit = motionUnit;
            _startIkPos = fightIk.transform.InverseTransformPoint(fightIk.GetIKSolver().GetPoints()[^1].transform.position);
            _startRot = Quaternion.FromToRotation(Vector3.forward, _startIkPos);
            _tgtRot = Quaternion.FromToRotation(Vector3.forward, nowMotionUnit.tgtPos);
            _startDist = Vector3.Distance(Vector3.zero, _startIkPos);
            _tgtDist = Vector3.Distance(Vector3.zero, nowMotionUnit.tgtPos);
            IkInterferencePrevention(usedBoneList);
        }

        /// <summary>
        /// IK干渉防止
        /// ２つの同じボーンを使用しているIKを同時に動かすと干渉して動きがおかしくなるので、優先度の低い方の干渉するボーンのWeightを0にする。
        /// </summary>
        /// <param name="usedBoneList">使用ずみBoneのリスト</param>
        public void IkInterferencePrevention(List<Transform> usedBoneList)
        {
            var points = fightIk.GetIKSolver().GetPoints();
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var t = point.transform;
                if (usedBoneList.Any(x => x == t))
                {
                    point.weight = 0;
                }
                else
                {
                    point.weight = defaultWeights[i];
                    usedBoneList.Add(t);
                }
            }
        }

        private Vector3 CalcNowIkLocalPos(float lerp)
        {
            if (nowMotionUnit.lerpCurve.keys.Length > 0)
            {
                lerp = nowMotionUnit.lerpCurve.Evaluate(lerp);
            }
            switch (nowMotionUnit.motionMode)
            {
                case MotionMode.AngleAndDistance:
                    return Quaternion.Lerp(_startRot, _tgtRot, lerp) * Vector3.forward * Mathf.Lerp(_startDist, _tgtDist, lerp);
                case MotionMode.Straight:
                    return Vector3.Lerp(_startIkPos, nowMotionUnit.tgtPos, lerp);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}