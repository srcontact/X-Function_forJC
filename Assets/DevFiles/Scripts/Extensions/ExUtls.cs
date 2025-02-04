using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using MemoryPack;
using UnityEngine.AddressableAssets;

namespace clrev01.Extensions
{
    public static class ExUtls
    {
        [System.Serializable]
        public abstract class NestedClassBase
        {
            public P Clone<P>()
            {
                return (P)MemberwiseClone();
            }
        }

        public static T CloneDeep<T>(this T target)
        {
            var type = target.GetType();
            var bytes = MemoryPackSerializer.Serialize(type, target);
            return (T)MemoryPackSerializer.Deserialize(type, bytes);
        }

        public static OBJ SafeInstantiate<OBJ>(this OBJ obj, string tailWord = null) where OBJ : Object
        {
            if (obj == null) return null;
            var spawned = Object.Instantiate(obj);
            spawned.name = spawned.name.Replace("(Clone)", tailWord);
            return spawned;
        }
        public static GameObject PrefabSpawn(string name)
        {
            Debug.Log("PrefabSpwn");
            GameObject spawned = SafeInstantiate(Addressables.LoadAssetAsync<GameObject>("Objects/" + name).WaitForCompletion());
            if (spawned == null)
            {
                Debug.LogError("Prefab[" + name + "]isNotFound");
                return null;
            }
            return spawned;
        }

        public static Vector3 ClampRotate(Transform current, Transform tgt, float maxSpeed)
        {
            return ClampRotate(current, tgt.position, maxSpeed);
        }
        public static Vector3 ClampRotate(Transform current, Vector3 tgt, float maxSpeed)
        {
            Vector3 v = Quaternion.LookRotation(tgt - current.position).eulerAngles;
            v -= current.rotation.eulerAngles;

            EulerBridge(ref v.x);
            EulerBridge(ref v.y);
            EulerBridge(ref v.z);
            v = Vector3.ClampMagnitude(v, maxSpeed);
            return v;
        }
        public static void EulerBridge(ref float p)
        {
            if (Mathf.Abs(p).isOver(180)) return;
            p = -(p - 180);
        }

        #region Pyhsics
        [System.Serializable]
        public class PhysicsPar
        {
            #region speedV
            [SerializeField]
            private Vector3 _speedV;
            public Vector3 speedV
            {
                get { return _speedV; }
                set { _speedV = value; }
            }
            #endregion
            public Vector3 acceleV;

            public void AddAccele(Vector3 v)
            {
                acceleV += v;
            }
            public void AddG(float g)
            {
                acceleV.y -= g;
            }
            public void AcceleRegist()
            {
                speedV += acceleV / 60;
                acceleV = Vector3.zero;
            }
        }

        [System.Serializable]
        public class GravityPar
        {
            #region nowGS
            [SerializeField]
            private float _nowGS;
            public float nowGS
            {
                get { return _nowGS; }
                set { _nowGS = value; }
            }
            #endregion
            #region gPow
            [SerializeField]
            private float _gPow = -1;
            public float gPow
            {
                get
                {
                    if (_gPow < 0) return ACM.actionEnvPar.globalGPowMSec;
                    return _gPow;
                }
            }
            #endregion

            public void Progress(bool isGrounded)
            {
                if (isGrounded) nowGS = 0;
                else nowGS += gPow / 60;
            }
        }

        [System.Serializable]
        public class SpeedStatePar
        {
            #region controllableSpeed
            [SerializeField]
            private float _controllableSpeed;
            /// <summary>
            /// 制御可能スピード
            /// </summary>
            public float controllableSpeed
            {
                get { return _controllableSpeed; }
            }
            #endregion
            #region immediateStationarySpeed
            [SerializeField]
            private float _immediateStationarySpeed;
            /// <summary>
            /// 即静止可能なスピード
            /// </summary>
            public float immediateStationarySpeed
            {
                get { return _immediateStationarySpeed; }
            }
            #endregion

            public void Progress()
            { }
        }
        #endregion

#if UNITY_EDITOR
        public static void SafeDebugLog(object obj)
        {
            Debug.Log(obj);
        }
        public static void SafeDebugLogError(object obj)
        {
            Debug.LogError(obj);
        }

        public static void SafeDestroyOnEditor(UnityEngine.Object obj)
        {
            UnityEditor.EditorApplication.delayCall += () => UnityEngine.Object.DestroyImmediate(obj, true);
        }
#endif

        public static bool isBigger(this float this_, float tgt, float tolerance = float.Epsilon)
        {
            return (this_ - tgt) > tolerance;
        }
        public static bool isOver(this float this_, float tgt, float tolerance = float.Epsilon)
        {
            return !((this_ - tgt) < tolerance);
        }
        public static bool isZero(this float this_, float tolerance = float.Epsilon)
        {
            return Mathf.Abs(this_) < tolerance;
        }

        public static bool isBiggerV3(this float this_, float tgt)
        {
            return this_.isBigger(tgt, Vector3.kEpsilon);
        }
        public static bool isOverV3(this float this_, float tgt)
        {
            return this_.isOver(tgt, Vector3.kEpsilon);
        }
        public static bool isZeroV3(this float this_)
        {
            return this_.isZero(Vector3.kEpsilon);
        }

        public static bool isZeroV3(this Vector3 this_)
        {
            return
                this_.x.isZero(Vector3.kEpsilon) &&
                this_.y.isZero(Vector3.kEpsilon) &&
                this_.z.isZero(Vector3.kEpsilon);
        }

        public static bool isBiggerQ(this float this_, float tgt)
        {
            return this_.isBigger(tgt, Quaternion.kEpsilon);
        }
        public static bool isOverQ(this float this_, float tgt)
        {
            return this_.isOver(tgt, Quaternion.kEpsilon);
        }
        public static bool isZeroQ(this float this_)
        {
            return this_.isZero(Quaternion.kEpsilon);
        }
    }
}