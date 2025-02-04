using clrev01.Bases;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class ScaleIndicater : BaseOfCL
    {
        [SerializeField]
        Transform machineModel;
        #region defaultBounds
        [SerializeField]
        private Bounds _defaultBounds;
        public Bounds defaultBounds
        {
            get { return _defaultBounds; }
        }
        #endregion
        [SerializeField]
        float defaultSize;
        [SerializeField]
        int tgtMagnificationNum;
        [SerializeField]
        float slerpRate = 0.5f;
        [SerializeField]
        int upperLineIndicateNum = 1;
        float nowMagni;

        private void Awake()
        {
            defaultSize = scl.x;
        }
        private void Update()
        {
            float tgtMagni = Mathf.Pow(2, tgtMagnificationNum);
            nowMagni = Mathf.Lerp(nowMagni, tgtMagni, slerpRate);
            scl = Vector3.one * defaultSize / nowMagni;
        }
        public void SetIndicatePar(int magnificationNum)
        {
            tgtMagnificationNum = magnificationNum;
        }
    }
}