using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Programs.FuncPar;
using JetBrains.Annotations;
using MemoryPack;
using System;
using System.Collections.Generic;

namespace clrev01.Save.VariableData
{
    [Serializable]
    [MemoryPackable()]
    public partial class VariableDataLockOn : VariableData
    {
        public VariableDataIndex indexV = new();
        public override List<VariableType> selectableVariableTypes { get; } = new() { VariableType.LockOn, VariableType.LockOnList };

        [MemoryPackIgnore]
        [NonSerialized]
        protected IVariableValueLockOn value;


        public VariableDataLockOn()
        {
            name = "LockOn01";
            variableType = VariableType.LockOnList;
        }

        public string GetIndicateStr()
        {
            return $"[{UtlOfCL.GetEllipsisString(name, 16, 5)}{(variableType is VariableType.NumericList ? indexV.GetIndexStr() : null)}]";
        }
    }

    [MemoryPackable()]
    public partial class VariableDataLockOnGet : VariableDataLockOn
    {
        public void IndicateWithIndex(PgbepManager pgbepManager, List<VariableType> selectableVariableOverwrite = null)
        {
            pgbepManager.SetPgbepVariable(this, false, selectableVariableOverwrite);
            if (variableType is VariableType.LockOnList)
            {
                indexV.IndicateSwitchable(pgbepManager);
            }
        }
        public ObjectSearchTgt GetUseValue([NotNull] MachineLD ld)
        {
            value ??= ld.RegisterVariableDict<IVariableValueLockOn>(this);
            return value.GetLockOnValue(indexV.GetUseValue(ld));
        }
        public List<ObjectSearchTgt> GetUseList([NotNull] MachineLD ld)
        {
            value ??= ld.RegisterVariableDict<VariableValueLockOnList>(this);
            return (value as VariableValueLockOnList)?.Value;
        }
    }

    [MemoryPackable()]
    public partial class VariableDataLockOnSet : VariableDataLockOn
    {
        public AddToListMethodType addToListMethodType = AddToListMethodType.Replace;
        public NullHandlingType nullHandlingType = NullHandlingType.IgnoreNull;
        public unsafe void IndicateWithIndex(PgbepManager pgbepManager, List<VariableType> selectableVariableOverwrite = null)
        {
            pgbepManager.SetPgbepVariable(this, false, selectableVariableOverwrite);
            if (variableType is VariableType.LockOnList)
            {
                fixed (AddToListMethodType* atl = &addToListMethodType)
                {
                    pgbepManager.SetPgbepEnum(typeof(AddToListMethodType), (int*)atl);
                    if (addToListMethodType is not AddToListMethodType.Add and not AddToListMethodType.UniqueAdd) indexV.IndicateSwitchable(pgbepManager);
                }
            }
            fixed (NullHandlingType* nht = &nullHandlingType)
            {
                pgbepManager.SetPgbepEnum(typeof(NullHandlingType), (int*)nht);
            }
        }
        public void SetLockOn(MachineLD ld, ObjectSearchTgt v)
        {
            if (nullHandlingType is NullHandlingType.IgnoreNull && v is null) return;
            value ??= ld.RegisterVariableDict<IVariableValueLockOn>(this);
            value.SetLockOnValue(v, indexV.GetUseValue(ld), addToListMethodType);
        }
        public void SetLockOnList(MachineLD ld, params ObjectSearchTgt[] vl)
        {
            value ??= ld.RegisterVariableDict<VariableValueLockOnList>(this);
            var list = (value as VariableValueLockOnList)?.Value;
            if (list is null) return;
            list.Clear();
            if (nullHandlingType is NullHandlingType.IgnoreNull)
            {
                foreach (var v in vl)
                {
                    if (v is not null) list.Add(v);
                }
            }
            else list.AddRange(vl);
        }
    }
}