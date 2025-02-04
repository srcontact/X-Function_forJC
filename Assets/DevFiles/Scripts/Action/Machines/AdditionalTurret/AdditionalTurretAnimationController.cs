using System;
using Animancer;
using clrev01.Bases;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.AdditionalTurret
{
    public class AdditionalTurretAnimationController : AnimationController
    {
        [SerializeField]
        private ClipTransition aimingPose;

        [SerializeField]
        private AdditionalTurretState animState;
        [SerializeField]
        private int transitionFrame = 3;

        // private void FixedUpdate()
        // {
        //     UpdateAnimation((int)animState, null, transitionFrame);
        // }
        public override void UpdateAnimation(int state, Vector3? velocity = null, int? transitionCompletionFrame = null)
        {
            var duration = transitionCompletionFrame / 60f;
            switch ((AdditionalTurretState)state)
            {
                case AdditionalTurretState.DefaultPose:
                    PlayIdle(duration);
                    break;
                case AdditionalTurretState.AimingPose:
                    PlayClip(aimingPose, duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}