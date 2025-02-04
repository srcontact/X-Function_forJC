using clrev01.Menu;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Save.VariableData;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public abstract class PgbeUnmanagedPanel<T> : PgbePanel where T : unmanaged
    {
        [SerializeField]
        protected MenuButton modeChangeButton;
        protected unsafe T* tgtPointer;
        private VariableData _variableData;
        private List<VariableType> _selectableVariableTypes;


        protected virtual void Awake()
        {
            modeChangeButton.OnClick.AddListener(() =>
            {
                PGEM2.variableEditor.OpenEditorAddAndSelect(_variableData, initPGBEPM, _selectableVariableTypes);
            });
        }
        public unsafe void OnPgbeOpen(string title, T* data, VariableData vd = null, List<VariableType> selectableVariableTypes = null)
        {
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            if (titleLabel != null) titleLabel.text = title;
            tgtPointer = data;
            SetIndicate(*tgtPointer);
            modeChangeButton.gameObject.SetActive(vd != null);
            _variableData = vd;
            _selectableVariableTypes = selectableVariableTypes;
        }

        protected abstract void SetIndicate(T data);

        protected override unsafe void ResetTgtData()
        {
            tgtPointer = null;
        }
    }
}