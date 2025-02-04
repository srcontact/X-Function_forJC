using clrev01.Bases;
using I2.Loc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace clrev01.Menu.BattleMenu.Arena
{
    [CreateAssetMenu(menuName = "Arena/ArenaData")]
    public class ArenaData : SOBaseOfCL
    {
        [SerializeField]
        private string dataName = "ArenaData";
        [field: SerializeField]
        public LocalizedString title { get; set; }
        public string titleStr => title != null ? title : dataName;
        [SerializeField]
        private List<ArenaBattleDataSet> arenaBattleDataList = new();
        public int arenaBattleCount => arenaBattleDataList.Count;

        public ArenaBattleDataSet GetArenaBattle(int id)
        {
            return id >= 0 && id < arenaBattleDataList.Count ? arenaBattleDataList[id] : null;
        }
    }

    [System.Serializable]
    public class ArenaBattleDataSet
    {
        public int code;
        public string name;
        [FormerlySerializedAs("arenaBattleData")]
        public ArenaBattleData battleData;
    }
}