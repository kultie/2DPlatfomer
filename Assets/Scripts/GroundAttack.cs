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
            var animator = GetComponentInParent<Animator>();
            yield return animator.PlayClip(actionClip);
        }
    }
}