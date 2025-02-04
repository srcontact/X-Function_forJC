using System;

namespace clrev01.ClAction.ObjectSearch
{
    [Flags]
    public enum SearchTgtType : long
    {
        Bullet = 1 << 0,
        Machine = 1 << 1,
        Missile = 1 << 2,
        Mine = 1 << 3,
        AerialSmallObject = 1 << 4,
        Shield = 1 << 16,
    }
}