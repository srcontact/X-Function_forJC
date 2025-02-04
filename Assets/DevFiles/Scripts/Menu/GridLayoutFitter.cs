using clrev01.Bases;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.Menu
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridLayoutFitter : BaseOfCL
    {
        [SerializeField]
        private Vector2 cellRatio = new Vector2(2, 2);

        private GridLayoutGroup _gridLayoutGroup;
        private RectTransform _rectTransform;


        [Button]
        private void OnRectTransformDimensionsChange()
        {
            if (_gridLayoutGroup == null) _gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            var rect = _rectTransform.rect;
            var spacing = _gridLayoutGroup.spacing;
            _gridLayoutGroup.cellSize = new Vector2((rect.width / cellRatio.x) - spacing.x, rect.height / cellRatio.y - spacing.y);
        }
    }
}