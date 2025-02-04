using System;
using clrev01.Bases;
using clrev01.Extensions;
using TMPro;
using UnityEngine;

namespace clrev01.Menu
{
    public class MouseOverTips : BaseOfCL
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private TextMeshProUGUI tipsObj;
        [SerializeField]
        private RectTransform tipsObjRect;
        [SerializeField]
        private RectTransform parentRect;
        [SerializeField]
        private TextSizeFitter textSizeFitter;
        private string _tips = "initText";
        private Vector2 _tipsPos;
        private int _latestUpdateFrame;

        private void OnEnable()
        {
            tipsObjRect.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (_latestUpdateFrame < Time.frameCount - 5)
            {
                tipsObjRect.gameObject.SetActive(false);
                return;
            }
            tipsObjRect.gameObject.SetActive(true);
            tipsObj.text = _tips;
            textSizeFitter.ExeFitting();
            ClampToWindow(_tipsPos);
        }

        void ClampToWindow(Vector3 openPos)
        {
            Vector3 minPosition = parentRect.rect.min - tipsObjRect.rect.min;
            Vector3 maxPosition = parentRect.rect.max - tipsObjRect.rect.max;

            openPos.x = Mathf.Clamp(openPos.x, minPosition.x, maxPosition.x);
            openPos.y = Mathf.Clamp(openPos.y, minPosition.y, maxPosition.y);

            tipsObjRect.localPosition = openPos;
        }

        public void UpdateTips(string tips)
        {
            _latestUpdateFrame = Time.frameCount;
            _tips = tips;
            _tipsPos = Input.mousePosition / canvas.scaleFactor;
        }
    }
}