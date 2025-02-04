using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class OptionPartsSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        private List<int> MechCustomOpSlots => StaticInfo.Inst.nowEditMech.mechCustom.optionParts;
        private List<int> MechCustomOpUsableNums => StaticInfo.Inst.nowEditMech.mechCustom.optionPartsUsableNum;
        protected override int PartsCount => OpHub.GetOptionPartsCount();
        protected override bool IndicateAmoNumInput => OpHub.GetOptionPartsData(SelectorPartsCode).defaultMaxUsableNum > 0;


        protected override void OnEnable()
        {
            base.OnEnable();
            SetInfoIndicate();
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();

        private void SetInfoIndicate()
        {
            var wd = OpHub.GetOptionPartsData(SelectorPartsCode);
            sb.Clear();
            wd.data?.GetParameterText(ref sb, SelectorAmoNum);
            infoIndicator.IndicateStr(wd.partsName, sb.ToString());
        }
        protected override int GetDefaultAmoNum(int code)
        {
            return OpHub.GetOptionPartsData(code).defaultUsableNum;
        }
        protected override int InitializeSelector()
        {
            while (MechCustomOpSlots.Count <= editSlotNum)
            {
                MechCustomOpSlots.Add(0);
            }
            while (MechCustomOpUsableNums.Count <= editSlotNum)
            {
                MechCustomOpUsableNums.Add(0);
            }
            SelectorPartsCode = MechCustomOpSlots[editSlotNum];
            SelectorAmoNum = MechCustomOpUsableNums[editSlotNum];
            var selectedPanelNum = OpHub.GetOptionPartsIndexInList(SelectorPartsCode);
            return selectedPanelNum;
        }
        protected override void OnAccept()
        {
            MechCustomOpSlots[editSlotNum] = SelectorPartsCode;
            MechCustomOpUsableNums[editSlotNum] = SelectorAmoNum;
            MPPM.ReturnPage();
        }
        protected override void OnAmoInput(string s)
        {
            if (int.TryParse(s, out var i))
            {
                SelectorAmoNum = Mathf.Min(i, OpHub.GetOptionPartsData(SelectorPartsCode).defaultMaxUsableNum);
                SetInfoIndicate();
            }
        }
        protected override void SettingPanel(CycleScrollPanel cp)
        {
            if (cp.tgtButtonInteractive)
            {
                var optionPartsData = OpHub.GetOptionPartsData(cp.itemId);
                buttonList[cp.panelId].SetIndicate(
                    optionPartsData.partsName,
                    optionPartsData.uiIcon
                );
            }
            else
            {
                buttonList[cp.panelId].SetIndicate("", null);
            }
        }
        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            var optionPartsData = OpHub.GetOptionPartsData(cp.itemId);
            SelectorPartsCode = optionPartsData.code;
            SetInfoIndicate();
        }
    }
}