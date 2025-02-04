using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeStringPanel : PgbePanel, IScrollHandler
    {
        [SerializeField]
        protected TMP_InputField inputField;
        private StringRefObj _tgtData;
        private Action _formatAfterInput;
        private ScrollRect _scrollRect;


        protected void Awake()
        {
            inputField.onValueChanged.AddListener(s => { OnValueChanged(s); });
            inputField.onEndEdit.AddListener(s => OnEndEdit(s));
            inputField.scrollSensitivity = 0;
        }
        public void OnPgbeOpen(StringRefObj data, Action formatAfterInput, TMP_InputField.ContentType contentType, ScrollRect scrollRect)
        {
            _tgtData = data;
            _scrollRect = scrollRect;
            inputField.contentType = contentType;
            inputField.text = _tgtData.obj;
            _formatAfterInput = formatAfterInput;
        }
        private void OnValueChanged(string s)
        {
            //入力中改行が一番したにある場合はスクロールを一番下ちょうどに移動する。（この処理がないとスクロールが入力位置からどんどんずれていく…）
            if (!string.IsNullOrEmpty(s) && s[^1] == '\n' && _scrollRect.verticalScrollbar.gameObject.activeSelf)
            {
                UniTask.Create(async () =>
                {
                    await UniTask.DelayFrame(1);
                    _scrollRect.verticalScrollbar.value = 0;
                }).Forget();
            }
        }
        private void OnEndEdit(string arg0)
        {
            //文末に改行がある場合は削除する（こうしないと文末が入力欄の下に行ってしまい文末が分かりにくくなってしまう）
            _tgtData.obj = arg0.TrimEnd('\r', '\n');
            _formatAfterInput.Invoke();
            inputField.text = _tgtData.obj;
            inputField.ForceLabelUpdate();
        }
        protected override void ResetTgtData()
        {
            _tgtData = null;
        }
        public void OnScroll(PointerEventData eventData)
        {
            _scrollRect.OnScroll(eventData);
        }
    }
}