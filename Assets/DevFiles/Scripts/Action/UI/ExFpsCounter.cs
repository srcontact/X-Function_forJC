using clrev01.Extensions;
using UnityEngine;
using static clrev01.Extensions.ExUtls;

namespace clrev01.ClAction.UI
{
    public class ExFpsCounter
    {
        [System.Serializable]
        public class FpsCounter : NestedClassBase
        {
            #region fps
            [SerializeField]
            private float _fps;
            public float fps
            {
                get { return _fps; }
                set { _fps = value; }
            }
            #endregion
            #region fpsUpdateCount
            [SerializeField]
            private int _fpsUpdateCount = 10;
            public int fpsUpdateCount
            {
                get { return _fpsUpdateCount; }
                set { _fpsUpdateCount = value; }
            }
            #endregion
            private float previousSecond = 0;
            private int count = 0;

            public void Run()
            {
                count++;
                if (count >= fpsUpdateCount - 1)
                {
                    float now = Time.realtimeSinceStartup;
                    float d = now - previousSecond;
                    fps = count / d;
                    count = 0;
                    previousSecond = now;
                }
            }
        }
    }
}