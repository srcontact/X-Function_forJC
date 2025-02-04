using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using Cysharp.Text;
using System.Text;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.OptionParts
{
    [CreateAssetMenu(menuName = "OptionPartsData/Radiation")]
    public class RadiationOptionPartsData : OptionPartsData
    {
        public int radiationInterval = 1;
        public int radiationAmount = 5;

        public override void ExeOptionParts(MachineLD ld, int slotNum)
        {
            var usingFrame = ld.statePar.optionPartsUseFrameDict[slotNum];
            if (radiationAmount > 0 &&
                (usingFrame - durationFrame) % radiationInterval == 0)
            {
                ld.statePar.heat = Mathf.Max(ld.statePar.heat - radiationAmount, StaticInfo.Inst.actionLevelHub.levels[StaticInfo.Inst.PlayMatch.playLevelNum].levelTemperature);
            }
        }

        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_optionals_ratdiation.radiationInterval} {radiationInterval} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals_ratdiation.radiationAmount} {radiationAmount}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals_ratdiation.radiationAmountSum} {radiationAmount * durationFrame / radiationInterval}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals.baseWeight} {weight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals.unitWeight} {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}