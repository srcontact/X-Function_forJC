using clrev01.ClAction.ObjectSearch;
using clrev01.Programs;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    public class ProximityFuseMineHD : MineHD<ProximityFuseMineCD, ProximityFuseMineLD, ProximityFuseMineHD>
    {
        private readonly ObjectSearchTgt[] _lockOnArray = new ObjectSearchTgt[1];

        protected override void ExeMove()
        {
            if ((ACM.actionFrame + ld.hd.uniqueID) % ld.cd.searchIntervalFrame != 0) return;
            _lockOnArray[0] = null;
            if (!ld.cd.proximityFuseRange.LockOn(
                    UtlOfProgram.IdentificationType.Enemy,
                    UtlOfProgram.ObjType.Machine,
                    UtlOfProgram.LockOnDistancePriorityType.Near,
                    ld.hd.transform,
                    UtlOfProgram.LockOnAngleOfMovementToSelfType.None,
                    0,
                    ld.hd.teamID,
                    _lockOnArray,
                    null
                )) return;
            var toTgt = _lockOnArray[0]!.pos - ld.hd.pos;
            if (Physics.Raycast(ld.hd.pos, toTgt.normalized, out var hitInfo, toTgt.magnitude, layerOfGround) &&
                (hitInfo.articulationBody == null || hitInfo.articulationBody.gameObject != _lockOnArray[0].gameObject)) return;
            ld.explosionCount++;
            ld.OnHit(null, ld.hd.pos, Vector3.up, HitType.ProximityFuse, -ld.hd.transform.forward);
        }
    }
}