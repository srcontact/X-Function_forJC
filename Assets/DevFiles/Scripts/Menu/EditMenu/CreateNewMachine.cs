using System;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using Cysharp.Threading.Tasks;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.EditMenu
{
    public class CreateNewMachine : MenuFunction
    {
        public EditMenuActiveCont activeCont;

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            if (StaticInfo.Inst.nowEditMech is not null)
            {
                WarningAndCreateNew().Forget();
            }
            else
            {
                ExeCreateNewMachine();
            }
        }

        private async UniTask WarningAndCreateNew()
        {
            const string warningTxt = "機体を新規作成します。\n現在編集中の機体は削除されます。\nよろしいですか？";
            MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                warningTxt,
                new[] { "Yes", "No" },
                new[] { (Action)(() => ExeCreateNewMachine()) }
            );
        }

        private void ExeCreateNewMachine()
        {
            StaticInfo.Inst.nowEditMech = new CustomData();
            StaticInfo.Inst.nowEditMech.InitializeData();
            activeCont.ActivateExe();
        }
    }
}