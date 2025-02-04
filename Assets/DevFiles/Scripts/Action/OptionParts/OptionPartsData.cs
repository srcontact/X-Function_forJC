using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using I2.Loc;
using System.Text;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.OptionParts
{
    public abstract class OptionPartsData : SOBaseOfCL, IInfoTextData, IWeightSetting
    {
        [field: SerializeField]
        public LocalizedString description { get; set; }
        public int durationFrame = 180;

        public float weight = 100;
        public int ammoWeight = 100;

        public virtual void InitOptionParts(MachineLD ld, int slotNum)
        { }
        public abstract void ExeOptionParts(MachineLD ld, int slotNum);
        public abstract void GetParameterText(ref Utf8ValueStringBuilder sb);

        public virtual void StandbyPoolActors(int num)
        { }
        public float GetWeightValue(int ammoNum = 0)
        {
            return weight + ammoWeight * ammoNum;
        }
        public void GetParameterText(ref Utf8ValueStringBuilder sb, int ammoNum)
        {
            GetParameterText(ref sb);
            sb.AppendLine($"{hardwareCustom_optionals.grossWeight} {GetWeightValue(ammoNum)} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}