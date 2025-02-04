using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace clrev01.ClAction.Machines.Motion
{
    [Serializable]
    public class MotionSegment
    {
        [Range(1, 120)]
        public int durationFrame = 3;
        public float useEnergy = 10;
        public bool thrusterOn;

        [BoxGroup("Animation")]
        public float animSegmentDurationTime = 1f;

        [BoxGroup("WaitHitZone")]
        public bool waitHitZone;
        [BoxGroup("WaitHitZone"), ShowIf("waitHitZone")]
        public int waitFrame = 30;
        [BoxGroup("WaitHitZone"), ShowIf("waitHitZone")]
        public Vector3 hitZoneSize, hitZoneOffset;

        public bool legMoveOverride;
        public bool isEndSegment { get; set; }
        [ShowIf("isEndSegment")]
        public bool endMotion;
        public List<IkMotionUnit> ikMotionUnits = new();
        public List<MotionHitSetting> hitSettings = new();
    }
}