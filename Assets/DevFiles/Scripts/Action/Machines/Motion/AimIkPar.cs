using System;
using System.Collections.Generic;
using clrev01.Bases;
using clrev01.ClAction.Machines.AdditionalTurret;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

namespace clrev01.ClAction.Machines.Motion
{
    public abstract class BaseAimIkPar
    {
        [SerializeField, ReadOnly]
        protected string name;
        public abstract AimIK useAimIk { get; }
        [SerializeField, ReadOnly]
        protected Vector3 defaultLocalPos;
        [SerializeField]
        Vector3 defaultDif = new(0, 0, 10);
        public abstract void SetInfo();

        public float lerpPar = 0.1f;

        public Vector3 wantToAimPosGlobal { get; private set; }
        public Vector3 nowAimPosGlobal { get; private set; }
        public Vector3 ikPos
        {
            get => useAimIk?.solver.IKPosition ?? Vector3.zero;
            private set => useAimIk.solver.SetIKPosition(value);
        }
        public bool aimingCompleted =>
            useAimIk &&
            nowAimPosGlobal == wantToAimPosGlobal &&
            _shooterTransform &&
            Vector3.Angle(nowAimPosGlobal - _shooterTransform.position, useAimIk.solver.transform.forward) <= useAimIk.solver.tolerance * 10;
        public bool aimingSustained =>
            useAimIk &&
            nowAimPosGlobal == wantToAimPosGlobal &&
            _shooterTransform &&
            Vector3.Angle(nowAimPosGlobal - _shooterTransform.position, useAimIk.solver.transform.forward) <= useAimIk.solver.tolerance * 100;

        /// <summary>
        /// 対応武装番号。この番号の武装の弾速などを参照する。
        /// </summary>
        [SerializeField]
        private int _correspondingWeaponNum = -1;
        public int correspondingWeaponNum => _correspondingWeaponNum;

        [SerializeField, Range(0, 360)]
        private float horizontalAimLimit = 180;
        [SerializeField, Range(-90, 90)]
        private float horizontalAimLimitOffset = 0;
        [SerializeField, Range(0, 360)]
        private float verticalAimLimit = 180;
        [SerializeField, Range(-90, 90)]
        private float verticalAimLimitOffset = 0;
        [SerializeField]
        private float poleWeight = 0;
        [SerializeField]
        private Vector3 polePositionFromShooterTransform = new Vector3(0, 1000, 0);

        public float? PosAccuracy { get; private set; }
        private Transform _shooterTransform;
        public bool nowAiming;

        public void Targeting(Vector3 tgtPosGlobal, Transform shooterTransform, Transform poleTransform, float maxRotateAnglePerFrame, Vector3 wobble, float? posAccuracy = null)
        {
            if (!useAimIk) return;
            nowAiming = true;
            PosAccuracy = posAccuracy;
            _shooterTransform = shooterTransform;
            Vector3 shooterPos = shooterTransform.position;
            var toTgtVector = shooterTransform.InverseTransformVector(tgtPosGlobal - shooterPos);
            var toTgtLength = toTgtVector.magnitude;
            var eulerAngles = Quaternion.FromToRotation(Vector3.forward, toTgtVector).eulerAngles;
            var toTgtAngle = eulerAngles.EulerAnglesNormalize180();
            toTgtAngle.x = Mathf.Clamp(toTgtAngle.x, -verticalAimLimit / 2 + verticalAimLimitOffset, verticalAimLimit / 2 + verticalAimLimitOffset);
            toTgtAngle.y = Mathf.Clamp(toTgtAngle.y, -horizontalAimLimit / 2 + horizontalAimLimitOffset, horizontalAimLimit / 2 + horizontalAimLimitOffset);
            wantToAimPosGlobal = shooterTransform.TransformPoint(Quaternion.Euler(toTgtAngle) * Vector3.forward * toTgtLength);
            var from = (nowAimPosGlobal - shooterPos).normalized;
            var to = (wantToAimPosGlobal - shooterPos).normalized;
            var fromToAngle = Vector3.Angle(from, to);
            var lengthToIkPos = (wantToAimPosGlobal - shooterPos).magnitude;
            nowAimPosGlobal = Vector3.Lerp(
                shooterPos + from * lengthToIkPos,
                shooterPos + to * lengthToIkPos,
                Mathf.Min(1, maxRotateAnglePerFrame / fromToAngle));
            ikPos = nowAimPosGlobal + wobble * lengthToIkPos;
            SetPoleTarget(poleTransform);
        }
        public void DefaultPositioning(Transform shooterTransform, Transform poleTransform, Vector3 shooterVelocity)
        {
            if (!useAimIk) return;
            nowAiming = false;
            ikPos = nowAimPosGlobal = wantToAimPosGlobal = Vector3.Lerp(nowAimPosGlobal, GetDefaultWorldPos(shooterTransform) + shooterVelocity / 60, lerpPar);
            SetPoleTarget(poleTransform);
        }

        public Vector3 GetDefaultWorldPos(Transform shooterTransform) => shooterTransform.TransformPoint(defaultLocalPos + defaultDif);

        public void SetPoleTarget(Transform poleTransform)
        {
            if (!useAimIk) return;
            if (poleTransform == null || !(poleWeight > 0)) return;
            useAimIk.solver.poleWeight = poleWeight;
            var r = Quaternion.LookRotation(poleTransform.forward, Vector3.up);
            useAimIk.solver.polePosition = r * polePositionFromShooterTransform + poleTransform.position;
        }
    }

    [System.Serializable]
    public class AimIkPar : BaseAimIkPar
    {
        [SerializeField]
        private AimIK aimIK;
        public override AimIK useAimIk => aimIK;
        public override void SetInfo()
        {
            if (!useAimIk) return;
            defaultLocalPos = aimIK.transform.InverseTransformPoint(aimIK.solver.bones[^1].transform.position);
            name = aimIK.solver.transform.gameObject.name;
        }
    }


    [System.Serializable]
    public class AdditionalTurretAimIkPar : BaseAimIkPar
    {
        [ReadOnly]
        public AdditionalTurretObj additionalTurretObj;
        public List<Transform> additionalTurretIkTransforms = new();
        public Transform additionalTurretSetTransform;
        public Vector3 additionalTurretScale = new(1, 1, 1);
        public Vector3 additionalTurretRotation;
        public override AimIK useAimIk => additionalTurretObj?.aimIK;

        public override void SetInfo()
        {
            name = correspondingWeaponNum < 0 ? $"NotSetCorrespondingWeaponNum" : $"Weapon{correspondingWeaponNum:00}";
        }

        public void SetDefaultLocalPos()
        {
            if (!useAimIk) return;
            defaultLocalPos = useAimIk.transform.InverseTransformPoint(useAimIk.solver.bones[^1].transform.position);
        }
    }
}