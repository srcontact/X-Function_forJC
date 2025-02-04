using static I2.Loc.ScriptLocalization;
using BurstLinq;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class GetListStatusFuncPar : FunctionFuncPar, IPGBFuncUnion, IGetStatusValueFuncPar
    {
        public override string BlockTypeStr => pgNodeName.getListStatus;
        public VariableData tgtList = new VariableDataNumericList();
        public VariableData tgtV = new VariableDataNumericSet();

        public enum ListType
        {
            NumericList,
            VectorList,
            LockOnList
        }

        public enum OperationType
        {
            GetValue,
            GetListLength,
            GetSum,
            GetAverage,
            GetMax,
            GetMin,
        }

        public enum LockOnParameterType
        {
            Distance,
            Position,
        }

        public ListType listType;
        public OperationType operationType;
        public int parameterType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (ListType* lt = &listType)
            fixed (OperationType* ot = &operationType)
            fixed (int* pt = &parameterType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_getListStatusFuncPar.listType, pgNodeParDescription_getListStatusFuncPar.listType);
                pgbepManager.SetPgbepEnum(typeof(ListType), (int*)lt);
                Type tgtListType;
                switch (operationType, listType)
                {
                    case (OperationType.GetValue, ListType.NumericList):
                        tgtListType = typeof(VariableDataNumericGet);
                        break;
                    case (OperationType.GetValue, ListType.VectorList):
                        tgtListType = typeof(VariableDataVector3Get);
                        break;
                    case (OperationType.GetValue, ListType.LockOnList):
                        tgtListType = typeof(VariableDataLockOnGet);
                        break;
                    case (_, ListType.NumericList):
                        tgtListType = typeof(VariableDataNumericList);
                        break;
                    case (_, ListType.VectorList):
                        tgtListType = typeof(VariableDataVector3DList);
                        break;
                    case (_, ListType.LockOnList):
                        tgtListType = typeof(VariableDataLockOnList);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Type tgtVType;
                switch (operationType)
                {
                    case OperationType.GetValue:
                        tgtVType = listType switch
                        {
                            ListType.NumericList => typeof(VariableDataNumericSet),
                            ListType.VectorList => typeof(VariableDataVector3Set),
                            ListType.LockOnList => typeof(VariableDataLockOnSet),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        break;
                    case OperationType.GetListLength:
                        tgtVType = typeof(VariableDataNumericSet);
                        break;
                    case OperationType.GetSum:
                    case OperationType.GetAverage:
                    case OperationType.GetMax:
                    case OperationType.GetMin:
                        tgtVType = (listType, parameterType) switch
                        {
                            (ListType.NumericList, _) => typeof(VariableDataNumericSet),
                            (ListType.VectorList, _) => typeof(VariableDataVector3Set),
                            (ListType.LockOnList, (int)LockOnParameterType.Distance) => typeof(VariableDataNumericSet),
                            (ListType.LockOnList, (int)LockOnParameterType.Position) => typeof(VariableDataVector3Set),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                tgtList = ResetVariableDataOnOtherType(tgtList, tgtListType);
                tgtV = ResetVariableDataOnOtherType(tgtV, tgtVType);

                pgbepManager.SetHeaderText(pgNodeParameter_getListStatusFuncPar.listVariable, pgNodeParDescription_getListStatusFuncPar.listVariable);
                switch (tgtList)
                {
                    case ListVariableData x:
                        pgbepManager.SetPgbepVariable(x, false);
                        break;
                    case VariableDataNumericGet x:
                        x.IndicateSwitchableFloat(pgbepManager);
                        break;
                    case VariableDataVector3Get x:
                        x.IndicateSwitchable(pgbepManager);
                        break;
                    case VariableDataLockOnGet x:
                        x.IndicateWithIndex(pgbepManager);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(tgtList)} : {tgtList?.GetType()}");
                }
                pgbepManager.SetHeaderText(pgNodeParameter_getListStatusFuncPar.operation, pgNodeParDescription_getListStatusFuncPar.operation);
                pgbepManager.SetPgbepEnum(typeof(OperationType), (int*)ot);
                if (listType is ListType.LockOnList)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_getListStatusFuncPar.parameterType, pgNodeParDescription_getListStatusFuncPar.parameterType);
                    pgbepManager.SetPgbepEnum(typeof(LockOnParameterType), pt);
                }
                pgbepManager.SetHeaderText(pgNodeParameter_getListStatusFuncPar.targetVariable, pgNodeParDescription_getListStatusFuncPar.targetVariable);
                switch (tgtV)
                {
                    case VariableDataNumericSet x:
                        x.IndicateSwitchable(pgbepManager);
                        break;
                    case VariableDataVector3Set x:
                        x.IndicateSwitchable(pgbepManager);
                        break;
                    case VariableDataLockOnSet x:
                        x.IndicateWithIndex(pgbepManager);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tgtV));
                }
            }
        }
        public void GetStatusValue(MachineLD ld)
        {
            switch (listType)
            {
                case ListType.NumericList:
                    GetStatusValueNumeric(ld);
                    break;
                case ListType.VectorList:
                    GetStatusValueVector3D(ld);
                    break;
                case ListType.LockOnList:
                    GetStatusValueLockOn(ld);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void GetStatusValueNumeric(MachineLD ld)
        {
            var tlv = ((VariableDataNumericList)tgtList).GetUseValue(ld);
            if (tlv == null || tlv.Count == 0) return;
            float res = 0;
            switch (operationType)
            {
                case OperationType.GetValue:
                    res = (tgtList as VariableDataNumericGet)?.GetUseValueFloat(ld) ?? 0;
                    //todo:ログウィンドウ実施時には取得できなかった時の警告を実装
                    break;
                case OperationType.GetListLength:
                    res = tlv.Count;
                    break;
                case OperationType.GetSum:
                    res = tlv.Sum();
                    break;
                case OperationType.GetAverage:
                    res = tlv.Sum() / tlv.Count;
                    break;
                case OperationType.GetMax:
                    res = tlv.Max();
                    break;
                case OperationType.GetMin:
                    res = tlv.Min();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ((VariableDataNumericSet)tgtV).SetNumericValue(ld, res);
        }
        private void GetStatusValueVector3D(MachineLD ld)
        {
            var tlv = ((VariableDataVector3DList)tgtList).GetUseValue(ld);
            if (tlv == null || tlv.Count == 0) return;
            float resN = 0;
            Vector3 resV = new Vector3();
            switch (operationType)
            {
                case OperationType.GetValue:
                    resV = (tgtList as VariableDataVector3Get)?.GetUseValue(ld) ?? default;
                    //todo:ログウィンドウ実施時には取得できなかった時の警告を実装
                    break;
                case OperationType.GetListLength:
                    resN = tlv.Count;
                    break;
                case OperationType.GetSum:
                    resV = Vector3.zero;
                    foreach (var v in tlv)
                    {
                        resV += v;
                    }
                    break;
                case OperationType.GetAverage:
                    resV = Vector3.zero;
                    foreach (var v in tlv)
                    {
                        resV += v;
                    }
                    resV /= tlv.Count;
                    break;
                case OperationType.GetMax:
                    resV = tlv[0];
                    for (var i = 1; i < tlv.Count; i++)
                    {
                        resV = Vector3.Max(resV, tlv[i]);
                    }
                    break;
                case OperationType.GetMin:
                    resV = tlv[0];
                    for (var i = 1; i < tlv.Count; i++)
                    {
                        resV = Vector3.Min(resV, tlv[i]);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (tgtV)
            {
                case VariableDataNumericSet x:
                    x.SetNumericValue(ld, resN);
                    break;
                case VariableDataVector3Set x:
                    x.SetVector3dValue(ld, resV);
                    break;
            }
        }
        private List<ObjectSearchTgt> _tgts = new();
        private void GetStatusValueLockOn(MachineLD ld)
        {
            var tlv = ((VariableDataLockOnList)tgtList).GetUseValue(ld);
            if (tlv == null || tlv.Count == 0) return;
            float resN = 0;
            Vector3 resV = new Vector3();
            ObjectSearchTgt resL = null;
            switch (operationType)
            {
                case OperationType.GetValue:
                    resL = (tgtList as VariableDataLockOnGet)?.GetUseValue(ld);
                    //todo:ログウィンドウ実施時には取得できなかった時の警告を実装
                    break;
                case OperationType.GetListLength:
                    resN = tlv.Count;
                    break;
                default:
                    _tgts.Clear();
                    foreach (var x in tlv)
                    {
                        if (x != null) _tgts.Add(x);
                    }
                    if (_tgts.Count == 0) break;
                    switch ((LockOnParameterType)parameterType)
                    {
                        case LockOnParameterType.Distance:
                            switch (operationType)
                            {
                                case OperationType.GetSum:
                                    foreach (var x in _tgts) resN += Vector3.Distance(ld.hd.pos, x.pos);
                                    break;
                                case OperationType.GetAverage:
                                    foreach (var x in _tgts) resN += Vector3.Distance(ld.hd.pos, x.pos);
                                    resN /= _tgts.Count;
                                    break;
                                case OperationType.GetMax:
                                    foreach (var x in _tgts)
                                    {
                                        resN = Mathf.Max(resN, Vector3.Distance(ld.hd.pos, x.pos));
                                    }
                                    break;
                                case OperationType.GetMin:
                                    foreach (var x in _tgts)
                                    {
                                        resN = Mathf.Min(resN, Vector3.Distance(ld.hd.pos, x.pos));
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case LockOnParameterType.Position:
                            switch (operationType)
                            {
                                case OperationType.GetSum:
                                    resV = Vector3.zero;
                                    foreach (var v in _tgts)
                                    {
                                        resV += v.pos;
                                    }
                                    break;
                                case OperationType.GetAverage:
                                    resV = Vector3.zero;
                                    foreach (var v in _tgts)
                                    {
                                        resV += v.pos;
                                    }
                                    resV /= _tgts.Count;
                                    break;
                                case OperationType.GetMax:
                                    resV = _tgts[0].pos;
                                    for (var i = 1; i < _tgts.Count; i++)
                                    {
                                        var v = _tgts[i];
                                        resV = Vector3.Max(resV, v.pos);
                                    }
                                    break;
                                case OperationType.GetMin:
                                    resV = _tgts[0].pos;
                                    for (var i = 1; i < _tgts.Count; i++)
                                    {
                                        var v = _tgts[i];
                                        resV = Vector3.Min(resV, v.pos);
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }

            switch (tgtV)
            {
                case VariableDataNumericSet x:
                    x.SetNumericValue(ld, resN);
                    break;
                case VariableDataVector3Set x:
                    x.SetVector3dValue(ld, resV);
                    break;
                case VariableDataLockOnSet x:
                    x.SetLockOn(ld, resL);
                    break;
            }
        }

        public VariableData ResetVariableDataOnOtherType(VariableData variableData, Type t)
        {
            if (variableData.GetType() == t) return variableData;
            return (VariableData)Activator.CreateInstance(t);
        }
    }
}