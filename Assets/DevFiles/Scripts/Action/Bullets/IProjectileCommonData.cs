using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using System.Collections.Generic;
using System.Text;
using clrev01.ClAction.Effect;
using Sirenix.OdinInspector;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    public interface IProjectileCommonData : IInfoTextData, IWeightSetting
    {
        private static int nextfireingId = 0;
        public static int GetFiringId()
        {
            return nextfireingId++;
        }
        public static void ResetFiringId()
        {
            nextfireingId = 0;
        }
        public float InitialSpeed { get; set; }
        public float DragCoefficient { get; set; }
        public bool UseGravity { get; set; }
        public int StartHittingFrame { get; set; }
        public LayerMask HitTgtLayer { get; set; }
        public int LifeFrame { get; set; }
        public int SimultaneousFiringNum { get; set; }
        public int MinimumFiringInterval { get; set; }
        public float AimSpeedRatio { get; set; }
        public float AimSpeedRatioOnFiring { get; set; }
        public float RecoilPower { get; set; }
        public float HeatValueAtFire { get; set; }
        public float UseEnergyOnFire { get; set; }
        public List<SpawnOnHitInfo> SpawnOnHitInfos { get; set; }
        public VfxSlaveObjectCD muzzleFlashVfxCD { get; set; }
        public float muzzleFlashSize { get; set; }
        public void Shoot(Vector3 shootPosition, Vector3 shootDirection, Vector3 shooterSpeed, ObjectSearchTgt tgt, ObjectSearchTgt shooter, int teamId = -1, int shooterId = -1);
        public VfxSlaveObjectHD MuzzleFlashSpawn(Vector3 shootPosition, Vector3 shootDirection, Vector3 shooterSpeed);
        public void StandbyPoolActors(int standbyNum);
        public void GetParameterText(ref Utf8ValueStringBuilder sb, int ammoNum);
    }
}