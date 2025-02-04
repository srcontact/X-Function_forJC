using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeTogglePanel : PgbeUnmanagedPanel<bool>
    {
        [SerializeField]
        private Toggle toggle;

        protected override void Awake()
        {
            base.Awake();
            toggle.onValueChanged.AddListener((b) => OnToggleChanged(b));
        }

        private unsafe void OnToggleChanged(bool b)
        {
            *tgtPointer = b;
            SetIndicate(*tgtPointer);
            initPGBEPM.Invoke();
        }

        protected override void SetIndicate(bool data)
        {
            toggle.SetIsOnWithoutNotify(data);
        }
    }
}