using clrev01.Bases;
using clrev01.Menu;
using clrev01.Programs;
using System;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGEM
{
    public class PGEMultipurposeMenu : BaseOfCL
    {
        [SerializeField]
        MenuButton button;
        [SerializeField]
        MenuPage testPage;
        [SerializeField]
        VariableEditor.VariableEditor variableEditor;

        public enum PGEMenuOption
        {
            TestMenu,
            VariableSetting,
        }

        Camera _mainCamera;
        Camera mainCamera
        {
            get
            {
                if (_mainCamera == null) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        public void Awake()
        {
            button.OnClick.AddListener(() => OnOpenMenu());
        }

        private void OnOpenMenu()
        {
            Vector3 qmp = mainCamera.WorldToScreenPoint(transform.position);
            PGEM2.quickMenuDialog.OpenQuickMenu(
                Enum.GetNames(typeof(PGEMenuOption)).ToList(),
                (i) => OnSelectMenu((PGEMenuOption)i),
                qmp);
        }

        private void OnSelectMenu(PGEMenuOption i)
        {
            switch (i)
            {
                case PGEMenuOption.TestMenu:
                    MPPM.OpenPage(testPage);
                    break;
                case PGEMenuOption.VariableSetting:
                    PGEM2.variableEditor.OpenEditor();
                    break;
                default:
                    break;
            }
        }
    }
}