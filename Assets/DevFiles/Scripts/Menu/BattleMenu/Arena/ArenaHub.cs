using clrev01.HUB;
using clrev01.Menu.InformationIndicator;
using clrev01.Save;
using Cysharp.Text;
using I2.Loc;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Menu.BattleMenu.Arena
{
    [CreateAssetMenu(menuName = "Hub/ArenaDataHub")]
    public class ArenaHub : HubBase<ArenaHubData>
    {

    }

    public class ArenaHubData : HubData, IInfoTextData
    {
        [SerializeField]
        private string dataName = "ArenaData";
        [field: SerializeField]
        public LocalizedString title { get; set; }
        public string Name
        {
            get => title != null ? title : dataName;
        }
        [field: SerializeField]
        public LocalizedString description { get; set; }
        [SerializeField]
        public ArenaData arenaData = new();

        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            if (description != null) sb.AppendLine(description);
        }
    }
}