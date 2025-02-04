using clrev01.Bases;
using clrev01.Menu;
using UnityEngine;
using UnityEngine.EventSystems;

namespace clrev01.ClAction.UI
{
    public class ActionMenu : BaseOfCL, IPointerClickHandler
    {
        [SerializeField]
        MenuButton openButton, closeButton;

        public void Initialize()
        {
            openButton.OnClick.AddListener(() => OnClickOpen());
            closeButton.OnClick.AddListener(() => OnClickClose());
            gameObject.SetActive(false);
        }
        private void OnClickClose()
        {
            gameObject.SetActive(false);
        }
        private void OnClickOpen()
        {
            gameObject.SetActive(true);
        }
        public void OnPointerClick(PointerEventData eventData)
        { }
    }
}