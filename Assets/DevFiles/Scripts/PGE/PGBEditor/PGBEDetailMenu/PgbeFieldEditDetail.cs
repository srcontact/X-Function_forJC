using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Menu;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class PgbeFieldEditDetail : BaseOfCL
    {
        [SerializeField]
        MenuPage menuPage;
        //todo:座標系がグローバルのときは自機の表示を消したい
        [SerializeField]
        GameObject settingPanel;
        [SerializeField]
        private MenuButton origSettingButton;
        private readonly List<(MenuButton button, TextMeshProUGUI text)> _settingButtons = new();
        [SerializeField]
        private MenuButton acceptButton;
        IFieldEditObject fieldClone;
        [SerializeField]
        IFieldEditObject haveFieldFuncPar;
        Action<IFieldEditObject> resultSetAction;
        [SerializeField]
        private PgbepManager _pgbepManager;
        [SerializeField]
        private TabMenu _tabMenu;
        [SerializeField]
        BoxFieldDetail boxFieldDetail;
        [SerializeField]
        CircleFieldDetail circleFieldDetail;
        [SerializeField]
        SphereFieldDetail sphereFieldDetail;
        [SerializeField]
        private MenuButton variableButton;

        private bool _ignoreSphereField;
        private bool _ignore3D;


        private void Awake()
        {
            _pgbepManager.initOnEditAction = () => ResetParameterArea(_tabMenu.nowSelectedTabNumber);
            acceptButton.OnClick.AddListener(() => Accept());
            _tabMenu.SetIndicateTabContentAction(i => IndicateTabContent(i));
        }

        private void Update()
        {
            if (boxFieldDetail.gameObject.activeSelf) boxFieldDetail.UpdateIndicator();
            if (circleFieldDetail.gameObject.activeSelf) circleFieldDetail.UpdateIndicator();
            if (sphereFieldDetail.gameObject.activeSelf) sphereFieldDetail.UpdateIndicator();
        }

        private void IndicateTabContent(int? i)
        {
            boxFieldDetail.Activate(fieldClone);
            circleFieldDetail.Activate(fieldClone);
            sphereFieldDetail.Activate(fieldClone);
            _tabMenu.SetTab(fieldClone.TabStrings);
            _tabMenu.SetTabActives(fieldClone.IndicateTab);
            ResetParameterArea(i);
        }
        private void ResetParameterArea(int? i)
        {
            _pgbepManager.ResetPgbeps(false);
            if (i != null) fieldClone.SetEditIndicate(_pgbepManager, i.Value);
            else fieldClone.SetEditIndicate(_pgbepManager);
        }
        public void OpenPanel(IFieldEditObject haveFieldFuncPar, Action<IFieldEditObject> resultSetAction, bool ignore3D, bool ignoreSphere)
        {
            this.haveFieldFuncPar = haveFieldFuncPar;
            this.resultSetAction = resultSetAction;
            _ignoreSphereField = ignoreSphere;
            _ignore3D = ignore3D;
            fieldClone = this.haveFieldFuncPar.CloneDeep();
            MenuPagePanelManager.Inst.OpenPage(menuPage);
            _tabMenu.SetTab(fieldClone.TabStrings);
            _tabMenu.SetTabActives(fieldClone.IndicateTab);
            IndicateTabContent(null);
            SetSettingButtons();
        }
        private void SetSettingButtons()
        {
            if (fieldClone.settingList.Count == 0)
            {
                settingPanel.gameObject.SetActive(false);
                return;
            }
            settingPanel.gameObject.SetActive(true);
            _settingButtons.ForEach(x => x.button.gameObject.SetActive(false));
            foreach (var settingPar in fieldClone.settingList)
            {
                var sb = _settingButtons.Find(x => !x.button.gameObject.activeSelf);
                if (sb.button == null)
                {
                    sb.button = origSettingButton.SafeInstantiate();
                    var sbt = sb.button.transform;
                    sbt.parent = settingPanel.transform;
                    sbt.localPosition = Vector3.zero;
                    sbt.localScale = Vector3.one;
                    sb.text = sb.button.GetComponentInChildren<TextMeshProUGUI>();
                    _settingButtons.Add(sb);
                }
                else
                {
                    sb.button.gameObject.SetActive(true);
                }
                sb.text.text = settingPar.menus[settingPar.nowSelected.Invoke()];
                sb.button.OnClick.RemoveAllListeners();
                sb.button.OnClick.AddListener(() =>
                {
                    PGEM2.quickMenuDialog.OpenQuickMenu(
                        settingPar.menus.ToList(),
                        i =>
                        {
                            fieldClone = settingPar.func(i);
                            IndicateTabContent(null);
                            SetSettingButtons();
                        },
                        Camera.main.WorldToScreenPoint(sb.button.transform.position)
                    );
                });
            }
        }
        void Accept()
        {
            if (haveFieldFuncPar != null &&
                fieldClone != null)
            {
                resultSetAction.Invoke(fieldClone);
            }
            PGEM2.editMenu.InitializeEditPanels(true);
            MenuPagePanelManager.Inst.ReturnPage();
        }
    }
}