using clrev01.Bases;
using RootMotion.FinalIK;
using UnityEngine;

namespace clrev01.ClAction.Machines.AdditionalTurret
{
    public class AdditionalTurretObj : PoolableBehaviour
    {
        [SerializeField]
        public AdditionalTurretAnimationController animationController { get; private set; }
        [SerializeField]
        public AimIK aimIK { get; private set; }
        [SerializeField]
        public Transform shootPoint { get; private set; }
    }
}