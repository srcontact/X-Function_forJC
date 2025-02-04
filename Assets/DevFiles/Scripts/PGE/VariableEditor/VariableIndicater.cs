using clrev01.Menu;
using clrev01.Menu.CycleScroll;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.VariableEditor
{
    public class VariableIndicater : MenuFunction
    {
        public CycleScrollPanel cycleScrollPanel;
        public string variableName;
        [SerializeField]
        TextMeshProUGUI text;
        [SerializeField]
        public TMP_InputField inputField;
        public VariableEditor variableEditor;


        public void ShowSetting(string indicateText)
        {
            variableName = indicateText;
            text.text = indicateText;
            inputField.text = indicateText;
            inputField.gameObject.SetActive(false);
        }
        public void HideSetting()
        {
            text.text = "";
        }
    }
}