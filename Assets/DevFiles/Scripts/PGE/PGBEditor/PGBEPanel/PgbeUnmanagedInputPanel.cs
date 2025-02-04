using TMPro;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public abstract class PgbeUnmanagedInputPanel<T> : PgbeUnmanagedPanel<T> where T : unmanaged
    {
        [SerializeField]
        protected TMP_InputField inputField;

        protected override void Awake()
        {
            base.Awake();
            inputField.onEndEdit.AddListener((string s) => OnEndEdit(s));
        }
        public unsafe void OnEndEdit(string s)
        {
            T res;
            if (!TryParseExe(s, out res))
            {
                inputField.text = (*tgtPointer).ToString();
                return;
            }
            *tgtPointer = res;
            SetIndicate(*tgtPointer);
            initPGBEPM.Invoke();
        }
        protected abstract bool TryParseExe(string s, out T res);
    }
}