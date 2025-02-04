using System;
using System.Collections.Generic;
using System.Linq;
using clrev01.Menu.BattleMenu.Arena;
using clrev01.Save;
using MemoryPack;
using UnityEngine;

namespace clrev01.Menu.BattleMenu
{
    [System.Serializable]
    [MemoryPackable]
    public partial class BattleResultData : SaveData
    {
        public int elapsedFrame;
        public List<List<float>> remainingHpData = new();
        public List<int> numberOfDefeats = new();
        [MemoryPackIgnore]
        public float friendHpSum => remainingHpData[0].Sum();
        [MemoryPackIgnore]
        public int defeatEnemyNum => numberOfDefeats.Skip(1).Sum();

        public bool GetIsCleared(BattleRuleType ruleType)
        {
            var remainHpSum = remainingHpData.ConvertAll(x => x.Sum());
            var maxHpSum = remainHpSum.Max();
            var friendDefeat = numberOfDefeats[0];
            var minDefeat = numberOfDefeats.Min();
            return ruleType switch
            {
                BattleRuleType.RemainingHp => Mathf.Approximately(friendHpSum, maxHpSum) && remainHpSum.Count(x => Mathf.Approximately(x, maxHpSum)) == 1,
                BattleRuleType.NumberOfDefeats => friendDefeat == minDefeat && numberOfDefeats.Count(x => x == minDefeat) == 1,
                BattleRuleType.Elimination => numberOfDefeats.Skip(1).All(x => x == 0),
                BattleRuleType.ScoreAttack => true,
                _ => throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null)
            };
        }

        public string GetResultText(BattleRuleType ruleType)
        {
            return $"Result:" +
                   $"\n {(GetIsCleared(ruleType) ? "[Cleared]" : "[Not Cleared]")}" +
                   $"\n Defeated Friend : {numberOfDefeats[0]}" +
                   $"\n Defeat Enemy : {numberOfDefeats.Sum() - numberOfDefeats[0]}" +
                   $"\n Friend Remaining HP : {remainingHpData[0].Sum() * 100}%" +
                   $"\n Enemy Remaining HP : {(remainingHpData.Sum(x => x.Sum()) - remainingHpData[0].Sum()) * 100}%" +
                   $"\n Elapsed Time : {elapsedFrame / 60f}";
        }
    }
}