using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
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
    [Serializable]
    [MemoryPackable()]
    public partial class BoxSearchFieldParVariable : BoxSearchFieldPar, ISearchFieldUnion, IBoxFieldEditObject
    {
        private const float SizeMin = 0, SizeMax = 4000;
        private const float OffsetMin = -4000, OffsetMax = 4000;
        private const float RotateMin = -180, RotateMax = 180;

        public VariableDataVector3Get sizeV = new() { constValue = new(100, 100, 100) };
        public VariableDataVector3Get offsetV = new();
        public VariableDataVector3Get rotateV = new();
        public VariableDataNumericGet rotate2dV = new();
        public bool is2D;
        public override bool Is2D => is2D;

        public void GetValueFromVariable(MachineLD ld)
        {
            Size = sizeV.GetUseValue(ld, Vector3.zero, Vector3.one * SizeMax);
            Offset = offsetV.GetUseValue(ld, Vector3.one * OffsetMin, Vector3.one * OffsetMax);
            if (!is2D)
            {
                Rotate = rotateV.GetUseValue(ld, Vector3.one * RotateMin, Vector3.one * RotateMax);
            }
            else
            {
                Rotate2d = rotate2dV.GetUseValueFloat(ld, RotateMin, RotateMax);
            }
        }

        public bool LockOn(MachineLD ld, IdentificationType identificationType, ObjType searchObjType, LockOnDistancePriorityType lockOnDistancePriorityType,
            Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] results, List<ObjectSearchTgt> ignoreList)
        {
            GetValueFromVariable(ld);
            return LockOn(identificationType, searchObjType, lockOnDistancePriorityType, hdTransform, lockOnAngleOfMovementToSelfType, angleOfMovementToSelf, teamNum, results, ignoreList);
        }

        [MemoryPackIgnore]
        public SearchFieldType FieldType => SearchFieldType.Box;
        [MemoryPackIgnore]
        public string FieldFigureTitle => FieldType.ToLocalizedString();
        public bool Search(MachineLD ld, IdentificationType identificationType, ObjType searchObjType, Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum, List<ObjectSearchTgt> ignoreList)
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
                        TabType.Size => true,
                        TabType.Rotate => true,
                        TabType.Offset => true,
                        _ => false
                    };
                });
            }
        }

        private enum TabType
        {
            Size,
            Rotate,
            Offset,
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
            switch ((TabType)fieldEditTabType)
            {
                case TabType.Size:
                    sizeV.IndicateSwitchableForFieldEditor(pgbepManager, pgNodeParameter_boxSearchFieldParVariable.size, new PgbepManager.FloatSliderSettingPar(SizeMin, SizeMax, 0), new[] { true, !is2D, true });
                    break;
                case TabType.Rotate:
                    var rotateSliderPar = new PgbepManager.FloatSliderSettingPar(RotateMin, RotateMax, 0);
                    if (is2D)
                    {
                        pgbepManager.SetHeaderText(pgNodeParameter_boxSearchFieldParVariable.rotate, pgNodeParDescription_boxSearchFieldParVariable.rotate);
                        rotate2dV.IndicateSwitchableFloat(pgbepManager, rotateSliderPar);
                        rotate2dV.IndicateSwitchableFloat(pgbepManager, rotateSliderPar);
                    }
                    else
                    {
                        rotateV.IndicateSwitchableForFieldEditor(pgbepManager, pgNodeParameter_boxSearchFieldParVariable.rotate, rotateSliderPar);
                    }
                    break;
                case TabType.Offset:
                    var offsetSliderPar = new PgbepManager.FloatSliderSettingPar(OffsetMin, OffsetMax, 0);
                    offsetV.IndicateSwitchableForFieldEditor(pgbepManager, pgNodeParameter_boxSearchFieldParVariable.offset, offsetSliderPar, new[] { true, !is2D, true });
                    break;
            }
        }

        public Bounds GetFieldBounds()
        {
            var o = offsetV.useVariable ? Vector3.one * 512 : offsetV.constValue;
            var s = sizeV.useVariable ? Vector3.one * 512 : sizeV.constValue;
            return new Bounds(o, s);
        }
        public string GetFieldText(string[] titles)
        {
            var sz = sizeV.GetIndicateStr(is2D ? new[] { false, true, false } : null, "m");
            var ro = is2D ? rotate2dV.GetIndicateStr("°") : rotateV.GetIndicateStr(null, "°");
            var of = offsetV.GetIndicateStr(is2D ? new[] { false, true, false } : null, "m");
            return $"{titles[0]}:{sz} {titles[1]}:{ro} {titles[2]}:{of}";
        }
        public string GetFieldShortText()
        {
            return GetFieldText(new[] { "Sz", "Ro", "Of" });
        }
        public string GetFieldLongText()
        {
            return GetFieldText(new[]
            {
                pgNodeParameter_boxSearchFieldParVariable.size, pgNodeParameter_boxSearchFieldParVariable.rotate, pgNodeParameter_boxSearchFieldParVariable.offset
            });
        }

        public (Vector3 size, Vector3 offset, Vector3 rotate) GetIndicateInfo()
        {
            var bounds = GetFieldBounds();
            var dist = Mathf.Max(Vector3.Distance(bounds.max, Vector3.zero), Vector3.Distance(bounds.min, Vector3.zero));
            var s = sizeV.useVariable
                ? dist * new Vector3(
                    IFieldEditObject.PingPongIndicateValue(13 * 10),
                    is2D ? 0 : IFieldEditObject.PingPongIndicateValue(17 * 10),
                    IFieldEditObject.PingPongIndicateValue(23 * 10)
                )
                : sizeV.constValue;
            var o = IFieldEditObject.GetOffsetIndicateValue(dist, offsetV, offsetV.constValue, is2D);
            var r = is2D
                ? rotate2dV.useVariable
                    ? 360 * new Vector3(
                        0,
                        IFieldEditObject.PingPongIndicateValue(13f * 30),
                        0
                    )
                    : new Vector3(0, rotateV.constValue.y, 0)
                : rotateV.useVariable
                    ? IFieldEditObject.RandomRotate3D
                    : rotateV.constValue;
            return (s, o, r);
        }
    }
}