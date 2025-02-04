using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.ObjectSearch;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.ClAction.Shield
{
    public class ShieldHD : SlaveObjectHD<ShieldCD, ShieldLD, ShieldHD>
    {
        private IShieldUser _shieldUser;
        private ShieldCD shieldCd => _shieldUser.shieldCd;
        public Rigidbody shieldRigidbody;
        public Collider shieldCollider;
        public bool shieldActive => gameObject.activeSelf;
        protected override bool stillAlive => false;
        private float _shieldSize;
        public MeshRenderer meshRenderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private readonly int _emissionID = Shader.PropertyToID("_EmissionColor");
        private int _latestHitFrame = -1;

        private bool _active;
        private float radius { get; set; }
        private Vector3 _position;
        private bool _isLocal;
        private IEnumerable<Collider> _ignoreColliers;
        private Transform _userTransform;


        public void InitializeShield(IShieldUser shieldUser, IEnumerable<Collider> userColliers, int teamId, Transform userTransform)
        {
            ShieldSetting(false, 0, Vector3.zero, false);
            UpdateShield(Vector3.zero);
            RegisterHitColliders();
            _shieldUser = shieldUser;
            _userTransform = userTransform;
            _ignoreColliers = userColliers;
            foreach (var userCollier in _ignoreColliers)
            {
                Physics.IgnoreCollision(shieldCollider, userCollier);
            }
            teamID = teamId;
            shieldCd.onHitVfx.StandbyPoolActors(20);
        }

        public void ShieldSetting(bool active, float radius, Vector3 position, bool isLocal)
        {
            _active = active;
            this.radius = radius;
            _position = position;
            _isLocal = isLocal;
        }

        public void UpdateShield(Vector3 bodyVelocity)
        {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            if (_active != meshRenderer.enabled)
            {
                meshRenderer.enabled = _active;
                shieldCollider.enabled = _active;
            }
            if (!_active)
            {
                objectSearchTgt.ignoreRegisterFlag = true;
                return;
            }
            objectSearchTgt.ignoreRegisterFlag = false;
            _shieldSize = radius;
            scl = Vector3.one * radius;
            var position = _position;
            if (_isLocal) position += bodyVelocity / 60;
            shieldRigidbody.position = position;
        }

        public void UpdateColor()
        {
            if (shieldCd == null) return;
            var onHitColorRatio = shieldCd.onHitColorRatio.Evaluate(ACM.actionFrame - _latestHitFrame);
            var shieldEmissionColor = shieldCd.onHitColor * onHitColorRatio + shieldCd.normalColor * (1f - onHitColorRatio);
            EmissionColorChange(shieldEmissionColor);
        }

        private void EmissionColorChange(Color shieldEmissionColor)
        {
            _materialPropertyBlock ??= new MaterialPropertyBlock();
            _materialPropertyBlock.SetColor(_emissionID, shieldEmissionColor);
            meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private readonly ObjectSearchTgt[] _resultList = new ObjectSearchTgt[48];
        public void HitDetectInside()
        {
            ObjectSearch.ObjectSearch.Inst.SearchFieldGet(
                v => CheckInField(v),
                GetBounds(),
                transform,
                teamID,
                (IdentificationType)int.MaxValue,
                ObjType.Bullet | ObjType.Missile,
                LockOnDistancePriorityType.Near,
                LockOnAngleOfMovementToSelfType.None,
                0,
                _resultList,
                null
            );
            foreach (var ost in _resultList)
            {
                if (ost == null) break;
                var hd = ost.hardBase as BulletHD;
                if (hd == null) continue;
                hd.OnHit(shieldCollider, hd.pos, Vector3.Normalize(hd.pos - pos), HitType.DirectHit);
            }
            for (int i = 0; i < _resultList.Length; i++)
            {
                _resultList[i] = null;
            }
        }

        private Bounds GetBounds()
        {
            return new Bounds(_position, Vector3.one * (radius * 2));
        }

        public bool CheckInField(Vector3 tgtPos)
        {
            return Vector3.Distance(tgtPos, _position) <= radius;
        }

        public override void AddDamage(int penetrationDamage, int impactDamage, int heatDamage, Vector3 impactPoint, Vector3 impactVelocity)
        {
            base.AddDamage(penetrationDamage, impactDamage, heatDamage, impactPoint, impactVelocity);
            _latestHitFrame = ACM.actionFrame;
            EmissionColorChange(shieldCd.onHitColor);
            if (impactVelocity.magnitude < Vector3.kEpsilon) impactVelocity = Vector3.Normalize(pos - _userTransform.position);
            if (impactVelocity.magnitude < Vector3.kEpsilon) return;
            var hitVfx = shieldCd.onHitVfx.InstActor(impactPoint, Quaternion.LookRotation(impactVelocity));
            var damage = penetrationDamage + impactDamage + heatDamage;
            hitVfx.scl = Vector3.one * Mathf.Max(damage / 200, 1);
            _shieldUser.AddShieldDamage(penetrationDamage + impactDamage + heatDamage, impactDamage * impactVelocity.normalized, _shieldSize);
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            var hd = ACM.GetHardFromCollider(collisionInfo.collider);
            if (hd == null) return;
            int layer = 1 << collisionInfo.gameObject.layer;
            if (layer == layerOfShield)
            {
                var opponentShieldHd = hd as ShieldHD;
                if (opponentShieldHd == null) return;
                var radiusRatio = opponentShieldHd.radius / radius;
                opponentShieldHd.AddDamage(0, (int)(shieldCd.onShieldToShieldPenetrateDamage * radiusRatio * radiusRatio), 0, pos, collisionInfo.impulse);
            }
            else
            {
                hd.AddDamage(0, shieldCd.onPenetrateDamageToOpponent, 0, pos, collisionInfo.impulse);
                AddDamage(0, shieldCd.onPenetrateDamageToSelf, 0, pos, collisionInfo.impulse);
            }
        }

        public override void Disable()
        {
            base.Disable();
            foreach (var userCollier in _ignoreColliers)
            {
                Physics.IgnoreCollision(shieldCollider, userCollier, false);
            }
        }
    }
}