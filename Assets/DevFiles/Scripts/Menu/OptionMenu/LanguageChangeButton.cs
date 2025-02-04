using I2.Loc;
using System.Linq;

namespace clrev01.Menu.OptionMenu
{
    public class LanguageChangeButton : MenuFunction
    {
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            var languageFields = (typeof(I2.Loc.ScriptLocalization.language)).GetProperties().ToList();
            var languages = languageFields.ConvertAll(x => (string)x.GetValue(null));
            MenuPagePanelManager.Inst.dialogManager.quickMenuDialog.OpenQuickMenu(languages, (i) => { LocalizationManager.CurrentLanguage = languages[i]; });
        }
    }
}