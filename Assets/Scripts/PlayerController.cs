using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Kultie.Platformer2DSystem
{
    [DefaultExecutionOrder(1)]
    [RequireComponent(typeof(PhysicEntity), typeof(GravitationalEntity))]
    public class PlayerController : MonoBehaviour
    {
        public enum State
        {
            Air,
            Grounded,
            Coyote,
            Dash,

            Idle,
            Run,
            Jump,
            Attack1,
            Attack2,
            Hurt,
            DashAttack,
            Dead
        };

        [SerializeField] private HorizontalMovementSetting movementSetting;
        [SerializeField] private JumpSetting jumpSetting;
        [SerializeField] private DashSetting dashSetting;
        private State _currentState;

        public State CurrentState
        {
            set
            {
                StateUpdate(_currentState, value);
                _currentState = value;
            }
            get { return _currentState; }
        }

        private float _velocityX;
        private float _velocityXSmoothing;
        private PhysicEntity _physic;
        private GravitationalEntity _gravitational;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private float _airTime;

        private int _currentJumpCount;
        private int _currentDashCount;
        private float _queryJumpTime;
        private float _facing = 1;

        private void Awake()
        {
            _physic = GetComponent<PhysicEntity>();
            _gravitational = GetComponent<GravitationalEntity>();
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
            _currentJumpCount = jumpSetting.jumpCount;
            _currentDashCount = dashSetting.dashCount;
            CurrentState = State.Air;
        }

        private void StateUpdate(State currentState, State nextState)
        {
            switch (nextState)
            {
                case State.Jump:
                    _animator.Play("Jump");
                    break;
                case State.Run:
                    _currentJumpCount = jumpSetting.jumpCount;
                    _currentDashCount = dashSetting.dashCount;
                    _animator.Play("Run");
                    if (_dashCoolDown != null)
                    {
                        StopCoroutine(_dashCoolDown);
                    }

                    break;
                case State.Air:
                    _animator.Play("Fall");
                    break;
                case State.Coyote:
                    StartCoroutine(TransitionToFallState());

                    IEnumerator TransitionToFallState()
                    {
                        yield return new WaitForSeconds(jumpSetting.coyoteTime);
                        _currentState = State.Air;
                    }

                    _animator.Play("Fall");
                    break;
                case State.Idle:
                    _currentJumpCount = jumpSetting.jumpCount;
                    _currentDashCount = dashSetting.dashCount;
                    _animator.Play("Idle");
                    if (_dashCoolDown != null)
                    {
                        StopCoroutine(_dashCoolDown);
                    }

                    break;
                case State.Dash:
                    if (_physic.collisions.below)
                    {
                        _animator.Play("Dash");
                    }
                    else
                    {
                        _animator.Play("AirDash");
                    }

                    break;
            }
        }

        private void Update()
        {
            // UpdateGroundState();
            // DashCheck();
            // JumpCheck();
            // HorizontalMove();

            switch (CurrentState)
            {
                case State.Coyote:
                case State.Air:
                    AirState();
                    break;
                case State.Grounded:
                    break;
                case State.Dash:
                    break;
                case State.Idle:
                    IdleState();
                    break;
                case State.Run:
                    RunState();
                    break;
                case State.Jump:
                    JumpState();
                    break;
                case State.Attack1:
                    break;
                case State.Attack2:
                    break;
                case State.Hurt:
                    break;
                case State.DashAttack:
                    break;
                case State.Dead:
                    break;
            }
        }


        private void StartDash()
        {
            CurrentState = State.Dash;
            _currentDashCount--;
            StartCoroutine(Leap());

            IEnumerator ProcessDash()
            {
                float time = _physic.collisions.below ? dashSetting.dashTime : dashSetting.airDashTime;
                CurrentState = State.Dash;
                _gravitational.EnableGravity(false, true);
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    _physic.SetVelocityX(_facing * dashSetting.dashSpeed * Time.deltaTime);
                    yield return null;
                }

                _physic.SetVelocityX(0);
                _gravitational.EnableGravity(true, true);
                if (_physic.collisions.below)
                {
                    CurrentState = State.Idle;
                }
                else
                {
                    CurrentState = State.Air;
                }
            }

            IEnumerator Leap()
            {
                float time = dashSetting.dashTime;
                CurrentState = State.Dash;
                _gravitational.Jump(dashSetting.dashSpeed / 2);
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    _velocityX = _facing * dashSetting.dashSpeed;
                    _physic.SetVelocityX(_velocityX * Time.deltaTime);
                    yield return null;
                }

                // _physic.SetVelocityX(0);
                if (_physic.collisions.below)
                {
                    CurrentState = State.Idle;
                }
                else
                {
                    CurrentState = State.Air;
                }
            }
        }

        private void StartJump()
        {
            CurrentState = State.Jump;
            _gravitational.Jump(jumpSetting.jumpHeight);
            _currentJumpCount--;
        }

        #region State Update

        private void IdleState()
        {
            float targetVelocityX = CheckInput();

            if (targetVelocityX != 0)
            {
                CurrentState = State.Run;
                return;
            }

            if (!_physic.collisions.below)
            {
                CurrentState = State.Coyote;
                return;
            }


            GroundMoveProcess(targetVelocityX);
            JumpCheck();
            DashCheck();
        }

        private void RunState()
        {
            float targetVelocityX = CheckInput();

            if (targetVelocityX == 0)
            {
                CurrentState = State.Idle;
                return;
            }

            if (!_physic.collisions.below)
            {
                CurrentState = State.Coyote;
            }

            GroundMoveProcess(targetVelocityX);
            JumpCheck();
            DashCheck();
        }

        private void AirState()
        {
            float targetVelocityX = CheckInput();

            AirMoveProcess(targetVelocityX);
            JumpCheck();
            DashCheck();
            if (_physic.collisions.below)
            {
                if (targetVelocityX == 0)
                {
                    CurrentState = State.Idle;
                }
                else
                {
                    CurrentState = State.Run;
                }
            }
        }

        private void JumpState()
        {
            float targetVelocityX = CheckInput();
            AirMoveProcess(targetVelocityX);
            if (_gravitational.FallVelocity < 0)
            {
                CurrentState = State.Air;
                return;
            }

            JumpCheck();
            DashCheck();
        }

        #endregion

        #region Legacy

        void HorizontalMove()
        {
            if (CurrentState == State.Dash) return;
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            float targetVelocityX = input.x * movementSetting.moveSpeed;
            if (input.x != 0)
            {
                _facing = input.x;
            }

            _velocityX = Mathf.SmoothDamp(_velocityX, targetVelocityX, ref _velocityXSmoothing,
                _physic.collisions.below
                    ? movementSetting.accelerationTimeGrounded
                    : movementSetting.accelerationTimeAirborne);
            _physic.SetVelocityX(_velocityX * Time.deltaTime);
        }

        void UpdateGroundState()
        {
            if (_physic.collisions.below)
            {
                CurrentState = State.Grounded;
                _airTime = 0;
                _currentJumpCount = jumpSetting.jumpCount;
                if (Time.realtimeSinceStartup - _queryJumpTime <= jumpSetting.timeToSpamJump)
                {
                    StartJump();
                }
            }
            else
            {
                _airTime += Time.deltaTime;
                if (CurrentState == State.Grounded)
                {
                    if (_airTime >= jumpSetting.coyoteTime)
                    {
                        CurrentState = State.Air;
                    }
                    else
                    {
                        CurrentState = State.Coyote;
                    }
                }
            }
        }

        #endregion

        #region MoveProcess

        void GroundMoveProcess(float targetVelocity)
        {
            _velocityX = Mathf.SmoothDamp(_velocityX, targetVelocity, ref _velocityXSmoothing,
                movementSetting.accelerationTimeGrounded);
            _physic.SetVelocityX(_velocityX * Time.deltaTime);
        }

        void AirMoveProcess(float targetVelocity)
        {
            _velocityX = Mathf.SmoothDamp(_velocityX, targetVelocity, ref _velocityXSmoothing,
                movementSetting.accelerationTimeAirborne);
            _physic.SetVelocityX(_velocityX * Time.deltaTime);
        }

        #endregion

        #region InputCheck

        float CheckInput()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            float targetVelocityX = input.x * movementSetting.moveSpeed;
            if (input.x != 0)
            {
                _facing = input.x;
                _renderer.flipX = _facing < 0;
            }

            return targetVelocityX;
        }

        void JumpCheck()
        {
            if (CurrentState == State.Idle || CurrentState == State.Run)
            {
                if (Time.realtimeSinceStartup - _queryJumpTime <= jumpSetting.timeToSpamJump)
                {
                    StartJump();
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CurrentState == State.Coyote || CurrentState == State.Idle || CurrentState == State.Run)
                {
                    StartJump();
                }
                else if ((CurrentState == State.Jump || CurrentState == State.Air) && _currentJumpCount > 0)
                {
                    StartJump();
                }
                else if (CurrentState == State.Air && _currentJumpCount == 0)
                {
                    _queryJumpTime = Time.realtimeSinceStartup;
                }
            }
        }

        private bool _dashCoolingDown;
        private Coroutine _dashCoolDown;

        void DashCheck()
        {
            if (Input.GetKeyDown(KeyCode.C) && _currentDashCount > 0 && CurrentState != State.Dash)
            {
                StartDash();
                if (!_dashCoolingDown && _currentDashCount == 0)
                {
                    _dashCoolDown = StartCoroutine(DashCoolDown());
                }
            }

            IEnumerator DashCoolDown()
            {
                _dashCoolingDown = true;
                yield return new WaitForSeconds(dashSetting.dashCoolDown);
                _currentDashCount = dashSetting.dashCount;
                _dashCoolingDown = false;
            }
        }

        #endregion

        #region Configuration

        [Serializable]
        private class HorizontalMovementSetting
        {
            public float moveSpeed = 6;
            public float accelerationTimeAirborne = .2f;
            public float accelerationTimeGrounded = .1f;
        }

        [Serializable]
        private class JumpSetting
        {
            public float jumpHeight = 8;
            public int jumpCount = 2;
            public float coyoteTime = 0.2f;
            public float timeToSpamJump = 0.2f;
        }

        [Serializable]
        private class DashSetting
        {
            public int dashCount = 2;
            public float dashTime = 0.5f;
            public float airDashTime = 0.15f;
            public float dashCoolDown = 5f;
            public float dashSpeed = 10;
        }

        #endregion
    }
}