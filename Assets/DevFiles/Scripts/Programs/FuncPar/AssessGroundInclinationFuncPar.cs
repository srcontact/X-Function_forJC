using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class AssessGroundInclinationFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessGroundInclination;
        public CoordinateSystemType coordinateSystemType;

        public enum PositonDesignationType
        {
            Auto,
            Manual,
        }

        public PositonDesignationType positonDesignationType;

        public TimeUnitType timeUnitType = TimeUnitType.Frame;
        public VariableDataNumeric timeValue;

        public VariableDataVector3 positionDesignation;


        public override bool BranchExecute(MachineLD ld)
        {
            throw new NotImplementedException();
        }

        public override void SetPointers(PgbepManager pgbepManager)
        {
            throw new NotImplementedException();
        }
    }
}