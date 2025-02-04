using clrev01.Bases;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.InformationIndicator
{
    public class InfoIndicatorText : BaseOfCL
    {
        [SerializeField]
        private TextMeshProUGUI title, body;
        [SerializeField]
        private RectTransform bodyScrollRectTransform, textRectTransform;
        [SerializeField]
        private float titleHorizontalRatio = 0.9f, titleVerticalRatio = 0.1f;
        [SerializeField]
        private float bodyHorizontalRatio = 0.6f, bodyVerticalRatio = 0.1f;
        /// <summary>
        /// Bodyのフォントサイズに対するタイトルのフォントサイズの割合の最大
        /// </summary>
        [SerializeField]
        private float maxTitleFontSizeRatio = 1.5f;


        private void Awake()
        {
            var tlf = gameObject.AddComponent<TextLinkFunction>();
        }
        private void OnRectTransformDimensionsChange()
        {
            UpdateLayout().Forget();
        }

        public void SetTexts(string titleStr, string bodyStr)
        {
            title.text = titleStr;
            body.text = bodyStr;
            UpdateLayout().Forget();
        }

        private async UniTask UpdateLayout()
        {
            await UniTask.DelayFrame(2);
            if (body != null) FontSizeSetting(body, bodyHorizontalRatio, bodyVerticalRatio);
            if (title != null)
            {
                FontSizeSetting(title, titleHorizontalRatio, titleVerticalRatio, (int)(body.fontSize * maxTitleFontSizeRatio));
                bodyScrollRectTransform.sizeDelta = new Vector2(0, -title.preferredHeight);
            }
        }
        private void FontSizeSetting(TextMeshProUGUI text, float horizontalRatio = 0, float verticalRatio = 0, int maxFontSize = 0)
        {
            if (text.text is null) return;
            for (int i = 1; i < 100; i++)
            {
                text.fontSize = i;
                var rectSize = textRectTransform.rect.size;
                var lineCount = text.text.Split("\n").Length;
                if (
                    (maxFontSize != 0 && text.fontSize >= maxFontSize) ||
                    (horizontalRatio != 0 && text.preferredWidth >= rectSize.x * horizontalRatio) ||
                    (verticalRatio != 0 && text.preferredHeight / lineCount >= rectSize.y * verticalRatio)
                ) break;
            }
        }
    }
}