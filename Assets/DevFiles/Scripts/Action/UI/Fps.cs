using clrev01.Bases;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.UI
{
    public class Fps : BaseOfCL, IRunner
    {
        [SerializeField]
        private Text fpsText, fiexedFpsText;
        public void RunBeforePhysics()
        { }
        public void RunAfterPhysics()
        { }

        public void RunOnUpdate()
        {
            fpsText.text = "FPS:" + ACM.fpsCounter.fps.ToString("00.00");
            fiexedFpsText.text = "fFPS:" + ACM.fixedFpsCounter.fps.ToString("00.00");
        }
    }
}