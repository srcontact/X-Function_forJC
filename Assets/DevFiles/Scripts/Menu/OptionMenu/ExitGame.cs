using UnityEngine;

namespace clrev01.Menu.OptionMenu
{
    public class ExitGame : MenuFunction
    {
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            Application.Quit();
        }
    }
}