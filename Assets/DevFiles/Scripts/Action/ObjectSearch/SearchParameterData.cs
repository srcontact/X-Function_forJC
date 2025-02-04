using System;

namespace clrev01.ClAction.ObjectSearch
{
    [Serializable]
    public class SearchParameterData
    {
        public float visibleAccuracy = 0;
        public float concealedAccuracy = 5;
        public int visibleUpdateFrequency = 1;
        public int concealedUpdateFrequency = 10;
        public float antiJammingRate;
    }
}