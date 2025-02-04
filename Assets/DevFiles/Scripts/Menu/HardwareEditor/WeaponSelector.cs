using clrev01.Bases;
using clrev01.ClAction;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Bases.UtlOfEdit;

namespace clrev01.Menu.HardwareEditor
{
    public class WeaponSelector : PartsSelector
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        private List<int> MechCustomWeaponSlots => StaticInfo.Inst.nowEditMech.mechCustom.weapons;
        private List<int> MechCustomWeaponAmoNum => StaticInfo.Inst.nowEditMech.mechCustom.weaponAmoNum;
        private WeaponSelectableSetting WeaponsSettings => MHUB.GetData(EditMachineCode).machineCD.usableWeapons[editSlotNum];
        private List<WeaponNamedBool> SelectableWeapons => WeaponsSettings.enumBoolSets.FindAll(x => x.onOff);
        protected override int PartsCount => SelectableWeapons.Count;
        private int DefaultCode => WeaponsSettings.defaultWeapon;
        protected override bool IndicateAmoNumInput => WHUB.GetGlobalMaxAmoNum(SelectorPartsCode) > 0;


        protected override void OnEnable()
        {
            base.OnEnable();
            SetInfoIndicate();
        }
        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();
        private void SetInfoIndicate()
        {
            var titleStr = WHUB.GetBulletName(SelectorPartsCode);
            var wd = WHUB.GetBulletCD(SelectorPartsCode);
            sb.Clear();
            if (wd is not null) wd.GetParameterText(ref sb, SelectorAmoNum);
            infoIndicator.IndicateStr(titleStr, sb.ToString());
        }
        protected override int InitializeSelector()
        {
            while (MechCustomWeaponSlots.Count <= editSlotNum)
            {
                MechCustomWeaponSlots.Add(0);
            }
            while (MechCustomWeaponAmoNum.Count <= editSlotNum)
            {
                MechCustomWeaponAmoNum.Add(0);
            }
            SelectorPartsCode = MechCustomWeaponSlots[editSlotNum];
            SelectorAmoNum = MechCustomWeaponAmoNum[editSlotNum];
            if (SelectableWeapons.All(x => x.weaponCode != SelectorPartsCode))
            {
                SelectorPartsCode = DefaultCode;
            }
            var selectedPanelNum = SelectableWeapons.FindIndex(x => x.weaponCode == SelectorPartsCode);
            return selectedPanelNum;
        }
        protected override int GetDefaultAmoNum(int code)
        {
            var wd = SelectableWeapons.FirstOrDefault(x => x.weaponCode == code);
            return wd?.maxAmoNum ?? -1;
        }
        protected override void OnAccept()
        {
            MechCustomWeaponSlots[editSlotNum] = SelectorPartsCode;
            MechCustomWeaponAmoNum[editSlotNum] = SelectorAmoNum;
            MPPM.ReturnPage();
        }
        protected override void OnAmoInput(string s)
        {
            if (int.TryParse(s, out var i))
            {
                SelectorAmoNum = Mathf.Clamp(i, 0, SelectableWeapons.Find(x => x.weaponCode == SelectorPartsCode).maxAmoNum);
                SetInfoIndicate();
            }
        }
        protected override void SettingPanel(CycleScrollPanel cp)
        {
            if (cp.tgtButtonInteractive)
            {
                var code = SelectableWeapons[cp.itemId].weaponCode;
                var bc = (WHUB.GetBulletCD(code) as CommonDataBase);
                var uiIcon = bc == null ? null : bc.uiIcon;
                buttonList[cp.panelId].SetIndicate(
                    WHUB.GetBulletName(code).ToString(),
                    uiIcon
                );
            }
            else
            {
                buttonList[cp.panelId].SetIndicate("", null);
            }
        }

        protected override void OnSelectButton(CycleScrollPanel cp)
        {
            SelectorPartsCode = SelectableWeapons[cp.itemId].weaponCode;
            SetInfoIndicate();
        }
    }
}