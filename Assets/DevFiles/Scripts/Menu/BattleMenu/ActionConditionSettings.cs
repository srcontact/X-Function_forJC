using clrev01.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.Menu.BattleMenu
{
    public class ActionConditionSettings : BaseOfCL
    {
        [SerializeField]
        Toggle UseRandomSeedInputedToggle;
        [SerializeField]
        TMP_InputField RandomSeedInputField;
        [SerializeField]
        MenuButton AutoInputSeedButton;
        [SerializeField]
        GameObject RandomSeedInputFieldObj;

        private void Awake()
        {
            UseRandomSeedInputedToggle.onValueChanged.AddListener((b) => OnRandomSettingChange(b));
            RandomSeedInputField.onEndEdit.AddListener((s => OnRandomSeedManualChange(s)));
            AutoInputSeedButton.OnClick.AddListener(() => OnClickRandomizeSeed());
        }

        private void OnEnable()
        {
            UseRandomSeedInputedToggle.isOn = StaticInfo.Inst.fixedRandomSeed;
            RandomSeedInputField.text = StaticInfo.Inst.unityRandomSeed.ToString();
            RandomSeedInputFieldObj.SetActive(StaticInfo.Inst.fixedRandomSeed);
        }


        private void OnRandomSettingChange(bool b)
        {
            StaticInfo.Inst.fixedRandomSeed = b;
            RandomSeedInputFieldObj.SetActive(StaticInfo.Inst.fixedRandomSeed);
        }

        private void OnRandomSeedManualChange(string s)
        {
            int i;
            if (!int.TryParse(s, out i))
            {
                RandomSeedInputField.text = StaticInfo.Inst.unityRandomSeed.ToString();
                return;
            }
            StaticInfo.Inst.unityRandomSeed = i;
        }

        private void OnClickRandomizeSeed()
        {
            StaticInfo.Inst.SetUnityRandomSeedUseDateTimeNow();
            RandomSeedInputField.text = StaticInfo.Inst.unityRandomSeed.ToString();
        }
    }
}