using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Kultie.Platformer2DSystem
{
    public static class AnimationExtension
    {
        public static IEnumerator PlayClip(this Animator animator, AnimationClip clip,
            Action<PlayableGraph, AnimationClipPlayable> onUpdate = null)
        {
            float duration = clip.length;
            var playableGraph = PlayableGraph.Create();
            var animatorOutput = AnimationPlayableOutput.Create(playableGraph, "Action", animator);
            var playableClip = AnimationClipPlayable.Create(playableGraph, clip);
            animatorOutput.SetSourcePlayable(playableClip);
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            playableGraph.Play();
            while (playableClip.GetTime() <= duration)
            {
                onUpdate?.Invoke(playableGraph, playableClip);
                yield return null;
            }

            playableGraph.Destroy();
        }
    }
}