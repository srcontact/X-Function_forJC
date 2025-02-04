using clrev01.Bases;
using clrev01.Programs;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using System;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Save
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class PGBData
    {
        public PGBEditorPar editorPar = new PGBEditorPar();
        public IPGBFuncUnion funcPar = new MoveFuncPar();
        [MemoryPackIgnore]
        public int currentExecutedFrame = int.MinValue;

        [MemoryPackConstructor]
        public PGBData() { }
        public PGBData(Type nodeType, Vector3 editorPos, int myIndex, int nextIndex = -1, int fNextIndex = -1)
        {
            funcPar = (IPGBFuncUnion)Activator.CreateInstance(nodeType);
            editorPar = new PGBEditorPar(myIndex, nextIndex, fNextIndex, editorPos);
        }

        public virtual void ConnectionChanging(PGBData connectChangeTgt, int connectNum)
        {
            if (connectChangeTgt == null) return;

            var type = funcPar.GetType();
            //接続するPGBのいずれかがどのルーチンにも属していないか、同じルーチンに属していれば接続する
            if (connectChangeTgt == this ||
                funcPar.IsConnectable(connectChangeTgt.funcPar) &&
                (connectChangeTgt.editorPar.routineNum == -1 || editorPar.routineNum == -1 || editorPar.routineNum == connectChangeTgt.editorPar.routineNum))
            {
                var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
                switch (connectNum)
                {
                    case 0:
                        connectChangeTgt.editorPar.nextIndex = editorPar.myIndex;
                        break;
                    case 1:
                        connectChangeTgt.editorPar.falseNextIndex = editorPar.myIndex;
                        break;
                }
                StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, new List<int>() { connectChangeTgt.editorPar.myIndex });
                StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
                PGEM2.SetRoutineNum();
            }
        }

        public bool GetReturnFlag(bool branchFlag = true)
        {
            int next;
            if (!branchFlag && funcPar is BranchFuncPar) next = editorPar.falseNextIndex;
            else next = editorPar.nextIndex;
            return next < 0 || next == editorPar.myIndex;
        }
    }
}