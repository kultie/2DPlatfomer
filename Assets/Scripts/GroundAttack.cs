using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Kultie.Platformer2DSystem
{
    public class GroundAttack : EntityAction
    {
        [SerializeField] private AnimationClip actionClip;
        [SerializeField] private Animator animator;

        private PlayableGraph _playableGraph;

        public override IEnumerator Process(IEntity caster)
        {
            // AnimationPlayableUtilities.PlayClip(animator, actionClip, out _playableGraph);
            // _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            _playableGraph = PlayableGraph.Create();
            var animatorOutput = AnimationPlayableOutput.Create(_playableGraph, "Action", animator);
            var playableClip = AnimationClipPlayable.Create(_playableGraph, actionClip);
            animatorOutput.SetSourcePlayable(playableClip);
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _playableGraph.Play();
            float duration = actionClip.length;
            // animator.speed = 0.5f;
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                yield return null;
            }

            // yield return new WaitForSeconds(actionClip.length);

            // while (!playableClip.IsDone())
            // {
            //     // _playableGraph.Evaluate(Time.deltaTime);
            //     yield return null;
            // }

            _playableGraph.Destroy();
        }
    }
}