using clrev01.Bases;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu
{
    public class TransitionMenuFunc : MenuFunction
    {
        [SerializeField]
        MenuPage nextPage;

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            TransitionPage().Forget(); //選択時イベントが終わってからページ遷移するようにしている。
        }

        public async UniTask TransitionPage()
        {
            if (nextPage == null) return;
            await UniTask.Yield();
            MPPM.OpenPage(nextPage);
        }
    }
}