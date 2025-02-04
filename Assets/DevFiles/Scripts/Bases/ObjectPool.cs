using clrev01.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Bases
{
    public class ObjectPool : BaseOfCL
    {
        private static readonly Dictionary<int, ObjectPool> objectPools = new();
        private readonly Stack<PoolableBehaviour> _poolStack = new();
        private int _guidHash;
        private PoolableBehaviour _origObj;

        public static ObjectPool GetObjectPool(string guid)
        {
            if (objectPools.TryGetValue(guid.GetHashCode(), out var objectPool) &&
                objectPool != null &&
                objectPool.gameObject != null)
            {
                return objectPool;
            }
            objectPool = new GameObject($"Pool_{guid}").AddComponent<ObjectPool>();
            objectPool._guidHash = guid.GetHashCode();
            objectPools[guid.GetHashCode()] = objectPool;
            return objectPool;
        }

        public void SetOrigObject(PoolableBehaviour orig)
        {
            _origObj = orig;
        }

        public PoolableBehaviour GetPooledObject(out bool getFromPool)
        {
            while (_poolStack.TryPop(out var obj) && obj != null && obj.gameObject != null)
            {
                getFromPool = true;
                return obj;
            }
#if UNITY_EDITOR
            // Debug.Log($"{_origObj.gameObject.name}_プールにオブジェクトがないので新規作成。");
#endif
            var no = _origObj.SafeInstantiate();
            no.objectPool = this;
            getFromPool = false;
            return no;
        }

        public void StandbyObject()
        {
            var no = _origObj.SafeInstantiate();
            no.gameObject.SetActive(false);
            no.objectPool = this;
            PoolingObject(no);
        }

        public void PoolingObject(PoolableBehaviour obj)
        {
            if (!gameObject || !obj || !obj.gameObject)
            {
                _poolStack.Clear();
                objectPools[_guidHash] = null;
                _origObj = null;
                return;
            }
            obj.transform.SetParent(transform, false);
            _poolStack.Push(obj);
        }

        private void OnDestroy()
        {
            _poolStack.Clear();
            objectPools[_guidHash] = null;
            _origObj = null;
        }
    }
}