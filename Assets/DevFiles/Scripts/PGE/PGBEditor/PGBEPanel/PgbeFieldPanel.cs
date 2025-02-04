using clrev01.Menu;
using clrev01.PGE.FieldFigure;
using clrev01.PGE.PGBEditor.PGBEDetailMenu;
using clrev01.Programs.FieldPar;
using System;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeFieldPanel : PgbePanel
    {
        [SerializeField]
        MenuButton button;
        private PgbeFieldEditDetail _fieldEditDetail;
        private IFieldEditObject _haveFieldFuncPar;
        private Action<IFieldEditObject> _resultSetAction;
        [SerializeField]
        private BoxFieldPanel boxFieldPanel;
        [SerializeField]
        private CircleFieldPanel circleFieldPanel;
        [SerializeField]
        private FieldPanel fieldPanel;
        [SerializeField]
        private TextMeshProUGUI parameterText;
        private bool _ignore3D;
        private bool _ignoreSphere;
        private void Awake()
        {
            button.OnClick.AddListener(() => OnClicked());
        }

        private void OnClicked()
        {
            _fieldEditDetail.OpenPanel(_haveFieldFuncPar, _resultSetAction, _ignore3D, _ignoreSphere);
        }

        public void OnOpen(string title, IFieldEditObject haveFieldFuncPar, Action<IFieldEditObject> resultSetAction, PgbeFieldEditDetail fieldEditDetail, bool ignore3D, bool ignoreSphere)
        {
            if (titleLabel != null) titleLabel.text = title;
            _fieldEditDetail = fieldEditDetail;
            _haveFieldFuncPar = haveFieldFuncPar;
            _resultSetAction = resultSetAction;
            _ignore3D = ignore3D;
            _ignoreSphere = ignoreSphere;
            SetIndicate();
        }
        private void SetIndicate()
        {
            titleLabel.text = _haveFieldFuncPar.FieldFigureTitle;
            boxFieldPanel.gameObject.SetActive(false);
            circleFieldPanel.gameObject.SetActive(false);
            fieldPanel.gameObject.SetActive(false);
            switch (_haveFieldFuncPar, _haveFieldFuncPar.Is2D)
            {
                case (IBoxFieldEditObject x, true):
                    var box = x.GetIndicateInfo();
                    boxFieldPanel.gameObject.SetActive(true);
                    boxFieldPanel.SetBoxPos(box.size, box.offset, box.rotate.x, box.rotate.z, _haveFieldFuncPar, (false, true, false));
                    break;
                case (ICircleFieldEditObject x, true):
                    var cir = x.GetIndicateInfo();
                    circleFieldPanel.gameObject.SetActive(true);
                    circleFieldPanel.SetCirclePos(cir.farRadius, cir.nearRadius, cir.angle, cir.rotate, new Vector2(cir.offset.x, cir.offset.y), _haveFieldFuncPar, (false, true, false));
                    break;
                default:
                    fieldPanel.gameObject.SetActive(true);
                    fieldPanel.SetIndicate(_haveFieldFuncPar);
                    break;
            }
            parameterText.text = _haveFieldFuncPar.GetFieldLongText();
        }

        protected override void ResetTgtData()
        {
            _haveFieldFuncPar = null;
        }
    }
}