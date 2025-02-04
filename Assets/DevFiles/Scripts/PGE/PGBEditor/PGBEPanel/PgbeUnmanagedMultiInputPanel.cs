using Sirenix.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public abstract class PgbeUnmanagedMultiInputPanel<T> : PgbeUnmanagedPanel<T> where T : unmanaged
    {
        [SerializeField]
        protected TMP_InputField[] inputFields;

        protected override void Awake()
        {
            base.Awake();
            for (var i = 0; i < inputFields.Length; i++)
            {
                var i1 = i;
                inputFields[i].onEndEdit.AddListener(s => OnEndEdit(i1, s));
            }
        }
        public void SetInputFieldsActive(IReadOnlyList<bool> activeList)
        {
            if (activeList == null)
            {
                inputFields.ForEach(x => x.gameObject.SetActive(true));
                return;
            }
            for (var i = 0; i < activeList.Count && i < inputFields.Length; i++)
            {
                if (activeList[i] != inputFields[i].gameObject.activeSelf)
                {
                    inputFields[i].gameObject.SetActive(activeList[i]);
                }
            }
        }
        public unsafe void OnEndEdit(int i, string s)
        {
            if (!TryParseExe(i, s, out var res))
            {
                SetIndicate(*tgtPointer);
                return;
            }
            *tgtPointer = res;
            SetIndicate(*tgtPointer);
            initPGBEPM.Invoke();
        }
        protected abstract bool TryParseExe(int i, string s, out T res);
    }
}