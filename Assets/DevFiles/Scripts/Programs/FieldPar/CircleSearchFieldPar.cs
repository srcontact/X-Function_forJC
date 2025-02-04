using System;
using System.Collections.Generic;
using clrev01.ClAction.ObjectSearch;
using clrev01.Save;
using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    [Serializable]
    public class CircleSearchFieldPar : IFieldSearchObject
    {
        [MemoryPack.MemoryPackIgnore]
        public virtual float FarRadius { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float NearRadius { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float Angle { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float Rotate { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float Height { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual Vector3 Offset { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual bool Is2D { get; protected set; }

        private Func<Vector3, bool> _checkInFieldFunc;


        public bool CheckInField(Vector3 tgtPos)
        {
            if (!Is2D)
            {
                var ab = _minHeight - _maxHeight;
                var ac = tgtPos - _maxHeight;
                float e = Vector3.Dot(ac, ab);
                float f = Vector3.Dot(ab, ab);
                if (e < 0 || e > f) return false;
                float dist = Vector3.Dot(ac, ac) - e * e / f;
                if (dist < NearRadius * NearRadius || dist > FarRadius * FarRadius) return false;
            }

            Vector3 toTgtVector = new Vector3(tgtPos.x, 0, tgtPos.z) - new Vector3(_center.x, 0, _center.z);

            if (Angle < 360)
            {
                float tgtAngle = Vector3.Angle(_forwardDirection, toTgtVector);
                if (tgtAngle > Angle / 2) return false;
            }

            if (Is2D)
            {
                float dist = toTgtVector.sqrMagnitude;
                if (dist < NearRadius * NearRadius || dist > FarRadius * FarRadius) return false;
            }

            return true;
        }
        [NonSerialized]
        private Vector3 _center, _maxHeight, _minHeight, _forwardDirection;
        public void CalcField(Transform searcher)
        {
            var searcherForward = Vector3.ProjectOnPlane(Vector3.Cross(searcher.transform.right, Vector3.up), Vector3.up).normalized;
            var searcherRotationY = Quaternion.LookRotation(searcherForward, Vector3.up);
            var fieldRotate = Quaternion.Euler(0, Rotate, 0);
            var o = Offset;
            if (Is2D)
            {
                _center = searcherRotationY * o + searcher.position;
            }
            else
            {
                _center = searcherRotationY * o + searcher.position;
                _maxHeight = _minHeight = _center;
                _maxHeight.y += Height / 2;
                _minHeight.y -= Height / 2;
            }
            _forwardDirection = fieldRotate * searcherForward;
            _forwardDirection.y = 0;
            _forwardDirection = _forwardDirection.normalized;
        }
        public void CalcAABB(out Bounds bounds)
        {
            if (Angle >= 360)
            {
                bounds = new Bounds(_center, new Vector3(FarRadius * 2, Is2D ? 1000000 : Height, FarRadius * 2));
            }
            else
            {
                bounds = new Bounds(_center, new Vector3(0, Is2D ? 1000000 : Height, 0));
                var fAngle = Vector3.Angle(_forwardDirection, Vector3.forward);
                var rAngle = Vector3.Angle(_forwardDirection, Vector3.right);
                var bAngle = Vector3.Angle(_forwardDirection, Vector3.back);
                var lAngle = Vector3.Angle(_forwardDirection, Vector3.left);
                var halfAngle = Angle / 2;
                if (fAngle < halfAngle)
                {
                    bounds.Encapsulate(_center + Vector3.forward * FarRadius);
                }
                if (rAngle < halfAngle)
                {
                    bounds.Encapsulate(_center + Vector3.right * FarRadius);
                }
                if (bAngle < halfAngle)
                {
                    bounds.Encapsulate(_center + Vector3.back * FarRadius);
                }
                if (lAngle < halfAngle)
                {
                    bounds.Encapsulate(_center + Vector3.left * FarRadius);
                }
                bounds.Encapsulate(_center + Quaternion.Euler(0, halfAngle, 0) * _forwardDirection * FarRadius);
                bounds.Encapsulate(_center + Quaternion.Euler(0, -halfAngle, 0) * _forwardDirection * FarRadius);
            }
        }

        public bool LockOn(UtlOfProgram.IdentificationType identificationType, UtlOfProgram.ObjType searchObjType,
            UtlOfProgram.LockOnDistancePriorityType lockOnDistancePriorityType,
            Transform hdTransform, UtlOfProgram.LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] searched, List<ObjectSearchTgt> ignoreList)
        {
            CalcField(hdTransform);
            CalcAABB(out var bounds);
            for (int i = 0; i < searched.Length; i++)
            {
                searched[i] = null;
            }
            ObjectSearch.Inst.SearchFieldGet(
                _checkInFieldFunc ??= v => CheckInField(v), bounds, hdTransform, teamNum, identificationType, searchObjType, lockOnDistancePriorityType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, searched, ignoreList);
            return searched[0] != null;
        }

        public bool Search(UtlOfProgram.IdentificationType identificationType, UtlOfProgram.ObjType searchObjType,
            Transform hdTransform, UtlOfProgram.LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum,
            List<ObjectSearchTgt> ignoreList)
        {
            CalcField(hdTransform);
            CalcAABB(out var bounds);
            return ObjectSearch.Inst.SearchField(
                _checkInFieldFunc ??= v => CheckInField(v), bounds, hdTransform, identificationType, searchObjType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, detectionComparatorType, detectionNum, teamNum, ignoreList);
        }
        public bool AssessTgtPos(Transform hdTransform, Vector3 lockOnTgtPos)
        {
            CalcField(hdTransform);
            return CheckInField(lockOnTgtPos);
        }
    }
}