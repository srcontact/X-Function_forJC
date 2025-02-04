using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Save.VariableData;
using System;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Save
{
    public static class UtlOfVariable
    {
        public static bool IsListVariableType(this VariableType variableType) => (int)variableType >= (int)VariableType.NumericList;

        // public static unsafe void IndicateWithTgtNum(this IVariableData variableData, PgbepManager pgbepManager, int* targetNumber, VariableDataNumericGet tgtNumberV = null)
        // {
        //     pgbepManager.SetPgbepVariable(variableData, false, variableData.selectableVariableTypes);
        //     if (!variableData.variableType.IsListVariableType()) return;
        //     if (tgtNumberV is { useVariable: true }) pgbepManager.SetPgbepVariable(tgtNumberV, true);
        //     else pgbepManager.SetPgbepInt(targetNumber, null, tgtNumberV);
        // }

        public static string GetIndexStr(this VariableData.VariableData variableData, VariableDataNumericGet indexV, int index)
        {
            if (!variableData.useVariable) return null;
            var indexStr = indexV != null ? $"[{(indexV.useVariable ? indexV.name : index.ToString())}]" : null;
            return indexStr;
        }

        public static Type GetVariableValueType(this VariableType variableType)
        {
            return variableType switch
            {
                VariableType.Numeric => typeof(VariableValueNumeric),
                VariableType.Vector3D => typeof(VariableValueVector3),
                VariableType.LockOn => typeof(VariableValueLockOn),
                VariableType.NumericList => typeof(VariableValueNumericList),
                VariableType.Vector3DList => typeof(VariableValueVector3DList),
                VariableType.LockOnList => typeof(VariableValueLockOnList),
                _ => throw new ArgumentOutOfRangeException(nameof(variableType), variableType, null)
            };
        }

        public static float? CalcNumeric(MachineLD ld, CalcNumericOperatorType operatorType, float p1, float? p2)
        {
            float? res = null;
            switch (operatorType)
            {
                case CalcNumericOperatorType.Assignment:
                    res = p1;
                    break;
                case CalcNumericOperatorType.Addition:
                    res = p1 + p2;
                    break;
                case CalcNumericOperatorType.Subtraction:
                    res = p1 - p2;
                    break;
                case CalcNumericOperatorType.Multiplication:
                    res = p1 * p2;
                    break;
                case CalcNumericOperatorType.Division:
                    if (p2 == 0) res = 0;
                    else res = p1 / p2;
                    break;
                case CalcNumericOperatorType.Modulo:
                    if (p2 == 0) res = p1;
                    else res = p1 % p2;
                    break;
                case CalcNumericOperatorType.RoundedDown:
                    res = Mathf.Floor(p1);
                    break;
                case CalcNumericOperatorType.Absolute:
                    res = Mathf.Abs(p1);
                    break;
                case CalcNumericOperatorType.Max:
                    if (p2 != null) res = Mathf.Max(p1, p2.Value);
                    break;
                case CalcNumericOperatorType.Min:
                    if (p2 != null) res = Mathf.Min(p1, p2.Value);
                    break;
                case CalcNumericOperatorType.Square:
                    res = Mathf.Sqrt(p1);
                    break;
                case CalcNumericOperatorType.Sin:
                    res = Mathf.Sin(p1);
                    break;
                case CalcNumericOperatorType.Cos:
                    res = Mathf.Cos(p1);
                    break;
                case CalcNumericOperatorType.Tan:
                    res = Mathf.Tan(p1);
                    break;
                case CalcNumericOperatorType.Atan:
                    res = Mathf.Atan(p1);
                    break;
                case CalcNumericOperatorType.Not:
                    res = ~(int)p1;
                    break;
                case CalcNumericOperatorType.And:
                    res = (int)p1 & (int)p2;
                    break;
                case CalcNumericOperatorType.Or:
                    res = (int)p1 | (int)p2;
                    break;
                case CalcNumericOperatorType.Xor:
                    res = (int)p1 ^ (int)p2;
                    break;
                case CalcNumericOperatorType.Random:
                    if (p2 != null) res = UnityEngine.Random.Range(p1, p2.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return res;
        }

        public static (float? resN, Vector3? resV) CalcVector3d(MachineLD ld, CalcVector3dOperatorType calcVector3dOperatorType, Vector3 pv1, Vector3? pv2, float? pn2)
        {
            float? resN = null;
            Vector3? resV = null;
            switch (calcVector3dOperatorType)
            {
                case CalcVector3dOperatorType.Assignment:
                    resV = pv1;
                    break;
                case CalcVector3dOperatorType.Addition:
                    resV = pv1 + pv2;
                    break;
                case CalcVector3dOperatorType.Subtraction:
                    resV = pv1 - pv2;
                    break;
                case CalcVector3dOperatorType.MultiplicationNumeric:
                    resV = pv1 * pn2;
                    break;
                case CalcVector3dOperatorType.MultiplicationVector3D:
                    if (pv2 != null) resV = Vector3.Scale(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Division:
                    resV = pv1 / pn2;
                    break;
                case CalcVector3dOperatorType.Max:
                    if (pv2 != null) resV = Vector3.Max(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Min:
                    if (pv2 != null) resV = Vector3.Min(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Normalize:
                    resV = pv1.normalized;
                    break;
                case CalcVector3dOperatorType.Magnitude:
                    resN = pv1.magnitude;
                    break;
                case CalcVector3dOperatorType.Angle:
                    if (pv2 != null) resN = Vector3.Angle(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Distance:
                    if (pv2 != null) resN = Vector3.Distance(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Project:
                    if (pv2 != null) resV = Vector3.Project(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.ProjectOnPlane:
                    if (pv2 != null) resV = Vector3.ProjectOnPlane(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Cross:
                    if (pv2 != null) resV = Vector3.Cross(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.Dot:
                    if (pv2 != null) resN = Vector3.Dot(pv1, pv2.Value);
                    break;
                case CalcVector3dOperatorType.ConvertPointGlobalToLocal:
                    resV = ld.hd.transform.InverseTransformPoint(pv1);
                    break;
                case CalcVector3dOperatorType.ConvertDirectionGlobalToLocal:
                    resV = ld.hd.transform.InverseTransformDirection(pv1);
                    break;
                case CalcVector3dOperatorType.ConvertVectorGlobalToLocal:
                    resV = ld.hd.transform.InverseTransformVector(pv1);
                    break;
                case CalcVector3dOperatorType.ConvertPointLocalToGlobal:
                    resV = ld.hd.transform.TransformPoint(pv1);
                    break;
                case CalcVector3dOperatorType.ConvertDirectionLocalToGlobal:
                    resV = ld.hd.transform.TransformDirection(pv1);
                    break;
                case CalcVector3dOperatorType.ConvertVectorLocalToGlobal:
                    resV = ld.hd.transform.TransformVector(pv1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return (resN, resV);
        }

        public static ObjectSearchTgt ManageLockOn(MachineLD ld, ManageLockOnOperatorType operatorType, ObjectSearchTgt p1)
        {
            switch (operatorType)
            {
                case ManageLockOnOperatorType.Assignment:
                    return p1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operatorType), operatorType, null);
            }
        }

        public static float GetTargetNumericStatusValue(MachineLD ld, TgtStatusValueType tgtStatusValueType, ObjectSearchTgt tgt, SpeedUnitType speedUnitType, SearchTgtType aimingObjectType)
        {
            float res = 0;
            switch (tgtStatusValueType)
            {
                case TgtStatusValueType.Speed:
                    res = tgt != null ? GetTargetVelocity(ld, tgt).magnitude * speedUnitType.GetSpeedUnitRatio() : 0;
                    break;
                case TgtStatusValueType.HorizontalSpeed:
                    var speed = tgt != null ? GetTargetVelocity(ld, tgt) : Vector3.zero;
                    speed.y = 0;
                    res = speed.magnitude * speedUnitType.GetSpeedUnitRatio();
                    break;
                case TgtStatusValueType.VerticalSpeed:
                    res = tgt != null ? GetTargetVelocity(ld, tgt).y * speedUnitType.GetSpeedUnitRatio() : 0;
                    break;
                case TgtStatusValueType.RelativeSpeed:
                    res = tgt != null ? (GetTargetVelocity(ld, tgt) - ld.hd.rigidBody.linearVelocity).magnitude * speedUnitType.GetSpeedUnitRatio() : 0;
                    break;
                case TgtStatusValueType.RelativeHorizontalSpeed:
                    var speed2d = tgt != null ? GetTargetVelocity(ld, tgt) - ld.hd.rigidBody.linearVelocity : Vector3.zero;
                    speed2d.y = 0;
                    res = speed2d.magnitude * speedUnitType.GetSpeedUnitRatio();
                    break;
                case TgtStatusValueType.RelativeVerticalSpeed:
                    res = tgt != null ? (GetTargetVelocity(ld, tgt).y - ld.hd.rigidBody.linearVelocity.y) * speedUnitType.GetSpeedUnitRatio() : 0;
                    break;
                case TgtStatusValueType.HealthPoint:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.HpRemaining;
                                break;
                            case BulletHD bhd:
                                res = bhd.ld.HpRemaining;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.HpPercent:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.HpPercent;
                                break;
                            case BulletHD bhd:
                                res = bhd.ld.HpPercent;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.HeatValue:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.statePar.heat;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.HeatPercent:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.HeatPercent;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.ShieldHp:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.ShieldHpRemaining;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.ShieldHpPercent:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.ShieldHpPercent;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.ImpactValue:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.statePar.impact;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.ImpactPercent:
                    if (tgt == null) res = 0;
                    else
                    {
                        switch (tgt.hardBase)
                        {
                            case MachineHD mhd:
                                res = mhd.ld.ImpactPercent;
                                break;
                            default:
                                res = 0;
                                break;
                        }
                    }
                    break;
                case TgtStatusValueType.NumberOfAimingTgt:
                    if (tgt == null) res = 0;
                    else if (tgt.hardBase.objectSearchTgt == null && tgt.hardBase.objectSearchTgt.aimingAtMeDict == null) res = 0;
                    else
                    {
                        res = 0;
                        foreach (var y in tgt.hardBase.objectSearchTgt.aimingAtMeDict)
                        {
                            if ((y.Value.ObjectSearchType & aimingObjectType) != 0) res++;
                        }
                    }
                    break;
                case TgtStatusValueType.LandingFrame:
                    if (tgt == null) res = 0;
                    else
                    {
                        var targetVelocity = GetTargetVelocity(ld, tgt);
                        var raycastResult2 = Physics.Raycast(tgt.hardBase.pos, targetVelocity.normalized, out var raycastHit2, 4000, layerOfGround);
                        res = raycastResult2 ? raycastHit2.distance / targetVelocity.magnitude * 60 : 4000 / targetVelocity.magnitude * 60;
                    }
                    break;
                case TgtStatusValueType.DistanceToTgt:
                    if (tgt == null) res = 0;
                    else
                    {
                        res = Vector3.Distance(ld.hd.pos, tgt.pos);
                    }
                    break;
                case TgtStatusValueType.PositionMeasurementAccuracy:
                    if (tgt == null) res = 0;
                    else res = GetPosAccuracy(ld, tgt);
                    break;
            }
            return res;
        }

        public static Vector3 GetTargetVectorStatusValue(MachineLD ld, TgtStatusValueType tgtStatusValueType, ObjectSearchTgt tgt)
        {
            var res = new Vector3();
            switch (tgtStatusValueType)
            {
                case TgtStatusValueType.Position:
                    res = tgt != null ? tgt.GetTargetPosition(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, GetPosAccuracy(ld, tgt)) : Vector3.zero;
                    break;
                case TgtStatusValueType.Position2D:
                    res = tgt != null ? tgt.GetTargetPosition(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, GetPosAccuracy(ld, tgt)) : Vector3.zero;
                    res.y = 0;
                    break;
                case TgtStatusValueType.RelativePosition:
                    res = tgt != null ? tgt.GetTargetPosition(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, GetPosAccuracy(ld, tgt)) - ld.hd.pos : Vector3.zero;
                    break;
                case TgtStatusValueType.RelativePosition2D:
                    res = tgt != null ? tgt.GetTargetPosition(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, GetPosAccuracy(ld, tgt)) - ld.hd.pos : Vector3.zero;
                    res.y = 0;
                    break;
                case TgtStatusValueType.Rotation:
                    res = tgt.hardBase.rot.eulerAngles;
                    break;
                case TgtStatusValueType.Rotation2D:
                    var rot = tgt.hardBase.rot.eulerAngles;
                    rot.x = rot.z = 0;
                    res = rot;
                    break;
                case TgtStatusValueType.MoveVelocity:
                    res = tgt != null ? GetTargetVelocity(ld, tgt) : Vector3.zero;
                    break;
                case TgtStatusValueType.MoveVelocity2D:
                    res = tgt != null ? GetTargetVelocity(ld, tgt) : Vector3.zero;
                    res.y = 0;
                    break;
                case TgtStatusValueType.RelativeMoveVelocity:
                    res = tgt != null ? GetTargetVelocity(ld, tgt) - ld.hd.rigidBody.linearVelocity : Vector3.zero;
                    break;
                case TgtStatusValueType.RelativeMoveVelocity2D:
                    res = tgt != null ? GetTargetVelocity(ld, tgt) - ld.hd.rigidBody.linearVelocity : Vector3.zero;
                    res.y = 0;
                    break;
                case TgtStatusValueType.LandingPointNormal:
                    if (tgt == null) res = Vector3.zero;
                    else
                    {
                        var posAccuracy = GetPosAccuracy(ld, tgt);
                        var raycastResult2 = Physics.Raycast(tgt.GetTargetPosition(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, posAccuracy), tgt.GetTargetVelocity(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, posAccuracy).normalized, out var raycastHit2, 4000, layerOfGround);
                        res = raycastResult2 ? raycastHit2.normal : Vector3.zero;
                    }
                    break;
            }
            return res;
        }
        
        private static Vector3 GetTargetVelocity(MachineLD ld, ObjectSearchTgt tgt)
        {
            return tgt.GetTargetVelocity(ld.hd.teamID, ld.hd.uniqueID, ld.searchParameterData, GetPosAccuracy(ld, tgt));
        }
        private static float GetPosAccuracy(MachineLD ld, ObjectSearchTgt tgt)
        {
            return tgt.GetPosAccuracy(ld.hd.pos, ld.searchParameterData, ld.hd.objectSearchTgt.jammedSize);
        }
    }
}