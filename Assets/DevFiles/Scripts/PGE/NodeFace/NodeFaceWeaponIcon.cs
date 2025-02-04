using clrev01.Bases;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.PGE.NodeFace
{
    public class NodeFaceWeaponIcon : BaseOfCL
    {
        [SerializeField]
        private Image icon;

        public void SetWeaponIcon(int? weaponCode)
        {
            if (!weaponCode.HasValue) return;

            var uiIcon = (WHUB.GetBulletCD(weaponCode.Value) as CommonDataBase)?.uiIcon;
            icon.sprite = uiIcon ? uiIcon : WHUB.blankIcon;
        }
    }
}