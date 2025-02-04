using clrev01.ClAction.Machines;

namespace clrev01.Programs.FuncPar.FuncParType
{
    [System.Serializable]
    public abstract class BranchFuncPar : PGBFuncPar
    {
        public override int calcCost => 10;
        public abstract bool BranchExecute(MachineLD ld);
    }
}