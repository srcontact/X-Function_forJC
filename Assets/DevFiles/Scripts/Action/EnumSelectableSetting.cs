using clrev01.Bases;
using System;
using System.Collections.Generic;
using static clrev01.Bases.UtlOfEdit;

namespace clrev01.ClAction
{
    public abstract class EnumSelectableSetting<E, NB> : EnumSelectableSettingBase where E : struct where NB : NamedBool
    {
        public override Type enumType => typeof(E);
        public List<NB> enumBoolSets = new List<NB>();
    }
}