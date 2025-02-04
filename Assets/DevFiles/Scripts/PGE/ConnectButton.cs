using clrev01.Menu;
using clrev01.PGE.PGB;
using clrev01.Programs;
using clrev01.Programs.FuncPar.Comment;
using clrev01.Programs.FuncPar.FuncParType;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class ConnectButton : MenuFunction
    {
        [SerializeField]
        private int connectionNum = 0;

        public enum ConnectButtonType
        {
            True,
            False,
            Comment,
        }

        [SerializeField]
        private ConnectButtonType connectButtonType;
        [SerializeField]
        private ColorBlockAsset trueColor, falseColor, commentColor;

        protected override void Awake()
        {
            base.Awake();
            PGEM2.connectButtons.Add(this);
            ColorUpdate();
        }
        public void ActiveUpdate()
        {
            if (PGEM2.selectPgbs.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            var active = true;
            foreach (var pgb in PGEM2.selectPgbs)
            {
                active = active && pgb.pgbd.funcPar switch
                {
                    BranchFuncPar => connectButtonType is ConnectButtonType.True or ConnectButtonType.False,
                    CommentFuncPar => connectButtonType is ConnectButtonType.Comment,
                    _ => connectButtonType is ConnectButtonType.True
                };
            }
            gameObject.SetActive(active);
        }
        private void ColorUpdate()
        {
            tgtButton.colorSetting = connectButtonType switch
            {
                ConnectButtonType.True => trueColor,
                ConnectButtonType.False => falseColor,
                ConnectButtonType.Comment => commentColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            ConnectExe();
        }

        private void ConnectExe()
        {
            if (PGEM2.currentClickedPGB == null) return;
            if (PGEM2.nowConnectionChangingPGB != null) return;
            PGEM2.nowConnectionChangingPGB = PGEM2.currentClickedPGB.pgbd;
            PGEM2.connectNum = connectionNum;
        }

        public override void ExeOnBeginDrag(PointerEventData eventData)
        {
            base.ExeOnBeginDrag(eventData);
            ConnectExe();
            PGEM2.ShowSelectCircle();
        }

        public override void ExeOnDrag(PointerEventData eventData)
        {
            base.ExeOnDrag(eventData);
            PGEM2.MoveSelectCircle();
        }

        public override void ExeOnEndDrag(PointerEventData eventData)
        {
            base.ExeOnEndDrag(eventData);
            PGBlock2 selected = PGEM2.GetPgbInRadius();
            if (selected != null)
            {
                selected.ExeOnDrop(eventData);
            }
            else
            {
                PGEM2.pointerCursor.SetPosition();
                PGEM2.OpenConnectQuickMenuOnClickBackground(PGEM2.nowConnectionChangingPGB);
            }
            PGEM2.nowConnectionChangingPGB = null;
            PGEM2.HideSelectCircle();
        }

        private void OnValidate()
        {
            ColorUpdate();
        }
    }
}