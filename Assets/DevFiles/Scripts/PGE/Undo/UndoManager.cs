using clrev01.Extensions;
using clrev01.Save;
using System;
using System.Collections.Generic;
using System.Linq;

namespace clrev01.PGE.Undo
{
    public class UndoManager
    {
        //最新から戻った回数
        int returnCount = 0;
        int loggingNum = 0;

        public class UndoInfo
        {
            public int loggingNum;
            public List<(int index, PGBData pgbd)> pgbdLogs;

            public UndoInfo(int loggingNum)
            {
                this.loggingNum = loggingNum;
            }
        }

        UndoInfo currentUndoInfo;

        public List<UndoInfo> dataLog = new List<UndoInfo>();

        PGData _nowMonitorPG;
        public PGData nowMonitorPG
        {
            get => _nowMonitorPG;
            set
            {
                _nowMonitorPG = value;
                returnCount = 0;
                dataLog.Clear();
                var isValid = UpdatePgbdStart();
                UpdatePgbdLog(isValid);
                UpdatePgbdEnd(isValid);
            }
        }

        public bool UpdatePgbdStart()
        {
            if (currentUndoInfo != null) return false;
            while (returnCount > 0)
            {
                dataLog.RemoveAt(dataLog.Count - 1);
                returnCount--;
            }
            currentUndoInfo = new UndoInfo(loggingNum)
            {
                pgbdLogs = new List<(int index, PGBData pgbd)>()
            };
            return true;
        }
        public void UpdatePgbdLog(bool isValid, List<int> updateIndexes = null)
        {
            if (!isValid) return;
            if (currentUndoInfo == null)
            {
                throw new Exception("Not Undo Started!");
            }
            if (updateIndexes == null)
            {
                for (var i = 0; i < nowMonitorPG.pgList.Count; i++)
                {
                    var pgbd = nowMonitorPG.pgList[i];
                    currentUndoInfo.pgbdLogs.Add(pgbd == null ? (i, null) : (i, pgbd.CloneDeep()));
                }
            }
            else
            {
                foreach (var index in updateIndexes)
                {
                    var newData = nowMonitorPG.pgList[index];
                    currentUndoInfo.pgbdLogs.Add((index, newData?.CloneDeep()));
                }
            }
        }
        public void UpdatePgbdEnd(bool isValid)
        {
            if (!isValid) return;
            if (currentUndoInfo == null)
            {
                throw new Exception("Not Undo Started!");
            }
            dataLog.Add(currentUndoInfo);
            currentUndoInfo = null;
            loggingNum++;
        }

        public void ExecuteUndo()
        {
            if (returnCount >= dataLog.Count - 1) return;
            var undoLog = dataLog[dataLog.Count - returnCount - 1];
            if (undoLog.pgbdLogs != null)
            {
                for (int i = undoLog.pgbdLogs.Count - 1; i >= 0; i--)
                {
                    var undoIndex = undoLog.pgbdLogs[i].index;
                    PGBData oldData = null;
                    for (int j = dataLog.Count - returnCount - 1 - 1; j >= 0; j--)
                    {
                        if (dataLog[j].pgbdLogs.Any(x => x.index == undoIndex))
                        {
                            var current = dataLog[j].pgbdLogs.Find(x => x.index == undoIndex);
                            oldData = current.pgbd?.CloneDeep();
                            break;
                        }
                    }
                    nowMonitorPG.pgList[undoIndex] = oldData;
                }
            }
            returnCount++;
        }
        public void ExecuteRedo()
        {
            if (returnCount <= 0) return;
            returnCount--;
            var redoLog = dataLog[dataLog.Count - returnCount - 1];
            if (redoLog.pgbdLogs != null)
            {
                for (int i = redoLog.pgbdLogs.Count - 1; i >= 0; i--)
                {
                    var redoIndex = redoLog.pgbdLogs[i].index;
                    PGBData newData = null;
                    for (int j = dataLog.Count - returnCount - 1; j < dataLog.Count; j++)
                    {
                        if (dataLog[j].pgbdLogs.Any(x => x.index == redoIndex))
                        {
                            var current = dataLog[j].pgbdLogs.Find(x => x.index == redoIndex);
                            newData = current.pgbd?.CloneDeep();
                            break;
                        }
                    }
                    nowMonitorPG.pgList[redoIndex] = newData;
                }
            }
        }
    }
}