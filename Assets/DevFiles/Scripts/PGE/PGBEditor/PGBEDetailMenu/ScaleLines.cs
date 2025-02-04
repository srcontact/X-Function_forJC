using clrev01.Bases;
using clrev01.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class ScaleLines : BaseOfCL
    {
        [SerializeField]
        int upperLineIndicateNum = 2, lineNum = 5;
        [SerializeField]
        float defaultTextSize = 1;
        [SerializeField]
        LineRenderer origLine;
        [SerializeField]
        TextMeshProUGUI origTxt;
        [SerializeField]
        float txtSizeRate = 1.5f;
        List<LineRenderer> lines = new List<LineRenderer>();
        List<TextMeshProUGUI> txts = new List<TextMeshProUGUI>();
        [SerializeField]
        Vector3 axis1 = new Vector3(1, 0, 0), axis2 = new Vector3(0, 0, 1);
        #region visibleLine
        [SerializeField]
        private bool _visibleLine = true;
        public bool visibleLine
        {
            get { return _visibleLine; }
            set
            {
                _visibleLine = value;
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i].gameObject.SetActive(_visibleLine);
                }
            }
        }
        #endregion
        #region visibleTxt
        [SerializeField]
        private bool _visibleTxt = true;
        public bool visibleTxt
        {
            get { return _visibleTxt; }
            set
            {
                _visibleTxt = value;
                for (int i = 0; i < txts.Count; i++)
                {
                    txts[i].gameObject.SetActive(_visibleTxt);
                }
            }
        }
        #endregion

        private void Awake()
        {
            for (int i = 0; i < lineNum; i++)
            {
                LineRenderer lr = origLine.SafeInstantiate();
                lr.transform.parent = transform;
                lr.transform.localPosition = Vector3.zero;
                lr.transform.localRotation = Quaternion.identity;
                lr.transform.localScale = Vector3.one;
                lr.gameObject.SetActive(true);
                lines.Add(lr);
                for (int j = 0; j < 2; j++)
                {
                    TextMeshProUGUI t = origTxt.SafeInstantiate();
                    t.transform.parent = transform;
                    t.transform.localPosition = Vector3.zero;
                    t.transform.localScale = Vector3.one;
                    t.gameObject.SetActive(true);
                    txts.Add(t);
                }
            }
        }
        private void Update()
        {
            for (int i = 0; i < txts.Count; i++)
            {
                txts[i].transform.rotation = Quaternion.identity;
            }
        }
        public void IndicateScales(int lineMagni, float defaultFieldScale)
        {
            lineMagni += upperLineIndicateNum;
            for (int i = 0; i < lines.Count; i++)
            {
                float pow = Mathf.Pow(2, (lineMagni - i));
                float len = defaultFieldScale / 2 * pow;
                Vector3 a1 = axis1 * len;
                Vector3 a2 = axis2 * len;
                Vector3[] pl = new Vector3[]
                {
                    a1 + a2, a1 - a2, -a1 - a2, -a1 + a2
                };
                lines[i].positionCount = pl.Length;
                lines[i].SetPositions(pl);
                for (int j = 0; j < 2; j++)
                {
                    TextMeshProUGUI t = txts[i * 2 + j];
                    t.text = (j % 2 == 0 ? len : -len).ToString() + "m";
                    t.transform.localPosition = j % 2 == 0 ? a1 : -a1;
                    t.transform.localScale =
                        Vector3.one * (defaultTextSize * Mathf.Pow(2, lineMagni) * Mathf.Pow(txtSizeRate, (lines.Count - i - 1)));
                    t.transform.rotation = Quaternion.identity;
                }
            }
        }
    }
}