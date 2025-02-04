using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace clrev01.Bases
{
    public abstract class BaseOfCL : SerializedMonoBehaviour
    {
        #region gameObject

        private GameObject _gameObject;
        public new GameObject gameObject
        {
            get
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlayingOrWillChangePlaymode) return _gameObject;
#endif
                if (!base.gameObject || !_gameObject)
                {
                    _gameObject = base.gameObject;
                }
                return _gameObject;
            }
        }

        #endregion

        #region transform

        private Transform _transform;
        public new Transform transform
        {
            get
            {
                if (_transform == null) _transform = gameObject.transform;
                return _transform;
            }
        }

        #endregion

        public Vector3 pos
        {
            get => transform.position;
            set => transform.position = value;
        }
        public Vector3 lpos
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
        public Quaternion rot
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }
        public Quaternion lrot
        {
            get => transform.localRotation;
            set => transform.localRotation = value;
        }

        public Vector3 scl
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }
    }
}