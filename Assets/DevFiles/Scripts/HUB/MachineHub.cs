using System.Linq;
using UnityEngine;

namespace clrev01.HUB
{
    [CreateAssetMenu(menuName = "Hub/MachineHub")]
    public class MachineHub : HubBase<MachineData>
    {
        public bool ContainsData(int mc)
        {
            return datas.Any(x => x.Code == mc);
        }
    }
}