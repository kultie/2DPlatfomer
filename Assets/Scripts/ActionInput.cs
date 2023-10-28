using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    public class ActionInput : MonoBehaviour
    {
        private PlayerController _owner;
        [SerializeField] private EntityAction groundAttack;
        [SerializeField] private EntityAction skill1;
        [SerializeField] private EntityAction skill2;
        [SerializeField] private EntityAction airAttack;
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
            if (Input.GetKeyDown(KeyCode.A))
            {
                _owner.StartAction(skill1.Process(_owner));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                _owner.StartAction(skill2.Process(_owner));
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