using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Collections.Generic;
using EnumLocalizationWithI2Localization;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;
using static EnumLocalizationWithI2Localization.LocalizedEnumUtility;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessMoveDirectionFuncPar : BranchFuncPar, IPGBFuncUnion, ICircleFieldEditObject, ISphereFieldEditObject
    {
        public override string BlockTypeStr => pgNodeName.assessMoveDirection;

        public enum DirectionType
        {
            Horizontal,
            All,
        }

        public enum InputType
        {
            Angles,
            Vector,
        }

        public DirectionType directionType;
        public InputType inputType;
        public VariableDataNumericGet horizontalRotateV = new();
        public VariableDataNumericGet verticalRotateV = new();
        public VariableDataVector3Get vectorV = new() { constValue = Vector3.forward };
        public VariableDataNumericGet angleV = new() { constValue = 90 };
        public VariableDataNumericGet minimumSpeedEvaluateV = new() { constValue = 50 };
        public SpeedUnitType speedUnitType = SpeedUnitType.KiloMeterPerHour;
        public CoordinateSystemType coordinateSystemType;


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            //todo:判定角度やその回転がわかりにくいので、いずれ見やすくしたいところ
            fixed (CoordinateSystemType* cst = &coordinateSystemType)
            fixed (SpeedUnitType* sut = &speedUnitType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessMoveDirectionFuncPar.coordinateSystem, pgNodeParDescription_assessMoveDirectionFuncPar.coordinateSystem);
                pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                pgbepManager.SetHeaderText(pgNodeParameter_assessMoveDirectionFuncPar.moveDirection, pgNodeParDescription_assessMoveDirectionFuncPar.moveDirection);
                pgbepManager.SetPgbepField(this, res =>
                {
                    var r = (AssessMoveDirectionFuncPar)res;
                    directionType = r.directionType;
                    inputType = r.inputType;
                    horizontalRotateV = r.horizontalRotateV;
                    verticalRotateV = r.verticalRotateV;
                    vectorV = r.vectorV;
                    angleV = r.angleV;
                });
                pgbepManager.SetHeaderText(pgNodeParameter_assessMoveDirectionFuncPar.minimumSpeedEvaluate, pgNodeParDescription_assessMoveDirectionFuncPar.minimumSpeedEvaluate);
                pgbepManager.SetPgbepEnum(typeof(SpeedUnitType), (int*)sut);
                minimumSpeedEvaluateV.IndicateSwitchableFloat(pgbepManager);
            }
        }
        public override bool BranchExecute(MachineLD ld)
        {
            var velocity = ld.hd.rigidBody.linearVelocity;
            var moveDirection = coordinateSystemType switch
            {
                CoordinateSystemType.Global => velocity,
                CoordinateSystemType.Local => ld.hd.transform.InverseTransformVector(velocity),
                _ => throw new ArgumentOutOfRangeException()
            };
            if (directionType is DirectionType.Horizontal) moveDirection.y = 0;
            var mse = minimumSpeedEvaluateV.GetUseValueFloat(ld);
            var speedUnitRatio = speedUnitType.GetSpeedUnitRatio();
            if (moveDirection.sqrMagnitude * speedUnitRatio * speedUnitRatio <= mse * mse) return false;
            moveDirection = moveDirection.normalized;
            Vector3 vector;
            switch (inputType)
            {
                case InputType.Angles:
                    var hv = Mathf.Deg2Rad * horizontalRotateV.GetUseValueFloat(ld);
                    var vv = Mathf.Deg2Rad * verticalRotateV.GetUseValueFloat(ld);
                    vector = new Vector3(
                        Mathf.Sin(hv) * Mathf.Cos(vv),
                        Mathf.Sin(vv),
                        Mathf.Cos(hv) * Mathf.Cos(vv));
                    break;
                case InputType.Vector:
                    vector = vectorV.GetUseValue(ld);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Vector3.Angle(moveDirection, vector) <= angleV.GetUseValueFloat(ld);
        }
        [MemoryPackIgnore]
        public SearchFieldType FieldType =>
            directionType switch
            {
                DirectionType.Horizontal => SearchFieldType.Circle,
                DirectionType.All => SearchFieldType.Sphere,
                _ => throw new ArgumentOutOfRangeException()
            };
        [MemoryPackIgnore]
        public string FieldFigureTitle => directionType.ToLocalizedString();
        [MemoryPackIgnore]
        public IReadOnlyList<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)> settingList =>
            new List<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)>
            {
                (GetLocalizedNamesArray(typeof(DirectionType)),
                    i =>
                    {
                        directionType = (DirectionType)i;
                        return this;
                    },
                    () => (int)directionType
                ),
                (GetLocalizedNamesArray(typeof(InputType)),
                    i =>
                    {
                        inputType = (InputType)i;
                        return this;
                    },
                    () => (int)inputType
                )
            };
        [MemoryPackIgnore]
        public IReadOnlyList<string> TabStrings => new[] { pgNodeParameter_assessMoveDirectionFuncPar.moveDirection };
        [MemoryPackIgnore]
        public IReadOnlyList<bool> IndicateTab => new[] { true };
        public unsafe void SetEditIndicate(PgbepManager pgbepManager, int fieldEditTabType = 0)
        {
            pgbepManager.SetHeaderText(pgNodeParameter_assessMoveDirectionFuncPar.direction, pgNodeParDescription_assessMoveDirectionFuncPar.direction);
            switch (inputType)
            {
                case InputType.Angles:
                    pgbepManager.SetPgbepAngle(hAngleLimit: null, vAngleLimit: null, hVd: horizontalRotateV, vVd: directionType is DirectionType.All ? verticalRotateV : null);
                    break;
                case InputType.Vector:
                    vectorV.IndicateSwitchable(pgbepManager, new[] { true, directionType is DirectionType.All, true });
                    break;
            }
            pgbepManager.SetHeaderText(pgNodeParameter_assessMoveDirectionFuncPar.angle, pgNodeParDescription_assessMoveDirectionFuncPar.angle);
            angleV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 180));
        }
        public override float?[] GetNodeFaceValue()
        {
            return new float?[] { -45f };
        }
        public Bounds GetFieldBounds()
        {
            return new Bounds(Vector3.zero, Vector3.one * 200);
        }
        public string GetFieldText(string[] titles)
        {
            var a = $"{titles[0]}:{angleV.GetIndicateStr("°")}";
            string dir;
            switch (directionType)
            {
                case DirectionType.All:
                    dir = inputType switch
                    {
                        InputType.Angles => $"{titles[1]}:({horizontalRotateV.GetIndicateStr("°")},{verticalRotateV.GetIndicateStr("°")})",
                        InputType.Vector => $"{titles[2]}:{vectorV.GetIndicateStr(null, "m")}",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case DirectionType.Horizontal:
                    dir = inputType switch
                    {
                        InputType.Angles => $"{titles[1]}:{horizontalRotateV.GetIndicateStr("°")}",
                        InputType.Vector => $"{titles[2]}:{vectorV.GetIndicateStr(new[] { false, true, false }, "m")}",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return $" {dir} {a}";
        }
        public string GetFieldShortText()
        {
            return GetFieldText(new[] { "An", "Ro", "Vec" });
        }
        public string GetFieldLongText()
        {
            return GetFieldText(new[]
            {
                InputType.Angles.ToLocalizedString(),
                pgNodeParameter_assessMoveDirectionFuncPar.direction,
                InputType.Vector.ToLocalizedString()
            });
        }
        public override string[] GetNodeFaceText()
        {
            var mse = $"{(minimumSpeedEvaluateV.useVariable || minimumSpeedEvaluateV.constValue > 0 ? $" MSE:{minimumSpeedEvaluateV.GetIndicateStr()}m/s" : "")}";
            return new[] { $"CST:{coordinateSystemType} DT:{directionType} IT:{inputType}{GetFieldShortText()}{mse}" };
        }
        [MemoryPackIgnore]
        public bool Is2D => directionType is DirectionType.Horizontal;
        (float farRadius, float nearRadius, float horizontalAngle, float verticalAngle1, float verticalAngle2, Vector2 rotate, Vector3 offset) ISphereFieldEditObject.GetIndicateInfo()
        {
            var a = 90 - (angleV.useVariable ? IFieldEditObject.PingPongIndicateValue(180 * 13f * 10) : angleV.constValue);
            Vector2 indicateRotate;
            switch (inputType)
            {
                case InputType.Angles:
                    var v = verticalRotateV.useVariable ? IFieldEditObject.PingPongIndicateValue(360 * 17f * 10) : verticalRotateV.constValue;
                    var h = horizontalRotateV.useVariable ? IFieldEditObject.PingPongIndicateValue(360 * 11f * 10) : horizontalRotateV.constValue;
                    indicateRotate = new Vector2(v, h);
                    break;
                case InputType.Vector:
                    indicateRotate = vectorV.useVariable ? IFieldEditObject.RandomRotate3D : Quaternion.FromToRotation(Vector3.forward, vectorV.constValue).eulerAngles;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            indicateRotate.x += 90;
            return (100, 0, 360, 90, a, indicateRotate, Vector3.zero);
        }
        (float farRadius, float nearRadius, float angle, float rotate, float height, Vector3 offset) ICircleFieldEditObject.GetIndicateInfo()
        {
            var a = (angleV.useVariable ? IFieldEditObject.PingPongIndicateValue(180 * 13f * 10) : angleV.constValue);
            float indicateRotate;
            switch (inputType)
            {
                case InputType.Angles:
                    indicateRotate = horizontalRotateV.useVariable ? IFieldEditObject.PingPongIndicateValue(360 * 11f * 10) : horizontalRotateV.constValue;
                    break;
                case InputType.Vector:
                    indicateRotate = vectorV.useVariable ? IFieldEditObject.PingPongIndicateValue(11f * 30, 360, -180) : Quaternion.FromToRotation(Vector3.forward, vectorV.constValue).eulerAngles.y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return (100, 0, a * 2, indicateRotate, 0, Vector3.zero);
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return this;
        }
    }
}