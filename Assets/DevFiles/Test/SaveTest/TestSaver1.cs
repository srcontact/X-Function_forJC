using clrev01.Save;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "セーブ関連/セーブサイズテスト")]
public class TestSaver1 : Saver<TestSaver1.TestSaveData>
{
    public int saveV = 10000;


    [System.Serializable]
    public class TestSaveData : SaveData
    {
        public List<Vector3> vList = new();
    }

    public override void DataSave()
    {
        data.vList.Clear();
        for (int i = 0; i < saveV; i++)
        {
            data.vList.Add(new Vector3(i * Random.value, i * Random.value, i * Random.value));
        }
        base.DataSave();
    }
    public override string fileExt
    {
        get
        {
            return "testD";
        }
    }
    public override string dataName
    {
        get
        {
            return "TestD";
        }
    }
}