using clrev01.Bases;
using clrev01.ClAction.Machines;
using TMPro;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.EditMenu
{
    public class MachineInformation : DataInformation<CustomData>
    {
        public TextMeshProUGUI numberText;
        public TextMeshProUGUI titleText;
        public GameObject editTgtIndicator;
        public override string emptyText => "/// EMPTY ///";


        protected override void SettingIndicate()
        {
            base.SettingIndicate();
            editTgtIndicator.SetActive(StaticInfo.Inst.nowEditMech == indicateData);
            titleText.text = indicateData == null ? emptyText : indicateData.dataName;
        }

        public override string infoText
        {
            get
            {
                var mechCustom = indicateData.mechCustom;

                // 必要な情報を順に文字列連結
                var res = "";

                // 機体モデル
                res += $"{MHUB.GetData(mechCustom.machineCode)?.MachineName}\n";

                // パワープラント
                res += $"{PpHub.datas.Find(x => mechCustom.powerPlants.Count > 0 && x.Code == mechCustom.powerPlants[0])?.Name}\n";

                // CPU
                res += $"{CHub.datas.Find(x => x.Code == mechCustom.cpu)?.Name}\n";

                // アーマー
                res += $"{ATHub.datas.Find(x => x.Code == mechCustom.armorType)?.Name}\n";

                // シールド
                res += $"{ShldHub.GetShieldName(mechCustom.shields.Count > 0 ? mechCustom.shields[0] : -1)}\n";

                // スラスター
                res += $"{THub.datas.Find(x => x.Code == mechCustom.thrusterType)?.Name}\n";

                // 武器
                res += "Weapons:\n";
                foreach (var weaponCode in mechCustom.weapons)
                {
                    res += $"    {WHUB.GetBulletName(weaponCode)}\n";
                }

                // オプションパーツ
                res += "Optionals:\n";
                foreach (var opNum in mechCustom.optionParts)
                {
                    res += $"    {OpHub.GetOptionPartsData(opNum).partsName}\n";
                }

                return res;
            }
        }
    }
}