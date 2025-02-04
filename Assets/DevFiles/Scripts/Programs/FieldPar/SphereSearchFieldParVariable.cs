using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Save;
using clrev01.Save.VariableData;
using Dest.Math;
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
    public partial class SphereSearchFieldParVariable : SphereSearchFieldPar, ISearchFieldUnion, ISphereFieldEditObject
    {
        private const float RadiusMin = 0, RadiusMax = 4000;
        private const float HAngleMin = 0, HAngleMax = 360;
        private const float VAngleMin = -90, VAngleMax = 90;
        private const float RotateMin = -180, RotateMax = 180;
        private const float OffsetMin = -4000, OffsetMax = 4000;

        public VariableDataNumericGet farRadiusV = new() { constValue = 100 };
        public VariableDataNumericGet nearRadiusV = new();
        public VariableDataNumericGet horizontalAngleV = new() { constValue = 360 };
        public VariableDataNumericGet verticalAngle1V = new() { constValue = 90 };
        public VariableDataNumericGet verticalAngle2V = new() { constValue = -90 };
        public VariableDataVector3Get rotateV = new();
        public VariableDataVector3Get offsetV = new();

        public void GetValueFromVariable(MachineLD ld)
        {
            FarRadius = farRadiusV.GetUseValueFloat(ld, RadiusMin, RadiusMax);
            NearRadius = nearRadiusV.GetUseValueFloat(ld, RadiusMin, RadiusMax);
            HorizontalAngle = horizontalAngleV.GetUseValueFloat(ld, HAngleMin, HAngleMax);
            VerticalAngle1 = verticalAngle1V.GetUseValueFloat(ld, VAngleMin, VAngleMax);
            VerticalAngle2 = verticalAngle2V.GetUseValueFloat(ld, VAngleMin, VAngleMax);
            Rotate = rotateV.GetUseValue(ld, Vector3.one * RotateMin, Vector3.one * RotateMax);
            Offset = offsetV.GetUseValue(ld, Vector3.one * OffsetMin, Vector3.one * OffsetMax);
        }

        public bool AssessTgtPos(MachineLD ld, Transform hdTransform, Vector3 lockOnTgtPos)
        {
            GetValueFromVariable(ld);
            return AssessTgtPos(hdTransform, lockOnTgtPos);
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
        public SearchFieldType FieldType => SearchFieldType.Sphere;
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
        }


        IReadOnlyList<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)> IFieldEditObject.settingList =>
            new List<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)>
            {
                IFieldEditObject.GetChangeFieldSettingPar(this)
            };
        public IReadOnlyList<string> TabStrings => GetLocalizedNames(typeof(TabType));

        public unsafe void SetEditIndicate(PgbepManager pgbepManager, int fieldEditTabType = 0)
        {
            switch ((TabType)fieldEditTabType)
            {
                case TabType.Radius:
                    pgbepManager.SetHeaderText(pgNodeParameter_sphereSearchFieldParVariable.farRadius, pgNodeParDescription_sphereSearchFieldParVariable.farRadius);
                    var radiusSliderPar = new PgbepManager.FloatSliderSettingPar(RadiusMin, RadiusMax, 0);
                    farRadiusV.IndicateSwitchableFloat(pgbepManager, radiusSliderPar);
                    pgbepManager.SetHeaderText(pgNodeParameter_sphereSearchFieldParVariable.nearRadius, pgNodeParDescription_sphereSearchFieldParVariable.nearRadius);
                    nearRadiusV.IndicateSwitchableFloat(pgbepManager, radiusSliderPar);
                    break;
                case TabType.Angle:
                    pgbepManager.SetHeaderText(pgNodeParameter_sphereSearchFieldParVariable.horizontalAngle, pgNodeParDescription_sphereSearchFieldParVariable.horizontalAngle);
                    horizontalAngleV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(HAngleMin, HAngleMax, 0));
                    pgbepManager.SetHeaderText(pgNodeParameter_sphereSearchFieldParVariable.verticalAngle1, pgNodeParDescription_sphereSearchFieldParVariable.verticalAngle1);
                    var verticalAngleSliderPar = new PgbepManager.FloatSliderSettingPar(VAngleMin, VAngleMax, 0);
                    verticalAngle1V.IndicateSwitchableFloat(pgbepManager, verticalAngleSliderPar);
                    pgbepManager.SetHeaderText(pgNodeParameter_sphereSearchFieldParVariable.verticalAngle2, pgNodeParDescription_sphereSearchFieldParVariable.verticalAngle2);
                    verticalAngle2V.IndicateSwitchableFloat(pgbepManager, verticalAngleSliderPar);
                    break;
                case TabType.Rotate:
                    var rotateSliderPar = new PgbepManager.FloatSliderSettingPar(RotateMin, RotateMax, 0);
                    rotateV.IndicateSwitchableForFieldEditor(pgbepManager, pgNodeParameter_sphereSearchFieldParVariable.rotate, rotateSliderPar, new[] { true, true, false });
                    break;
                case TabType.Offset:
                    var offsetSliderPar = new PgbepManager.FloatSliderSettingPar(OffsetMin, OffsetMax, 0);
                    offsetV.IndicateSwitchableForFieldEditor(pgbepManager, pgNodeParameter_sphereSearchFieldParVariable.offset, offsetSliderPar);
                    break;
            }
        }

        public Bounds GetFieldBounds()
        {
            var fr = Vector3.one * (farRadiusV.useVariable ? 512 : farRadiusV.constValue * 2);
            var o = offsetV.useVariable ? Vector3.one * 512 : offsetV.constValue;
            return new Bounds(o, fr);
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
            var ha = horizontalAngleV.GetIndicateStr("°");
            var va = $"{verticalAngle1V.GetIndicateStr("°")}~{verticalAngle2V.GetIndicateStr("°")}";
            var ro = rotateV.GetIndicateStr(null, "°");
            var o = offsetV.GetIndicateStr(null, "m");
            return $"{titles[0]}:{ra} {titles[1]}:{ha} {titles[2]}:{va} {titles[3]}:{ro} {titles[4]}:{o}";
        }
        public string GetFieldShortText()
        {
            return GetFieldText(new[] { "Rd", "HA", "VA", "Ro", "Of" });
        }
        public string GetFieldLongText()
        {
            return GetFieldText(new[]
            {
                pgNodeParameter_sphereSearchFieldParVariable.radius,
                pgNodeParameter_sphereSearchFieldParVariable.horizontalAngle,
                pgNodeParameter_sphereSearchFieldParVariable.verticalAngle,
                pgNodeParameter_sphereSearchFieldParVariable.rotate,
                pgNodeParameter_sphereSearchFieldParVariable.offset
            });
        }

        public (float farRadius, float nearRadius, float horizontalAngle, float verticalAngle1, float verticalAngle2, Vector2 rotate, Vector3 offset) GetIndicateInfo()
        {
            //todo:スフィアフィールドのランダム表示を実装すること
            var bounds = GetFieldBounds();
            var dist = Mathf.Max(Vector3.Distance(bounds.max, Vector3.zero), Vector3.Distance(bounds.min, Vector3.zero));
            var o = IFieldEditObject.GetOffsetIndicateValue(dist, offsetV, offsetV.constValue, false);
            return (farRadiusV.constValue, nearRadiusV.constValue, horizontalAngleV.constValue, verticalAngle1V.constValue, verticalAngle2V.constValue, rotateV.constValue, o);
        }
    }
}