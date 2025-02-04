using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class CpuSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        protected override int PartsCount => CHub.datas.Count;
        protected override bool IndicateAmoNumInput => false;
        private int editCpuCode
        {
            get => StaticInfo.Inst.nowEditMech.mechCustom.cpu;
            set => StaticInfo.Inst.nowEditMech.mechCustom.cpu = value;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            var data = CHub.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.Name, data);
        }
        protected override int InitializeSelector()
        {
            SelectorPartsCode = editCpuCode;
            return CHub.GetDataIndex(SelectorPartsCode);
        }
        protected override void OnAccept()
        {
            editCpuCode = SelectorPartsCode;
            MPPM.ReturnPage();
        }
        protected override void OnAmoInput(string s)
        { }
        protected override void SettingPanel(CycleScrollPanel panel)
        {
            buttonList[panel.panelId].SetIndicate(
                panel.tgtButtonInteractive ? CHub.datas[panel.itemId].Name : "",
                null
            );
        }
        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            SelectorPartsCode = CHub.datas[cp.itemId].Code;
            var data = CHub.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.Name, data);
        }
    }
}