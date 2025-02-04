namespace clrev01.HUB
{
    public class HubDataCache<Ht, Dt> where Ht : HubBase<Dt> where Dt : HubData
    {
        private int? _currentCode;
        private Dt _cacheData;

        public Dt GetValue(Ht hub, int code)
        {
            if (_currentCode != code)
            {
                _cacheData = hub.datas[0];
                foreach (var data in hub.datas)
                {
                    if (data.Code != code) continue;
                    _cacheData = data;
                    break;
                }
                _currentCode = code;
            }
            return _cacheData;
        }
    }
}