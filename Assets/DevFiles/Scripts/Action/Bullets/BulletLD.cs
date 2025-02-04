using clrev01.Bases;
using clrev01.ClAction.Bullets.DevFiles.Scripts.Action.Bullets;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Extensions.ExMissileGuidance;

namespace clrev01.ClAction.Bullets
{
    [System.Serializable]
    public class BulletLD : LocalData<BulletCD, BulletLD, BulletHD>, IProjectileLocalData
    {
        public int shooterId { get; set; } = -1;
        public ObjectSearchTgt target { get; set; }
        [SerializeField]
        public bool isHit { get; set; }
        [SerializeField]
        public Vector3 accele { get; set; }
        [SerializeField]
        public Vector3 jumpV { get; set; }
        private Vector3 _losLog, _desiredRotation;
        private Vector3 _previousProNavVector;
        public int damage;
        public float HpPercent => HpRemaining / cd.BulletHealthPoint;
        public float HpRemaining => Mathf.Max(cd.BulletHealthPoint - damage, 0);

        public override void ResetEveryFrame()
        {
            base.ResetEveryFrame();
            accele = Vector3.zero;
            jumpV = Vector3.zero;
            hd.objectSearchTgt.jammingSize = 0;
            hd.objectSearchTgt.jammedSize = 0;
        }
        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            if (isHit || ExeFrameCount > cd.LifeFrame)
            {
                hd.Disable();
            }
            if (target != null && !target.gameObject.activeSelf) target = null;
            Move();
        }

        public void OnHit(IHaveHitCollider hitHard, Vector3 hitPos, Vector3 hitPointNormal, HitType hitType, Vector3 hitEffectVector)
        {
            isHit = true;
            foreach (var spawnOnHitInfo in cd.SpawnOnHitInfos)
            {
                spawnOnHitInfo.SpawnExe(hitPos, hitEffectVector, hitPointNormal, hitHard is HardBase @base ? @base.objectSearchTgt : null, hd.objectSearchTgt, hitType);
            }
            if (hitHard != null && hitType is HitType.DirectHit) hitHard.AddDamage(cd.bulletPow, cd.InitialSpeed, hd.rBody.linearVelocity, hitPos, cd.LifeFrame, ExeFrameCount);
        }

        public void Move()
        {
            switch (cd.BulletType)
            {
                case BulletType.Straight:
                    StraightMove();
                    break;
                case BulletType.Guided:
                    GuidedMove();
                    break;
            }
        }
        private void StraightMove()
        {
            AirBreakCalc();
            if (cd.moveCommonPar.useGraviry) GravityCalc();
            MoveExe();
            RotateExe();
        }
        private void GuidedMove()
        {
            AirBreakCalc();
            if (cd.AcceleEndFrame >= ExeFrameCount)
            {
                SideAirBreak();
            }
            if (cd.moveCommonPar.useGraviry) GravityCalc();
            GuidedRotateCalc();
            GuidedAcceleCalc();
            MoveExe();
            SpeedLimit();
        }
        public void AirBreakCalc()
        {
            accele -= hd.rBody.linearVelocity * (cd.moveCommonPar.nvreak * ACM.actionEnvPar.globalAirBreakPar);
        }
        private void SideAirBreak()
        {
            var sideRatio = 1 - Vector3.Dot(hd.rBody.linearVelocity.normalized, hd.transform.forward);
            var breakPow = hd.rBody.linearVelocity * (sideRatio * cd.GuidedSideNvreak * ACM.actionEnvPar.globalAirBreakPar);
            accele -= breakPow;
        }

        public void GravityCalc()
        {
            accele += new Vector3(0, -ACM.actionEnvPar.globalGPowMSec, 0);
        }

        public void GuidedRotateCalc()
        {
            if (target == null) return;
            var position = target.GetTargetPosition(hd.teamID, hd.uniqueID, cd.guidedSearchParameterData, target.GetPosAccuracy(hd.pos, cd.guidedSearchParameterData, hd.objectSearchTgt.jammedSize));
            var distance = Vector3.Distance(position, hd.pos);
            var tgtPosition = position + cd.GetGuidedHeightOffset(distance) * Vector3.up;
            var proNavRatio = cd.GetProNavRatio(distance);
            var proNavTgtVector = Vector3.zero;
            if (proNavRatio < 1)
            {
                proNavTgtVector = GetProNavDirection(
                    tgtPosition,
                    hd.rBody,
                    cd.NavigationalConstant,
                    1f / 60,
                    ref _losLog,
                    _previousProNavVector,
                    cd.AdvanceMode
                );
                _previousProNavVector = proNavTgtVector;
            }
            var tailChaseTgtVector = Vector3.zero;
            if (proNavRatio > 0)
            {
                tailChaseTgtVector = GetTailChaseDirection(tgtPosition, hd.pos, cd.GetAccele(ExeFrameCount), ACM.actionEnvPar.globalGPowMSec);
            }
            var tgtVector = Vector3.Lerp(proNavTgtVector, tailChaseTgtVector, proNavRatio);

            var velocity = hd.rBody.linearVelocity;
            var vectorDif = tgtVector * cd.GuidedMaxSpeed - velocity;
            var rotateVector = vectorDif + velocity;

            var resultRot = Quaternion.RotateTowards(hd.rBody.rotation, Quaternion.LookRotation(rotateVector), cd.GetRotateRatio(hd.rBody.linearVelocity.magnitude));
            hd.rBody.MoveRotation(resultRot);
        }

        public void GuidedAcceleCalc()
        {
            accele += cd.GetAccele(ExeFrameCount) * hd.rBody.transform.forward;
        }

        public void MoveExe()
        {
            hd.rBody.linearVelocity += jumpV + accele / 60;
        }
        private void SpeedLimit()
        {
            if (hd.rBody.linearVelocity.sqrMagnitude > cd.GuidedMaxSpeed * cd.GuidedMaxSpeed)
            {
                hd.rBody.linearVelocity = hd.rBody.linearVelocity.normalized * cd.GuidedMaxSpeed;
            }
        }
        public void RotateExe()
        {
            hd.rBody.MoveRotation(Quaternion.LookRotation(hd.rBody.linearVelocity));
        }
    }
}