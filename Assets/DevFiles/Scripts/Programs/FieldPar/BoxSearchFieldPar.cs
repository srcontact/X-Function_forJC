using System;
using System.Collections.Generic;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using clrev01.Save;
using Dest.Math;
using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    [Serializable]
    public class BoxSearchFieldPar : IFieldSearchObject
    {
        [MemoryPack.MemoryPackIgnore]
        public virtual Vector3 Size { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual Vector3 Offset { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual Vector3 Rotate { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual float Rotate2d { get; protected set; }
        [MemoryPack.MemoryPackIgnore]
        public virtual bool Is2D { get; protected set; }

        public void CalcField(Transform searcher)
        {
            var searcherForward = Vector3.Cross(searcher.transform.right, Vector3.up);
            var searcherRotation = Quaternion.LookRotation(searcherForward, Vector3.up).eulerAngles;
            if (!Is2D)
            {
                Quaternion globalSearchRot =
                    (Quaternion.Euler(0, searcherRotation.y, 0) * Quaternion.Euler(Rotate)).normalized;
                Vector3 globalSearchPos = globalSearchRot * Offset + searcher.position;
                _box = new Box3(
                    globalSearchPos, globalSearchRot * Vector3.right,
                    globalSearchRot * Vector3.up, globalSearchRot * Vector3.forward, Size / 2);
            }
            else
            {
                var s = new Vector3(Size.x, 1000000, Size.z);
                var r = new Vector3(0, Rotate2d, 0);
                var o = new Vector3(Offset.x, 0, Offset.z);
                var globalSearchRot =
                    (Quaternion.Euler(0, searcherRotation.y, 0) * Quaternion.Euler(r)).normalized;
                var globalSearchPos = globalSearchRot * o + searcher.position;
                _box = new Box3(
                    globalSearchPos, globalSearchRot * Vector3.right,
                    globalSearchRot * Vector3.up, globalSearchRot * Vector3.forward, s / 2);
            }
        }
        [NonSerialized]
        private Box3 _box;
        public bool CheckInField(Vector3 tgtPos)
        {
            return _box.Contains(tgtPos);
        }
        [NonSerialized]
        private Vector3[] _vl;
        public void CalcAABB(out Bounds bounds)
        {
            _vl ??= new Vector3[8];
            _box.CalcVertices(_vl);
            bounds = AAB3.CreateFromPoints(_vl);
        }
        private Func<Vector3, bool> _checkInFieldFunc;
        public bool Search(UtlOfProgram.IdentificationType identificationType, UtlOfProgram.ObjType searchObjType, Transform hdTransform, UtlOfProgram.LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum, List<ObjectSearchTgt> ignoreList)
        {
            CalcField(hdTransform);
            if (_box.Extents.x.isZeroV3() &&
                _box.Extents.z.isZeroV3()) return false;
            CalcAABB(out var bounds);
            return ObjectSearch.Inst.SearchField(
                _checkInFieldFunc ??= v => CheckInField(v), bounds, hdTransform, identificationType, searchObjType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, detectionComparatorType, detectionNum, teamNum, ignoreList);
        }
        public bool LockOn(UtlOfProgram.IdentificationType identificationType, UtlOfProgram.ObjType searchObjType, UtlOfProgram.LockOnDistancePriorityType lockOnDistancePriorityType, Transform hdTransform, UtlOfProgram.LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] results, List<ObjectSearchTgt> ignoreList)
        {
            CalcField(hdTransform);
            if (_box.Extents.x.isZeroV3() &&
                _box.Extents.z.isZeroV3()) return false;
            CalcAABB(out var bounds);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = null;
            }
            ObjectSearch.Inst.SearchFieldGet(
                _checkInFieldFunc ??= v => CheckInField(v), bounds, hdTransform, teamNum, identificationType, searchObjType, lockOnDistancePriorityType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, results, ignoreList);
            return results[0] != null;
        }
        public bool AssessTgtPos(Transform hdTransform, Vector3 lockOnTgtPos)
        {
            CalcField(hdTransform);
            return CheckInField(lockOnTgtPos);
        }
    }
}