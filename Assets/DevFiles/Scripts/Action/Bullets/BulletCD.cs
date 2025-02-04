using clrev01.Bases;
using clrev01.ClAction.Bullets.DevFiles.Scripts.Action.Bullets;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using Cysharp.Text;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text;
using clrev01.ClAction.Effect;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.Bullets
{
    [CreateAssetMenu(menuName = "CommonData/BulletCD")]
    public partial class BulletCD : CommonData<BulletCD, BulletLD, BulletHD>, IProjectileCommonData
    {
        protected override string parentName => "Bullets";
        [field: SerializeField, BoxGroup("Weight")]
        public float weaponWeight { get; set; } = 100;
        [field: SerializeField, BoxGroup("Weight")]
        public float ammoWeight { get; set; } = 5;
        public BulletMoveCommonPar moveCommonPar;
        public float speed = 10;
        [SerializeField]
        public float InitialSpeed
        {
            get => speed;
            set => speed = value;
        }
        public float DragCoefficient
        {
            get => moveCommonPar.nvreak;
            set => moveCommonPar.nvreak = value;
        }
        public bool UseGravity
        {
            get => moveCommonPar.useGraviry;
            set => moveCommonPar.useGraviry = value;
        }
        [SerializeField]
        private int lifeFrame = 30;
        public int LifeFrame
        {
            get => lifeFrame;
            set => lifeFrame = value;
        }
        [Range(0, 90)]
        public float diffusionAngle = 0;
        [SerializeField]
        [Range(1, 50)]
        private int simultaneousFiringNum = 1;
        public int SimultaneousFiringNum
        {
            get => simultaneousFiringNum;
            set => simultaneousFiringNum = value;
        }
        public bool ignoreHitSiblingObjects;
        public bool sphereDetection = false;
        public float bulletScale = 0.1f;
        [SerializeField]
        private List<SpawnOnHitInfo> spawnOnHitInfos = new();
        public List<SpawnOnHitInfo> SpawnOnHitInfos
        {
            get => spawnOnHitInfos;
            set => spawnOnHitInfos = value;
        }
        [BoxGroup("MuzzleFlash"), SerializeField]
        public VfxSlaveObjectCD muzzleFlashVfxCD { get; set; }
        [BoxGroup("MuzzleFlash"), SerializeField]
        public float muzzleFlashSize { get; set; } = 1;

        public PowerPar bulletPow = new PowerPar();

        public bool useLineHitting = true;

        public bool useProximityFuse;
        /// <summary>
        /// 近接信管の反応距離の設定値
        /// 弾速＊本パラメータで反応距離を求める
        /// </summary>
        [BoxGroup("ProximityFuse"), ShowIf("useProximityFuse")]
        public float proximityFuseLength = 5;
        [BoxGroup("ProximityFuse"), ShowIf("useProximityFuse")]
        public float proximityFuseRadius = 20;
        [BoxGroup("ProximityFuse"), ShowIf("useProximityFuse"), Range(1, 10)]
        public int proximityFuseStartFrame = 1;

        public bool usePredictionImpactFrameFuse;
        [BoxGroup("PredictionImpactFrameFuse"), ShowIf("usePredictionImpactFrameFuse")]
        public int predictionImpactFrame = 5;

        [SerializeField]
        public int StartHittingFrame { get; set; } = 0;
        [SerializeField]
        public LayerMask HitTgtLayer { get; set; }

        [SerializeField]
        private int minimumFiringInterval = 30;
        [SerializeField]
        private float aimSpeedRatio = 1;
        [SerializeField]
        private float aimSpeedRatioOnFiring = 1;
        [SerializeField]
        private float recoilPower = 10;
        [SerializeField]
        private float heatValueAtFire = 20;
        [SerializeField]
        private float useEnergyOnFire = 0;
        public int MinimumFiringInterval
        {
            get => minimumFiringInterval;
            set => minimumFiringInterval = value;
        }
        public float AimSpeedRatio
        {
            get => aimSpeedRatio;
            set => aimSpeedRatio = value;
        }
        public float AimSpeedRatioOnFiring
        {
            get => aimSpeedRatioOnFiring;
            set => aimSpeedRatioOnFiring = value;
        }
        public float RecoilPower
        {
            get => recoilPower;
            set => recoilPower = value;
        }
        public float HeatValueAtFire
        {
            get => heatValueAtFire;
            set => heatValueAtFire = value;
        }
        public float UseEnergyOnFire
        {
            get => useEnergyOnFire;
            set => useEnergyOnFire = value;
        }
        protected override bool reuseLd => true;
        [BoxGroup("BulletHealthPoint"), SerializeField]
        private int bulletHealthPoint = 10;
        public int BulletHealthPoint => bulletHealthPoint;
        public float baseImpactResistRate = 1;


        [SerializeField]
        private BulletType bulletType;
        public BulletType BulletType => bulletType;
        private bool IsGuided => BulletType == BulletType.Guided;
        [BoxGroup("GuidedAccele"), SerializeField, ShowIf("IsGuided")]
        private AnimationCurve AcceleCurve;
        [BoxGroup("GuidedAccele"), SerializeField, ShowIf("IsGuided")]
        private float acceleCurveAcceleMagni = 1000;
        public float AcceleCurveAcceleMagni => acceleCurveAcceleMagni;
        [BoxGroup("GuidedAccele"), SerializeField, ShowIf("IsGuided")]
        private int acceleCurveFrameMagni = 30;
        [BoxGroup("GuidedAccele"), SerializeField, ShowIf("IsGuided")]
        private int acceleEndFrame = 200;
        public int AcceleEndFrame => acceleEndFrame;
        public float GetAccele(int exeFrame)
        {
            if (acceleEndFrame < exeFrame) return 0;
            return AcceleCurve.Evaluate((float)exeFrame / acceleCurveFrameMagni) * acceleCurveAcceleMagni;
        }
        public float GetThrustPower(int exeFrame)
        {
            if (acceleEndFrame < exeFrame) return 0;
            return AcceleCurve.Evaluate((float)exeFrame / acceleCurveFrameMagni);
        }
        [BoxGroup("GuidedSpeed"), SerializeField, ShowIf("IsGuided")]
        private float guidedMaxSpeed = 1000;
        public float GuidedMaxSpeed => guidedMaxSpeed;
        [BoxGroup("GuidedSideBreak"), SerializeField, ShowIf("IsGuided")]
        private float guidedSideNvreak;
        public float GuidedSideNvreak => guidedSideNvreak;
        [BoxGroup("GuidedRotate"), SerializeField, ShowIf("IsGuided")]
        private AnimationCurve rotateRatioCurve;
        public float GetRotateRatio(float speed)
        {
            return rotateRatioCurve.Evaluate(speed);
        }
        [BoxGroup("GuidedHeightOffset"), SerializeField, ShowIf("IsGuided")]
        private AnimationCurve guidedHeightOffsetCurve;
        [BoxGroup("GuidedHeightOffset"), SerializeField, ShowIf("IsGuided")]
        private float ghoCurveHeightMagni = 1, ghoCurveDistanceMagni = 1000;
        public float GetGuidedHeightOffset(float distance)
        {
            return guidedHeightOffsetCurve.Evaluate(distance / ghoCurveDistanceMagni) * ghoCurveHeightMagni;
        }

        [BoxGroup("GuidedProNav"), SerializeField, ShowIf("IsGuided")]
        private AnimationCurve proNavRatioCurve;
        public float GetProNavRatio(float distance)
        {
            return proNavRatioCurve.Evaluate(distance);
        }
        [BoxGroup("GuidedProNav"), SerializeField, ShowIf("IsGuided")]
        private bool advanceMode;
        public bool AdvanceMode => advanceMode;
        [BoxGroup("GuidedProNav"), SerializeField, ShowIf("IsGuided")]
        private float navigatoinalConstant = 3;
        public float NavigationalConstant => navigatoinalConstant;
        [BoxGroup("GuidedSearchParameter"), SerializeField, ShowIf("IsGuided")]
        public SearchParameterData guidedSearchParameterData = new();


        protected override void ResetLd(BulletLD ld)
        {
            base.ResetLd(ld);
            ld.isHit = false;
        }

        public void Shoot(Vector3 shootPosition, Vector3 shootDirection, Vector3 shooterSpeed, ObjectSearchTgt tgt, ObjectSearchTgt shooter, int teamId = -1, int shooterId = -1)
        {
            var firingId = IProjectileCommonData.GetFiringId();
            var rotate = Quaternion.FromToRotation(Vector3.forward, shootDirection);
            for (var i = 0; i < simultaneousFiringNum; i++)
            {
                if (diffusionAngle > 0)
                {
                    var randRotate = Quaternion.Lerp(quaternion.identity, Random.rotation, diffusionAngle / 360);
                    rotate *= randRotate;
                    shootDirection = randRotate * shootDirection;
                }
                var bh = InstActor(shootPosition, rotate);
                bh.OnShoot(shootDirection * InitialSpeed + shooterSpeed, tgt, shooter, teamId, shooterId, firingId);
            }
        }

        public VfxSlaveObjectHD MuzzleFlashSpawn(Vector3 shootPosition, Vector3 shootDirection, Vector3 shooterSpeed)
        {
            if (!muzzleFlashVfxCD) return null;
            var fireEffect = muzzleFlashVfxCD.InstActor(shootPosition, Quaternion.identity);
            return fireEffect;
        }

        public override void StandbyPoolActors(int standbyNum)
        {
            base.StandbyPoolActors(standbyNum);
            foreach (var spawnOnHitInfo in SpawnOnHitInfos)
            {
                spawnOnHitInfo.StandbyPoolActor(standbyNum);
            }
            muzzleFlashVfxCD?.StandbyPoolActors(1);
        }

        public void GetParameterText(ref Utf8ValueStringBuilder sb, int ammoNum)
        {
            GetParameterText(ref sb);
            sb.AppendLine($"{hardwareCustom_weapon.grossWeight} {GetWeightValue(ammoNum)} kg".Tagging("align", "flush").Tagging("u"));
        }
        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_weapon.penetratingPower} {bulletPow.penetrationPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.impactPower} {bulletPow.impactPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.heatPower} {bulletPow.heatPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.muzzleVelocity} {InitialSpeed} m/s".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.healthPoint} {bulletHealthPoint} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.flightTime} {lifeFrame} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.minimumFiringInterval} {MinimumFiringInterval} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.aimSpeedRatio} {AimSpeedRatio * 100} %".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.recoilPower} {RecoilPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.heatValueAtFire} {HeatValueAtFire}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.useEnergyAtFire} {UseEnergyOnFire}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.weaponWeight} {weaponWeight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.ammoWeight} {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weaponWeight + ammoWeight * ammoNum;
        }
    }
}