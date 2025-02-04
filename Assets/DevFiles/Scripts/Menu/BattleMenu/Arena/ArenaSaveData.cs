using System.Collections.Generic;
using clrev01.Save;
using MemoryPack;

namespace clrev01.Menu.BattleMenu.Arena
{
    [System.Serializable]
    [MemoryPackable]
    public partial class ArenaSaveData : SaveData
    {
        public Dictionary<int, Dictionary<int, BattleResultData>> battleResults = new();

        public BattleResultData GetBattleResult(int arenaCode, int battleCode)
        {
            if (!battleResults.TryGetValue(arenaCode, out var arenaResults)) return null;
            return arenaResults.GetValueOrDefault(battleCode);
        }
    }
}