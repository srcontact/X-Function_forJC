using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.PGE.NodeFace
{
    public class NodeFaceWeaponList : BaseOfCL
    {
        [SerializeField]
        private Color onColor = Color.white, offColor = Color.black;
        [SerializeField]
        private List<Image> weaponImages = new();

        public void SetIndicate(long? weapons)
        {
            if (!weapons.HasValue)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            var weaponList = StaticInfo.Inst.nowEditMech.mechCustom.weapons;
            for (int i = 0; i < weaponImages.Count; i++)
            {
                if (i >= weaponList.Count)
                {
                    weaponImages[i].sprite = WHUB.nullIcon;
                    weaponImages[i].color = offColor;
                    continue;
                }
                weaponImages[i].color = (weapons & 1 << i) > 0 ? onColor : offColor;
                var weaponIcon = (WHUB.GetBulletCD(weaponList[i]) as CommonDataBase)?.uiIcon;
                if (weaponIcon == null)
                {
                    weaponImages[i].sprite = WHUB.blankIcon;
                    continue;
                }
                weaponImages[i].sprite = weaponIcon;
            }
        }
    }
}