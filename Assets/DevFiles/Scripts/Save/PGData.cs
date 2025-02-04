using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Programs.FuncPar;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace clrev01.Save
{
    [Serializable]
    [MemoryPackable()]
    public partial class PGData
    {
        /// <summary>
        /// PGのノードの最大数
        /// </summary>
        public const int PgListCountMax = 1024;

        #region defaultExeResource
        //todo:defaultExeResourceは保存しない。
        [SerializeField]
        private int _defaultExeResource = 100;
        [MemoryPackIgnore]
        public int defaultExeResource
        {
            get => _defaultExeResource;
            set => _defaultExeResource = value;
        }
        #endregion
        #region maxExeResource
        [SerializeField]
        //todo:maxExeResourceは保存しない。
        private int _maxExeResource = 1500;
        public int maxExeResource
        {
            get => _maxExeResource;
            set => _maxExeResource = value;
        }
        #endregion
        public List<PGBData> pgList;


        public PGData()
        {
            pgList = new List<PGBData>
            {
                new() { funcPar = new StartFuncPar() }
            };
        }

        private int CheckIndexAndAdd()
        {
            int myIndex = pgList.FindIndex(0, x => x == null);
            if (myIndex == -1)
            {
                pgList.Add(null);
                myIndex = pgList.Count - 1;
            }
            return myIndex;
        }

        public PGBData CreatePGBD(Type nodeType, Vector3 editorPos, int nextIndex = -1, int fNextIndex = -1)
        {
            var myIndex = CheckIndexAndAdd();
            PGBData nd = new PGBData(nodeType, editorPos, myIndex, nextIndex, fNextIndex);
            pgList[myIndex] = nd;
            var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
            StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, new List<int>() { myIndex });
            StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
            return nd;
        }

        public void PastePGBDs(Vector3 editorPos, List<PGBData> origl)
        {
            List<int> oldNums = new List<int>();
            List<int> newNums = new List<int>();
            List<PGBData> cpl = origl.ConvertAll(x => x.CloneDeep());
            SubroutineNameRepetitionCorrection(
                pgList.Where(x => x is { funcPar: SubroutineRootFuncPar }).ToList().ConvertAll(x => (SubroutineRootFuncPar)x.funcPar),
                cpl.Where(x => x is { funcPar: SubroutineRootFuncPar }).ToList().ConvertAll(x => (SubroutineRootFuncPar)x.funcPar));

            foreach (var p in cpl)
            {
                oldNums.Add(p.editorPar.myIndex);
                var addIndex = CheckIndexAndAdd();
                p.editorPar.myIndex = addIndex;
                newNums.Add(p.editorPar.myIndex);
                p.editorPar.EditorPos += new Vector2(editorPos.x, editorPos.y);
                pgList[p.editorPar.myIndex] = p;
            }
            foreach (var p in cpl)
            {
                OverwriteOldNum(oldNums, newNums, ref p.editorPar.nextIndex);
                OverwriteOldNum(oldNums, newNums, ref p.editorPar.falseNextIndex);
            }
            var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
            StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, cpl.ConvertAll(x => x.editorPar.myIndex));
            StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
        }

        private const string Pattern = "_[0-9]{3}$";
        /// <summary>
        /// サブルーチンの名前被りを修正する
        /// </summary>
        /// <param name="existingSubroutines">被ってはいけないサブルーチンルートノードのリスト</param>
        /// <param name="checkTgts">検証を行う対象</param>
        public void SubroutineNameRepetitionCorrection(IEnumerable<SubroutineRootFuncPar> existingSubroutines, IEnumerable<SubroutineRootFuncPar> checkTgts)
        {
            foreach (var sef in checkTgts)
            {
                while (existingSubroutines.Any(x => x.subroutineName.obj.Equals(sef.subroutineName.obj)))
                {
                    var regex = Regex.Match(sef.subroutineName.obj, Pattern);
                    if (regex.Success)
                    {
                        var str = regex.Value.Remove(0, 1);
                        var num = int.Parse(str) + 1;
                        var removed = sef.subroutineName.obj.Remove(sef.subroutineName.obj.Length - 4, 4);
                        sef.subroutineName.obj = removed + "_" + num.ToString("000");
                    }
                    else
                    {
                        sef.subroutineName.obj += "_001";
                    }
                }
            }
        }

        void OverwriteOldNum(List<int> oldNums, List<int> newNums, ref int tgti)
        {
            if (!oldNums.Contains(tgti)) tgti = -1;
            else tgti = newNums[oldNums.IndexOf(tgti)];
        }

        public void DeletePGBDs(List<PGBData> dl)
        {
            List<int> delNums = new List<int>();
            for (int i = 0; i < dl.Count; i++)
            {
                for (int j = 0; j < pgList.Count; j++)
                {
                    if (dl[i].funcPar is StartFuncPar) continue;
                    if (dl[i] == pgList[j])
                    {
                        delNums.Add(pgList[j].editorPar.myIndex);
                        //PGList[j].block = null;
                        pgList[j] = null;
                    }
                }
            }

            List<int> editedOnDeleteList = new List<int>();
            editedOnDeleteList.AddRange(delNums);
            for (int i = 0; i < pgList.Count; i++)
            {
                if (pgList[i] == null) continue;
                bool edited = false;
                foreach (var dn in delNums)
                {
                    if (pgList[i].editorPar.nextIndex == dn)
                    {
                        pgList[i].editorPar.nextIndex = -1;
                        edited = true;
                    }
                    if (pgList[i].editorPar.falseNextIndex == dn)
                    {
                        pgList[i].editorPar.falseNextIndex = -1;
                        edited = true;
                    }
                }
                if (edited) editedOnDeleteList.Add(i);
            }
            var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
            StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, editedOnDeleteList);
            StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
        }


        public Bounds CalcPgbdBounds()
        {
            Vector3 programCenter = Vector3.zero;
            int count = 0;
            foreach (var pgbd in pgList)
            {
                if (pgbd == null) continue;
                programCenter += new Vector3(pgbd.editorPar.EditorPos.x, pgbd.editorPar.EditorPos.y, 0);
                count++;
            }
            programCenter /= count;
            Bounds programBounds = new Bounds(programCenter, Vector3.one * 10);
            foreach (var pgbd in pgList)
            {
                if (pgbd == null) continue;
                programBounds.Encapsulate(pgbd.editorPar.EditorPos);
            }
            return programBounds;
        }
    }
}