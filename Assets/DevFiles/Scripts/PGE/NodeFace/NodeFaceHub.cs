using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace clrev01.PGE.NodeFace
{
    [CreateAssetMenu(menuName = "Hub/NodeFaceHub", order = 0)]
    public class NodeFaceHub : SOBaseOfCL
    {
        public NodeFaceData defaultNodeFace;
        public List<NodeFaceData> nodeFaceDataList = new List<NodeFaceData>();

        [Serializable]
        public class NodeFaceData
        {
            [ReadOnly]
            public int key = -1;
            [ReadOnly]
            public string type;
            public ComponentReferenceSet<NodeFaceBasic> nodeFace;
            public List<AssetReferenceSet<Sprite>> icons;

            public NodeFaceBasic GetNodeFace(GameObject bindTgt)
            {
                var nf = nodeFace.GetInstanceUsePool(out _);
                if (nf.key != key)
                {
                    nf.key = key;
                    nf.SetIcons(icons);
                }
                return nf;
            }
        }

        public NodeFaceData GetNodeFaceData(Type type)
        {
            var nodeFaceData = nodeFaceDataList.Find(x => x.type == type.ToString());
            return nodeFaceData ?? defaultNodeFace;
        }

        public int GetNodeKey(Type type)
        {
            var nodeFaceData = nodeFaceDataList.Find(x => x.type == type.ToString());
            return nodeFaceData?.key ?? -1;
        }

        private void OnValidate()
        {
            var pgbFuncUnions = ((MemoryPackUnionAttribute[])Attribute.GetCustomAttributes(typeof(IPGBFuncUnion), typeof(MemoryPackUnionAttribute))).ToList();
            if (pgbFuncUnions.Count <= 0) return;

            foreach (var union in pgbFuncUnions)
            {
                var finds = nodeFaceDataList.FindAll(x => x != null && x.key == union.Tag);
                if (finds.Count > 1)
                {
                    for (int i = finds.Count - 1; i >= 1; i--)
                    {
                        nodeFaceDataList.RemoveAt(nodeFaceDataList.FindLastIndex(x => ReferenceEquals(x, finds[i])));
                    }
                }
                else if (finds.Count == 0)
                {
                    nodeFaceDataList.Add(new NodeFaceData() { key = union.Tag, type = union.Type.ToString() });
                }
            }
            nodeFaceDataList = nodeFaceDataList.Where(x => x != null && pgbFuncUnions.Any(y => y.Type.ToString() == x.type)).OrderBy(x => x.key).ToList();
        }
    }
}