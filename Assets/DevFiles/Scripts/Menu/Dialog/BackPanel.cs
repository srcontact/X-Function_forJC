using clrev01.Bases;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace clrev01.Menu.Dialog
{
    public class BackPanel : BaseOfCL, IPointerClickHandler
    {
        public UnityEvent onClick = new UnityEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
    }
}