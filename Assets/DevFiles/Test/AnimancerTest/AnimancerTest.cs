using System;
using Animancer;
using clrev01.Bases;
using RootMotion.FinalIK;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace clrev01.Test.AnimancerTest
{
    public class AnimancerTest : BaseOfCL
    {
        [SerializeField]
        private bool useAnimancer = true;
        [SerializeField]
        private AnimancerComponent animancerComponent;
        private AnimancerLayer _layer1 => animancerComponent.Layers[0];
        private AnimancerLayer _layer2 => animancerComponent.Layers[1];
        [SerializeField]
        private List<AnimSet> clips1 = new();
        [SerializeField]
        private List<AnimSet> clips2 = new();
        [SerializeField]
        private int currnetClipIndex1 = 0;
        [SerializeField]
        private int currnetClipIndex2 = 0;
        [SerializeField]
        private float fadeDuration = 0.25f;

        [SerializeField]
        private bool useIK = true;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private bool useFixTransforms = true;
        [SerializeField]
        private List<IkSet> ikList = new();
        [SerializeField]
        private Vector2 direction;

        public abstract class AnimSet
        {
            public bool active;
            protected AnimancerState state;
            [SerializeField]
            protected bool useSetTime;
            [SerializeField, ShowIf("useSetTime")]
            protected float setTime = 0;
            public abstract void ExePlay(AnimancerLayer animLayer, float fadeDuration, Vector2 dir);
            public void ExeSetTimeAnim()
            {
                if (useSetTime && state is not null)
                {
                    state.MoveTime(setTime, false);
                }
            }
        }

        public class ClipSet : AnimSet
        {
            public AnimationClip clip;

            public override void ExePlay(AnimancerLayer animLayer, float fadeDuration, Vector2 dir)
            {
                if (!animLayer.IsPlayingClip(clip)) state = animLayer.Play(clip, fadeDuration);
                ExeSetTimeAnim();
            }
        }

        public class MixerSet : AnimSet
        {
            public MixerTransition2D mixer;

            // private MixerState<Vector2> _state;
            public override void ExePlay(AnimancerLayer animLayer, float fadeDuration, Vector2 dir)
            {
                if (mixer.State is null) mixer.CreateState();
                if (mixer.State == null) return;
                mixer.State.Parameter = dir;
                state = animLayer.Play(mixer.State, fadeDuration);
                ExeSetTimeAnim();
            }
        }

        public class IkSet
        {
            public IK ik;
            [Range(0, 1f)]
            public float weight = 1;
        }

        private void Awake()
        {
            // animancerComponent.Playable.PauseGraph();
        }

        private void OnEnable()
        {
            animancerComponent.Animator.StopPlayback();
            // animancerComponent.Playable.PauseGraph();
            // animancerComponent.Playable.ApplyAnimatorIK = true;
        }

        private void FixedUpdate()
        {
            foreach (var ik in ikList)
            {
                ik.ik.GetIKSolver().IKPositionWeight = 0;
            }

            AnimationClipStateUpdate(clips1, ref currnetClipIndex1);
            AnimationClipStateUpdate(clips2, ref currnetClipIndex2);
            if (useAnimancer)
            {
                clips1[currnetClipIndex1].ExePlay(_layer1, fadeDuration, direction);
                clips2[currnetClipIndex2].ExePlay(_layer2, fadeDuration, direction);
                animancerComponent.Evaluate(Time.fixedDeltaTime);
            }

            Physics.SyncTransforms();
            Physics.Simulate(Time.fixedDeltaTime);

            if (useIK)
            {
                foreach (var ik in ikList)
                {
                    IKSolver ikSolver = ik.ik.GetIKSolver();
                    ikSolver.IKPositionWeight = ik.weight;
                    ikSolver.SetIKPosition(target.position);
                    ikSolver.FixTransforms();
                    ikSolver.Update();
                }
            }
        }

        private void AnimationClipStateUpdate(List<AnimSet> clips, ref int currnetClipIndex)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].active && currnetClipIndex != i)
                {
                    currnetClipIndex = i;
                    break;
                }
            }

            for (int i = 0; i < clips.Count; i++)
            {
                if (currnetClipIndex != i)
                {
                    clips[i].active = false;
                }
            }
        }
    }
}