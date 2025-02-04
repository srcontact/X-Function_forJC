using clrev01.Bases;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    [System.Serializable]
    public class MachineStatePar
    {
        public int damage;
        public float heat;
        [ReadOnly]
        public float _impact;
        public int latestImpactDamageFrame = int.MinValue;
        public float impact
        {
            get => _impact;
            set
            {
                if (_impact < value) latestImpactDamageFrame = ACM.actionFrame;
                _impact = value;
            }
        }
        public float currentFrameImpactValue;
        public int shieldDamage;
        public bool shieldBreakFlag;
        public int latestShieldStartFrame = int.MinValue;
        public int latestShieldUseFrame = int.MinValue;
        public float energyUsed;
        public int energySupplyRestartFrame = int.MinValue;
        public int usedFuel;
        public int destroyedFrame;
        public Dictionary<int, int> optionPartsUseFrameDict = new();
        public Dictionary<int, int> optionPartsUseCount = new();
        public int quickThrustUseableFrame = int.MinValue;

        public MachineStatePar(int startDamage = 0, int startShieldDamage = 0, float startHeat = 0, int startUsedFuel = 0)
        {
            damage = startDamage;
            shieldDamage = startShieldDamage;
            heat = startHeat;
            usedFuel = startUsedFuel;
            destroyedFrame = int.MinValue;
        }
    }
}