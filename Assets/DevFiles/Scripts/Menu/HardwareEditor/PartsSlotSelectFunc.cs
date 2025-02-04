using clrev01.Bases;
using clrev01.Save;
using System;
using TMPro;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.HardwareEditor
{
    public class PartsSlotSelectFunc : MenuFunction
    {
        [SerializeField]
        private PartsSelector partsSelector;
        private MachineCustomPar machineCustomPar => StaticInfo.Inst.nowEditMech.mechCustom;
        [SerializeField]
        private string textStr = "Parts";

        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private int editWeaponNum;

        public enum PartsType
        {
            Weapons,
            Options,
        }

        [SerializeField]
        private PartsType partsType;

        protected override void Awake()
        {
            base.Awake();
            text.text = textStr + (editWeaponNum + 1);
        }
        private void OnEnable()
        {
            var machineCd =
                MHUB.GetData(machineCustomPar.machineCode).machineCD;
            var count = partsType switch
            {
                PartsType.Weapons => machineCd.usableWeapons.Count,
                PartsType.Options => machineCd.optionalUsableNum,
                _ => throw new ArgumentOutOfRangeException()
            };
            gameObject.SetActive(editWeaponNum < count);
        }
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            partsSelector.SetSlotNum(editWeaponNum);
        }
    }
}