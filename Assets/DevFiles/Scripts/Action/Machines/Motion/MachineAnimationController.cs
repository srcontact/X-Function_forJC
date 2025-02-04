using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.Motion
{
    public class MachineAnimationController : AnimationController
    {
        [SerializeField]
        private MixerTransition2D move = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private MixerTransition2D dash = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private ClipTransition jump = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private ClipTransition inAir = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private ClipTransition breaking = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private ClipTransition cover = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private ClipTransition guard = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private ClipTransition uncontrollable = new() { FadeDuration = 3 / 60f };
        [SerializeField]
        private MixerTransition2D recovery = new() { FadeDuration = 3 / 60f };


        public override void UpdateAnimation(int state, Vector3? velocity = null, int? transitionCompletionFrame = null)
        {
            float? duration = transitionCompletionFrame.HasValue ? MathF.Max(transitionCompletionFrame.Value - ACM.actionFrame, 0) / 60f : null;
            switch ((ActionState)state)
            {
                case ActionState.Neutral:
                    PlayIdle(duration);
                    break;
                case ActionState.Move:
                    if (move.State is null) move.CreateState();
                    if (velocity != null && move.State != null) move.State.Parameter = new Vector2(velocity.Value.x, velocity.Value.z);
                    PlayMt2(move, duration);
                    break;
                case ActionState.Dash:
                    if (dash.State is null) dash.CreateState();
                    if (velocity != null && dash.State != null) dash.State.Parameter = new Vector2(velocity.Value.x, velocity.Value.z);
                    PlayMt2(dash, duration);
                    break;
                case ActionState.Jump:
                    PlayClip(jump, duration);
                    break;
                case ActionState.InAir:
                    PlayClip(inAir, duration);
                    break;
                case ActionState.Breaking:
                    PlayClip(breaking, duration);
                    break;
                case ActionState.Cover:
                    PlayClip(cover, duration);
                    break;
                case ActionState.Guard:
                    PlayClip(guard, duration);
                    break;
                case ActionState.Uncontrollable:
                    PlayClip(uncontrollable, duration);
                    break;
                case ActionState.Recovery:
                    if (recovery.State is null) recovery.CreateState();
                    if (velocity != null && recovery.State != null) recovery.State.Parameter = new Vector2(velocity.Value.x, velocity.Value.z);
                    PlayMt2(recovery, duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        public bool guardActionAvailable => guard.Clip;
        public bool coverActionAvailable => cover.Clip;
    }
}