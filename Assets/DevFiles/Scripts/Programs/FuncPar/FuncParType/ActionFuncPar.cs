using clrev01.ClAction.Machines;

namespace clrev01.Programs.FuncPar.FuncParType
{
    [System.Serializable]
    public abstract class ActionFuncPar : PGBFuncPar
    {
        public override int calcCost => 20;

        public virtual bool CheckChangeAction(MachineLD ld)
        {
            return false;
        }

        public virtual void OnChangeAction(MachineLD ld) { }

        public virtual void InitOnExecute(MachineLD ld) { }

        public abstract bool ActionExecute(MachineLD ld);

        public abstract bool ActionOverwritable(ActionFuncPar other, MachineLD ld);

        public virtual bool CheckIsExecutable(MachineLD ld)
        {
            return true;
        }

        public virtual bool CheckEnd(MachineLD ld)
        {
            return false;
        }

        public virtual void OnEndAction(MachineLD ld) { }

        public virtual void OnCancelRun(MachineLD ld)
        {
            OnEndAction(ld);
        }

        public abstract float EnergyCost(MachineLD ld);

        public abstract float HeatCost(MachineLD ld);
    }
}