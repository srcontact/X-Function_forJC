using System;
using System.Collections.Generic;
using clrev01.ClAction.ObjectSearch;
using clrev01.Save;
using Dest.Math;
using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    [Serializable]
    public class SphereSearchFieldPar : IFieldSearchObject
    {
        [MemoryPack.MemoryPackIgnore]
        public virtual float FarRadius { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float NearRadius { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float HorizontalAngle { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float VerticalAngle1 { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float VerticalAngle2 { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual Vector3 Rotate { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual Vector3 Offset { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual bool Is2D { get; protected set; } = false;

        private Func<Vector3, bool> _checkInFieldFunc;
        [NonSerialized]
        private Vector3 _sphereCenter, _verticalV, _horizontalV, _horizontalPlaneV;
        [NonSerialized]
        private Sphere3 _farSphere, _nearSphere;
        [NonSerialized]
        private float _verticalAngle1F, _verticalAngle2F, _horizontalAngleF;

        public void CalcField(Transform searcher)
        {
            var searcherForward = Vector3.Cross(searcher.transform.right, Vector3.up);
            if (searcherForward.sqrMagnitude <= Vector3.kEpsilon)
            {
                searcherForward = Vector3.ProjectOnPlane(searcher.forward, Vector3.up).normalized;
            }
            var searcherRotation = Quaternion.LookRotation(searcherForward, Vector3.up).eulerAngles;
            var prjRot =
                Quaternion.Euler(0, searcherRotation.y, 0) * Quaternion.Euler(Rotate.x, Rotate.y, 0);
            _sphereCenter = searcher.position + prjRot * Offset;
            _farSphere = new Sphere3(_sphereCenter, FarRadius);
            _nearSphere = new Sphere3(_sphereCenter, NearRadius);
            _horizontalAngleF = -Mathf.Cos((HorizontalAngle / 2) * Mathf.Deg2Rad);
            _verticalAngle1F = -Mathf.Cos((VerticalAngle1 + 90) * Mathf.Deg2Rad);
            _verticalAngle2F = -Mathf.Cos((VerticalAngle2 + 90) * Mathf.Deg2Rad);
            if (Mathf.Abs(VerticalAngle1) > 90 && Mathf.Abs(VerticalAngle2) > 90)
            {
                _verticalV = Vector3.zero;
            }
            else
            {
                _verticalV = prjRot * Vector3.down;
            }

            if (HorizontalAngle >= 360)
            {
                _horizontalV = Vector3.zero;
            }
            else
            {
                _horizontalV = prjRot * Vector3.forward;
                _horizontalPlaneV = prjRot * Vector3.up;
            }
        }
        public bool CheckInField(Vector3 tgtPos)
        {
            if (!_farSphere.Contains(tgtPos) || _nearSphere.Contains(tgtPos)) return false;
            var v = _sphereCenter - tgtPos;
            if (_horizontalV != Vector3.zero)
            {
                var hDot = Vector3.Dot(_horizontalV, Vector3.ProjectOnPlane(v, _horizontalPlaneV).normalized);
                if (hDot > _horizontalAngleF) return false;
            }
            if (_verticalV != Vector3.zero)
            {
                var vDot = Vector3.Dot(_verticalV, v.normalized);
                if (_verticalAngle1F > _verticalAngle2F &&
                    (vDot > _verticalAngle1F || vDot < _verticalAngle2F)) return false;
                if (_verticalAngle1F < _verticalAngle2F &&
                    (vDot < _verticalAngle1F || vDot > _verticalAngle2F)) return false;
            }

            return true;
        }
        public void CalcAABB(out Bounds bounds)
        {
            bounds = new Bounds(
                _farSphere.Center,
                _farSphere.Radius * 2 * Vector3.one
            );
        }
        public bool Search(UtlOfProgram.IdentificationType identificationType, UtlOfProgram.ObjType searchObjType, Transform hdTransform, UtlOfProgram.LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum, List<ObjectSearchTgt> ignoreList)
        {
            CalcField(hdTransform);
            CalcAABB(out var bounds);
            return ObjectSearch.Inst.SearchField(
                _checkInFieldFunc ??= v => CheckInField(v), bounds, hdTransform, identificationType, searchObjType,
                lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, detectionComparatorType, detectionNum, teamNum,
                ignoreList);
        }
        public bool LockOn(UtlOfProgram.IdentificationType identificationType, UtlOfProgram.ObjType searchObjType, UtlOfProgram.LockOnDistancePriorityType lockOnDistancePriorityType, Transform hdTransform, UtlOfProgram.LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] results, List<ObjectSearchTgt> ignoreList)
        {
            CalcField(hdTransform);
            CalcAABB(out var bounds);
            results ??= new ObjectSearchTgt[12];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = null;
            }
            ObjectSearch.Inst.SearchFieldGet(_checkInFieldFunc ??= v => CheckInField(v), bounds, hdTransform, teamNum, identificationType,
                searchObjType, lockOnDistancePriorityType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, results, ignoreList);
            return results[0] != null;
        }
        public bool AssessTgtPos(Transform hdTransform, Vector3 lockOnTgtPos)
        {
            CalcField(hdTransform);
            return CheckInField(lockOnTgtPos);
        }
    }
}