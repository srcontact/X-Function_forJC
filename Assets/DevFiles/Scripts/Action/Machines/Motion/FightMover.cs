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
    public class FightMover
    {
        public MachineAnimationController machineAnimationController;
        public List<IkMotionMoverUnit> ikMotionMoverUnits = new();
        public List<Transform> fightHitObjs = new();
        public List<MotionData> fightMotionData = new();
        [NonSerialized]
        public MotionData nowMotionData;
        [ReadOnly]
        private int _motionSegmentNum = 0;
        [ReadOnly]
        private int _segmentFrameCount = 0;
        private List<Transform> _usedBoneList = new();
        private List<IHaveHitCollider> _alreadyHitHardList = new();
        public MotionSegment nowMotionSegment
        {
            get
            {
                if (_motionSegmentNum >= 0 && nowMotionData != null && _motionSegmentNum < nowMotionData.motionSegments.Count) return nowMotionData.motionSegments[_motionSegmentNum];
                return null;
            }
        }
        public Vector3 homingLosLog, previousHomingVector;

        public bool checkEnd => _motionSegmentNum >= nowMotionData.motionSegments.Count;
        public bool homingExe => _motionSegmentNum == 0 & _segmentFrameCount == 0;

        public bool inHomingRange { get; set; }


        /// <summary>
        /// 初期化処理。
        /// HDの初期化時に呼ぶもの。
        /// </summary>
        public void ExeOnMachineHdInit()
        {
            foreach (var fightIkPar in ikMotionMoverUnits)
            {
                fightIkPar.ExeOnMachineHdInit();
            }
        }

        /// <summary>
        /// 格闘モーション開始時処理
        /// </summary>
        public void OnStartFight(int fightMotionNum)
        {
            _motionSegmentNum = 0;
            _segmentFrameCount = 0;
            nowMotionData = fightMotionData[fightMotionNum];
            if (nowMotionData.animationClip.Clip) machineAnimationController.PlayClip(nowMotionData.animationClip, 0, nowMotionData.animLayer);
            else machineAnimationController.StopAnim(nowMotionData.animLayer);
        }

        private void OnSegmentStart()
        {
            foreach (var x in ikMotionMoverUnits)
            {
                x.nowMotionUnit = null;
            }
            _usedBoneList.Clear();
            _alreadyHitHardList.Clear();
            foreach (var x in nowMotionSegment.ikMotionUnits)
            {
                if (x.fightIkNum < 0 || x.fightIkNum >= ikMotionMoverUnits.Count) continue;
                var fightIkPar = ikMotionMoverUnits[x.fightIkNum];
                fightIkPar.OnSegmentStart(x, _usedBoneList);
            }
        }

        /// <summary>
        /// 格闘モーション実行処理
        /// </summary>
        public void AdvanceFight(List<Collider> hdColliders, int hdUniqueId, Vector3 hdSpeed, Transform hdTransform)
        {
            if (_segmentFrameCount == 0) OnSegmentStart();
            float animTime = 0;
            for (int i = 0; i < _motionSegmentNum; i++)
            {
                animTime += nowMotionData.motionSegments[i].animSegmentDurationTime;
            }
            var segmentFrameRatio = (float)(_segmentFrameCount + 1) / nowMotionSegment.durationFrame;
            animTime += nowMotionSegment.animSegmentDurationTime * segmentFrameRatio;
            machineAnimationController.MoveTime(animTime, false, nowMotionData.animLayer);
            foreach (var x in nowMotionSegment.ikMotionUnits)
            {
                ikMotionMoverUnits[x.fightIkNum].AdvanceFight((float)(_segmentFrameCount + 1) / nowMotionSegment.durationFrame);
            }
            foreach (var hitSetting in nowMotionSegment.hitSettings)
            {
                hitSetting.ExeHitDetection(fightHitObjs[hitSetting.hitObjNum], hdColliders, _alreadyHitHardList, hdUniqueId, hdSpeed, nowMotionSegment.durationFrame, _segmentFrameCount);
            }
            foreach (var x in nowMotionSegment.ikMotionUnits)
            {
                ikMotionMoverUnits[x.fightIkNum].ExeHitDetection(hdColliders, _alreadyHitHardList, hdUniqueId, hdSpeed, nowMotionSegment.durationFrame, _segmentFrameCount);
            }
            _segmentFrameCount++;
            if (_segmentFrameCount >= nowMotionSegment.durationFrame &&
                !(nowMotionSegment.waitHitZone && inHomingRange && _segmentFrameCount < nowMotionSegment.durationFrame + nowMotionSegment.waitFrame && !CheckHitZone(hdColliders, hdTransform)))
            {
                _segmentFrameCount = 0;
                _motionSegmentNum++;
            }
        }

        private Collider[] _res = new Collider[10];
        private bool CheckHitZone(List<Collider> hdColliders, Transform hdTransform)
        {
            _res.Fill(null);
            var hitNum = Physics.OverlapBoxNonAlloc(
                hdTransform.TransformPoint(nowMotionSegment.hitZoneOffset),
                nowMotionSegment.hitZoneSize / 2,
                _res,
                hdTransform.rotation,
                layerOfMachine + layerOfMissile + layerOfMine + layerOfAerialSmallObject + layerOfShield);
            var checkHitZone = hitNum > 0 && _res.Any(x => x != null && !hdColliders.Contains(x));
            return checkHitZone;
        }

        public void SetInfos()
        {
            ikMotionMoverUnits.ForEach(x => x.SetInfo());
        }
        public void OnEndFight()
        {
            machineAnimationController.StopAnim(nowMotionData.animLayer);
            nowMotionData = null;
        }
    }
}