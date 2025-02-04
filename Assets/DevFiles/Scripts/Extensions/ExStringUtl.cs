using I2.Loc;

namespace clrev01.Extensions
{
    public static class ExStringUtl
    {
        public static string SpaceToNbSp(this string str)
        {
            return str.Replace(" ", " ");
        }
        public static string SpaceToNbSp(this LocalizedString str)
        {
            return str.ToString().SpaceToNbSp();
        }
        public static string Tagging(this string str, string tagStr, string tagValue = null)
        {
            return $"<{tagStr}{(tagValue is not null ? @$"=""{tagValue}""" : "")}>{str}</{tagStr}>";
        }
        public static string Tagging(this LocalizedString str, string tagStr, string tagValue = null)
        {
            return str.ToString().Tagging(tagStr, tagValue);
        }
        public static string LinkTag(this string str, string linkStr)
        {
            return linkStr is null ? str : str.Tagging("link", linkStr);
        }
        public static string LinkTag(this LocalizedString str, string linkStr)
        {
            return str.ToString().LinkTag(linkStr);
        }
        public static string GetNormalInfoStr(this string str)
        {
            return str.Tagging("align", "flush").Tagging("u");
        }
        public static string GetOpeningTag(string tagStr, string tagValue = null)
        {
            return $"<{tagStr}{(tagValue is not null ? @$"=""{tagValue}""" : "")}>";
        }
        public static string GetClosingTag(string tagStr)
        {
            return $"</{tagStr}>";
        }
    }
}