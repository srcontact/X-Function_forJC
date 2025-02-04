using clrev01.Bases;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.Menu
{
    [CreateAssetMenu(menuName = "Menu/ColorBlock")]
    public class ColorBlockAsset : SOBaseOfCL
    {
        #region colorBlock
        [SerializeField]
        private ColorBlock _colorBlock;
        public ColorBlock colorBlock => _colorBlock;
        #endregion
    }
}