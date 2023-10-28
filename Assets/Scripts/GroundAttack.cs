using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Kultie.Platformer2DSystem
{
    public class GroundAttack : EntityAction
    {
        [SerializeField] private AnimationClip actionClip;
        [SerializeField] private AnimancerComponent animator;

        private PlayableGraph _playableGraph;

        public override IEnumerator Process(IEntity caster)
        {
            // AnimationPlayableUtilities.PlayClip(animator, actionClip, out _playableGraph);
            // _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            // _playableGraph = PlayableGraph.Create();
            // var animatorOutput = AnimationPlayableOutput.Create(_playableGraph, "Action", animator);
            // var playableClip = AnimationClipPlayable.Create(_playableGraph, actionClip);
            // animatorOutput.SetSourcePlayable(playableClip);
            // _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            // _playableGraph.Play();
            // float duration = actionClip.length;
            // // animator.speed = 0.5f;
            // while (duration > 0)
            // {
            //     duration -= Time.deltaTime;
            //     yield return null;
            // }

            var state = animator.Play(actionClip);
            // state.Time = 0;
            // float time = 0;
            // yield return state;
            // state.Speed = Random.Range(0.02f, 0.5f);
            while (state.NormalizedTime < 1)
            {
                // state.Speed = 0.5f;
                yield return null;
            }

            state.Speed = 1;
            // animator.Playable.PauseGraph();
            // animator.Stop();
            // while (state.NormalizedTime < 1)
            // {
            //     state.Time += Time.deltaTime;
            //     animator.Evaluate(Time.deltaTime);
            //     yield return null;
            // }

            animator.Stop();

            // yield return new WaitForSeconds(actionClip.length);

            // while (!playableClip.IsDone())
            // {
            //     // _playableGraph.Evaluate(Time.deltaTime);
            //     yield return null;
            // }
            Debug.Log("Finish");

            // _playableGraph.Destroy();
        }
    }
}