using clrev01.Bases;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace clrev01.ClAction.ObjectSearch
{
    public partial class ObjectSearch : SingletonOfCL<ObjectSearch>
    {
        [SerializeField]
        private float searchSpaceSize;
        [SerializeField]
        private int divisionNum = 6;
        [SerializeField, ReadOnly]
        private float unitLength;
        [SerializeField, ReadOnly]
        private int indexNum;
        private Dictionary<SearchTgtType, FixedList<ObjectSearchTgt>[]> _searchListDict;
        private readonly SearchTgtType[] _searchTgtTypes = (SearchTgtType[])Enum.GetValues(typeof(SearchTgtType));

        public class FixedList<T> where T : class
        {
            List<T> list = new List<T>();
            Stack<int> nullIndexStack = new Stack<int>();

            public int count => list.Count;
            public int notNullCount => list.Count - nullIndexStack.Count;
            public int Add(T add)
            {
                if (nullIndexStack.Count > 0)
                {
                    int index = nullIndexStack.Pop();
                    list[index] = add;
                    return index;
                }
                else
                {
                    list.Add(add);
                    return list.Count - 1;
                }
            }
            public void RemoveAt(int index)
            {
                list[index] = null;
                nullIndexStack.Push(index);
            }
            public T this[int index]
            {
                get => list[index];
            }
        }

        public override void Awake()
        {
            base.Awake();
            Initialize();
        }
        private void Initialize()
        {
            unitLength = searchSpaceSize / Mathf.Pow(2, divisionNum);
            indexNum = (int)(Math.Pow(2, divisionNum) * Math.Pow(2, divisionNum) * Math.Pow(2, divisionNum));
            _searchListDict = new Dictionary<SearchTgtType, FixedList<ObjectSearchTgt>[]>();
            foreach (var type in _searchTgtTypes)
            {
                _searchListDict.Add(type, new FixedList<ObjectSearchTgt>[indexNum]);
            }
        }

        public void RegisterTgt(ObjectSearchTgt tgt)
        {
            if (!tgt.transform.hasChanged) return;
            int mortonNum = (int)CalcMortonNum(tgt.pos);
            if (mortonNum < 0 || mortonNum > _searchListDict[tgt.ObjectSearchType].Length - 1) return;
            if (tgt.chunkNum == mortonNum) return;

            if (tgt.chunkNum != -1)
            {
                _searchListDict[tgt.ObjectSearchType][tgt.chunkNum].RemoveAt(tgt.indexNum);
            }
            //Debug.Log(tgt.chunkNum + ":" + mortonNum);
            _searchListDict[tgt.ObjectSearchType][mortonNum] ??= new FixedList<ObjectSearchTgt>();
            tgt.indexNum = _searchListDict[tgt.ObjectSearchType][mortonNum].Add(tgt);
            tgt.chunkNum = mortonNum;
        }
        public void UnregisterTgt(ObjectSearchTgt tgt)
        {
            if (tgt.chunkNum < 0) return;
            _searchListDict[tgt.ObjectSearchType][tgt.chunkNum].RemoveAt(tgt.indexNum);
            if (_searchListDict[tgt.ObjectSearchType][tgt.chunkNum].count <= 0) _searchListDict[tgt.ObjectSearchType][tgt.chunkNum] = null;
            tgt.chunkNum = tgt.indexNum = -1;
        }
        private uint CalcMortonNum(Vector3 tgtPos)
        {
            return CalcMortonNum(tgtPos.x, tgtPos.y, tgtPos.z);
        }
        private uint CalcMortonNum(float xf, float yf, float zf)
        {
            uint x = (uint)CalcChunkNum(xf);
            uint y = (uint)CalcChunkNum(yf);
            uint z = (uint)CalcChunkNum(zf);
            return LookUpMortonNumber(x, y, z);
        }
        private int CalcChunkNum(float f)
        {
            //ここの後ろでマイナス分のマス数を追加している。
            var res = Mathf.FloorToInt(f / unitLength) + (int)Math.Pow(2, divisionNum - 1);
            if (res >= Math.Pow(2, divisionNum)) res = (int)Math.Pow(2, divisionNum) - 1;
            if (res < 0) res = 0;
            return res;
        }

        private const int MaxValue = int.MaxValue;

        private struct Vector3u
        {
            public uint x, y, z;

            public Vector3u(uint x, uint y, uint z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        /// <summary>
        /// モートン番号を求める関数。
        /// 参考:https://gist.github.com/JLChnToZ/ec41b1b45987d0e1b40ceabc13920559
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private uint LookUpMortonNumber(uint x, uint y, uint z)
        {
            uint result = 0;
            int offset = 0;
            for (uint mask = 1; (x >= mask || y >= mask || z >= mask) && mask < MaxValue; mask <<= 1)
            {
                result |= (x & mask) << offset++ | (z & mask) << offset++ | (y & mask) << offset;
            }
            return result;
        }
        /// <summary>
        /// モートン番号からマスのXYZ座標を求める関数。
        /// 参考:https://gist.github.com/JLChnToZ/ec41b1b45987d0e1b40ceabc13920559
        /// </summary>
        /// <param name="value"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void InverseLookUpMortonNumber(uint value, out uint x, out uint y, out uint z)
        {
            x = 0;
            y = 0;
            z = 0;
            int offset = 0;
            for (uint mask = 1; value >= 1 << offset + 1 && mask < MaxValue; mask <<= 1)
            {
                x |= value >> offset++ & mask;
                z |= value >> offset++ & mask;
                y |= value >> offset & mask;
            }
        }
        private void InverseLookUpMortonNumber(uint value, out Vector3u v)
        {
            InverseLookUpMortonNumber(value, out var x, out var y, out var z);
            v = new Vector3u(x, y, z);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawSearchAABB();
            MortonOrderOmissionTest();
        }

        [SerializeField]
        private bool drawSearchAABB;
        [SerializeField]
        private List<Bounds> drawBounds = new List<Bounds>();
        private void DrawSearchAABB()
        {
            if (!drawSearchAABB) return;

            foreach (var bounds in drawBounds)
            {
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            drawBounds.Clear();
        }

        [SerializeField]
        private bool mortonOrderOmissionTest;
        [SerializeField, Button]
        private bool randomBounds;
        [SerializeField]
        private Bounds testBounds;
        [SerializeField]
        private Vector2 nowMortonNums;
        [SerializeField]
        private List<Vector2> tRangeList = new();
        private void MortonOrderOmissionTest()
        {
            if (randomBounds)
            {
                randomBounds = !randomBounds;
                testBounds = new Bounds(Random.insideUnitSphere * searchSpaceSize * 1.1f, new Vector3(Random.value, Random.value, Random.value) * searchSpaceSize * 1.1f);
            }
            if (!mortonOrderOmissionTest || _searchListDict is { Count: <= 0 }) return;
            Gizmos.DrawWireCube(transform.position, Vector3.one * searchSpaceSize);
            Gizmos.DrawCube(testBounds.center, testBounds.size);
            var rl = new List<(uint min, uint max)>();
            SettingMortonOrder(testBounds, rl);
            tRangeList = rl.ConvertAll(x => new Vector2(x.min, x.max));
            foreach (var range in tRangeList)
            {
                for (uint i = (uint)range.x; i <= range.y; i++)
                {
                    InverseLookUpMortonNumber(i, out var x, out var y, out var z);
                    var offset = (int)Math.Pow(2, divisionNum - 1);
                    var center = new Vector3(
                        (x - offset) * unitLength + unitLength / 2,
                        (y - offset) * unitLength + unitLength / 2,
                        (z - offset) * unitLength + unitLength / 2
                    );
                    Gizmos.DrawWireCube(center, Vector3.one * unitLength);
                }
            }
        }
#endif
    }
}