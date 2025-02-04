using System.Collections.Generic;

namespace clrev01.ClAction
{
    public class EnumSelectableSettingBase
    {
        public virtual System.Type enumType { get; }
        public virtual int GetCount(bool selectableOnly)
        {
            return 0;
        }
        public virtual List<bool> boolList
        {
            get;
        }
        /// <summary>
        /// ドロップダウンなどの選択番号から対応するEnumの番号を返す。
        /// </summary>
        /// <param name="selectedNum"></param>
        /// <returns></returns>
        public virtual int GetSelectedEnum(int selectedNum)
        {
            return selectedNum;
        }
        public virtual string GetSelectedEnumString(int selectedNum)
        {
            return string.Empty;
        }
        /// <summary>
        /// Enumの番号からドロップダウンの選択番号を計算する。
        /// </summary>
        /// <param name="enumNum"></param>
        /// <returns></returns>
        public virtual int GetDropdownSelectNum(int enumNum)
        {
            return 0;
        }
    }
}