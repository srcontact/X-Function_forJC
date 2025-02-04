using clrev01.Bases;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class ParameterInd : BaseOfCL
    {
        [SerializeField]
        private TextMeshProUGUI title, parameter;

        public string parameterStr
        {
            get { return parameter.text; }
            set { parameter.text = value; }
        }
    }
}