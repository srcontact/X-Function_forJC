using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class GetEnvironmentStatusValueFuncPar : FunctionFuncPar, IPGBFuncUnion, IGetStatusValueFuncPar
    {
        public override string BlockTypeStr => pgNodeName.getEnvironmentStatus;
        public EnvironmentStatusValueType statusType;
        public VariableDataNumericSet tgtVn = new() { };
        public VariableDataVector3Set tgtVv = new() { };
        public DetectObjectFlags lineDetectObjectFlags = DetectObjectFlags.Ground;
        public CoordinateSystemType coordinateSystemType;
        public VariableDataVector3Get lineOriginV = new();
        public VariableDataVector3Get lineDirectionV = new() { constValue = new Vector3(0, -1, 0) };
        public VariableDataNumericGet lineLengthV = new() { constValue = 10 };

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (EnvironmentStatusValueType* st = &statusType)
            fixed (DetectObjectFlags* dof = &lineDetectObjectFlags)
            fixed (CoordinateSystemType* cst = &coordinateSystemType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.statusType, pgNodeParDescription_getEnvironmentStatusValueFuncPar.statusType);
                pgbepManager.SetPgbepEnum(typeof(EnvironmentStatusValueType), (int*)st);

                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.detectObjectType, pgNodeParDescription_getEnvironmentStatusValueFuncPar.detectObjectType);
                pgbepManager.SetPgbepFlagsEnum(typeof(DetectObjectFlags), (long*)dof);
                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.coordinateSystem, pgNodeParDescription_getEnvironmentStatusValueFuncPar.coordinateSystem);
                pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.origin, pgNodeParDescription_getEnvironmentStatusValueFuncPar.origin);
                lineOriginV.IndicateSwitchable(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.direction, pgNodeParDescription_getEnvironmentStatusValueFuncPar.direction);
                lineDirectionV.IndicateSwitchable(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.lineLength, pgNodeParDescription_getEnvironmentStatusValueFuncPar.lineLength);
                lineLengthV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 4000));

                tgtVv.useVariable = isVector3Tgt;
                tgtVn.useVariable = !isVector3Tgt;

                pgbepManager.SetHeaderText(pgNodeParameter_getEnvironmentStatusValueFuncPar.targetVariable, pgNodeParDescription_getEnvironmentStatusValueFuncPar.targetVariable);
                if (isVector3Tgt) pgbepManager.SetPgbepVariable(tgtVv, false);
                else pgbepManager.SetPgbepVariable(tgtVn, false);
            }
        }

        private bool isVector3Tgt => statusType is EnvironmentStatusValueType.SurfaceNormal;

        public void GetStatusValue(MachineLD ld)
        {
            if (tgtVn.useVariable) GetNumericStatusValue(ld);
            else if (tgtVv.useVariable) GetVectorStatusValue(ld);
        }
        private void GetNumericStatusValue(MachineLD ld)
        {
            float res = 0;
            GetLineHittingPar(ld, out var origin, out var direction, out var length, out var layer);
            var raycastResult = Physics.Raycast(origin, direction, out var raycastHit, length, layer);
            switch (statusType)
            {
                case EnvironmentStatusValueType.DistanceToSurface:
                    res = raycastResult ? raycastHit.distance : length;
                    break;
                case EnvironmentStatusValueType.InclinationAngle:
                    res = raycastResult ? 0 : Vector3.Angle(raycastHit.normal, Vector3.up);
                    break;
            }
            tgtVn.SetNumericValue(ld, res);
        }
        private void GetVectorStatusValue(MachineLD ld)
        {
            Vector3 res = new Vector3();
            switch (statusType)
            {
                case EnvironmentStatusValueType.SurfaceNormal:
                    GetLineHittingPar(ld, out var origin, out var direction, out var length, out var layer);
                    var raycastResult = Physics.Raycast(origin, direction, out var raycastHit, length, layer);
                    res = raycastResult ? raycastHit.normal : Vector3.zero;
                    break;
            }
            tgtVv.SetVector3dValue(ld, res);
        }

        public void GetLineHittingPar(MachineLD machineLd, out Vector3 origin, out Vector3 direction, out float length, out int layer)
        {
            origin = lineOriginV.GetUseValue(machineLd);
            direction = lineDirectionV.GetUseValue(machineLd);
            if (coordinateSystemType is CoordinateSystemType.Local)
            {
                origin = machineLd.hd.transform.TransformPoint(origin);
                direction = machineLd.hd.transform.TransformVector(direction);
            }
            direction = direction.normalized;
            length = lineLengthV.GetUseValueFloat(machineLd);
            layer = GetDetectObjectLayer(lineDetectObjectFlags);
        }

        public override string[] GetNodeFaceText()
        {
            var dot = GetEnumFlagText(typeof(DetectObjectFlags), (long)lineDetectObjectFlags);

            var org = lineOriginV.GetIndicateStr();
            var dir = lineDirectionV.GetIndicateStr();
            var l = lineLengthV.GetIndicateStr();
            var str1 = $"\nDOT:{dot} CS:{coordinateSystemType}\nOrg:{org}\nDir:{dir}\nL:{l}";
            return new[] { $"ST:{statusType}{str1}\nTgtV:[{(isVector3Tgt ? tgtVv.name : tgtVn.name)}]" };
        }
    }
}