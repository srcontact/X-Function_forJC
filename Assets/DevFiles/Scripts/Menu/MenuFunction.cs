using clrev01.Bases;
using UnityEngine;
using UnityEngine.EventSystems;

namespace clrev01.Menu
{
    public class MenuFunction : PoolableBehaviour
    {
        [SerializeField]
        protected MenuButton tgtButton;
        public bool tgtButtonInteractive
        {
            get => tgtButton.interactable;
            set => tgtButton.interactable = value;
        }

        protected virtual void Awake()
        {
            tgtButton.OnClick.AddListener(ExeOnClick);
            tgtButton.OnClickDuringSelection.AddListener(ExeOnClickDuringSelection);
            tgtButton.OnDoubleClick.AddListener(ExeOnDoubleClick);
            tgtButton.OnRightClick.AddListener(ExeOnRightClick);
            tgtButton.OnDrop += ExeOnDrop;
            tgtButton.OnBeginDrag += ExeOnBeginDrag;
            tgtButton.OnDrag += ExeOnDrag;
            tgtButton.OnEndDrag += ExeOnEndDrag;
        }

        public virtual void ExeOnClick()
        { }
        public virtual void ExeOnClickDuringSelection()
        { }
        public virtual void ExeOnDoubleClick()
        { }
        public virtual void ExeOnRightClick()
        { }
        public virtual void ExeOnDrop(PointerEventData eventData)
        { }
        public virtual void ExeOnBeginDrag(PointerEventData eventData)
        { }
        public virtual void ExeOnDrag(PointerEventData eventData)
        { }
        public virtual void ExeOnEndDrag(PointerEventData eventData)
        { }

        public static bool IsVisibleInViewport(RectTransform rectTransform, Camera targetCamera)
        {
            // ワールド座標上の四隅を取得
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // 四隅のいずれかがカメラビュー内 (0～1) に含まれていれば「可視」とみなす
            foreach (Vector3 corner in corners)
            {
                Vector3 viewportPos = targetCamera.WorldToViewportPoint(corner);

                // z < 0 の場合はカメラの背面なので不可視
                if (viewportPos.z < 0f)
                    continue;

                // ビューポート座標が [0,1] の範囲内であれば映っている
                if (viewportPos.x >= 0f && viewportPos.x <= 1f &&
                    viewportPos.y >= 0f && viewportPos.y <= 1f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}