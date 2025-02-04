using clrev01.Bases;
using Cysharp.Text;
using System.Text;
using UnityEngine;

namespace clrev01.Menu.InformationIndicator
{
    public class InfoIndicator : BaseOfCL
    {
        [SerializeField]
        private InfoIndicatorText infoIndicatorText;


        public void IndicateStr(string title, string body)
        {
            infoIndicatorText.SetTexts(title, body);
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();

        public void IndicateInfoText(string titleStr, IInfoTextData infoTextData)
        {
            if (infoTextData == null)
            {
                infoIndicatorText.SetTexts($"None", null);
                return;
            }
            sb.Clear();
            if (infoTextData.description != null) sb.AppendLine(infoTextData.description);
            else sb.AppendLine("None Description.");
            sb.AppendLine();
            infoTextData.GetParameterText(ref sb);
            infoIndicatorText.SetTexts(titleStr, sb.ToString());
        }
    }
}