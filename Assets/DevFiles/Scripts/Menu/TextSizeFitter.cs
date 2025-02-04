using clrev01.Bases;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace clrev01.Menu
{
    public class TextSizeFitter : BaseOfCL
    {
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private RectTransform rectTransform;

        private void Start()
        {
            ExeFitting();
        }

        private void Update()
        {
            if (text.havePropertiesChanged) ExeFitting();
        }

        public void ExeFitting()
        {
            rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
        }

        private void OnValidate()
        {
            rectTransform = GetComponent<RectTransform>();
            if (text != null && rectTransform != null) ExeFitting();
        }

        [Button]
        private void ExeValidate()
        {
            OnValidate();
        }
    }
}