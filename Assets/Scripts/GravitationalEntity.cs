using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    [DefaultExecutionOrder(-2)]
    [RequireComponent((typeof(PhysicEntity)))]
    public class GravitationalEntity : MonoBehaviour
    {
        [SerializeField] private float gravity = -20;
        private PhysicEntity _physic;
        private float _verticalVelocity;
        public float FallVelocity => _verticalVelocity;
        private bool _jumpRequested;
        private float _jumpForce;
        private bool _isGravityEnable = true;
        private void Awake()
        {
            _physic = GetComponent<PhysicEntity>();
            
            // _gravity = -(2 * jumpHeight / Mathf.Pow(timeToJumpAex, 2));
            // _jumpVelocity = Mathf.Abs(_gravity) * timeToJumpAex;
        }

        private void Update()
        {
            if (_physic.collisions.below || _physic.collisions.above)
            {
                _verticalVelocity = 0;
            }

            if (_jumpRequested)
            {
                _jumpRequested = false;
                _verticalVelocity = _jumpForce;
            }

            if (_isGravityEnable)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            _physic.SetVelocityY(_verticalVelocity * Time.deltaTime);
        }

        public void ResetVerticalVelocity()
        {
            _verticalVelocity = 0;
        }

        public void Jump(float jumpForce)
        {
            _jumpRequested = true;
            _jumpForce = jumpForce;
            // _collision.SetVelocityY(_verticalVelocity);
        }

        public void SetVelocity(float value)
        {
            _verticalVelocity = value;
        }

        public void EnableGravity(bool value, bool resetVelocity)
        {
            if (resetVelocity)
            {
                _verticalVelocity = 0;
            }

            _isGravityEnable = value;
        }
    }
}