using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.PGE.PGBEditor;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using EnumLocalizationWithI2Localization;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FieldPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class ShieldFieldPar : ISphereFieldEditObject
    {
        public VariableDataNumericGet radiusV = new() { constValue = 5 };
        public VariableDataVector3Get offsetV = new();
        public CoordinateSystemType coordinateSystemType;

        [NonSerialized]
        [MemoryPackIgnore]
        private int _shieldCode = int.MinValue;
        [MemoryPackIgnore]
        public bool Is2D => false;


        public unsafe void SetEditIndicate(PgbepManager pgbepManager, int fieldEditTabType = 0)
        {
            var minMax = ShldHub.GetShieldMinMax(StaticInfo.Inst.nowEditMech.mechCustom.shields[0], coordinateSystemType);
            switch ((TabType)fieldEditTabType)
            {
                case TabType.Radius:
                    pgbepManager.SetHeaderText(pgNodeParameter_shieldFieldPar.radius, pgNodeParDescription_shieldFieldPar.radius);
                    radiusV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(minMax.radiusMin, minMax.radiusMax, 0));
                    break;
                case TabType.Offset or TabType.CenterPosition:
                    var str = coordinateSystemType is CoordinateSystemType.Global ? TabType.CenterPosition.ToLocalizedString() : TabType.Offset.ToLocalizedString();
                    var offsetSliderPar = new PgbepManager.FloatSliderSettingPar(minMax.offsetMin, minMax.offsetMax, 0);
                    offsetV.IndicateSwitchableForFieldEditor(pgbepManager, str, offsetSliderPar);
                    break;
            }
        }
        public IReadOnlyList<bool> IndicateTab
        {
            get
            {
                return ((TabType[])Enum.GetValues(typeof(TabType))).ToList().ConvertAll(x =>
                {
                    switch (x)
                    {
                        case TabType.Radius:
                            return true;
                        case TabType.Offset:
                            return coordinateSystemType is CoordinateSystemType.Local;
                        case TabType.CenterPosition:
                            return coordinateSystemType is CoordinateSystemType.Global;
                        default:
                            return false;
                    }
                });
            }
        }

        private enum TabType
        {
            Radius,
            Offset,
            CenterPosition
        }

        [MemoryPackIgnore]
        public SearchFieldType FieldType => SearchFieldType.Sphere;
        [MemoryPackIgnore]
        public string FieldFigureTitle => FieldType.ToLocalizedString();
        public IReadOnlyList<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)> settingList => new List<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)>();
        public IReadOnlyList<string> TabStrings => LocalizedEnumUtility.GetLocalizedNames(typeof(TabType));
        public Bounds GetFieldBounds()
        {
            var r = Vector3.one * (radiusV.useVariable ? 512 : radiusV.constValue * 2);
            var o = offsetV.useVariable ? Vector3.one * 512 : offsetV.constValue;
            return new Bounds(o, r);
        }
        public string GetFieldShortText()
        {
            return $"Rd:{radiusV.GetIndicateStr("m")} CS:{coordinateSystemType} Of:{offsetV.GetIndicateStr(null, "m")}";
        }
        public string GetFieldLongText()
        {
            return $"{pgNodeParameter_shieldFieldPar.radius}:{radiusV.GetIndicateStr("m")} " +
                   $"{pgNodeParameter_shieldFieldPar.offset}:{offsetV.GetIndicateStr(null, "m")}";
        }
        public (float farRadius, float nearRadius, float horizontalAngle, float verticalAngle1, float verticalAngle2, Vector2 rotate, Vector3 offset) GetIndicateInfo()
        {
            //todo:シールドフィールドのランダム表示を実装すること
            var bounds = GetFieldBounds();
            var dist = Mathf.Max(Vector3.Distance(bounds.max, Vector3.zero), Vector3.Distance(bounds.min, Vector3.zero));
            var o = IFieldEditObject.GetOffsetIndicateValue(dist, offsetV, offsetV.constValue, false);
            return (radiusV.constValue, 0, 360, 90, -90, Vector2.zero, o);
        }
    }
}