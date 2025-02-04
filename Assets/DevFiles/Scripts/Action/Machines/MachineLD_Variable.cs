using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Save;
using clrev01.Save.VariableData;
using System;
using System.Collections.Generic;

namespace clrev01.ClAction.Machines
{
    public partial class MachineLD
    {
        /// <summary>
        /// 設定ずみの変数の辞書
        /// </summary>
        [NonSerialized]
        public Dictionary<VariableType, Dictionary<int, VariableValueBase>> variableValueDict = new();
        public List<VariableValueBase> indicateVariableList = new();


        /// <summary>
        /// VariableLockOnListDictに変数を設定する
        /// </summary>
        /// <param name="variableData">変数データ</param>
        /// <returns>設定対象の変数の現在の値を返す</returns>
        public T RegisterVariableDict<T>(VariableData variableData) where T : VariableValueBase
        {
            return RegisterVariableDict<T>(variableData.variableType, variableData.name);
        }

        public T RegisterVariableDict<T>(VariableType variableType, string name) where T : VariableValueBase
        {
            T vv;
            var hash = name.GetHashCode();
            if (!variableValueDict.TryGetValue(variableType, out var tvd))
            {
                tvd = new Dictionary<int, VariableValueBase>();
                variableValueDict.Add(variableType, tvd);
            }
            if (!tvd.ContainsKey(hash))
            {
                vv = (T)Activator.CreateInstance(variableType.GetVariableValueType());
                vv.Name = name;
                tvd.Add(hash, vv);
                indicateVariableList.Add(vv);
            }
            else vv = (T)tvd[hash];
            return vv;
        }
    }
}