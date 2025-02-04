using clrev01.Bases;
using clrev01.Extensions;
using clrev01.PGE.FieldFigure;
using clrev01.PGE.PGEM;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.NodeFace
{
    public class NodeFaceBasic : PoolableBehaviour
    {
        public int key { get; set; } = -1;
        private RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = gameObject.GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
        public BaseOfCL baseOfCl => this;
        public NodeFaceText title, cost;
        public List<Image> icons = new();
        public List<NodeFaceText> nodeFaceTexts = new();
        public List<NodeFaceGauge> gauges = new();
        public NodeFaceStopActionIcon stopActionIcon;
        public NodeFaceWeaponList weaponList;
        public NodeFaceWeaponIcon weaponIcon;
        public BoxFieldPanel boxFieldPanel;
        public CircleFieldPanel circleFieldPanel;
        public FieldPanel fieldPanel;


        public void SetIcons(List<AssetReferenceSet<Sprite>> icons)
        {
            for (int i = 0; i < this.icons.Count && i < icons.Count; i++)
            {
                this.icons[i].sprite = icons[i].GetAsset(PGEManager.Inst.gameObject);
            }
        }
        public void SetPgbFunc(IPGBFuncUnion pgbFunc)
        {
            if (title != null) title.UpdateIndicate(true, pgbFunc.BlockTypeStr);
            if (cost != null) cost.UpdateIndicate(pgbFunc.calcCost > 0, pgbFunc.calcCost.ToString("00"));
            var texts = pgbFunc.GetNodeFaceText();
            for (int i = 0; i < nodeFaceTexts.Count; i++)
            {
                if (texts != null && i < texts.Length) nodeFaceTexts[i].UpdateIndicate(texts[i] != null, texts[i]);
                else nodeFaceTexts[i].UpdateIndicate(false, null);
            }
            var values = pgbFunc.GetNodeFaceValue();
            if (values != null)
            {
                for (int i = 0; i < gauges.Count && i < values.Length; i++)
                {
                    gauges[i].SetGaugeValue(values[i]);
                }
            }
            if (stopActionIcon != null) stopActionIcon.SetStopActionIcon(pgbFunc.GetNodeFaceStopActionType());
            if (weaponList != null) weaponList.SetIndicate(pgbFunc.GetNodeFaceSelectedWeapons());
            if (weaponIcon != null) weaponIcon.SetWeaponIcon(pgbFunc.GetNodeFaceWeaponIcon());
            var fp = pgbFunc.GetNodeFaceIFieldEditObject();
            if (fieldPanel != null) fieldPanel.gameObject.SetActive(false);
            if (boxFieldPanel != null) boxFieldPanel.gameObject.SetActive(false);
            if (circleFieldPanel != null) circleFieldPanel.gameObject.SetActive(false);
            if (fp != null)
            {
                if (!fp.Is2D)
                {
                    fieldPanel.gameObject.SetActive(true);
                    fieldPanel.SetIndicate(fp);
                }
                else
                {
                    switch (fp)
                    {
                        case IBoxFieldEditObject fieldPar:
                            var bfp = fieldPar.GetIndicateInfo();
                            if (boxFieldPanel != null)
                            {
                                boxFieldPanel.gameObject.SetActive(true);
                                boxFieldPanel.SetBoxPos(
                                    new Vector2(bfp.size.x, bfp.size.z),
                                    new Vector2(bfp.offset.x, bfp.offset.z),
                                    fieldPar.Is2D ? 0 : bfp.rotate.x, -bfp.rotate.y,
                                    fieldPar, (true, false, true)
                                );
                            }
                            break;
                        case ICircleFieldEditObject fieldPar:
                            var cfp = fieldPar.GetIndicateInfo();
                            if (circleFieldPanel != null)
                            {
                                circleFieldPanel.gameObject.SetActive(true);
                                circleFieldPanel.SetCirclePos(
                                    cfp.farRadius,
                                    cfp.nearRadius,
                                    cfp.angle,
                                    cfp.rotate,
                                    new Vector2(cfp.offset.x, cfp.offset.z),
                                    fieldPar,
                                    (false, true, false)
                                );
                            }
                            break;
                    }
                }
            }
        }
    }
}