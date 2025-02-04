using clrev01.Bases;
using MapMagic.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace clrev01.HUB
{
    [CreateAssetMenu(menuName = "Hub/ActionLevelHub")]
    public class ActionLevelHub : SOBaseOfCL
    {
        [System.Serializable]
        public class LevelHolder
        {
            public string levelName = "Level";
            public Graph mapMagicGraph;
            public float height = 100;
            [Range(0, 6)]
            public int mapResolution = 3;
            public float levelTemperature = 25;
        }

        public List<ActionLevelSize> levelSizes = new List<ActionLevelSize>();
        public int levelSizeMargin = 2000;
        public int radarCameraMargin = 100;
        public List<LevelHolder> levels = new List<LevelHolder>();

        public ValueDropdownList<int> playLevelValueDropdownList
        {
            get
            {
                var res = new ValueDropdownList<int>();
                res.AddRange(StaticInfo.Inst.actionLevelHub.levels.Select((level, i) => new ValueDropdownItem<int>(level.levelName, i)));
                return res;
            }
        }
        public ValueDropdownList<int> levelSizeValueDropdownList
        {
            get
            {
                var res = new ValueDropdownList<int>();
                res.AddRange(StaticInfo.Inst.actionLevelHub.levelSizes.Select((level, i) => new ValueDropdownItem<int>(level.sizeName, i)));
                return res;
            }
        }
    }
}