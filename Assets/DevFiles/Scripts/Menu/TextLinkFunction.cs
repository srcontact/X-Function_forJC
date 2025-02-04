using clrev01.Bases;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace clrev01.Menu
{
    public class TextLinkFunction : BaseOfCL, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private readonly List<TextMeshProUGUI> _textMeshes = new();
        private bool _isEntered;
        private PointerEventData _enterPointerEventData;


        private void Awake()
        {
            _textMeshes.Clear();
            _textMeshes.AddRange(GetComponentsInChildren<TextMeshProUGUI>(true));
        }

        private void Update()
        {
            if (!_isEntered) return;
            foreach (var tm in _textMeshes)
            {
                DetectLink(tm, _enterPointerEventData, s => OpenTips(s));
            }
        }

        private void DetectLink(TextMeshProUGUI textMesh, PointerEventData eventData, Action<string> linkFunc)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMesh, eventData.position, eventData.pressEventCamera);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = textMesh.textInfo.linkInfo[linkIndex];
                linkFunc.Invoke(linkInfo.GetLinkID());
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            foreach (var tm in _textMeshes)
            {
                DetectLink(tm, eventData, (s) => OpenDescription(s));
            }
        }

        private void OpenDescription(string linkID)
        {
            MenuPagePanelManager.Inst.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(linkID, new[] { "OK" });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isEntered = true;
            _enterPointerEventData = eventData;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _isEntered = false;
        }
        private static void OpenTips(string s)
        {
            MenuPagePanelManager.Inst.mouseOverTips.UpdateTips(s);
        }
    }
}