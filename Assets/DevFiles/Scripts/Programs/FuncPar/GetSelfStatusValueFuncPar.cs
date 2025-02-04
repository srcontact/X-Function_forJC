using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class GetSelfStatusValueFuncPar : FunctionFuncPar, IPGBFuncUnion, IGetStatusValueFuncPar
    {
        public override string BlockTypeStr => pgNodeName.getSelfStatus;
        public SelfStatusValueType statusType;
        public VariableDataNumericSet tgtVn = new() { };
        public VariableDataVector3Set tgtVv = new() { };
        public SearchTgtType aimingObjectType = SearchTgtType.Machine;
        public SpeedUnitType speedUnitType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (SelfStatusValueType* st = &statusType)
            fixed (SearchTgtType* aot = &aimingObjectType)
            fixed (SpeedUnitType* sut = &speedUnitType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_getSelfStatusValueFuncPar.statusType, pgNodeParDescription_getSelfStatusValueFuncPar.statusType);
                pgbepManager.SetPgbepEnum(typeof(SelfStatusValueType), (int*)st);

                if (statusType is SelfStatusValueType.NumberOfAimingMe)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_getSelfStatusValueFuncPar.aimingObjectType, pgNodeParDescription_getSelfStatusValueFuncPar.aimingObjectType);
                    pgbepManager.SetPgbepFlagsEnum(typeof(SearchTgtType), (long*)aot);
                }

                switch (statusType)
                {
                    case SelfStatusValueType.Speed:
                    case SelfStatusValueType.HorizontalSpeed:
                    case SelfStatusValueType.VerticalSpeed:
                        pgbepManager.SetHeaderText(pgNodeParameter_getSelfStatusValueFuncPar.speedUnit, pgNodeParDescription_getSelfStatusValueFuncPar.speedUnit);
                        pgbepManager.SetPgbepEnum(typeof(SpeedUnitType), (int*)sut);
                        break;
                }

                var useVector3dTgt = IsUseVectorVariable();
                tgtVn.useVariable = !useVector3dTgt;
                tgtVv.useVariable = useVector3dTgt;

                pgbepManager.SetHeaderText(pgNodeParameter_getSelfStatusValueFuncPar.targetVariable, pgNodeParDescription_getSelfStatusValueFuncPar.targetVariable);
                if (useVector3dTgt) pgbepManager.SetPgbepVariable(tgtVv, false);
                else pgbepManager.SetPgbepVariable(tgtVn, false);
            }
        }
        private bool IsUseVectorVariable()
        {
            switch (statusType)
            {
                case SelfStatusValueType.Position:
                case SelfStatusValueType.Position2D:
                case SelfStatusValueType.Rotation:
                case SelfStatusValueType.Rotation2D:
                case SelfStatusValueType.MoveVelocity:
                case SelfStatusValueType.MoveVelocity2D:
                case SelfStatusValueType.GroundNormal:
                case SelfStatusValueType.GroundInclinationDirection:
                case SelfStatusValueType.LandingPointNormal:
                    return true;
                default:
                    return false;
            }
        }

        public void GetStatusValue(MachineLD ld)
        {
            if (tgtVn.useVariable) GetNumericStatusValue(ld);
            else if (tgtVv.useVariable) GetVectorStatusValue(ld);
        }

        public void GetNumericStatusValue(MachineLD ld)
        {
            var hd = ld.hd;
            var rigidBody = hd.rigidBody;
            float res = 0;
            switch (statusType)
            {
                case SelfStatusValueType.Speed:
                    res = rigidBody.linearVelocity.magnitude * speedUnitType.GetSpeedUnitRatio();
                    break;
                case SelfStatusValueType.HorizontalSpeed:
                    var velocity = rigidBody.linearVelocity;
                    velocity.y = 0;
                    res = velocity.magnitude * speedUnitType.GetSpeedUnitRatio();
                    break;
                case SelfStatusValueType.VerticalSpeed:
                    res = rigidBody.linearVelocity.y * speedUnitType.GetSpeedUnitRatio();
                    break;
                case SelfStatusValueType.HealthPoint:
                    res = ld.HpRemaining;
                    break;
                case SelfStatusValueType.HpPercent:
                    res = ld.HpPercent;
                    break;
                case SelfStatusValueType.HeatValue:
                    res = ld.statePar.heat;
                    break;
                case SelfStatusValueType.HeatPercent:
                    res = ld.HeatPercent;
                    break;
                case SelfStatusValueType.ShieldHp:
                    res = ld.ShieldHpRemaining;
                    break;
                case SelfStatusValueType.ShieldHpPercent:
                    res = ld.ShieldHpPercent;
                    break;
                case SelfStatusValueType.ImpactValue:
                    res = ld.statePar.impact;
                    break;
                case SelfStatusValueType.ImpactPercent:
                    res = ld.ImpactPercent;
                    break;
                case SelfStatusValueType.NumberOfAimingMe:
                    if (hd.objectSearchTgt == null && hd.objectSearchTgt.aimingAtMeDict == null) res = 0;
                    else
                    {
                        foreach (var y in hd.objectSearchTgt.aimingAtMeDict)
                        {
                            if ((y.Value.ObjectSearchType & aimingObjectType) != 0) res++;
                        }
                    }
                    break;
                case SelfStatusValueType.GroundInclinationAngle:
                    res = Vector3.Angle(ld.movePar.groundNormal, Vector3.up);
                    break;
                case SelfStatusValueType.LandingFrame:
                    var raycastResult1 = Physics.Raycast(hd.pos, rigidBody.linearVelocity.normalized, out var raycastHit1, 4000, layerOfGround);
                    res = raycastResult1 ? raycastHit1.distance / rigidBody.linearVelocity.magnitude * 60 : 4000 / rigidBody.linearVelocity.magnitude * 60;
                    break;
            }
            tgtVn.SetNumericValue(ld, res);
        }

        public void GetVectorStatusValue(MachineLD ld)
        {
            var hd = ld.hd;
            var rigidBody = hd.rigidBody;
            Vector3 res = new Vector3();
            switch (statusType)
            {
                case SelfStatusValueType.Position:
                    res = hd.pos;
                    break;
                case SelfStatusValueType.Position2D:
                    var pos = hd.pos;
                    pos.y = 0;
                    res = pos;
                    break;
                case SelfStatusValueType.Rotation:
                    res = ld.hd.rot.eulerAngles;
                    break;
                case SelfStatusValueType.Rotation2D:
                    var rot = ld.hd.rot.eulerAngles;
                    rot.x = rot.z = 0;
                    res = rot;
                    break;
                case SelfStatusValueType.MoveVelocity:
                    res = rigidBody.linearVelocity;
                    break;
                case SelfStatusValueType.MoveVelocity2D:
                    var velocity = rigidBody.linearVelocity;
                    velocity.y = 0;
                    res = velocity;
                    break;
                case SelfStatusValueType.GroundNormal:
                    res = ld.movePar.groundNormal;
                    break;
                case SelfStatusValueType.GroundInclinationDirection:
                    res = -Vector3.ProjectOnPlane(ld.movePar.groundNormal, Vector3.up).normalized;
                    break;
                case SelfStatusValueType.LandingPointNormal:
                    var raycastResult1 = Physics.Raycast(hd.pos, rigidBody.linearVelocity.normalized, out var raycastHit1, 4000, layerOfGround);
                    res = raycastResult1 ? raycastHit1.normal : Vector3.zero;
                    break;
            }
            tgtVv.SetVector3dValue(ld, res);
        }

        public override string[] GetNodeFaceText()
        {
            var str1 = statusType switch
            {
                SelfStatusValueType.Speed or SelfStatusValueType.HorizontalSpeed or SelfStatusValueType.VerticalSpeed => $"\nSU:{speedUnitType}",
                SelfStatusValueType.NumberOfAimingMe => $"\nAOT:{GetEnumFlagText(typeof(SearchTgtType), (long)aimingObjectType, 2)}",
                _ => ""
            };
            var str2 = $"\nTgtV:[{(IsUseVectorVariable() ? tgtVv.name : tgtVn.name)}]";
            return new[] { $"ST:{statusType}{str1}{str2}" };
        }
    }
}