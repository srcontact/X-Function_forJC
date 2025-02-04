using clrev01.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace clrev01.ClAction.OptionParts
{
    [CreateAssetMenu(menuName = "Hub/OptionPartsHub")]
    public class OptionPartsHub : SOBaseOfCL
    {
        [Serializable]
        public class OptionDataSet
        {
            public int code;
            public string partsName;
            public int defaultUsableNum = 5;
            public int defaultMaxUsableNum = 5;
            public Sprite uiIcon;
            public OptionPartsData data;
        }

        [SerializeField]
        private List<OptionDataSet> optionPartsList = new();


        public OptionDataSet GetOptionPartsData(int code)
        {
            foreach (var op in optionPartsList)
            {
                if (op.code == code) return op;
            }
            return optionPartsList[0];
        }
        public int GetOptionPartsCount()
        {
            return optionPartsList.Count;
        }
        public int GetOptionPartsIndexInList(int code)
        {
            return optionPartsList.FindIndex(x => x.code == code);
        }

        private void OnValidate()
        {
            if (optionPartsList.GroupBy(x => x.code).Any(x => x.Count() > 1))
            {
                Debug.LogError($"!OptionPartsDataのcodeが重複している!");
            }
        }
    }
}