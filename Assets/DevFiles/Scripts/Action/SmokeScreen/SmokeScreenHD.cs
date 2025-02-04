using System;
using clrev01.Bases;
using clrev01.ClAction.Effect;
using clrev01.ClAction.Effect.Smoke;
using clrev01.ClAction.ObjectSearch;
using clrev01.Programs;
using UnityEngine;

namespace clrev01.ClAction.SmokeScreen
{
    public class SmokeScreenHD : Hard<SmokeScreenCD, SmokeScreenLD, SmokeScreenHD>
    {
        public SmokeScreenVfxControl smokeVfxControl;
        private float _size;
        private HardBase _generatorObj;

        public void OnSpawn(HardBase generatorObj, float maxDepenetrationVelocity)
        {
            _generatorObj = generatorObj;
            rigidBody.maxDepenetrationVelocity = maxDepenetrationVelocity;
            smokeVfxControl.Init(ld.cd.lifeFrame);
        }
        private readonly ObjectSearchTgt[] _resultList = new ObjectSearchTgt[48];

        public override void RunInitialFrame()
        {
            base.RunInitialFrame();
            ObjectSearch.ObjectSearch.Inst.SearchFieldGet(
                v => CheckInField(v, _size / 2),
                GetBounds(_size / 2),
                transform,
                teamID,
                (UtlOfProgram.IdentificationType)int.MaxValue,
                (UtlOfProgram.ObjType)int.MaxValue,
                UtlOfProgram.LockOnDistancePriorityType.Near,
                UtlOfProgram.LockOnAngleOfMovementToSelfType.None,
                0,
                _resultList,
                null
            );
            foreach (var objInSmoke in _resultList)
            {
                if (!objInSmoke) break;
                objInSmoke.jammingSize += ld.cd.jammingSize;
                objInSmoke.jammedSize += ld.cd.jammingSize;
            }
        }
        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            if (ld.lifeCount > ld.cd.lifeFrame || !_generatorObj || !_generatorObj.gameObject.activeSelf)
            {
                Disable();
                return;
            }
            ld.lifeCount++;

            if (_generatorObj)
            {
                pos = _generatorObj.transform.position;
                rot = _generatorObj.transform.rotation;
            }
            _size = ld.cd.GetSizePerFrame(ld.ExeFrameCount);
            scl = Vector3.one * _size;
        }
        private Bounds GetBounds(float radius)
        {
            return new Bounds(pos, Vector3.one * (radius * 2));
        }
        public bool CheckInField(Vector3 tgtPos, float radius)
        {
            return Vector3.Distance(tgtPos, pos) <= radius;
        }
        public override void RunAfterPhysics()
        {
            base.RunAfterPhysics();
            var residual = ld.lifeCount - ld.cd.effectiveFrame;
            if (residual < 0) smokeVfxControl.EffectExe(1);
            else
            {
                var rate = 1 - (float)residual / ld.cd.residualFrame;
                smokeVfxControl.EffectExe(rate);
            }
        }
        public override void OnDotonExe()
        {
            OnDotonResetPosition();
        }
    }
}