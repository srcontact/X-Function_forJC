using clrev01.Bases;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    [CreateAssetMenu(menuName = "CommonData/MachineCD")]
    public class MachineCD : CommonData<MachineCD, MachineLD, MachineHD>, IWeightSetting
    {
        protected override string parentName => "Machines";
        public int machineCode;
        public float mechWeight = 5000;
        public float maxAdditionalLoadingCap = 5000;
        public float baseStability = 250;
        public float stabilityRecoveryRate = 5;
        public float stabilityRecoveryRateInAir = 2;
        public int maxHearthPoint = 10000;
        public float allowableTemperature = 1000;
        public float baseCoolingPerformance = 1;
        public float baseImpactResistValue = 50;
        public float baseImpactPhysicsRate = 10;
        public float defaultWeaponMaxRotateAnglePerFrame = 15;
        public List<WeaponSelectableSetting> usableWeapons = new List<WeaponSelectableSetting>();
        public int optionalUsableNum = 5;
        #region machineMover
        [SerializeField]
        private MachineMoveCommonPar moveCommonPar;
        public MachineMoveCommonPar MoveCommonPar => moveCommonPar;
        public int destroyMotionFrame = 120;
        #endregion

        protected override void ResetLd(MachineLD ld)
        {
            base.ResetLd(ld);
            ld.movePar.moveCommonPar = MoveCommonPar;
            ld.movePar.rBody = ld.hd.rigidBody;
        }

        public void OnValidate()
        {
            if (usableWeapons != null)
                foreach (var uw in usableWeapons)
                {
                    for (int i = 0; i < WHUB.datas.Count; i++)
                    {
                        var bd = WHUB.datas[i];
                        if (uw.enumBoolSets.All(x => x.weaponCode != bd.Code))
                        {
                            uw.enumBoolSets.Add(new UtlOfEdit.WeaponNamedBool(bd.Code) { bulletCDSet = bd });
                        }
                    }
                    for (int i = uw.enumBoolSets.Count - 1; i >= 0; i--)
                    {
                        var ebs = uw.enumBoolSets[i];
                        if (WHUB.datas.All(x => x.Code != ebs.weaponCode))
                        {
                            uw.enumBoolSets.Remove(ebs);
                        }
                    }
                    uw.enumBoolSets = uw.enumBoolSets.OrderBy(x => WHUB.datas.FindIndex(y => y.Code == x.weaponCode)).ToList();
                }
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return mechWeight;
        }
    }
}