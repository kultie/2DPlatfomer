using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    [CreateAssetMenu(menuName = "Entity Action/Player/Normal")]
    public class NormalAttack : EntityAction
    {
        [SerializeField] private string animationState;
        public override IEnumerator Process(IEntity caster)
        {
            caster.Animator.Play(animationState);
            yield return null;
            while (true)
            {
                if (caster.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    yield break;
                }
                yield return null;
            }
        }
    }
}