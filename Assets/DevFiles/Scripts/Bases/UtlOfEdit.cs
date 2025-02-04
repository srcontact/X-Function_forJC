using clrev01.HUB;
using Sirenix.OdinInspector;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Bases
{
    public static class UtlOfEdit
    {
        [System.Serializable]
        public class NamedBool
        {
            public virtual string name
            {
                get;
            }
            public bool onOff = false;
        }

        [System.Serializable]
        public class WeaponNamedBool : NamedBool
        {
            public override string name
            {
                get
                {
                    return bulletCDSet.name.ToString();
                }
            }
            [ReadOnly]
            public int weaponCode;
            [ReadOnly]
            public WeaponData bulletCDSet;
            /// <summary>
            /// 機体固有のデフォルト搭載弾数。
            /// </summary>
            public int localDefaultAmoNum = -1;
            /// <summary>
            /// 機体固有の最大搭載弾数。
            /// </summary>
            public int localMaxAmoNum = -1;

            public int globalDefaultAmoNum
            {
                get
                {
                    return WHUB.GetGlobalDefaultAmoNum(weaponCode);
                }
            }
            public int globalMaxAmoNum
            {
                get
                {
                    return WHUB.GetGlobalMaxAmoNum(weaponCode);
                }
            }

            public int defaultAmoNum
            {
                get
                {
                    if (localDefaultAmoNum < 0) return globalDefaultAmoNum;
                    else return localDefaultAmoNum;
                }
            }
            public int maxAmoNum
            {
                get
                {
                    if (localMaxAmoNum < 0) return globalMaxAmoNum;
                    else return localMaxAmoNum;
                }
            }

            public WeaponNamedBool(int n)
            {
                weaponCode = n;
            }
        }

        [System.Serializable]
        public struct SelectEditData
        {
            public int selectNum;
            public int numInputNum;
        }


        public enum EditPanelMode
        {
            simpleButton,
            input,
        }

        public enum MachineInfoIndMode
        {
            none,
            main,
            simple,
        }
    }
}