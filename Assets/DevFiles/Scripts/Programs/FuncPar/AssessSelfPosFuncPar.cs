using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;
using System.Linq;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessSelfPosFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessSelfPos;

        public VariableDataLockOnGet targetLockOnList = new();
        public ISearchFieldUnion searchFieldPar = new BoxSearchFieldParVariable();

        public enum ReferencePartType
        {
            Body,
            Turret,
        }

        public ReferencePartType referencePart;
        public long turretNumber = long.MaxValue;


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (ReferencePartType* rp = &referencePart)
            fixed (long* turn = &turretNumber)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfPosFuncPar.target, pgNodeParDescription_assessSelfPosFuncPar.target);
                targetLockOnList.IndicateWithIndex(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfPosFuncPar.referencePart, pgNodeParDescription_assessSelfPosFuncPar.referencePart);
                pgbepManager.SetPgbepEnum(typeof(ReferencePartType), (int*)rp);
                if (referencePart is ReferencePartType.Turret)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessSelfPosFuncPar.weaponNumber, pgNodeParDescription_assessSelfPosFuncPar.weaponNumber);
                    pgbepManager.SetPgbepSelectFlagsOption(turn, Enumerable.Range(1, 7).ToList().ConvertAll(x => $"Weapon{x}"));
                }
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfPosFuncPar.searchField, pgNodeParDescription_assessSelfPosFuncPar.searchField);
                pgbepManager.SetPgbepField(searchFieldPar, (res) => { searchFieldPar = (ISearchFieldUnion)res; });
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            var lockOnTgt = targetLockOnList.GetUseValue(ld);
            if (lockOnTgt == null) return false;
            switch (referencePart)
            {
                case ReferencePartType.Body:
                    return searchFieldPar.AssessTgtPos(ld, lockOnTgt.transform, ld.hd.pos);
                case ReferencePartType.Turret:
                    var tgtHd = lockOnTgt.hardBase as MachineHD;
                    if (tgtHd == null) return false;
                    var shootPoints = tgtHd.useShootPoints;
                    for (int i = 0; i < shootPoints.Count; i++)
                    {
                        var l = 1L << i;
                        if ((turretNumber & l) != l) continue;
                        if (searchFieldPar.AssessTgtPos(ld, shootPoints[i], ld.hd.pos)) return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { $"{targetLockOnList.GetIndicateStr()} {searchFieldPar.GetFieldShortText()}" };
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return searchFieldPar;
        }
    }
}