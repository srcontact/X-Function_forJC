using clrev01.Bases;
using System;
using Addler.Runtime.Core.LifetimeBinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.Utilities;
using Object = UnityEngine.Object;

namespace clrev01.Extensions
{
    public abstract class AssetReferenceSetBase<AR, T> where AR : AssetReference where T : class
    {
        [SerializeField]
        protected AR assetReference;
        [NonSerialized]
        private AsyncOperationHandle<T> _operationHandle;

        public T GetAsset(GameObject bindTgt = null)
        {
            if (_operationHandle.IsValid()) return _operationHandle.Result;
            if (assetReference.AssetGUID.IsNullOrWhitespace()) return default;
            if (assetReference.IsValid()) assetReference.ReleaseAsset();
            _operationHandle = LoadAssetAsyncExe(bindTgt);
            return _operationHandle.WaitForCompletion();
        }

        public bool IsNotSetAsset()
        {
            return assetReference.AssetGUID.IsNullOrWhitespace();
        }

        protected abstract AsyncOperationHandle<T> LoadAssetAsyncExe(GameObject bindTgt);

        public abstract void SetAssetReferenceWithGuid(string guid);
    }

    [System.Serializable]
    public class AssetReferenceSet<T> : AssetReferenceSetBase<AssetReference, T> where T : class
    {
#if UNITY_EDITOR
        [ShowInInspector, AssetSelector(Paths = "Assets/DevFiles/Addressables")]
        protected T assetSelector
        {
            get => assetReference.editorAsset as T;
            set => assetReference.SetEditorAsset(value as Object);
        }
#endif
        protected override AsyncOperationHandle<T> LoadAssetAsyncExe(GameObject bindTgt)
        {
            bindTgt ??= StaticInfo.Inst.gameObject;
            return assetReference.LoadAssetAsync<T>().BindTo(bindTgt);
        }

        public override void SetAssetReferenceWithGuid(string guid)
        {
            assetReference ??= new AssetReference(guid);
        }
    }

    [System.Serializable]
    public class ComponentReferenceSet<T> : AssetReferenceSetBase<ComponentReference<T>, T> where T : PoolableBehaviour
    {
        private static GameObject _commonBindTgtObj;
        [NonSerialized]
        private ObjectPool _objectPool;

        public T GetInstanceUsePool(GameObject bindTgt, out bool getFromPool)
        {
            if (_objectPool == null || _objectPool.gameObject == null)
            {
                _objectPool = ObjectPool.GetObjectPool(assetReference.AssetGUID);
                _objectPool.SetOrigObject(GetAsset(bindTgt));
            }
            return (T)_objectPool.GetPooledObject(out getFromPool);
        }

        public T GetInstanceUsePool(out bool getFromPool)
        {
            if (!_commonBindTgtObj)
            {
                _commonBindTgtObj = new GameObject($"CommonBindTgtObj_{typeof(T).Name}");
            }
            return GetInstanceUsePool(_commonBindTgtObj, out getFromPool);
        }

        public void StandbyPoolObjects(GameObject bindTgt, int standbyNum)
        {
            if (_objectPool == null || _objectPool.gameObject == null)
            {
                _objectPool = ObjectPool.GetObjectPool(assetReference.AssetGUID);
                _objectPool.SetOrigObject(GetAsset(bindTgt));
            }
            for (int i = 0; i < standbyNum; i++)
            {
                _objectPool.StandbyObject();
            }
        }

        protected override AsyncOperationHandle<T> LoadAssetAsyncExe(GameObject bindTgt)
        {
            bindTgt ??= StaticInfo.Inst.gameObject;
            return assetReference.LoadAssetAsync().BindTo(bindTgt);
        }

        public override void SetAssetReferenceWithGuid(string guid)
        {
            assetReference = new ComponentReference<T>(guid);
        }
    }

//以下、https://github.com/Unity-Technologies/Addressables-Sample より引用
    /// <summary>
    /// Creates an AssetReference that is restricted to having a specific Component.
    /// * This is the class that inherits from AssetReference.  It is generic and does not specify which Components it might care about.  A concrete child of this class is required for serialization to work.* At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At runtime it can load/instantiate the GameObject, then return the desired component.  API matches base class (LoadAssetAsync & InstantiateAsync).
    /// </summary>
    /// <typeparam name="TComponent"> The component type.</typeparam>
    [System.Serializable]
    public class ComponentReference<TComponent> : AssetReference
    {
        public ComponentReference(string guid) : base(guid)
        { }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(position, Quaternion.identity, parent), GameObjectReady);
        }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(parent, instantiateInWorldSpace), GameObjectReady);
        }
        public AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.LoadAssetAsync<GameObject>(), GameObjectReady);
        }

        AsyncOperationHandle<TComponent> GameObjectReady(AsyncOperationHandle<GameObject> arg)
        {
            var comp = arg.Result.GetComponent<TComponent>();
            return Addressables.ResourceManager.CreateCompletedOperation<TComponent>(comp, string.Empty);
        }

        public override bool ValidateAsset(Object obj)
        {
            var go = obj as GameObject;
            return go != null && go.GetComponent<TComponent>() != null;
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            //this load can be expensive...
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
        }

        public void ReleaseInstance(AsyncOperationHandle<TComponent> op)
        {
            // Release the instance
            var component = op.Result as Component;
            if (component != null)
            {
                Addressables.ReleaseInstance(component.gameObject);
            }

            // Release the handle
            Addressables.Release(op);
        }
    }
}