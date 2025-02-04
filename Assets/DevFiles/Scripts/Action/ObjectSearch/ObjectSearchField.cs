using clrev01.Programs;
using clrev01.Save;
using System;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.ClAction.ObjectSearch
{
    public partial class ObjectSearch
    {
        public bool SearchField(
            Func<Vector3, bool> checker,
            Bounds bounds,
            Transform searcher,
            IdentificationType identificationType,
            ObjType objType,
            LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf,
            ComparatorType detectionComparatorType,
            int detectionNum,
            int teamId,
            List<ObjectSearchTgt> ignoreList)
        {
            return SearchField(checker, bounds, searcher, identificationType, teamId, objType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, (detectionComparatorType, detectionNum), null, ignoreList);
        }
        public bool SearchFieldGet(
            Func<Vector3, bool> checker,
            Bounds bounds,
            Transform searcher,
            int teamId,
            IdentificationType identificationType,
            ObjType objType,
            LockOnDistancePriorityType lockOnDistancePriorityType,
            LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf,
            ObjectSearchTgt[] resultList,
            List<ObjectSearchTgt> ignoreList)
        {
            return SearchField(checker, bounds, searcher, identificationType, teamId, objType, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, null, (lockOnDistancePriorityType, resultList), ignoreList);
        }

        private readonly SearchTgtType[] _searchTypes = (SearchTgtType[])Enum.GetValues(typeof(SearchTgtType));
        private readonly List<(uint min, uint max)> _rangeList = new();

        private readonly (ObjectSearchTgt tgt, float sqrLength)?[] _lockOnTempArray = new (ObjectSearchTgt, float)?[2048];
        bool SearchField(
            Func<Vector3, bool> checker,
            Bounds bounds,
            Transform searcher,
            IdentificationType identificationType,
            int teamId,
            ObjType objType,
            LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf,
            //サーチのみ
            (ComparatorType detectionComparatorType, int detectionNum)? searchPar,
            //ロックオンのみ
            (LockOnDistancePriorityType lockOnDistancePriorityType, ObjectSearchTgt[] resultList)? lockOnPar,
            List<ObjectSearchTgt> ignoreList
        )
        {
#if UNITY_EDITOR
            drawBounds.Add(bounds);
#endif
            var searchTgt = GetSearchTgtType(objType);
            _rangeList.Clear();
            SettingMortonOrder(bounds, _rangeList);
            var dot = Mathf.Cos(angleOfMovementToSelf * Mathf.Deg2Rad);
            if (lockOnPar is { resultList: not null })
            {
                for (var i = 0; i < _lockOnTempArray.Length; i++)
                {
                    if (_lockOnTempArray[i] == null) break;
                    _lockOnTempArray[i] = null;
                }
            }
            var searchedNum = 0;

            foreach (var range in _rangeList)
            {
                foreach (var st in _searchTypes)
                {
                    if ((searchTgt & st) != st) continue;
                    var searchList = _searchListDict[st];
                    for (var i = range.min; i <= range.max; i++)
                    {
                        var chunk = searchList[i];
                        if (chunk is not { notNullCount: > 0 }) continue;
                        for (var j = 0; j < chunk.count; j++)
                        {
                            var tgt = chunk[j];
                            if (tgt == null) continue;
                            if (tgt.gameObject == searcher.gameObject) continue;
                            if (ignoreList != null && ignoreList.Contains(tgt)) continue;
                            if (teamId >= 0)
                            {
                                //todo:不明機(Unknown)の判定を追加する場合はここに処理を追加する必要がある。多分。
                                if (((identificationType & IdentificationType.Enemy) != IdentificationType.Enemy || tgt.hardBase.teamID == teamId) &&
                                    ((identificationType & IdentificationType.Friend) != IdentificationType.Friend || tgt.hardBase.teamID != teamId)) continue;
                            }
                            var tgtPos = tgt.pos;
                            var toTgtVector = searcher.position - tgtPos;
                            if (tgt.hardBase.objectSearchTgt.ObjectSearchType is not SearchTgtType.Shield)
                            {
                                switch (lockOnAngleOfMovementToSelfType)
                                {
                                    case LockOnAngleOfMovementToSelfType.SmallerThan:
                                        if (Vector3.Dot(toTgtVector.normalized, tgt.movementVector) < dot) continue;
                                        break;
                                    case LockOnAngleOfMovementToSelfType.BiggerThan:
                                        if (Vector3.Dot(toTgtVector.normalized, tgt.movementVector) > dot) continue;
                                        break;
                                }
                            }
                            if (!checker.Invoke(tgtPos)) continue;

                            searchedNum += 1;
                            if (searchPar.HasValue)
                            {
                                switch (searchPar.Value.detectionComparatorType)
                                {
                                    case ComparatorType.EqualTo:
                                        if (searchPar.Value.detectionNum < searchedNum) return false;
                                        break;
                                    case ComparatorType.Over:
                                        if (searchPar.Value.detectionNum <= searchedNum) return true;
                                        break;
                                    case ComparatorType.GreaterThan:
                                        if (searchPar.Value.detectionNum < searchedNum) return true;
                                        break;
                                    case ComparatorType.Under:
                                        if (searchPar.Value.detectionNum < searchedNum) return false;
                                        break;
                                    case ComparatorType.LessThan:
                                        if (searchPar.Value.detectionNum <= searchedNum) return false;
                                        break;
                                }
                            }
                            else if (lockOnPar.HasValue)
                            {
                                if (searchedNum - 1 >= 0 && searchedNum - 1 < _lockOnTempArray.Length) _lockOnTempArray[searchedNum - 1] = (tgt, toTgtVector.sqrMagnitude);
                            }
                        }
                    }
                }
            }
            if (searchPar.HasValue)
            {
                switch (searchPar.Value.detectionComparatorType)
                {
                    case ComparatorType.EqualTo:
                        return searchPar.Value.detectionNum == searchedNum;
                    case ComparatorType.Over:
                        return searchPar.Value.detectionNum <= searchedNum;
                    case ComparatorType.GreaterThan:
                        return searchPar.Value.detectionNum < searchedNum;
                    case ComparatorType.Under:
                        return searchPar.Value.detectionNum >= searchedNum;
                    case ComparatorType.LessThan:
                        return searchPar.Value.detectionNum > searchedNum;
                    default:
                        return false;
                }
            }
            if (lockOnPar.HasValue)
            {
                var addedLength = lockOnPar.Value.lockOnDistancePriorityType == LockOnDistancePriorityType.Near ? float.MinValue : float.MaxValue;
                for (var i = 0; i < lockOnPar.Value.resultList.Length; i++)
                {
                    (ObjectSearchTgt tgt, float sqrLength) t = (null, lockOnPar.Value.lockOnDistancePriorityType == LockOnDistancePriorityType.Near ? float.MaxValue : float.MinValue);
                    foreach (var tgt in _lockOnTempArray)
                    {
                        if (!tgt.HasValue) break;
                        switch (lockOnPar.Value.lockOnDistancePriorityType)
                        {
                            case LockOnDistancePriorityType.Near:
                                if (tgt.Value.sqrLength < t.sqrLength && tgt.Value.sqrLength > addedLength) t = tgt.Value;
                                break;
                            case LockOnDistancePriorityType.Far:
                                if (tgt.Value.sqrLength > t.sqrLength && tgt.Value.sqrLength < addedLength) t = tgt.Value;
                                break;
                        }
                    }
                    if (t.tgt != null)
                    {
                        lockOnPar.Value.resultList[i] = t.tgt;
                        addedLength = t.sqrLength;
                    }
                    else
                    {
                        return lockOnPar.Value.resultList.Length > 0;
                    }
                }
            }
            return false;
        }
        private SearchTgtType GetSearchTgtType(ObjType objType)
        {
            SearchTgtType searchTgt = 0;
            if ((objType & ObjType.Machine) > 0)
            {
                searchTgt |= SearchTgtType.Machine;
            }
            if ((objType & ObjType.Bullet) == ObjType.Bullet)
            {
                searchTgt |= SearchTgtType.Bullet;
            }
            if ((objType & ObjType.Missile) == ObjType.Missile)
            {
                searchTgt |= SearchTgtType.Missile;
            }
            if ((objType & ObjType.Mine) == ObjType.Mine)
            {
                searchTgt |= SearchTgtType.Mine;
            }
            if((objType & ObjType.AerialSmallObject) == ObjType.AerialSmallObject)
            {
                searchTgt |= SearchTgtType.AerialSmallObject;
            }
            if ((objType & ObjType.Shield) == ObjType.Shield)
            {
                searchTgt |= SearchTgtType.Shield;
            }
            return searchTgt;
        }
        private void SettingMortonOrder(Bounds bounds, List<(uint, uint)> rangeList)
        {
            GetMortonMinMax(bounds, out var mortonMax, out var mortonMin);
            GetSearchRange(mortonMin, mortonMax, rangeList);
        }
        private void GetMortonMinMax(Bounds bounds, out uint mortonMax, out uint mortonMin)
        {
            mortonMax = CalcMortonNum(bounds.max);
            if (mortonMax > indexNum - 1) mortonMax = (uint)indexNum - 1;
            mortonMin = CalcMortonNum(bounds.min);
        }
        private readonly Stack<(uint min, uint max)> _tempQueue = new();
        private void GetSearchRange(uint mortonMin, uint mortonMax, ICollection<(uint, uint)> rangeList)
        {
            _tempQueue.Clear();
            _tempQueue.Push((mortonMin, mortonMax));
            while (_tempQueue.Count > 0)
            {
                var v = _tempQueue.Pop();
                var checkNeedDivision = v.min ^ v.max;
                if (CheckEndSplit(v.min, v.max, checkNeedDivision))
                {
                    rangeList.Add(v);
                    continue;
                }
                SplitInfoCalc(checkNeedDivision, out var layer, out var uintPow);
                InverseLookUpMortonNumber(v.min, out var minV);
                InverseLookUpMortonNumber(v.max, out var maxV);
                SplitExe(v.min, v.max, _tempQueue, layer, minV, maxV, uintPow);
            }
        }
        private bool CheckEndSplit(uint mortonMin, uint mortonMax, uint checkNeedDivision)
        {
            if (mortonMin == mortonMax)
            {
                return true;
            }
            for (var i = divisionNum * 3; i >= 0; i--)
            {
                var oneOnly = Uint2Pow(i) - 1;
                if (checkNeedDivision != oneOnly || (mortonMax & oneOnly) != oneOnly) continue;
                return true;
            }
            return false;
        }
        private void SplitInfoCalc(uint checkNeedDivision, out int layer, out uint uintPow)
        {
            layer = 0;
            for (var i = 0; i < divisionNum * 3; i++)
            {
                if (checkNeedDivision >> i == 0) break;
                layer = i;
            }
            var nowDivNum = layer / 3;
            uintPow = Uint2Pow(nowDivNum);
        }
        private void SplitExe(uint min, uint max, Stack<(uint min, uint max)> tempQueue, int layer, Vector3u minV, Vector3u maxV, uint uintPow)
        {
            uint litMax = min, bigMin = max;
            switch (layer % 3)
            {
                case 0:
                    for (var x = minV.x; x < maxV.x; x++)
                    {
                        if ((x + 1) % uintPow != 0) continue;
                        litMax = LookUpMortonNumber(x, maxV.y, maxV.z);
                        bigMin = LookUpMortonNumber(x + 1, minV.y, minV.z);
                    }
                    break;
                case 1:
                    for (var z = minV.z; z < maxV.z; z++)
                    {
                        if ((z + 1) % uintPow != 0) continue;
                        litMax = LookUpMortonNumber(maxV.x, maxV.y, z);
                        bigMin = LookUpMortonNumber(minV.x, minV.y, z + 1);
                    }
                    break;
                case 2:
                    for (var y = minV.y; y < maxV.y; y++)
                    {
                        if ((y + 1) % uintPow != 0) continue;
                        litMax = LookUpMortonNumber(maxV.x, y, maxV.z);
                        bigMin = LookUpMortonNumber(minV.x, y + 1, minV.z);
                    }
                    break;
            }
            tempQueue.Push((bigMin, max));
            tempQueue.Push((min, litMax));
        }

        private readonly uint[] _uint2PowArray = { 1u, 2u, 4u, 8u, 16u, 32u, 64u, 128u, 256u, 512u, 1024u, 2048u, 4096u };
        private uint Uint2Pow(int p)
        {
            return _uint2PowArray[p];
        }
    }
}