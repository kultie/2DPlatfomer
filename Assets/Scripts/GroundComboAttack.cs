using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using Kultie.Extensions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Kultie.Platformer2DSystem
{
    public class GroundComboAttack : EntityAction
    {
        [SerializeField] private AnimationClip[] actionClips;
        [SerializeField] private Animator animator;
        private PlayerController _owner;
        private CoroutineReference _comboTimeout;
        private int _actionIndex;
        private bool _comboStarted;

        private void Awake()
        {
            _owner = GetComponentInParent<PlayerController>();
            _owner.ON_STATE_CHANGE += CheckBreakCombo;
        }

        private void CheckBreakCombo(State previousState, State currentState)
        {
            //Assuming last state is action cause by combo progress
            if (currentState == State.Action)
            {
                return;
            }

            if (currentState == State.Idle)
            {
                return;
            }

            BreakCombo();
        }

        IEnumerator ComboTimeout()
        {
            yield return new WaitForSeconds(1f);
            BreakCombo();
        }

        public override IEnumerator Process(IEntity caster)
        {
            _comboTimeout.Stop();
            _comboStarted = true;
            if (_actionIndex == actionClips.Length - 1)
            {
                yield return null;
            }

            yield return animator.PlayClip(actionClips[_actionIndex]);
            _actionIndex++;
            _actionIndex %= actionClips.Length;
            _comboTimeout = this.Schedule(ComboTimeout());
        }

        private void BreakCombo()
        {
            _actionIndex = 0;
            _comboStarted = false;
        }
    }
}