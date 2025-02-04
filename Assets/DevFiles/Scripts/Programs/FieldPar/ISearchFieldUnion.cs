using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.Save;
using MemoryPack;
using System;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FieldPar
{
    [MemoryPackable()]
    [MemoryPackUnion(0, typeof(BoxSearchFieldParVariable))]
    [MemoryPackUnion(1, typeof(CircleSearchFieldParVariable))]
    [MemoryPackUnion(2, typeof(SphereSearchFieldParVariable))]
    public partial interface ISearchFieldUnion : IFieldEditObject, IFieldSearchObject
    {
        bool Search(MachineLD ld, IdentificationType identificationType, ObjType searchObjType, Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum, List<ObjectSearchTgt> ignoreList);
        bool LockOn(MachineLD ld, IdentificationType identificationType, ObjType searchObjType, LockOnDistancePriorityType lockOnDistancePriorityType,
            Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] results, List<ObjectSearchTgt> ignoreList);
        bool AssessTgtPos(MachineLD ld, Transform hdTransform, Vector3 lockOnTgtPos);
        void GetValueFromVariable(MachineLD ld);
    }
}