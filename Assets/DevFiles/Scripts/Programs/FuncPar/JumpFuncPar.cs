using static I2.Loc.ScriptLocalization;
using clrev01.ClAction;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.PGBEditor.PGBEPanel;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class JumpFuncPar : MoveTypeFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.jump;
        protected override float? iconRotateValue =>
            directionSettingType switch
            {
                DirectionSettingType.Angles => horizontalAngleV.useVariable ? 0 : horizontalAngleV.constValue,
                DirectionSettingType.Vector3D => vectorV.useVariable ? 0 : CalcHorizontalAngle(vectorV.constValue),
                _ => throw new ArgumentOutOfRangeException()
            };
        protected override string iconRotateString =>
            directionSettingType switch
            {
                DirectionSettingType.Angles => $"{horizontalAngleV.GetIndicateStr()}°",
                DirectionSettingType.Vector3D => vectorV.GetIndicateStr(),
                _ => throw new ArgumentOutOfRangeException()
            };
        protected override float? powerGaugeValue => powerV.GetGaugeValue(0.01f);
        protected override string powerText => powerV.GetIndicateStr("%", 0.01f);
        protected override float? verticalRotateValue =>
            90 - directionSettingType switch
            {
                DirectionSettingType.Angles => verticalAngleV.useVariable ? 0 : verticalAngleV.constValue,
                DirectionSettingType.Vector3D => vectorV.useVariable ? 0 : CalcVerticalAngle(vectorV.constValue),
                _ => throw new ArgumentOutOfRangeException()
            };
        protected override string verticalRotateText =>
            directionSettingType switch
            {
                DirectionSettingType.Angles => verticalAngleV.GetIndicateStr("°"),
                DirectionSettingType.Vector3D => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        public DirectionSettingType directionSettingType;
        public VariableDataNumericGet horizontalAngleV = new();
        public VariableDataNumericGet verticalAngleV = new() { constValue = 45 };
        public VariableDataVector3Get vectorV = new() { constValue = Vector3.forward };
        public VariableDataNumericGet powerV = new() { constValue = 100 };
        public JumpAngleReference angleReference;
        public CoordinateSystemType coordinateSystemType;
        private int _endFrame;

        public enum JumpAngleReference
        {
            Global,
            SlopeOfGround
        }


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (DirectionSettingType* dst = &directionSettingType)
            fixed (JumpAngleReference* ar = &angleReference)
            fixed (CoordinateSystemType* cst = &coordinateSystemType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_jumpFuncPar.direction, pgNodeParDescription_jumpFuncPar.direction);
                pgbepManager.SetPgbepEnum(typeof(DirectionSettingType), (int*)dst);
                pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                switch (directionSettingType)
                {
                    case DirectionSettingType.Angles:
                        var jumpMinVerticalAngle = MHUB.GetData(PGEM2.nowEditCD.mechCustom.machineCode).machineCD.MoveCommonPar.jumpMinVerticalAngle;
                        var angleLimit360 = new AngleLimit(-360, 360);
                        var angleLimit180 = new AngleLimit(jumpMinVerticalAngle, 180f - jumpMinVerticalAngle);
                        pgbepManager.SetPgbepAngle(hAngleLimit: angleLimit360,
                            vAngleLimit: angleReference == JumpAngleReference.SlopeOfGround ? angleLimit180 : angleLimit360,
                            hVd: horizontalAngleV,
                            vVd: verticalAngleV
                        );
                        break;
                    case DirectionSettingType.Vector3D:
                        vectorV.IndicateSwitchable(pgbepManager);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                pgbepManager.SetHeaderText(pgNodeParameter_jumpFuncPar.angleReference, pgNodeParDescription_jumpFuncPar.angleReference);
                pgbepManager.SetPgbepEnum(typeof(JumpAngleReference), (int*)ar);
                pgbepManager.SetHeaderText(pgNodeParameter_jumpFuncPar.power, pgNodeParDescription_jumpFuncPar.power);
                powerV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 100));
            }
        }

        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            _endFrame = ActionManager.Inst.actionFrame;
        }
        public override bool ActionExecute(MachineLD ld)
        {
            if (ld.movePar.moveState != CharMoveState.isGrounded) return true;
            var v = MoveDirection(ld);
            var pow = powerV.GetUseValueFloat(ld, 0, 100) / 100;
            ld.movePar.ActJump(v, pow, angleReference, ld.cd.MoveCommonPar.jumpMinVerticalAngle, ld.CalcWeightPar());
            return false;
        }

        public override Vector3 MoveDirection(MachineLD ld)
        {
            Vector3 v;
            var jumpMinVerticalAngle = ld.cd.MoveCommonPar.jumpMinVerticalAngle;
            switch (directionSettingType)
            {
                case DirectionSettingType.Angles:
                    float ha = Mathf.Deg2Rad * horizontalAngleV.GetUseValueFloat(ld);
                    float va = Mathf.Deg2Rad * Mathf.Max(Mathf.Min(verticalAngleV.GetUseValueFloat(ld), 180f - jumpMinVerticalAngle), jumpMinVerticalAngle);
                    v = new Vector3(
                        Mathf.Sin(ha) * Mathf.Cos(va),
                        Mathf.Sin(va),
                        Mathf.Cos(ha) * Mathf.Cos(va));
                    break;
                case DirectionSettingType.Vector3D:
                    var uv = vectorV.GetUseValue(ld);
                    if (CalcVerticalAngle(uv) > jumpMinVerticalAngle)
                    {
                        var hv = uv;
                        hv.y = 0;
                        v = Quaternion.AngleAxis(jumpMinVerticalAngle, Vector3.Cross(hv, Vector3.up)) * hv;
                    }
                    else v = uv;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (coordinateSystemType is CoordinateSystemType.Local) v = ld.hd.transform.TransformVector(v);
            v = v.normalized;
            return v;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            return _endFrame < ActionManager.Inst.actionFrame;
        }

        public override float EnergyCost(MachineLD ld)
        {
            return ld.cd.MoveCommonPar.jumpUseEnergy * powerV.GetUseValueFloat(ld, 0, 100) / 100;
        }

        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }

        public override float GetPowerValue(MachineLD ld)
        {
            return powerV.GetUseValueFloat(ld, (float?)0, 100) / 100;
        }
        public override bool IsActiveDownforce => false;
    }
}