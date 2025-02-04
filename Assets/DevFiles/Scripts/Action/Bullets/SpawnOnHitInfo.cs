using System;
using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    [System.Serializable]
    public class SpawnOnHitInfo
    {
        [SerializeField]
        private CommonDataBase spawnObjectCD;
        [SerializeField]
        private bool spawnOnHit = true;
        [SerializeField]
        private DirectHitSpawnType directHitSpawnType;
        [SerializeField]
        private bool spawnOnProximityFuse = true;
        /// <summary>
        /// ヒット地点のノーマルに頭を向けるようにする。
        /// Bulletには無効。
        /// </summary>
        [SerializeField]
        private HeadRotateMode headRotateMode;
        private bool IsBulletSpawn => spawnObjectCD is BulletCD;
        private bool IsNotBulletSpawn => spawnObjectCD is not BulletCD;
        [SerializeField, ShowIf("IsBulletSpawn")]
        private bool aimHitObject;
        [SerializeField, ShowIf("IsNotBulletSpawn")]
        private float effectScale = 1;

        public void SpawnExe(Vector3 spawnPos, Vector3 velocity, Vector3 hitPointNormal, ObjectSearchTgt hitObj, ObjectSearchTgt spawner, HitType hitType)
        {
            if (!((spawnOnHit && hitType is HitType.DirectHit) || (spawnOnProximityFuse && hitType is HitType.ProximityFuse))) return;
            if (hitType is HitType.DirectHit &&
                (
                    (directHitSpawnType is DirectHitSpawnType.OnlyAddDamage && !hitObj) ||
                    (directHitSpawnType is DirectHitSpawnType.OnlyNotAddDamage && hitObj)
                )
               ) return;

            switch (spawnObjectCD)
            {
                case BulletCD bulletCd:
                    Vector3 shootDirection;
                    if (aimHitObject && hitObj != null && hitObj.hardBase.rigidBody != null)
                    {
                        var tgtPos = ExPrediction.LinePrediction(
                            hitObj.pos, hitObj.hardBase.rigidBody.linearVelocity,
                            bulletCd.InitialSpeed, bulletCd.moveCommonPar.nvreak * ACM.actionEnvPar.globalAirBreakPar,
                            spawnPos, velocity,
                            ACM.actionEnvPar.globalGPowMSec
                        );
                        shootDirection = tgtPos - spawnPos;
                    }
                    else
                    {
                        shootDirection = velocity;
                    }
                    bulletCd.Shoot(spawnPos, shootDirection.normalized, velocity, hitObj, spawner, spawner.hardBase.teamID, spawner.hardBase.uniqueID);
                    break;
                default:
                    var he = spawnObjectCD.InstActorH(spawnPos, Quaternion.identity);
                    he.transform.position = spawnPos;
                    switch (headRotateMode)
                    {
                        case HeadRotateMode.NoOption:
                            break;
                        case HeadRotateMode.HeadToHitPointNormal:
                            he.transform.rotation = Quaternion.LookRotation(he.transform.forward, hitPointNormal);
                            break;
                        case HeadRotateMode.HeadToReflectionVector:
                            he.transform.rotation = Quaternion.LookRotation(he.transform.forward, Vector3.Reflect(velocity.normalized, hitPointNormal));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    he.transform.localScale = Vector3.one * effectScale;
                    if (he.rigidBody != null) he.rigidBody.linearVelocity = velocity;
                    break;
            }
        }

        public void StandbyPoolActor(int parentStandbyNum)
        {
            if (spawnObjectCD is IProjectileCommonData cd)
            {
                cd.StandbyPoolActors(cd.SimultaneousFiringNum * parentStandbyNum);
                foreach (var spawnOnHitInfo in cd.SpawnOnHitInfos)
                {
                    spawnOnHitInfo.StandbyPoolActor(parentStandbyNum);
                }
            }
            else
            {
                spawnObjectCD.StandbyPoolActors(parentStandbyNum);
            }
        }
    }

    public enum DirectHitSpawnType
    {
        NoOption,
        OnlyAddDamage,
        OnlyNotAddDamage,
    }

    public enum HeadRotateMode
    {
        NoOption,
        HeadToHitPointNormal,
        HeadToReflectionVector,
    }
}