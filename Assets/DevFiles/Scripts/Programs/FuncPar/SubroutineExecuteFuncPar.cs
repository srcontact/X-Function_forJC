using static I2.Loc.ScriptLocalization;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using System.Collections.Generic;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class SubroutineExecuteFuncPar : SubroutineFuncPar
    {
        public override string BlockTypeStr => pgNodeName.subroutineExecute;

        public int subroutineRootToGo = -1;

        private Dictionary<int, string> _subroutineRootDict;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            GetSubroutineDict();

            pgbepManager.SetHeaderText(pgNodeParameter_subroutineExecuteFuncPar.goToSubroutine, pgNodeParDescription_subroutineExecuteFuncPar.goToSubroutine);
            fixed (int* srd = &subroutineRootToGo)
            {
                pgbepManager.SetPgbepSubroutine(srd, _subroutineRootDict);
            }
        }
        private void GetSubroutineDict()
        {
            _subroutineRootDict = new Dictionary<int, string>();
            var pgList = PGEM2.nowEditPD.pgList;
            _subroutineRootDict.Add(-1, "<none>");
            for (int i = 0; i < pgList.Count; i++)
            {
                if (pgList[i] == null || pgList[i].funcPar.GetType() != typeof(SubroutineRootFuncPar)) continue;
                _subroutineRootDict.Add(i, ((SubroutineRootFuncPar)pgList[i].funcPar).subroutineName.obj);
            }
        }

        public override string[] GetNodeFaceText()
        {
            GetSubroutineDict();
            var str = _subroutineRootDict.ContainsKey(subroutineRootToGo) ? _subroutineRootDict[subroutineRootToGo] : "";
            return new[] { str };
        }
    }
}