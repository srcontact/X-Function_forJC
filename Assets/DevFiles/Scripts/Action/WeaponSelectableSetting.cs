using clrev01.Bases;
using static clrev01.Bases.UtlOfEdit;

namespace clrev01.ClAction
{
    [System.Serializable]
    public class WeaponSelectableSetting : EnumSelectableSetting<int, WeaponNamedBool>
    {
        public int defaultWeapon = 0;
        public int GetDefaultWeaponDefaultAmoNum
        {
            get
            {
                int n = 0;
                for (int i = 0; i < enumBoolSets.Count; i++)
                {
                    if (enumBoolSets[i].weaponCode == defaultWeapon)
                    {
                        n = enumBoolSets[i].defaultAmoNum;
                        break;
                    }
                }
                return n;
            }
        }
        public float maxRotateAnglePerFrame = 60;
        public float normalWobbleLate = 0.01f;
        public float recoilWobbleDecayLate = 0.5f;
    }
}