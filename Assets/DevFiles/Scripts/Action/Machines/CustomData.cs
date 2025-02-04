using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Programs;
using clrev01.Programs.FuncPar;
using clrev01.Save;
using MemoryPack;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class CustomData : SaveData
    {
        public MachineCustomPar mechCustom = new MachineCustomPar();
        public PGData program = new PGData();

        public MachineHD InstActor(Vector3 position, Quaternion rotation)
        {
            MachineHD nhd = MHUB.GetData(mechCustom.machineCode).machineCD.InstActor(position, rotation);
            nhd.ld.customData = this;
            RegisterPgbDict(nhd.ld.pgExeData);
            nhd.SetAdditionalTurrets();
            return nhd;
        }

        public override void InitializeData()
        {
            base.InitializeData();
            mechCustom.RegisterDefaultWeapons();
        }

        /// <summary>
        /// アクション時に使用するプログラムのDictionaryを設定する。
        /// </summary>
        /// <param name="data"></param>
        public void RegisterPgbDict(PGExeData data)
        {
            CheckAndRecoveryStart();
            data.pgbdDict.Clear();
            data.copiedPGData = program.CloneDeep();
            foreach (var pgbData in data.copiedPGData.pgList)
            {
                if (pgbData == null) continue;
                data.pgbdDict.TryAdd(pgbData.editorPar.myIndex, pgbData);
            }
            data.programBounds = data.copiedPGData.CalcPgbdBounds();
        }

        private void CheckAndRecoveryStart()
        {
            //リストの先頭にスタートブロックがない場合は作成。
            program.pgList[0] ??= new PGBData();
            if (program.pgList[0].funcPar is not StartFuncPar)
            {
                program.pgList[0].funcPar = new StartFuncPar();
            }
            //リストのトップ以外にスタートブロックがある場合はとりあえずMoveに変更。
            for (int i = 1; i < program.pgList.Count; i++)
            {
                if (program.pgList[i] == null) continue;
                if (program.pgList[i].funcPar is StartFuncPar)
                {
                    program.pgList[i].funcPar = new MoveFuncPar();
                }
            }
        }
    }
}