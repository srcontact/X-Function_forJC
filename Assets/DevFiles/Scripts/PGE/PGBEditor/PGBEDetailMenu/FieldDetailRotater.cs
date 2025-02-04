using clrev01.Bases;
using clrev01.Menu;
using UnityEngine;
using UnityEngine.EventSystems;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class FieldDetailRotater : BaseOfCL
    {
        [SerializeField]
        MenuButton menuButton;
        [SerializeField]
        Transform rotateTgtY, rotateTgtXZ;
        [SerializeField]
        float rotateRate = 1;
        [SerializeField]
        Vector2 currentPos;
        [SerializeField]
        Vector3 rotY, rotXZ;
        private void Awake()
        {
            menuButton.OnBeginDrag += (PointerEventData e) => OnBeginDrag(e);
            menuButton.OnDrag += (PointerEventData e) => OnDrag(e);
            rotY = rotateTgtY.localRotation.eulerAngles;
            rotXZ = rotateTgtXZ.localRotation.eulerAngles;
        }
        private void OnBeginDrag(PointerEventData e)
        {
            currentPos = e.position;
        }
        private void OnDrag(PointerEventData e)
        {
            Vector2 drag = (currentPos - e.position) * rotateRate;
            Vector3 rdY = new Vector3(0, drag.x, 0);
            Vector3 rdXZ = new Vector3(-drag.y, 0, 0);
            rotY += rdY;
            rotXZ += rdXZ;
            rotateTgtXZ.localEulerAngles = rotXZ;
            rotateTgtY.localEulerAngles = rotY;
            currentPos = e.position;
        }
    }
}