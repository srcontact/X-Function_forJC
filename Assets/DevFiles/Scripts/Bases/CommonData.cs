using clrev01.ClAction;
using clrev01.Extensions;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using I2.Loc;
using System.Text;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Bases
{
    public class CommonData<C, L, H> : CommonDataBase, IInfoTextData
        where C : CommonData<C, L, H>
        where L : LocalData<C, L, H>, new()
        where H : Hard<C, L, H>
    {
        [field: SerializeField]
        public LocalizedString description { get; set; }

        #region origHD

        [SerializeField]
        private ComponentReferenceSet<H> origHdReference;
        public H origHD => origHdReference.GetAsset(ACM != null ? ACM.gameObject : StaticInfo.Inst.gameObject);

        #endregion

        protected virtual string parentName => "Others";

        #region parentObj

        private GameObject _parentObj;
        public GameObject parentObj
        {
            get
            {
                if (_parentObj != null) return _parentObj;
                _parentObj = GameObject.Find(parentName);
                if (_parentObj == null)
                {
                    _parentObj = new GameObject
                    {
                        name = parentName,
                        transform =
                        {
                            parent = ACM.transform
                        }
                    };
                }
                return _parentObj;
            }
        }

        #endregion

        protected virtual bool reuseLd => false;

        public virtual H InstActor(Vector3 position, Quaternion rotation)
        {
            H nh = origHdReference.GetInstanceUsePool(out var getFromPool);
            nh.gameObject.SetActive(true);
            if (getFromPool && nh.ld != null && reuseLd) ResetLd(nh.ld);
            else CreateLd(nh);
            if (ACM.doParentSet) nh.transform.SetParent(parentObj.transform);
            ACM.AddRun(nh);
            nh.transform.SetPositionAndRotation(position, rotation);
            if (nh.objectSearchTgt != null)
            {
                nh.objectSearchTgt.InitMovementVector(rotation * Vector3.forward, position);
            }
            return nh;
        }

        public override HardBase InstActorH(Vector3 position, Quaternion rotation)
        {
            return InstActor(position, rotation);
        }

        public override void StandbyPoolActors(int standbyNum)
        {
            origHdReference.StandbyPoolObjects(ACM.gameObject, standbyNum);
        }

        private void CreateLd(H nh)
        {
            nh.ld = new L();
            ResetLd(nh.ld);
        }

        protected virtual void ResetLd(L ld)
        {
            ld.SetUniqueID();
            ld.cd = (C)this;
            ld.spawnFrame = ActionManager.Inst.actionFrame;
        }
        public virtual void GetParameterText(ref Utf8ValueStringBuilder sb)
        { }
    }
}