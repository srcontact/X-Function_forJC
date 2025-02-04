using UnityEngine;

namespace clrev01.Extensions
{
    public static class ExUI
    {
        public static Vector3 GetPointerPos()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}