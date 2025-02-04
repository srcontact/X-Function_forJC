using System;
using clrev01.Save;
using MemoryPack.Compression;
using UnityEngine;

namespace clrev01.Menu.BattleMenu.Arena
{
    [Serializable]
    public class BattleTeamData
    {
        [SerializeField]
        private ScriptableTextAsset teamData;

        public TeamData GetTeamData()
        {
            using var dc = new BrotliDecompressor();
            return MemoryPack.MemoryPackSerializer.Deserialize<TeamData>(dc.Decompress(teamData.bytes));
        }
    }
}