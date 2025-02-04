using clrev01.ClAction;
using clrev01.ClAction.ObjectSearch;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Bases
{
    public abstract class HardBase : PoolableBehaviour, IRunner, IHaveHitCollider
    {
        private static int _nextUniqueID = 0;
        public int uniqueID { get; set; }
        public int firingId { get; set; } = -1;
        public int teamID = -1;
        public int machineIdInTeam = -1;

        #region rigidBody

        private bool _hasRigidbody = true;
        private Rigidbody _rigidBody;
        public Rigidbody rigidBody
        {
            get
            {
                if (_hasRigidbody && _rigidBody == null)
                {
                    _hasRigidbody = TryGetComponent(out _rigidBody);
                }
                return _rigidBody;
            }
        }

        #endregion

        public List<Collider> colliderList = new();
        public ObjectSearchTgt objectSearchTgt;

        public static void ResetUniqueId()
        {
            _nextUniqueID = 0;
        }
        public static int GetNextUniqueId() => _nextUniqueID;
        public void SetUniqueID()
        {
            uniqueID = _nextUniqueID;
            _nextUniqueID++;
        }

        public virtual void Awake()
        {
            SetUniqueID();
            RegisterHitColliders();
        }

        public void RegisterHitColliders()
        {
            foreach (var t in colliderList)
            {
                var iid = t.GetInstanceID();
                if (ACM.colliderDict.ContainsKey(iid)) continue;
                ACM.colliderDict.Add(iid, this);
            }
        }

        public virtual void DestroyThis()
        {
            Destroy(this);
        }

        public void AddDamage(PowerPar bulletPow, float baseSpeed, Vector3 speed, Vector3 impactPos, int maxFrame, int exeFrame)
        {
            var power = bulletPow.GetPower(baseSpeed, speed.magnitude, maxFrame, exeFrame);
            AddDamage(power.penetrationPower, power.impactPower, power.heatPower, impactPos, speed);
        }

        public virtual void AddDamage(int penetrationDamage, int impactDamage, int heatDamage, Vector3 impactPoint, Vector3 impactVelocity)
        { }

        public virtual void OnValidate()
        {
            colliderList.Clear();
            colliderList.AddRange(GetComponentsInChildren<Collider>());
            ObjectSearchTgt ost;
            if (TryGetComponent(out ost)) objectSearchTgt = ost;
        }
        public virtual void ResetEveryFrame()
        { }
        public virtual void RunInitialFrame()
        { }
        public abstract void RunBeforePhysics();
        public abstract void RunAfterPhysics();
        public virtual void RunOnUpdate()
        { }
        public virtual void RunOnAfterFixedUpdateAndAnimation()
        { }

        public abstract void OnDotonExe();

        public void OnDotonResetPosition()
        {
            if (
                !Physics.Raycast(new Vector3(pos.x, 10000, pos.z), Vector3.down, out var raycastHit, 20000, layerOfGround) &&
                !Physics.Raycast(new Vector3(0, 10000, 0), Vector3.down, out raycastHit, 20000, layerOfGround)
            ) return;
            var bounds = new Bounds();
            colliderList.ForEach(x => bounds.Encapsulate(x.bounds));
            pos = raycastHit.point + Vector3.up * bounds.extents.z;
            rigidBody.linearVelocity = Vector3.zero;
        }
    }
}