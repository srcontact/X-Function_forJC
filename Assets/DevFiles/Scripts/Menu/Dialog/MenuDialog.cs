using System;
using clrev01.Bases;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.Dialog
{
    public abstract class MenuDialog : BaseOfCL, IPointerClickHandler
    {
        [ReadOnly, NonSerialized]
        public bool closeOnTouchBackFlag;

        protected virtual void OnEnable()
        {
            MPPM.dialogManager.OnOpenDialog(this);
        }
        protected virtual void OnDisable()
        {
            MPPM.dialogManager.OnCloseDialog();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        { }
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (openSingle)
            {
                openSingle = false;
                OpenSingle();
            }
#endif
        }
#if UNITY_EDITOR
        [SerializeField]
        bool openSingle;
        void OpenSingle()
        {
            MPPM.dialogManager.OpenSingle(this);
        }
#endif
    }
}