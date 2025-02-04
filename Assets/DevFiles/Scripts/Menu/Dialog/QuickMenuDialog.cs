using clrev01.Extensions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.Menu.Dialog
{
    public class QuickMenuDialog : MenuDialog
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private RectTransform parentRect;
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private MenuButton originalPanel;
        [SerializeField]
        private MenuButton flagsAcceptButton;
        [SerializeField]
        private Transform dialogButtonAddTgt;
        [SerializeField]
        GridLayoutGroup gridLayoutGroup;
        [SerializeField]
        private int gridRowCountMax = 8;
        private bool _flagsMode;
        [SerializeField]
        private long flags;
        private Action<int> _answerEvent;
        private Action<long> _flagsAnswerEvent;
        [SerializeField]
        private List<MenuButton> allPanels = new();
        [SerializeField]
        private List<TextMeshProUGUI> texts = new();

        private void Awake()
        {
            flagsAcceptButton.OnClick.AddListener(() => EndQuickMenuFlags(flags));
        }
        public void OpenQuickMenu(IReadOnlyList<string> menuTexts, Action<int> action, Vector3? openPos = null, IReadOnlyList<(ColorBlockAsset cba, ColorBlockAsset cbat)> colorList = null)
        {
            _flagsMode = false;
            closeOnTouchBackFlag = true;
            _answerEvent = null;
            _answerEvent += action;
            flagsAcceptButton.gameObject.SetActive(false);
            OpenQuickMenuCommon(menuTexts, openPos, colorList);
        }
        public void OpenQuickMenuFlags(IReadOnlyList<string> menuTexts, Action<long> action, long flags, Vector3? openPos = null, IReadOnlyList<(ColorBlockAsset cba, ColorBlockAsset cbat)> colorList = null)
        {
            _flagsMode = true;
            closeOnTouchBackFlag = true;
            _flagsAnswerEvent = null;
            _flagsAnswerEvent += action;
            flagsAcceptButton.gameObject.SetActive(true);
            this.flags = flags;
            OpenQuickMenuCommon(menuTexts, openPos, colorList);
        }
        private void OpenQuickMenuCommon(IReadOnlyList<string> menuTexts, Vector3? openPos, IReadOnlyList<(ColorBlockAsset cba, ColorBlockAsset cbat)> colorList)
        {
            gameObject.SetActive(true);
            foreach (var p in allPanels)
            {
                p.gameObject.SetActive(false);
            }
            for (int i = 0; i < menuTexts.Count; i++)
            {
                MenuButton p;
                if (i < allPanels.Count)
                {
                    p = allPanels[i];
                }
                else
                {
                    p = originalPanel.SafeInstantiate();
                    Transform transform1;
                    (transform1 = p.transform).SetParent(dialogButtonAddTgt);
                    transform1.localScale = Vector3.one;
                    transform1.localPosition = Vector3.zero;
                    int ii = i;
                    p.OnClick.AddListener(() => OnSelectMenu(ii));
                    allPanels.Add(p);
                    texts.Add(p.GetComponentInChildren<TextMeshProUGUI>(true));
                }
                if (colorList != null && i >= 0 && i < colorList.Count)
                {
                    p.colorSetting = colorList[i].cba;
                    p.toggledColorSetting = colorList[i].cbat;
                }
                else
                {
                    p.colorSetting = null;
                    p.toggledColorSetting = null;
                }
                if (_flagsMode) SetButtonHighLight(p, i);
                else p.isHighlight = false;
                p.gameObject.SetActive(true);
                texts[i].text = menuTexts[i];
            }
            gridLayoutGroup.constraintCount = Mathf.Min(menuTexts.Count, gridRowCountMax);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            Vector3 canvasScaleFactor;
            if (openPos == null) canvasScaleFactor = Input.mousePosition / canvas.scaleFactor;
            else
            {
                canvasScaleFactor = new Vector3(openPos.Value.x, openPos.Value.y, 0) / canvas.scaleFactor;
            }
            UniTask.Create(async () =>
            {
                await UniTask.DelayFrame(2);
                ClampToWindow(canvasScaleFactor);
            }).Forget();
        }

        void ClampToWindow(Vector3 openPos)
        {
            Vector3 minPosition = parentRect.rect.min - rectTransform.rect.min;
            Vector3 maxPosition = parentRect.rect.max - rectTransform.rect.max;

            openPos.x = Mathf.Clamp(openPos.x, minPosition.x, maxPosition.x);
            openPos.y = Mathf.Clamp(openPos.y, minPosition.y, maxPosition.y);

            rectTransform.localPosition = openPos;
        }
        void OnSelectMenu(int num)
        {
            if (_flagsMode)
            {
                flags ^= 1L << num;
                SetButtonHighLight(allPanels[num], num);
                return;
            }
            else EndQuickMenu(num);
        }

        private void SetButtonHighLight(MenuButton menuButton, int num)
        {
            menuButton.isHighlight = (flags & (1L << num)) != 0;
        }

        void EndQuickMenu(int num)
        {
            _answerEvent.Invoke(num);
            _answerEvent = null;
            gameObject.SetActive(false);
        }
        void EndQuickMenuFlags(long num)
        {
            _flagsAnswerEvent.Invoke(num);
            _flagsAnswerEvent = null;
            gameObject.SetActive(false);
        }
    }
}