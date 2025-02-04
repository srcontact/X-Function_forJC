using clrev01.Bases;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.ClAction.UI
{
    public class SightingTargetSymbol : BaseOfCL
    {
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private Image image;
        [SerializeField]
        private List<TextMeshProUGUI> texts = new();

        public void UpdateText(params string[] strs)
        {
            if (texts.Count == 0) return;
            for (var i = 0; i < texts.Count; i++)
            {
                var text = texts[i];
                if (i >= 0 && i < strs.Length) text.text = strs[i];
            }
        }

        public void UpdatePos(Vector3 screenPos)
        {
            rect.position = screenPos;
        }

        public void UpdateColor(Color color)
        {
            image.color = color;
            foreach (var text in texts)
            {
                text.color = color;
            }
        }
    }
}