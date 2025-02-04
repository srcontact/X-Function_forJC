using clrev01.ClAction;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using I2.Loc;
using System.Text;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.HUB
{
    [System.Serializable]
    public class FcsData : HubData, IInfoTextData, IWeightSetting
    {
        [SerializeField]
        string name;
        public string Name => name;
        [field: SerializeField]
        public LocalizedString description { get; set; }

        public SearchParameterData searchParameterData = new()
        {
            visibleAccuracy = 1,
            concealedAccuracy = 20,
            visibleUpdateFrequency = 1,
            concealedUpdateFrequency = 10,
            antiJammingRate = 0.2f
        };
        public float weight = 10;


        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_fcs.visibleAccuracy.SpaceToNbSp()} {searchParameterData.visibleAccuracy}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_fcs.concealedAccuracy} {searchParameterData.concealedAccuracy}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_fcs.visibleUpdateFrequency} {searchParameterData.visibleUpdateFrequency}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_fcs.concealedUpdateFrequency} {searchParameterData.concealedUpdateFrequency}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_fcs.antiJammingRate} {searchParameterData.antiJammingRate}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_fcs.weight.SpaceToNbSp()} {GetWeightValue()} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weight;
        }
    }
}