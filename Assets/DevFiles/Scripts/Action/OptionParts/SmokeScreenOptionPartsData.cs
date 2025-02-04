using clrev01.ClAction.Machines;
using clrev01.ClAction.SmokeScreen;
using clrev01.Extensions;
using Cysharp.Text;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.OptionParts
{
    [CreateAssetMenu(menuName = "OptionPartsData/SmokeScreen")]
    public class SmokeScreenOptionPartsData : OptionPartsData
    {
        [SerializeField]
        private SmokeScreenCD smokeScreenCD;
        [SerializeField]
        private float spawnInitSpeed = 20;

        public override void StandbyPoolActors(int num)
        {
            base.StandbyPoolActors(1);
            smokeScreenCD.StandbyPoolActors(1);
        }
        public override void InitOptionParts(MachineLD ld, int slotNum)
        {
            base.InitOptionParts(ld, slotNum);
            var initVelocity = ld.hd.rigidBody.linearVelocity + spawnInitSpeed * ld.hd.transform.forward;
            smokeScreenCD.SpawnSmokeScreen(ld.hd, ld.hd.pos, ld.hd.rot);
        }
        public override void ExeOptionParts(MachineLD ld, int slotNum)
        { }
        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            smokeScreenCD.GetParameterText(ref sb);
            sb.AppendLine($"{hardwareCustom_optionals.baseWeight} {weight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_optionals.unitWeight} {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}