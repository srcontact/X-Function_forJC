using System;

namespace clrev01.Save
{
    [Flags]
    public enum DetectObjectFlags : long
    {
        Ground = 1 << 0,
        Friend = 1 << 1,
    }
}