using clrev01.ClAction.Machines;
using clrev01.Extensions;
using Cysharp.Text;
using System.Text;
using UnityEngine;

namespace clrev01.ClAction.OptionParts
{
    [CreateAssetMenu(menuName = "OptionPartsData/EnergySupply")]
    public class EnergySupplyOptionPartsData : OptionPartsData
    {
        public int whenUsedRecoveryInterval = 1;
        public int whenUsedRecoveryAmount = 10;

        public override void ExeOptionParts(MachineLD ld, int slotNum)
        {
            var usingFrame = ld.statePar.optionPartsUseFrameDict[slotNum];
            if (whenUsedRecoveryAmount > 0 &&
                (usingFrame - durationFrame) % whenUsedRecoveryInterval == 0)
            {
                ld.statePar.damage = Mathf.Max(ld.statePar.damage - whenUsedRecoveryAmount, 0);
            }
        }
        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"Recovery Interval {whenUsedRecoveryInterval} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Recovery Amount {whenUsedRecoveryAmount} Point".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Duration Frame {durationFrame} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Sum Recovery Amount {whenUsedRecoveryAmount * durationFrame / whenUsedRecoveryInterval} Point".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Base Weight {weight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Unit Weight {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}