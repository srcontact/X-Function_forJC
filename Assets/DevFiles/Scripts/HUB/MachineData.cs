using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using I2.Loc;
using Sirenix.OdinInspector;
using System.Linq;
using System.Text;
using UnityEngine;

namespace clrev01.HUB
{
    [System.Serializable]
    public class MachineData : HubData, IInfoTextData
    {
        [SerializeField, ValueDropdown("MachineNameList")]
        private string machineName = MachineNameList.FirstOrDefault();
        public string MachineName => (string)typeof(I2.Loc.ScriptLocalization.hardwareCustom_machineType).GetProperty(machineName)?.GetValue(null);
        public MachineCD machineCD => machineCdReference.GetAsset(StaticInfo.Inst.gameObject);
        [SerializeField]
        private AssetReferenceSet<MachineCD> machineCdReference;

        private static string[] MachineNameList
        {
            get
            {
                return typeof(I2.Loc.ScriptLocalization.hardwareCustom_machineType).GetProperties().ToList().ConvertAll(x => x.Name).ToArray();
            }
        }

        public LocalizedString description { get; set; }

        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            //sb.AppendLine($"Max Power {maxPower:0.##}".GetNormalInfoStr());
            //sb.AppendLine($"Use Energy At Thrust {thrustUseEnergy:0.##}".GetNormalInfoStr());
            //sb.AppendLine($"Heat At Thrust {thrustHeat:0.##}".GetNormalInfoStr());
            //sb.AppendLine($"Weight {weight} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}