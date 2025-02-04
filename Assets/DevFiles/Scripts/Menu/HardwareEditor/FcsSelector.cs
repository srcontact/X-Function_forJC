using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class FcsSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        protected override int PartsCount => FHub.datas.Count;
        protected override bool IndicateAmoNumInput => false;
        private int editCode
        {
            get => StaticInfo.Inst.nowEditMech.mechCustom.fcsType;
            set => StaticInfo.Inst.nowEditMech.mechCustom.fcsType = value;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            var data = FHub.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.Name, data);
        }
        protected override int InitializeSelector()
        {
            SelectorPartsCode = editCode;
            return FHub.GetDataIndex(SelectorPartsCode);
        }
        protected override void OnAccept()
        {
            editCode = SelectorPartsCode;
            MPPM.ReturnPage();
        }
        protected override void OnAmoInput(string s)
        { }
        protected override void SettingPanel(CycleScrollPanel panel)
        {
            buttonList[panel.panelId].SetIndicate(
                panel.tgtButtonInteractive ? FHub.datas[panel.itemId].Name : "",
                null
            );
        }
        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            SelectorPartsCode = FHub.datas[cp.itemId].Code;
            var data = FHub.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.Name, data);
        }
    }
}