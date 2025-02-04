using clrev01.Bases;
using UnityEngine;

namespace clrev01.Menu
{
    public class ChangeScene : MenuFunction
    {
        [SerializeField]
        bool isActive = true;
        [SerializeField]
        int nextScene;

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            if (!isActive) return;
            if (nextScene >= 0)
            {
                StaticInfo.Inst.NextScene(nextScene);
            }
            else StaticInfo.Inst.ReturnScene();
        }
    }
}