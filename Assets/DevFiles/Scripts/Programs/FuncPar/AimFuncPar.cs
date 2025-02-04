using static I2.Loc.ScriptLocalization;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AimFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.aim;
        public VariableDataLockOnGet aimTgtList = new();
        public long aimWeaponFlags = 0;


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (long* awf = &aimWeaponFlags)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_aimFuncPar.aimTarget, pgNodeParDescription_aimFuncPar.aimTarget);
                aimTgtList.IndicateWithIndex(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_aimFuncPar.aimWeapon, pgNodeParDescription_aimFuncPar.aimWeapon);
                pgbepManager.SetPgbepSelectFlagsOption(awf, pgbepManager.GetEquipmentList());
            }
        }

        public override long? GetNodeFaceSelectedWeapons()
        {
            return aimWeaponFlags;
        }

        public override string[] GetNodeFaceText()
        {
            return new[]
            {
                $"{aimTgtList.GetIndicateStr()}"
            };
        }
    }
}