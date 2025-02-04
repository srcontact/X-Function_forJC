using clrev01.Bases;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.OtherSetting
{
    public class MachineNameInput : BaseOfCL
    {
        [SerializeField]
        TMP_InputField inputField;
        string machineName
        {
            get => StaticInfo.Inst.nowEditMech.dataName ?? null;
            set
            {
                StaticInfo.Inst.nowEditMech.dataName = value;
                inputField.text = machineName;
            }
        }
        private void Awake()
        {
            inputField.onEndEdit.AddListener((string s) => OnNameInput(s));
        }
        private void OnEnable()
        {
            inputField.text = machineName;
        }
        private void OnNameInput(string s)
        {
            machineName = s;
        }
    }
}