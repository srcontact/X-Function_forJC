using clrev01.Menu.InformationIndicator;
using I2.Loc;

namespace clrev01.Extensions
{
    public static class ExILocalizetionUtl
    {
        public static void UpdateLocalizeData(IInfoTextData infoTextDate)
        {
            var source = LocalizationManager.Sources[0];
        }

        private static LocalizedString AddLocalizeTerm(string termStr, params string[] languages)
        {
            var source = LocalizationManager.Sources[0];

            if (source.ContainsTerm(termStr)) return termStr;

            var term = source.AddTerm(termStr, eTermType.Text, true);
            for (int i = 0; i < term.Languages.Length && i < languages.Length; i++)
            {
                term.Languages[i] = languages[i];
            }
            return termStr;
        }
    }
}