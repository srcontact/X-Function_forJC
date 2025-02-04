using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace clrev01.Menu.Dialog
{
    public class SimpleDialog : MenuDialog
    {
        [SerializeField]
        TextMeshProUGUI mainTxt;
        [SerializeField]
        List<TextMeshProUGUI> buttonTxts = new();
        [SerializeField]
        List<MenuButton> buttons = new();
        private Action _closeOnTouchBackAction;

        public void OpenSimpleDialogCloseOnTouchBack(
            string txt,
            string[] buttonTexts,
            Action[] selectedActions = null,
            Action closeOnTouchBackAction = null
        )
        {
            closeOnTouchBackFlag = true;
            _closeOnTouchBackAction = closeOnTouchBackAction;
            OpenSimpleDialogCommon(txt, buttonTexts, selectedActions);
        }

        public void OpenSimpleDialogCloseOnlyButton(
            string txt,
            string[] buttonTexts,
            Action[] selectedActions = null
        )
        {
            closeOnTouchBackFlag = false;
            _closeOnTouchBackAction = null;
            OpenSimpleDialogCommon(txt, buttonTexts, selectedActions);
        }

        private void OpenSimpleDialogCommon(string txt, string[] buttonTexts, Action[] selectedActions)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i < buttonTexts.Length)
                {
                    buttons[i].gameObject.SetActive(true);
                    buttonTxts[i].text = buttonTexts[i];
                    buttons[i].OnClick.AddListener(() => gameObject.SetActive(false));
                    if (selectedActions != null && i < selectedActions.Length)
                    {
                        buttons[i].OnClick.AddListener(selectedActions[i].Invoke);
                    }
                }
                else buttons[i].gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
            SettingMainText(txt).Forget();
        }

        private async UniTask SettingMainText(string txt)
        {
            mainTxt.text = "";
            await UniTask.Yield();

            mainTxt.text = txt;
            Vector2 v = mainTxt.GetPreferredValues();
            mainTxt.rectTransform.sizeDelta =
                new Vector2(mainTxt.rectTransform.sizeDelta.x, v.y);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (!closeOnTouchBackFlag) return;
            _closeOnTouchBackAction?.Invoke();
            gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _closeOnTouchBackAction = null;
            foreach (var button in buttons)
            {
                button.OnClick.RemoveAllListeners();
            }
        }
    }
}