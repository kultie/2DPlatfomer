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
        [SerializeField] private Animator animator;
        

        public override IEnumerator Process(IEntity caster)
        {
            yield return animator.PlayClip(actionClip);
        }
    }
}