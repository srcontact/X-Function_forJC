using Cysharp.Text;
using I2.Loc;
using System.Text;

namespace clrev01.Menu.InformationIndicator
{
    public interface IInfoTextData
    {
        public LocalizedString description { get; set; }
        public void GetParameterText(ref Utf8ValueStringBuilder sb);
    }
}