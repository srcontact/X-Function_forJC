using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using Cysharp.Text;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Extensions.ExStringUtl;
using static I2.Loc.ScriptLocalization;

namespace clrev01.Menu.InformationIndicator
{
    public class MachineInfoIndicator : BaseOfCL
    {
        [SerializeField]
        private InfoIndicator infoIndicator;
        private CustomData _indicateData;

        public bool highlightPowerPlant, highlightCpu, highlightProtection, highlightThruster, highlightWeapons, highlightOptionals;

        private void OnEnable()
        {
            SetIndicateData(StaticInfo.Inst.nowEditMech);
        }

        public void SetIndicateData(CustomData customData)
        {
            _indicateData = customData;
            InitIndicate();
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();

        private void InitIndicate()
        {
            sb.Clear();
            var mechCustom = _indicateData.mechCustom;
            sb.AppendLine($"{menu_edit_hardware.machineModel.SpaceToNbSp().LinkTag(link_edit_machine.machineType)} {MHUB.GetData(mechCustom.machineCode)?.MachineName}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine();

            if (highlightPowerPlant) sb.Append(GetOpeningTag("mark"));
            sb.AppendLine($"{menu_edit_hardware.powerPlant.SpaceToNbSp().LinkTag(link_edit_machine.powerPlant)} {PpHub.datas.Find(x => mechCustom.powerPlants.Count > 0 && x.Code == mechCustom.powerPlants[0])?.Name}".Tagging("align", "flush").Tagging("u"));
            if (highlightPowerPlant) sb.Append(GetClosingTag("mark"));
            sb.AppendLine();

            if (highlightCpu) sb.Append(GetOpeningTag("mark"));
            sb.AppendLine($"{menu_edit_hardware.electronics.SpaceToNbSp().LinkTag(link_edit_machine.electronics)}".Tagging("u"));
            sb.AppendLine($"{menu_edit_hardware.fcs.SpaceToNbSp().LinkTag(link_edit_machine.fcs)} {FHub.datas.Find(x => x.Code == mechCustom.fcsType)?.Name}".Tagging("u").Tagging("align", "flush").Tagging("indent", "50%"));
            sb.AppendLine($"{menu_edit_hardware.cpu.SpaceToNbSp().LinkTag(link_edit_machine.cpu)} {CHub.datas.Find(x => x.Code == mechCustom.cpu)?.Name}".Tagging("u").Tagging("align", "flush").Tagging("indent", "50%"));
            if (highlightCpu) sb.Append(GetClosingTag("mark"));
            sb.AppendLine();

            if (highlightProtection) sb.Append(GetOpeningTag("mark"));
            sb.AppendLine($"{menu_edit_hardware.protection.SpaceToNbSp().LinkTag(link_edit_machine.protection)}".Tagging("u"));
            sb.AppendLine($"{menu_edit_hardware.armor.SpaceToNbSp().LinkTag(link_edit_machine.armor)} {ATHub.datas.Find(x => x.Code == mechCustom.armorType)?.Name} {mechCustom.armorThickness * 10}mm".Tagging("u").Tagging("align", "flush").Tagging("indent", "50%"));
            sb.AppendLine($"{menu_edit_hardware.shield.SpaceToNbSp().LinkTag(link_edit_machine.shield)} {ShldHub.GetShieldName(mechCustom.shields.Count > 0 ? mechCustom.shields[0] : -1)}".Tagging("u").Tagging("align", "flush").Tagging("indent", "50%"));
            if (highlightProtection) sb.Append(GetClosingTag("mark"));
            sb.AppendLine();

            if (highlightThruster) sb.Append(GetOpeningTag("mark"));
            sb.AppendLine($"{menu_edit_hardware.thruster.SpaceToNbSp().LinkTag(link_edit_machine.thruster)} {THub.datas.Find(x => x.Code == mechCustom.thrusterType)?.Name}".Tagging("align", "flush").Tagging("u"));
            if (highlightThruster) sb.Append(GetClosingTag("mark"));
            sb.AppendLine();

            if (highlightWeapons) sb.Append(GetOpeningTag("mark"));
            sb.AppendLine($"{menu_edit_hardware.weapons.SpaceToNbSp().LinkTag(link_edit_machine.weapons)}".Tagging("u"));
            for (int i = 0; i < mechCustom.weapons.Count; i++)
            {
                var weaponAmo = mechCustom.weapons[i] == 0 ? "----" : $"{mechCustom.weaponAmoNum[i].ToString().PadLeft(4, ' ')}";
                sb.AppendLine($"{i} {WHUB.GetBulletName(mechCustom.weapons[i])}  {weaponAmo}".Tagging("u").Tagging("align", "flush").Tagging("indent", "50%"));
            }
            if (highlightWeapons) sb.Append(GetClosingTag("mark"));
            sb.AppendLine();

            if (highlightOptionals) sb.Append(GetOpeningTag("mark"));
            sb.AppendLine($"{menu_edit_hardware.options.SpaceToNbSp().LinkTag(link_edit_machine.optionals)}".Tagging("u"));
            for (int i = 0; i < mechCustom.optionParts.Count; i++)
            {
                var opNum = mechCustom.optionParts[i];
                var usableNum = opNum == 0 ? "----" : $"{mechCustom.optionPartsUsableNum[i].ToString().PadLeft(4, ' ')}";
                sb.AppendLine($"{i} {OpHub.GetOptionPartsData(opNum).partsName}  {usableNum}".Tagging("u").Tagging("align", "flush").Tagging("indent", "50%"));
            }
            if (highlightOptionals) sb.Append(GetClosingTag("mark"));
            sb.AppendLine();

            sb.AppendLine($"{menu_edit_hardware.grossWeight.SpaceToNbSp().LinkTag(link_edit_machine.grossWeight)} {mechCustom.CalcWeightSum()} kg".Tagging("align", "flush").Tagging("u"));


            infoIndicator.IndicateStr($"{_indicateData.dataName}", sb.ToString());
        }
    }
}