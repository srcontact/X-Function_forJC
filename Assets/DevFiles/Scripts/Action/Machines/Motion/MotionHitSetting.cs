using System;
using System.Collections.Generic;
using clrev01.Bases;
using Den.Tools;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.Motion
{
    [Serializable]
    public class MotionHitSetting
    {
        public int hitObjNum;
        public Vector3 hitBoxSize, hitBoxRotate;
        public float speedOfPowerBase = 2000;
        public PowerPar hitPower;


        private Collider[] _res = new Collider[10];
        public void ExeHitDetection(Transform hitObj, List<Collider> hdColliders, List<IHaveHitCollider> alreadyHitHardList, int hdUniqueId, Vector3 hdSpeed, int duration, int frameCount)
        {
            _res.Fill(null);
            var hitNum = Physics.OverlapBoxNonAlloc(
                hitObj.position,
                hitBoxSize / 2,
                _res,
                hitObj.rotation * Quaternion.Euler(hitBoxRotate),
                layerOfMachine + layerOfMissile + layerOfMine + layerOfAerialSmallObject + layerOfShield);
            if (hitNum > 0)
            {
                for (var i = 0; i < hitNum; i++)
                {
                    var collider = _res[i];
                    if (hdColliders.Contains(collider)) continue;
                    var hardBase = ACM.GetHardFromCollider(collider);
                    if (hardBase is HardBase @base && @base.uniqueID == hdUniqueId) continue;
                    if (alreadyHitHardList.Contains(hardBase)) continue;
                    alreadyHitHardList.Add(hardBase);
                    hardBase.AddDamage(hitPower, speedOfPowerBase, Vector3.one, hitObj.position, duration, frameCount);
                }
            }
        }
    }
}