using clrev01.ClAction.Machines;
using clrev01.Extensions;
using Cysharp.Text;
using System.Text;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.OptionParts
{
    [CreateAssetMenu(menuName = "OptionPartsData/Repair")]
    public class RepairOptionPartsData : OptionPartsData
    {
        public int whenUsedRecoveryInterval = 1;
        public int whenUsedRecoveryAmount = 10;
        public int whenUsedRecoveryOperableAcceleration = 0;

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
            sb.AppendLine($"{hardwareCustom_optionals_repair.recoveryInterval} {whenUsedRecoveryInterval} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals_repair.unitRecoveryAmount} {whenUsedRecoveryAmount} Point".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals_repair.durationFrame} {durationFrame} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals_repair.sumRecoveryAmount} {whenUsedRecoveryAmount * durationFrame / whenUsedRecoveryInterval} Point".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals.baseWeight} {weight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals.unitWeight} {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}