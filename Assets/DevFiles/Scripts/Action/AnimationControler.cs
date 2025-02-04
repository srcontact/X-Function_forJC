using System;
using System.Collections.Generic;
using Animancer;
using clrev01.Bases;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction
{
    public abstract class AnimationController : BaseOfCL
    {
        [SerializeField]
        protected AnimancerComponent animancerComponent;
        [SerializeField]
        protected ClipTransition idle = new() { FadeDuration = 3 / 60f };

        private void Awake()
        {
            animancerComponent.Stop();
        }
        public abstract void UpdateAnimation(int state, Vector3? velocity = null, int? transitionCompletionFrame = null);
        public void PlayClip(ClipTransition clip, float? fadeDuration = null, int layer = 0)
        {
            fadeDuration ??= idle.FadeDuration;
            if (clip.Clip) animancerComponent.Layers[layer].Play(clip, fadeDuration.Value / (int)GetFrameParSec() * 60);
            else if (layer == 0) PlayIdle(fadeDuration);
            else animancerComponent.Layers[layer].Stop();
        }
        public void PlayMt2(MixerTransition2D mt2, float? fadeDuration = null, int layer = 0)
        {
            fadeDuration ??= idle.FadeDuration;
            if (mt2.Animations.Length > 0) animancerComponent.Layers[layer].Play(mt2, fadeDuration.Value / (int)GetFrameParSec() * 60);
            else if (layer == 0) PlayIdle(fadeDuration);
            else animancerComponent.Layers[layer].Stop();
        }
        protected void PlayIdle(float? fadeDuration)
        {
            fadeDuration ??= idle.FadeDuration;
            if (idle.Clip) animancerComponent.Layers[0].Play(idle, fadeDuration.Value / (int)GetFrameParSec() * 60);
        }
        public void StopAnim(int layer)
        {
            animancerComponent.Layers[layer].Stop();
        }
        public void MoveTime(float time, bool normalized = false, int layer = 0)
        {
            animancerComponent.Layers[layer].CurrentState?.MoveTime(time, normalized);
        }
        public static FrameParSec GetFrameParSec()
        {
            return ACM?.frameParSec ?? FrameParSec.@default;
        }
    }
}