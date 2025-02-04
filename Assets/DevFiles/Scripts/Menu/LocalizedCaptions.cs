using I2.Loc;
using Sirenix.Utilities;
using UnityEngine;

namespace clrev01.Menu
{
    public class LocalizedCaptions
    {
        [SerializeField]
        private string mouseOverText;
        public string MouseOverText => localizedMouseOverText.ToString().IsNullOrWhitespace() ? mouseOverText : localizedMouseOverText;
        [SerializeField]
        public LocalizedString localizedMouseOverText;
        [SerializeField]
        public LocalizedString clickLink;
    }
}