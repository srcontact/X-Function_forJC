using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using Cysharp.Text;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text;
using clrev01.ClAction.Effect;
using I2.Loc;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.Bullets
{
    public abstract class MineCD<C, L, H> : CommonData<C, L, H>, IProjectileCommonData
        where C : MineCD<C, L, H>
        where L : MineLD<C, L, H>, new()
        where H : MineHD<C, L, H>
    {
        protected override string parentName => "Mine";
        [field: SerializeField, BoxGroup("Weight")]
        public float weaponWeight { get; set; } = 100;
        [field: SerializeField, BoxGroup("Weight")]
        public float ammoWeight { get; set; } = 20;
        [SerializeField]
        public float InitialSpeed { get; set; } = 50;
        [SerializeField]
        public float DragCoefficient { get; set; }
        [SerializeField]
        public bool UseGravity { get; set; } = true;
        [SerializeField]
        public int StartHittingFrame { get; set; }
        [SerializeField]
        public LayerMask HitTgtLayer { get; set; }
        [SerializeField]
        public int LifeFrame { get; set; } = int.MaxValue;
        [SerializeField]
        [Range(1, 50)]
        private int simultaneousFiringNum = 1;
        public int SimultaneousFiringNum
        {
            get => simultaneousFiringNum;
            set => simultaneousFiringNum = value;
        }
        [SerializeField]
        public int MinimumFiringInterval { get; set; } = 30;
        [SerializeField]
        public float AimSpeedRatio { get; set; } = 1;
        [SerializeField]
        public float AimSpeedRatioOnFiring { get; set; } = 1;
        [SerializeField]
        public float RecoilPower { get; set; } = 10;
        [SerializeField]
        public float HeatValueAtFire { get; set; } = 20;
        [SerializeField]
        public float UseEnergyOnFire { get; set; } = 0;
        [SerializeField]
        private List<SpawnOnHitInfo> spawnOnHitInfos = new();
        public List<SpawnOnHitInfo> SpawnOnHitInfos
        {
            get => spawnOnHitInfos;
            set => spawnOnHitInfos = value;
        }
        [BoxGroup("FireEffect")]
        [SerializeField]
        public VfxSlaveObjectCD muzzleFlashVfxCD { get; set; }
        [BoxGroup("FireEffect")]
        [SerializeField]
        public float muzzleFlashSize { get; set; } = 1;
        public PowerPar directHitPower = new();
        public int HealthPoint { get; protected set; } = 10;

        public void Shoot(Vector3 shootPosition, Vector3 shootDirection, Vector3 shooterSpeed, ObjectSearchTgt tgt, ObjectSearchTgt shooter, int teamId = -1, int shooterId = -1)
        {
            var firingId = IProjectileCommonData.GetFiringId();
            for (int i = 0; i < SimultaneousFiringNum; i++)
            {
                var hd = InstActor(shootPosition, Quaternion.Euler(shootDirection));
                hd.OnShoot(shootDirection * InitialSpeed + shooterSpeed, tgt, shooter, teamId, shooterId, firingId);
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

        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_weapon.penetratingPower} {directHitPower.penetrationPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.impactPower} {directHitPower.impactPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.heatPower} {directHitPower.heatPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.muzzleVelocity} {InitialSpeed} m/s".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.healthPoint} {HealthPoint} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.minimumFiringInterval} {MinimumFiringInterval} Frame".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.aimSpeedRatio} {AimSpeedRatio * 100} %".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.recoilPower} {RecoilPower}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.heatValueAtFire} {HeatValueAtFire}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.useEnergyAtFire} {UseEnergyOnFire}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.weaponWeight} {weaponWeight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_weapon.ammoWeight} {ammoWeight} kg".Tagging("align", "flush").Tagging("u"));
        }
        public void GetParameterText(ref Utf8ValueStringBuilder sb, int ammoNum)
        {
            GetParameterText(ref sb);
            sb.AppendLine($"{hardwareCustom_weapon.grossWeight} {GetWeightValue(ammoNum)} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weaponWeight + ammoWeight * ammoNum;
        }
    }
}