using UnityEngine;

namespace clrev01.Bases
{
    public class CameraSizer : BaseOfCL
    {
        [SerializeField]
        Canvas canvas;
        [SerializeField]
        float startCanvasSize;
        float currentCanvasSize;
        public float sizeRate = 1;

        private void Update()
        {
            float cs = canvas.transform.lossyScale.x;
            if (cs == currentCanvasSize) return;

            currentCanvasSize = cs;
            sizeRate = startCanvasSize / cs;
        }

        [ContextMenu("SetStartCanvasSize")]
        private void SetStartCanvasSize()
        {
            startCanvasSize = canvas.transform.lossyScale.x;
        }
    }
}