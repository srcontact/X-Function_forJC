using clrev01.Save;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Programs
{
    /// <summary>
    /// プログラムのローカル実行データ。
    /// 実行位置、リソース、プログラムの変数を格納。
    /// </summary>
    [System.Serializable]
    public class PGExeData
    {
        public int pgExePos = 0;
        public int exeResource = 0;
        [NonSerialized]
        public PGData copiedPGData;
        [NonSerialized]
        public Bounds programBounds;
        [NonSerialized]
        public Dictionary<int, PGBData> pgbdDict = new Dictionary<int, PGBData>();
    }
}