using clrev01.PGE.PGBEditor;
using clrev01.Save.VariableData;
using System;
using System.Collections.Generic;
using clrev01.ClAction.ObjectSearch;
using clrev01.Save;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;
using static EnumLocalizationWithI2Localization.LocalizedEnumUtility;

namespace clrev01.Programs.FieldPar
{
    public unsafe interface IFieldSearchObject
    {
        bool Is2D { get; }
        void CalcField(Transform searcher);
        bool CheckInField(Vector3 tgtPos);
        void CalcAABB(out Bounds bounds);
        bool Search(IdentificationType identificationType, ObjType searchObjType, Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, ComparatorType detectionComparatorType, int detectionNum, int teamNum, List<ObjectSearchTgt> ignoreList);
        bool LockOn(IdentificationType identificationType, ObjType searchObjType, LockOnDistancePriorityType lockOnDistancePriorityType,
            Transform hdTransform, LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType, float angleOfMovementToSelf, int teamNum, ObjectSearchTgt[] results, List<ObjectSearchTgt> ignoreList);
        bool AssessTgtPos(Transform hdTransform, Vector3 lockOnTgtPos);
    }

    public unsafe interface IFieldEditObject
    {
        bool Is2D { get; }
        SearchFieldType FieldType { get; }
        string FieldFigureTitle { get; }
        IReadOnlyList<(string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected)> settingList { get; }

        IReadOnlyList<string> TabStrings { get; }
        IReadOnlyList<bool> IndicateTab { get; }
        void SetEditIndicate(PgbepManager pgbepManager, int fieldEditTabType = 0);
        Bounds GetFieldBounds();
        public float GetIndicateBoundsMax()
        {
            var fieldBounds = GetFieldBounds();
            return Mathf.Max(Mathf.Abs(fieldBounds.max.x), Mathf.Abs(fieldBounds.max.y), Mathf.Abs(fieldBounds.max.z), Mathf.Abs(fieldBounds.min.x), Mathf.Abs(fieldBounds.min.y), Mathf.Abs(fieldBounds.min.z)) * 2;
        }
        string GetFieldShortText();
        string GetFieldLongText();

        public int CalcMagnificationNum(Bounds defaultBounds, float default3dFieldScale, (bool x, bool y, bool z)? ignoreAxis = null)
        {
            var fieldBounds = GetFieldBounds();
            if (ignoreAxis != null)
            {
                var fieldBoundsCenter = fieldBounds.center;
                var fieldBoundsSize = fieldBounds.size;
                if (ignoreAxis.Value.x) fieldBoundsCenter.x = fieldBoundsSize.x = 0;
                if (ignoreAxis.Value.y) fieldBoundsCenter.y = fieldBoundsSize.y = 0;
                if (ignoreAxis.Value.z) fieldBoundsCenter.z = fieldBoundsSize.z = 0;
                fieldBounds.center = fieldBoundsCenter;
                fieldBounds.size = fieldBoundsSize;
            }
            Vector3 maxv = Vector3.Max(fieldBounds.max, defaultBounds.max);
            Vector3 minv = Vector3.Min(fieldBounds.min, defaultBounds.min);
            float dist = Mathf.Max(
                maxv.x, maxv.y, maxv.z, Mathf.Abs(minv.x), Mathf.Abs(minv.y), Mathf.Abs(minv.z));
            int magnificationNum = 1;
            for (; default3dFieldScale * Mathf.Pow(2, magnificationNum) < dist; magnificationNum++) ;
            return magnificationNum;
        }
        protected static float PingPongIndicateValue(float pingPongFrame, float range = 1, float start = 0)
        {
            return Mathf.PingPong(Time.frameCount / pingPongFrame, 1) * range + start;
        }
        protected static float RepeatIndicateValue(float pingPongFrame, float range = 1, float start = 0)
        {
            return Mathf.Repeat(Time.frameCount / pingPongFrame, 1) * range + start;
        }
        protected static Vector3 RandomRotateAngle(Vector4 axisCycles)
        {
            return Quaternion.AngleAxis(
                RepeatIndicateValue(axisCycles.w, 360, -180),
                new Vector3(
                    RandomRotateAngleAxis(axisCycles.x),
                    RandomRotateAngleAxis(axisCycles.y),
                    RandomRotateAngleAxis(axisCycles.z)
                )
            ).eulerAngles;
        }
        private static float RandomRotateAngleAxis(float axisCycle)
        {
            return PingPongIndicateValue(axisCycle, 2, -1);
        }
        protected static Vector3 RandomRotate3D => RandomRotateAngle(new Vector4(11f, 13f, 17f, 23f) * 36);

        protected static Vector3 GetOffsetIndicateValue(float dist, VariableDataVector3Get offsetV, Vector3 offset, bool is2D)
        {
            return offsetV.useVariable
                ? Vector3.one * dist - dist * 2 * new Vector3(
                    IFieldEditObject.PingPongIndicateValue(23 * 13),
                    is2D ? 0 : IFieldEditObject.PingPongIndicateValue(17 * 13),
                    IFieldEditObject.PingPongIndicateValue(13 * 13)
                )
                : offset;
        }
        public static (string[] menus, Func<int, IFieldEditObject> func, Func<int> nowSelected) GetChangeFieldSettingPar(IFieldEditObject to)
        {
            return (
                GetLocalizedNamesArray(typeof(SearchFieldType)),
                i =>
                {
                    return (SearchFieldType)i switch
                    {
                        SearchFieldType.Box => new BoxSearchFieldParVariable { is2D = to.Is2D },
                        SearchFieldType.Circle => new CircleSearchFieldParVariable { is2D = to.Is2D },
                        SearchFieldType.Sphere => new SphereSearchFieldParVariable(),
                        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
                    };
                },
                () => (int)to.FieldType
            );
        }
    }
}