using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    public class ActionInput : MonoBehaviour
    {
        private PlayerController _owner;
        [SerializeField] private EntityAction airAttack;
        [SerializeField] private EntityAction groundAttack;
        private void Awake()
        {
            _owner = GetComponent<PlayerController>();
        }

        protected virtual void Update()
        {
            if (_owner.CurrentState == State.Air || _owner.CurrentState == State.Jump)
            {
                ProcessAirInput();
            }

            if (_owner.CurrentState == State.Idle || _owner.CurrentState == State.Run)
            {
                ProcessGroundInput();
            }
        }

        private void ProcessGroundInput()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _owner.StartAction(groundAttack.Process(_owner));
            }
        }

        private void ProcessAirInput()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _owner.StartAction(airAttack.Process(_owner));
            }
        }
    }
}