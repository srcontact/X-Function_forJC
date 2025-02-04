using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;
using static EnumLocalizationWithI2Localization.LocalizedEnumUtility;

namespace clrev01.Programs.FieldPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class CircleSearchFieldParVariable : CircleSearchFieldPar, ISearchFieldUnion, ICircleFieldEditObject
    {
        private const float OffsetMin = -4000, OffsetMax = 4000;
        private const float RadiusMin = 0, RadiusMax = 4000;
        private const float HeightMin = 0, HeightMax = 4000;
        private const float AngleMin = 0, AngleMax = 360;
        private const float RotateMin = -180, RotateMax = 180;

        public VariableDataNumericGet farRadiusV = new() { constValue = 100 };
        public VariableDataNumericGet nearRadiusV = new();
        public VariableDataNumericGet angleV = new() { constValue = 360 };
        public VariableDataNumericGet rotateV = new();
        public VariableDataNumericGet heightV = new() { constValue = 100 };
        public VariableDataVector3Get offsetV = new();
        public bool is2D;
        public override bool Is2D => is2D;

        public void GetValueFromVariable(MachineLD ld)
        {
            FarRadius = farRadiusV.GetUseValueFloat(ld, RadiusMin, RadiusMax);
            NearRadius = nearRadiusV.GetUseValueFloat(ld, RadiusMin, RadiusMax);
            Angle = angleV.GetUseValueFloat(ld, AngleMin, AngleMax);
            Rotate = rotateV.GetUseValueFloat(ld, RotateMin, RotateMax);
            Height = heightV.GetUseValueFloat(ld, HeightMin, HeightMax);
            Offset = offsetV.GetUseValue(ld, Vector3.one * OffsetMin, Vector3.one * OffsetMax);
        }

        public bool LockOn(MachineLD ld, IdentificationType identificationType, ObjType searchObjType,
            LockOnDistancePriorityType lockOnDistancePriorityType,
            Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] searched, List<ObjectSearchTgt> ignoreList)
        {
            GetValueFromVariable(ld);
            return LockOn(identificationType, searchObjType, lockOnDistancePriorityType, hdTransform, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, teamNum, searched, ignoreList);
        }
        [MemoryPackIgnore]
        public SearchFieldType FieldType => SearchFieldType.Circle;
        [MemoryPackIgnore]
        public string FieldFigureTitle => FieldType.ToLocalizedString();
        public bool Search(MachineLD ld, IdentificationType identificationType, ObjType searchObjType,
            Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType,
            float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum,
            List<ObjectSearchTgt> ignoreList)
        {
            GetValueFromVariable(ld);
            return Search(identificationType, searchObjType, hdTransform, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, detectionComparatorType, detectionNum, teamNum, ignoreList);
        }
        public bool AssessTgtPos(MachineLD ld, Transform hdTransform, Vector3 lockOnTgtPos)
        {
            GetValueFromVariable(ld);
            return AssessTgtPos(hdTransform, lockOnTgtPos);
        }

        public IReadOnlyList<bool> IndicateTab
        {
            get
            {
                return ((TabType[])Enum.GetValues(typeof(TabType))).ToList().ConvertAll(x =>
                {
                    return x switch
                    {
                        TabType.Radius => true,
                        TabType.Angle => true,
                        TabType.Rotate => true,
                        TabType.Offset => true,
                        TabType.Height => !is2D,
                        _ => false
                    };
                });
            }
        }

        private enum TabType
        {
            Radius,
            Angle,
            Rotate,
            Offset,
            Height,
        }


        IReadOnlyList<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)> IFieldEditObject.settingList =>
            new List<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)>
            {
                IFieldEditObject.GetChangeFieldSettingPar(this),
                (
                    new[] { "2D", "3D" },
                    i =>
                    {
                        is2D = i == 0;
                        return this;
                    },
                    () => is2D ? 0 : 1
                )
            };
        public IReadOnlyList<string> TabStrings => GetLocalizedNames(typeof(TabType));

        public unsafe void SetEditIndicate(PgbepManager pgbepManager, int fieldEditTabType = 0)
        {
            var lengthSliderPar = new PgbepManager.FloatSliderSettingPar(OffsetMin, OffsetMax, 0);
            switch ((TabType)fieldEditTabType)
            {
                case TabType.Radius:
                    pgbepManager.SetHeaderText(pgNodeParameter_circleSearchFieldParVariable.farRadius, pgNodeParDescription_circleSearchFieldParVariable.farRadius);
                    var radiusSliderPar = new PgbepManager.FloatSliderSettingPar(RadiusMin, RadiusMax, 0);
                    farRadiusV.IndicateSwitchableFloat(pgbepManager, radiusSliderPar);
                    pgbepManager.SetHeaderText(pgNodeParameter_circleSearchFieldParVariable.nearRadius, pgNodeParDescription_circleSearchFieldParVariable.nearRadius);
                    nearRadiusV.IndicateSwitchableFloat(pgbepManager, radiusSliderPar);
                    break;
                case TabType.Angle:
                    pgbepManager.SetHeaderText(pgNodeParameter_circleSearchFieldParVariable.angle, pgNodeParDescription_circleSearchFieldParVariable.angle);
                    angleV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(AngleMin, AngleMax, 0));
                    break;
                case TabType.Rotate:
                    pgbepManager.SetHeaderText(pgNodeParameter_circleSearchFieldParVariable.rotate, pgNodeParDescription_circleSearchFieldParVariable.rotate);
                    rotateV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(RotateMin, RotateMax, 0));
                    break;
                case TabType.Height:
                    if (!is2D)
                    {
                        pgbepManager.SetHeaderText(pgNodeParameter_circleSearchFieldParVariable.height, pgNodeParDescription_circleSearchFieldParVariable.height);
                        heightV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(HeightMin, HeightMax, 0));
                    }
                    break;
                case TabType.Offset:
                    offsetV.IndicateSwitchableForFieldEditor(pgbepManager, pgNodeParameter_circleSearchFieldParVariable.offset, lengthSliderPar, new[] { true, !is2D, true });
                    break;
            }
        }

        public Bounds GetFieldBounds()
        {
            var fr = farRadiusV.useVariable ? 512 : farRadiusV.constValue * 2;
            var h = heightV.useVariable ? 512 : heightV.constValue;
            var o = offsetV.useVariable ? Vector3.one * 512 : offsetV.constValue;
            var v = new Vector3(
                fr,
                !is2D ? h : 0,
                fr
            );
            return new Bounds(o, v);
        }
        public string GetFieldText(string[] titles)
        {
            string ra;
            var fr = farRadiusV.GetIndicateStr("m");
            if (nearRadiusV.constValue > 0 || nearRadiusV.useVariable)
            {
                var nr = nearRadiusV.GetIndicateStr("m");
                ra = $"{nr}~{fr}";
            }
            else ra = fr;
            var a = angleV.GetIndicateStr("°");
            var ro = rotateV.GetIndicateStr("°");
            var o = offsetV.GetIndicateStr(is2D ? new[] { false, true, false } : null, "m");
            if (is2D)
            {
                return $"{titles[0]}:{ra} {titles[1]}:{a} {titles[2]}:{ro} {titles[4]}:{o}";
            }
            var h = heightV.GetIndicateStr("m");
            return $"{titles[0]}:{ra} {titles[1]}:{a} {titles[2]}:{ro} {titles[3]}:{h} {titles[4]}:{o}";
        }
        public string GetFieldShortText()
        {
            return GetFieldText(new[] { "Rd", "An", "Ro", "H", "Of" });
        }
        public string GetFieldLongText()
        {
            return GetFieldText(new[]
            {
                pgNodeParameter_circleSearchFieldParVariable.radius,
                pgNodeParameter_circleSearchFieldParVariable.angle,
                pgNodeParameter_circleSearchFieldParVariable.rotate,
                pgNodeParameter_circleSearchFieldParVariable.height,
                pgNodeParameter_circleSearchFieldParVariable.offset,
            });
        }

        public (float farRadius, float nearRadius, float angle, float rotate, float height, Vector3 offset) GetIndicateInfo()
        {
            //todo:サークルフィールドのランダム表示を実装すること
            var bounds = GetFieldBounds();
            var dist = Mathf.Max(Vector3.Distance(bounds.max, Vector3.zero), Vector3.Distance(bounds.min, Vector3.zero));
            var o = IFieldEditObject.GetOffsetIndicateValue(dist, offsetV, offsetV.constValue, is2D);
            return (farRadiusV.constValue, nearRadiusV.constValue, angleV.constValue, rotateV.constValue, heightV.constValue, o);
        }
    }
}