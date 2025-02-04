using clrev01.Bases;
using clrev01.ClAction;
using clrev01.ClAction.Radar;
using clrev01.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.HUB
{
    [CreateAssetMenu(menuName = "Hub/RadarSymbolHub")]
    public class RadarSymbolHub : SOBaseOfCL
    {
        [SerializeField]
        private ComponentReferenceSet<RadarSymbol> machineSymbolReference;
        [SerializeField]
        List<Material> machineSymbolMatList = new();
        [SerializeField]
        private ComponentReferenceSet<RadarSymbol> bulletSymbolReference;
        [SerializeField]
        private ComponentReferenceSet<RadarSymbol> mineSymbolReference;

        public RadarSymbol GetMachineSymbol()
        {
            return machineSymbolReference.GetAsset(ActionManager.Inst.gameObject).SafeInstantiate();
        }
        public Material GetMachineSymbolMat(int teamNum)
        {
            if (machineSymbolMatList.Count <= teamNum) return machineSymbolMatList[0];
            return machineSymbolMatList[teamNum];
        }
        internal RadarSymbol GetBulletSymbol()
        {
            return bulletSymbolReference.GetAsset(ActionManager.Inst.gameObject).SafeInstantiate();
        }

        internal RadarSymbol GetMineSymbol()
        {
            return mineSymbolReference.GetAsset(ActionManager.Inst.gameObject).SafeInstantiate();
        }
    }
}