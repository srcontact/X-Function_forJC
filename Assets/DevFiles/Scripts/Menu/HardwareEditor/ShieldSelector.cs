using clrev01.Bases;
using clrev01.ClAction.Shield;
using clrev01.HUB;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class ShieldSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        private int _editTgtNum = 0;
        protected override int PartsCount => ShldHub.datas.Count;
        protected override bool IndicateAmoNumInput => false;

        private int EditShieldCode
        {
            get
            {
                List<int> shields = StaticInfo.Inst.nowEditMech.mechCustom.shields;
                while (shields.Count <= _editTgtNum)
                {
                    shields.Add(0);
                }
                if (ShldHub.datas.All(x => x.Code != shields[_editTgtNum]))
                {
                    shields[_editTgtNum] = 0;
                }
                return shields[_editTgtNum];
            }
            set
            {
                List<int> shields = StaticInfo.Inst.nowEditMech.mechCustom.shields;
                shields[_editTgtNum] = value;
            }
        }
        private ShieldCD EditShieldData => ShldHub.GetShieldPar(EditShieldCode);


        protected override int InitializeSelector()
        {
            SelectorPartsCode = EditShieldCode;
            var selectedPanelNum = ShldHub.datas.FindIndex(x => x.Code == SelectorPartsCode);
            infoIndicator.IndicateInfoText(EditShieldData?.name, EditShieldData);
            return selectedPanelNum;
        }
        protected override void OnAccept()
        {
            EditShieldCode = SelectorPartsCode;
            MPPM.ReturnPage();
        }
        protected override void OnAmoInput(string s)
        { }
        protected override void SettingPanel(CycleScrollPanel panel)
        {
            buttonList[panel.panelId].SetIndicate(
                panel.tgtButtonInteractive ? ShldHub.datas[panel.itemId].name : "",
                null
            );
        }
        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            ShieldData data = ShldHub.datas[cp.itemId];
            SelectorPartsCode = data.Code;
            infoIndicator.IndicateInfoText(data.name, data.shieldCd);
        }
    }
}