using clrev01.ClAction.Machines;
using clrev01.Extensions;
using Cysharp.Text;
using System.Text;
using UnityEngine;

namespace clrev01.ClAction.OptionParts
{
    [CreateAssetMenu(menuName = "OptionPartsData/Jamming")]
    public class JammingOptionPartsData : OptionPartsData
    {
        public float jammingSize = 300;
        public override void ExeOptionParts(MachineLD ld, int slotNum)
        {
            ld.hd.objectSearchTgt.jammingSize += jammingSize;
        }
        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"Jamming Size {durationFrame} m".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Duration Frame {durationFrame} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Base Weight {weight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"Unit Weight {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }
    }
}