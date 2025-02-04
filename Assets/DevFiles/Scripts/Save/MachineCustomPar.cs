using clrev01.Bases;
using clrev01.ClAction;
using clrev01.ClAction.Machines;
using clrev01.ClAction.Machines.RunningActionHolder;
using clrev01.ClAction.Shield;
using clrev01.HUB;
using MemoryPack;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Save
{
    /// <summary>
    /// 機体のカスタムデータ。
    /// ユーザーがカスタマイズする機体データ。
    /// 機体のスペックではない。
    /// </summary>
    [System.Serializable]
    [MemoryPackable()]
    public partial class MachineCustomPar
    {
        [SerializeField]
        private int _machineCode = 0;
        public int machineCode
        {
            get => _machineCode;
            set
            {
                var bk = _machineCode;
                _machineCode = MachineExistenceCheck(value);
                RegisterDefaultWeapons(bk != _machineCode);
            }
        }
        private int MachineExistenceCheck(int code) => MHUB.datas.All(x => x.Code != code) ? MHUB.datas[0].Code : code;
        public int armorThickness = 0;
        public List<int> weapons = new();
        public List<int> shields = new() { 0 };
        public List<int> weaponAmoNum = new();
        public List<int> powerPlants = new() { 0 };
        public int fuelAmount = 100;
        public int cpu = 0;
        public List<int> optionParts = new();
        public List<int> optionPartsUsableNum = new();
        public int armorType = 0;
        public int fcsType = 0;
        public int thrusterType = 0;

        [MemoryPackIgnore]
        private HubDataCache<MachineHub, MachineData> _machineData = new();
        [MemoryPackIgnore]
        private HubDataCache<PowerPlantHub, PowerPlantData> _powerPlantData = new();
        [MemoryPackIgnore]
        private HubDataCache<CpuHub, CpuData> _cpuData = new();
        [MemoryPackIgnore]
        private HubDataCache<FcsHub, FcsData> _fcsData = new();
        [MemoryPackIgnore]
        private HubDataCache<ArmorTypeHub, ArmorTypeData> _armorTypeData = new();
        [MemoryPackIgnore]
        private HubDataCache<ShieldHub, ShieldData> _shieldData = new();
        [MemoryPackIgnore]
        private HubDataCache<ThrusterHub, ThrusterData> _thrusterData = new();

        [MemoryPackIgnore]
        public MachineCD MachineCD => _machineData.GetValue(MHUB, machineCode).machineCD;
        [MemoryPackIgnore]
        public PowerPlantData PowerPlantData => _powerPlantData.GetValue(PpHub, powerPlants[0]);
        [MemoryPackIgnore]
        public CpuData CpuData => _cpuData.GetValue(CHub, cpu);
        [MemoryPackIgnore]
        public FcsData FcsData => _fcsData.GetValue(FHub, fcsType);
        [MemoryPackIgnore]
        public ArmorTypeData ArmorTypeData => _armorTypeData.GetValue(ATHub, armorType);
        [MemoryPackIgnore]
        public ShieldCD ShieldCd => _shieldData.GetValue(ShldHub, shields[0]).shieldCd;
        [MemoryPackIgnore]
        public ThrusterData ThrusterData => _thrusterData.GetValue(THub, thrusterType);


        public void RegisterDefaultWeapons(bool machineChange = false)
        {
            List<WeaponSelectableSetting> wss = MHUB.GetData(machineCode).machineCD.usableWeapons;
            if (machineChange || weapons.Count != wss.Count)
            {
                weapons ??= new List<int>();
                for (int i = 0; weapons.Count < wss.Count; i++)
                {
                    weapons.Add(wss[i].defaultWeapon);
                }
            }
            if (machineChange || weaponAmoNum.Count != wss.Count)
            {
                weaponAmoNum ??= new List<int>();
                for (int i = 0; weaponAmoNum.Count < wss.Count; i++)
                {
                    weaponAmoNum.Add(wss[i].enumBoolSets[weapons[i]].defaultAmoNum);
                }
            }
            var optionalUsableNum = MHUB.GetData(machineCode).machineCD.optionalUsableNum;
            if (machineChange || optionParts.Count != optionalUsableNum)
            {
                optionParts ??= new List<int>();
                for (int i = 0; optionParts.Count < optionalUsableNum; i++)
                {
                    optionParts.Add(0);
                }
            }
        }
        void InitializeList<T>(ref List<T> list, params T[] tl)
        {
            if (list.CountFromNull() < 1) list = new List<T>(tl);
        }

        public float CalcWeightSum(List<RunningShootHolder> runningShootHolder = null, Dictionary<int, int> optionPartsUseCount = null)
        {
            float weightSum = 0;

            weightSum +=
                (MachineCD == null ? 0 : MachineCD.GetWeightValue()) + (PowerPlantData?.GetWeightValue()).GetValueOrDefault() +
                (CpuData?.GetWeightValue()).GetValueOrDefault() + (FcsData?.GetWeightValue()).GetValueOrDefault() +
                (ArmorTypeData?.GetWeightValue(armorThickness)).GetValueOrDefault() +
                (ShieldCd == null ? 0 : ShieldCd.GetWeightValue()) + (ThrusterData?.GetWeightValue()).GetValueOrDefault();

            for (int i = 0; i < weapons.Count && i < weaponAmoNum.Count && (runningShootHolder is null || i < runningShootHolder.Count); i++)
            {
                var weapon = WHUB.GetBulletCD(weapons[i]);
                if (weapon is null) continue;
                var ammoNum = weaponAmoNum[i];
                var usedAmmoNum = runningShootHolder is null ? 0 : runningShootHolder[i].numberOfShots;
                weightSum += weapon.GetWeightValue(ammoNum - usedAmmoNum);
            }
            for (int i = 0; i < optionParts.Count && (optionPartsUsableNum is null || i < optionPartsUsableNum.Count); i++)
            {
                var optional = OpHub.GetOptionPartsData(optionParts[i]);
                if (optional == null || optional.data == null) continue;
                var ammoNum = optionPartsUsableNum[i];
                int usedAmmoNum = 0;
                optionPartsUseCount?.TryGetValue(i, out usedAmmoNum);
                weightSum += optional.data.GetWeightValue(ammoNum - usedAmmoNum);
            }

            return weightSum;
        }
    }
}