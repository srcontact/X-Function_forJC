using clrev01.Bases;
using clrev01.HUB;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using System.Text;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class ArmorSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        private int nowEditArmorType
        {
            get => StaticInfo.Inst.nowEditMech.mechCustom.armorType;
            set => StaticInfo.Inst.nowEditMech.mechCustom.armorType = value;
        }
        private int nowEditArmorThicness
        {
            get => StaticInfo.Inst.nowEditMech.mechCustom.armorThickness;
            set => StaticInfo.Inst.nowEditMech.mechCustom.armorThickness = value;
        }
        protected override int PartsCount => ATHub.datas.Count;
        protected override bool IndicateAmoNumInput => ATHub.GetData(SelectorPartsCode).minThickness > 0;


        protected override void OnEnable()
        {
            base.OnEnable();
            SetInfoIndicate();
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();
        
        private void SetInfoIndicate()
        {
            var data = ATHub.GetData(SelectorPartsCode);
            sb.Clear();
            if(data.description!=null) sb.AppendLine(data.description);
            else sb.AppendLine("None Description.");
            sb.AppendLine();
            data.GetParameterText(ref sb, SelectorAmoNum);
            infoIndicator.IndicateStr($"{data.Name} {SelectorAmoNum * 10}mm", sb.ToString());
        }

        protected override int InitializeSelector()
        {
            SelectorPartsCode = nowEditArmorType;
            SelectorAmoNum = nowEditArmorThicness;
            return ATHub.datas.FindIndex(x => x.Code == SelectorPartsCode);
        }
        protected override int GetDefaultAmoNum(int code)
        {
            ArmorTypeData data = ATHub.GetData(code);
            return Mathf.Clamp(SelectorAmoNum, data.minThickness, data.maxThickness);
        }

        protected override void OnAccept()
        {
            nowEditArmorType = SelectorPartsCode;
            nowEditArmorThicness = SelectorAmoNum;
            MPPM.ReturnPage();
        }

        protected override void OnAmoInput(string s)
        {
            if (int.TryParse(s, out var i))
            {
                ArmorTypeData armorTypeData = ATHub.GetData(SelectorPartsCode);
                SelectorAmoNum = Mathf.Clamp(i, armorTypeData.minThickness, armorTypeData.maxThickness);
                SetInfoIndicate();
            }
        }

        protected override void OnSelectButton(CycleScrollPanel panel)
        {
            SelectorPartsCode = ATHub.datas[panel.itemId].Code;
            SetInfoIndicate();
        }

        protected override void SettingPanel(CycleScrollPanel panel)
        {
            buttonList[panel.panelId].SetIndicate(
                panel.tgtButtonInteractive ? ATHub.datas[panel.itemId].Name : "",
                null
            );
        }
    }
}