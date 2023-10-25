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
            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var animatorOutput = AnimationPlayableOutput.Create(_playableGraph, "Action", animator);
            var playableClip = AnimationClipPlayable.Create(_playableGraph, actionClip);
            animatorOutput.SetSourcePlayable(playableClip);
            _playableGraph.Play();
            yield return new WaitForSeconds(actionClip.length);
           
            // while (!playableClip.IsDone())
            // {
            //     // _playableGraph.Evaluate(Time.deltaTime);
            //     yield return null;
            // }

            _playableGraph.Destroy();
        }
    }
}