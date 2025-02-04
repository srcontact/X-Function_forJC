using clrev01.Bases;
using System.Collections.Generic;

namespace clrev01.HUB
{
    public class HubBase<T> : SOBaseOfCL where T : HubData
    {
        public List<T> datas = new();

        public T GetData(int code)
        {
            foreach (var data in datas)
            {
                if (data.Code == code)
                {
                    return data;
                }
            }
            return null;
        }
        public int GetDataIndex(int code)
        {
            for (var i = 0; i < datas.Count; i++)
            {
                var data = datas[i];
                if (data.Code == code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}