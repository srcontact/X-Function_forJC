using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class PowerPlantSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        protected override int PartsCount => PpHub.datas.Count;
        private int editPowerPlantCode
        {
            get
            {
                StaticInfo.Inst.nowEditMech.mechCustom.powerPlants ??= new() { 0 };
                if (StaticInfo.Inst.nowEditMech.mechCustom.powerPlants.Count == 0) StaticInfo.Inst.nowEditMech.mechCustom.powerPlants.Add(0);
                return StaticInfo.Inst.nowEditMech.mechCustom.powerPlants[0];
            }

            set
            {
                StaticInfo.Inst.nowEditMech.mechCustom.powerPlants ??= new() { 0 };
                if (StaticInfo.Inst.nowEditMech.mechCustom.powerPlants.Count == 0) StaticInfo.Inst.nowEditMech.mechCustom.powerPlants.Add(0);
                StaticInfo.Inst.nowEditMech.mechCustom.powerPlants[0] = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            var data = PpHub.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.Name, data);
        }

        protected override int InitializeSelector()
        {
            SelectorPartsCode = editPowerPlantCode;
            return PpHub.GetDataIndex(SelectorPartsCode);
        }

        protected override void OnAccept()
        {
            editPowerPlantCode = SelectorPartsCode;
            MPPM.ReturnPage();
        }

        protected override void OnAmoInput(string s)
        { }

        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            SelectorPartsCode = PpHub.datas[cp.itemId].Code;
            var data = PpHub.GetData(SelectorPartsCode);
            infoIndicator.IndicateInfoText(data.Name, data);
        }

        protected override void SettingPanel(CycleScrollPanel panel)
        {
            buttonList[panel.panelId].SetIndicate(
                panel.tgtButtonInteractive ? PpHub.datas[panel.itemId].Name : "",
                null
            );
        }
    }
}