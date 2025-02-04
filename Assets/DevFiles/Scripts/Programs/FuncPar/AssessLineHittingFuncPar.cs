using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
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
    [Serializable]
    [MemoryPackable()]
    public partial class AssessLineHittingFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessLineHitting;

        public enum HittingType
        {
            Free,
            FiringLine,
        }

        public enum LineLengthType
        {
            ToTarget,
            Manual,
        }

        public HittingType hittingType;
        public DetectObjectFlags detectObjectFlags;
        public CoordinateSystemType coordinateSystemType;
        public VariableDataVector3Get originV = new();
        public VariableDataVector3Get directionV = new();
        public VariableDataLockOnGet targetList = new();
        public long firingPointType;
        public VariableDataNumericGet lineRadiusV = new();
        public int lineLengthMode;
        public VariableDataNumericGet lineLengthV = new();
        public bool groundInclinationDetectionFlag;
        public VariableDataNumericGet groundInclinationAngleV = new();
        public ComparatorType groundInclinationComparatorType;
        /// <summary>
        /// 自機から見た傾斜（Local）か、グローバルY軸に対する傾斜（Global）かを切り替える
        /// </summary>
        public CoordinateSystemType inclinationDirectionType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (HittingType* ht = &hittingType)
            fixed (DetectObjectFlags* dof = &detectObjectFlags)
            fixed (CoordinateSystemType* cst = &coordinateSystemType)
            fixed (long* fpt = &firingPointType)
            fixed (int* llm = &lineLengthMode)
            fixed (bool* hpidf = &groundInclinationDetectionFlag)
            fixed (CoordinateSystemType* idt = &inclinationDirectionType)
            fixed (ComparatorType* hpict = &groundInclinationComparatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.hittingType, pgNodeParDescription_assessLineHittingFuncPar.hittingType);
                pgbepManager.SetPgbepEnum(typeof(HittingType), (int*)ht);
                pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.detectObjectType, pgNodeParDescription_assessLineHittingFuncPar.detectObjectType);
                pgbepManager.SetPgbepFlagsEnum(typeof(DetectObjectFlags), (long*)dof);
                switch (hittingType)
                {
                    case HittingType.Free:
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.coordinateSystem, pgNodeParDescription_assessLineHittingFuncPar.coordinateSystem);
                        pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.origin, pgNodeParDescription_assessLineHittingFuncPar.origin);
                        originV.IndicateSwitchable(pgbepManager);
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.direction, pgNodeParDescription_assessLineHittingFuncPar.direction);
                        directionV.IndicateSwitchable(pgbepManager);
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.lineLength, pgNodeParDescription_assessLineHittingFuncPar.lineLength);
                        lineLengthV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 4000));
                        break;
                    case HittingType.FiringLine:
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.target, pgNodeParDescription_assessLineHittingFuncPar.target);
                        targetList.IndicateWithIndex(pgbepManager);
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.firingPointType, pgNodeParDescription_assessLineHittingFuncPar.firingPointType);
                        var equipmentList = pgbepManager.GetEquipmentList();
                        equipmentList.Insert(0, "Body");
                        pgbepManager.SetPgbepSelectFlagsOption(fpt, equipmentList);
                        pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.lineLengthMode, pgNodeParDescription_assessLineHittingFuncPar.lineLengthMode);
                        pgbepManager.SetPgbepEnum(typeof(LineLengthType), llm);
                        if (((LineLengthType)lineLengthMode) == LineLengthType.Manual)
                        {
                            pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.lineLength, pgNodeParDescription_assessLineHittingFuncPar.lineLength);
                            lineLengthV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 4000));
                        }
                        break;
                }
                pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.lineRadius, pgNodeParDescription_assessLineHittingFuncPar.lineRadius);
                lineRadiusV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 10));
                if ((detectObjectFlags & DetectObjectFlags.Ground) == DetectObjectFlags.Ground)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessLineHittingFuncPar.groundInclinationDetection, pgNodeParDescription_assessLineHittingFuncPar.groundInclinationDetection);
                    pgbepManager.SetPgbepToggle(hpidf);
                    if (groundInclinationDetectionFlag)
                    {
                        pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)idt);
                        groundInclinationAngleV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 90, 0));
                        pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)hpict);
                    }
                }
            }
        }

        [NonSerialized]
        [MemoryPackIgnore]
        private RaycastHit[] _rhl;
        public override bool BranchExecute(MachineLD ld)
        {
            var layer = GetDetectObjectLayer(detectObjectFlags);

            switch (hittingType)
            {
                case HittingType.Free:
                    var orig = originV.GetUseValue(ld);
                    var dir = directionV.GetUseValue(ld);
                    var length = lineLengthV.GetUseValueFloat(ld);
                    return LineHittingExe(
                        ld,
                        coordinateSystemType is CoordinateSystemType.Local ? ld.hd.transform.TransformPoint(orig) : orig,
                        coordinateSystemType is CoordinateSystemType.Local ? ld.hd.transform.TransformVector(dir) : dir,
                        length,
                        null,
                        layer
                    );
                case HittingType.FiringLine:
                    return FiringLineHitting(ld, layer);
            }
            return false;
        }

        private bool FiringLineHitting(MachineLD ld, int layer)
        {
            var tgt = targetList.GetUseValue(ld)?.transform;
            for (var i = 0; i < firingPointType; i++)
            {
                var l = 1L << i;
                if ((firingPointType & l) != l) continue;
                var firingPoint = i == 0 ? ld.hd.transform : ld.hd.useShootPoints[i - 1];
                var dir = tgt == null ? firingPoint.forward : (tgt.position - firingPoint.position).normalized;
                float length;
                if ((LineLengthType)lineLengthMode == LineLengthType.ToTarget)
                {
                    if (tgt == null) return false;
                    var v = tgt.position - firingPoint.position;
                    length = v.magnitude;
                }
                else length = lineLengthV.GetUseValueFloat(ld);
                if (LineHittingExe(ld, firingPoint.position, dir, length, tgt, layer)) return true;
            }
            return false;
        }
        private bool LineHittingExe(MachineLD ld, Vector3 orig, Vector3 dir, float length, Transform tgt, int layer)
        {
            var r = new Ray(orig, dir.normalized);
            _rhl ??= new RaycastHit[1];
            _rhl[0] = new RaycastHit();
            var radius = lineRadiusV.GetUseValueFloat(ld);
            if (radius <= 0) Physics.RaycastNonAlloc(r, _rhl, length, layer);
            else Physics.SphereCastNonAlloc(r, radius, _rhl, length, layer);
            var collider = _rhl[0].collider;
            if (collider == null) return false;
            if (collider.attachedRigidbody == null)
            {
                Debug.DrawLine(orig, orig + r.direction * length, Color.red);
                if (!groundInclinationDetectionFlag) return true;
                float angle;
                switch (inclinationDirectionType)
                {
                    case CoordinateSystemType.Local:
                        var crossVector = Vector3.Cross((_rhl[0].point - ld.hd.pos).normalized, ld.hd.transform.up);
                        angle = Vector3.SignedAngle(_rhl[0].normal, ld.hd.transform.up, crossVector);
                        break;
                    case CoordinateSystemType.Global:
                    default:
                        angle = Vector3.Angle(_rhl[0].normal, Vector3.up);
                        break;
                }
                var dAngle = groundInclinationAngleV.GetUseValueFloat(ld, (float?)0, 90);
                return groundInclinationComparatorType switch
                {
                    ComparatorType.EqualTo => Math.Abs(angle - dAngle) < float.Epsilon,
                    ComparatorType.Over => angle >= dAngle,
                    ComparatorType.GreaterThan => angle > dAngle,
                    ComparatorType.Under => angle <= dAngle,
                    ComparatorType.LessThan => angle < dAngle,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            if (collider.attachedRigidbody.transform != tgt &&
                collider.attachedRigidbody.transform != ld.hd.transform
               )
            {
                if (collider.gameObject.CompareTag(ld.tag)) return false;
                Debug.DrawLine(orig, orig + r.direction * length, Color.red);
                return true;
            }
            Debug.DrawLine(orig, orig + r.direction * length, Color.blue);
            return false;
        }

        public override string[] GetNodeFaceText()
        {
            var doStr = GetEnumFlagText(typeof(DetectObjectFlags), (long)detectObjectFlags, 1);
            var radiusStr = lineRadiusV.useVariable || lineRadiusV.constValue > 0 ? $"LR:{lineRadiusV.GetIndicateStr()}" : "";
            string switchStr;
            switch (hittingType)
            {
                case HittingType.Free:
                    switchStr = $"CS:{coordinateSystemType} Ori:{originV.GetIndicateStr()} Dir:{directionV.GetIndicateStr()} LL:{lineLengthV.GetIndicateStr()}";
                    break;
                case HittingType.FiringLine:
                    string fpStr = "";
                    bool setSeparator = false, allSelected = true;
                    var weaponCount = PGEM2.nowEditCD.mechCustom.weapons.ConvertAll(x => WHUB.GetBulletName(x)).Count + 1;
                    for (int i = 0; i < weaponCount; i++)
                    {
                        if (((1 << i) & firingPointType) == 0)
                        {
                            allSelected = false;
                        }
                        else
                        {
                            fpStr += $"{(setSeparator ? "," : "")}{i}";
                            setSeparator = true;
                        }
                    }
                    var lineLengthStr = ((LineLengthType)lineLengthMode is LineLengthType.Manual ? $" {lineLengthV.GetIndicateStr()}" : "");
                    switchStr = $"TGT:{targetList.GetIndicateStr()} FPT:{(allSelected ? "ALL" : fpStr)} LL:{(LineLengthType)lineLengthMode}{lineLengthStr}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var groundInclinationStr = $" GIA:{groundInclinationAngleV.GetIndicateStr()}";
            return new[] { $"HT:{hittingType} DO:{doStr} {switchStr} {radiusStr}{groundInclinationStr}" };
        }
    }
}