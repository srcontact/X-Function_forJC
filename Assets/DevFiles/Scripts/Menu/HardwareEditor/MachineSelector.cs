using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class MachineSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        protected override int PartsCount => MHUB.datas.Count;
        protected override bool IndicateAmoNumInput => false;
        private int editCode
        {
            get => StaticInfo.Inst.nowEditMech.mechCustom.machineCode;
            set => StaticInfo.Inst.nowEditMech.mechCustom.machineCode = value;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            var data = MHUB.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.MachineName, data);
        }
        protected override int InitializeSelector()
        {
            SelectorPartsCode = editCode;
            return MHUB.GetDataIndex(SelectorPartsCode);
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
                panel.tgtButtonInteractive ? MHUB.datas[panel.itemId].MachineName : "",
                null
            );
        }
        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            SelectorPartsCode = MHUB.datas[cp.itemId].Code;
            var data = MHUB.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.MachineName, data);
        }
    }
}