using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.HardwareEditor
{
    public abstract class PartsSelector : BaseOfCL
    {
        [SerializeField]
        protected GameObject amoInputObj;
        [SerializeField]
        protected TMP_InputField amoInput;
        [SerializeField]
        protected MenuButton acceptButton;
        [SerializeField]
        protected CycleScroll.CycleScroll cycleScroll;
        [SerializeField]
        protected List<PartsSelectorButtonFunc> buttonList = new();
        [ReadOnly]
        protected int editSlotNum;
        protected virtual bool IndicateAmoNumInput => false;

        protected int EditMachineCode => StaticInfo.Inst.nowEditMech.mechCustom.machineCode;

        private int _selectorPartsCode;
        protected int SelectorPartsCode
        {
            get => _selectorPartsCode;
            set
            {
                var b = _selectorPartsCode != value;
                _selectorPartsCode = value;
                if (b)
                {
                    SelectorAmoNum = GetDefaultAmoNum(value);
                }
            }
        }
        protected virtual int GetDefaultAmoNum(int code)
        {
            return 0;
        }

        private int _selectorAmoNum;
        protected int SelectorAmoNum
        {
            get => _selectorAmoNum;
            set
            {
                _selectorAmoNum = value;
                amoInputObj.SetActive(IndicateAmoNumInput);
                if (IndicateAmoNumInput)
                {
                    amoInput.text = SelectorAmoNum.ToString();
                    amoInput.interactable = true;
                }
                else
                {
                    amoInput.text = "-";
                    amoInput.interactable = false;
                }
            }
        }

        protected abstract int PartsCount { get; }

        protected virtual void Awake()
        {
            acceptButton.OnClick.AddListener(() => OnAccept());
            amoInput.onEndEdit.AddListener(s => OnAmoInput(s));
            var cpl =
                cycleScroll.Initialize(cp => SettingPanel(cp));
            foreach (var cp in cpl)
            {
                var buttonFunc = cp.GetComponent<PartsSelectorButtonFunc>();
                buttonList.Add(buttonFunc);
                cp.eventOnSelect.AddListener((cp1) => OnSelectButton(cp1));
            }
        }
        protected virtual void OnEnable()
        {
            var selectedPanelNum = InitializeSelector();
            cycleScroll.UpdatePage(PartsCount);
            cycleScroll.SetSelect(selectedPanelNum);
            cycleScroll.SetScrollPosToFirstSelect();
            amoInputObj.SetActive(IndicateAmoNumInput);
        }
        public virtual void SetSlotNum(int slotNum)
        {
            editSlotNum = slotNum;
        }
        protected abstract int InitializeSelector();
        protected abstract void OnAccept();
        protected abstract void OnAmoInput(string s);
        protected abstract void SettingPanel(CycleScrollPanel cp);
        protected abstract void OnSelectButton(CycleScrollPanel cp);
    }
}