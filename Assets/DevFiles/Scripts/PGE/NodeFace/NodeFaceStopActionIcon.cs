using clrev01.Bases;
using clrev01.Programs.FuncPar.FuncParType;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.NodeFace
{
    public class NodeFaceStopActionIcon : BaseOfCL
    {
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Dictionary<StopActionType, Sprite> _actionIcons = new();


        public void SetStopActionIcon(StopActionType? stopActionType)
        {
            if (stopActionType.HasValue && _actionIcons[stopActionType.Value] != null)
            {
                icon.gameObject.SetActive(true);
                icon.sprite = _actionIcons[stopActionType.Value];
            }
            else
            {
                icon.gameObject.SetActive(false);
            }
        }
        private void OnValidate()
        {
            var actionTypes = (StopActionType[])Enum.GetValues(typeof(StopActionType));
            foreach (var at in actionTypes)
            {
                if (_actionIcons.ContainsKey(at)) continue;
                _actionIcons.Add(at, null);
            }
        }
    }
}