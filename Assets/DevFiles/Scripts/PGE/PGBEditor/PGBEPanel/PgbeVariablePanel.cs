using clrev01.Menu;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Save.VariableData;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeVariablePanel : PgbePanel
    {
        [SerializeField]
        protected MenuButton mainButton;
        [SerializeField]
        protected TextMeshProUGUI mainButtonText;
        [SerializeField]
        protected MenuButton modeChangeButton;
        private VariableData VariableData { get; set; }
        private List<VariableType> _selectableVariableTypes;

        private void Awake()
        {
            modeChangeButton.OnClick.AddListener(() =>
            {
                VariableData.useVariable = false;
                initPGBEPM.Invoke();
            });
            mainButton.OnClick.AddListener(() => PGEM2.variableEditor.OpenEditorAddAndSelect(VariableData, initPGBEPM, _selectableVariableTypes));
        }

        public void OnOpen(VariableData vd, bool indicateModeChangeButton, List<VariableType> selectableVariableTypes = null)
        {
            VariableData = vd;
            _selectableVariableTypes = selectableVariableTypes;
            mainButtonText.text = vd.name;
            modeChangeButton.gameObject.SetActive(indicateModeChangeButton);
        }

        protected override void ResetTgtData()
        {
            VariableData = null;
        }
    }
}