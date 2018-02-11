//#define USE_ANGULAR_ACCEL

using System;
using System.Collections.Generic;

using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.World;

using UnityEngine;

namespace ggj2018.ggj2018.Players
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Serializable]
        private struct PauseState
        {
            Vector3 velocity;

            public void Save(Rigidbody rigidbody)
            {
                velocity = rigidbody.velocity;
                rigidbody.velocity = Vector3.zero;
            }

            public void Restore(Rigidbody rigidbody)
            {
                rigidbody.velocity = velocity;
            }
        }

        private WorldBoundary _verticalBoundaryCollision;

        private readonly List<WorldBoundary> _horizontalBoundaryCollisions = new List<WorldBoundary>();

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastMoveAxes;

#region Physics
        [SerializeField]
        [ReadOnly]
        private Vector3 _angularAcceleration;

        [SerializeField]
        [ReadOnly]
        private Vector3 _angularVelocity;

        [SerializeField]
        [ReadOnly]
        private Vector3 _bankForce;

        [SerializeField]
        [ReadOnly]
        private Vector3 _linearAcceleration;

        [SerializeField]
        [ReadOnly]
        private Vector3 _linearVelocity;

        public float Speed => _rigidbody.velocity.magnitude;

        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;
#endregion

        [SerializeField]
        [ReadOnly]
        private PauseState _pauseState;

        private Player _owner;

#region Unity Lifecycle
        private void Awake()
        {
            InitRigidbody();

            GameManager.Instance.PauseEvent += PauseEventHandler;
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }

        private void Update()
        {
            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            if(!_owner.IsLocalPlayer) {
                return;
            }

#if UNITY_EDITOR
            CheckForDebug();
#endif

            if(!CheckForBrake()) {
                CheckForBoost();
            }

            _lastMoveAxes = InputManager.Instance.GetMoveAxes(_owner.ControllerIndex);

            float dt = Time.deltaTime;

            RotateModel(_lastMoveAxes, dt);

            float boostPct = _owner.State.BoostRemainingSeconds / PlayerManager.Instance.PlayerData.BoostSeconds;
            _owner.Viewer.PlayerUI.SetSpeedAndBoost((int)Speed, boostPct);
        }

        private void FixedUpdate()
        {
            if(!_owner.IsLocalPlayer) {
                return;
            }

            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            float dt = Time.deltaTime;

            Turn(_lastMoveAxes, dt);
            Move(_lastMoveAxes, dt);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // this fixes the weird rotation that occurs on collision
            _rigidbody.ResetCenterOfMass();

            if(!_owner.IsLocalPlayer) {
                return;
            }

            if(CheckBuildingCollision(collision)) {
                return;
            }

            if(CheckWorldCollisionEnter(collision)) {
                return;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if(!_owner.IsLocalPlayer) {
                return;
            }

            if(CheckWorldCollisionExit(collision)) {
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(CheckGoalCollision(other)) {
                return;
            }

            if(CheckPlayerCollision(other)) {
                return;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _rigidbody.angularVelocity * 2.0f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _rigidbody.velocity * 2.0f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _bankForce * 2.0f);
        }
#endregion

        private void InitRigidbody()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = !GameManager.Instance.ConfigData.UseArcadeFlightControls;
#if USE_ANGULAR_ACCEL
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
#else
            _rigidbody.freezeRotation = true;
#endif
            _rigidbody.detectCollisions = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void Initialize(Player owner)
        {
            _owner = owner;

            _rigidbody.mass = owner.Bird.Type.Mass;
            _rigidbody.drag = owner.Bird.Type.Drag;
            _rigidbody.angularDrag = owner.Bird.Type.AngularDrag;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.Id} to {position}");
            _rigidbody.position = position;
        }

#region Input Handling
#if UNITY_EDITOR
        private void CheckForDebug()
        {
            if(Input.GetKeyDown(KeyCode.B)) {
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
            }

            if(InputManager.Instance.Pressed(_owner.ControllerIndex, InputManager.Button.LeftStick)) {
                _owner.State.DebugStun();
            }

            if(InputManager.Instance.Pressed(_owner.ControllerIndex, InputManager.Button.RightStick)) {
                _owner.State.DebugKill();
            }
        }
#endif

        private bool CheckForBrake()
        {
            if(InputManager.Instance.Pressed(_owner.ControllerIndex, InputManager.Button.B)) {
                _owner.State.StartBrake();
                return true;
            }

            if(_owner.State.IsBraking && InputManager.Instance.Released(_owner.ControllerIndex, InputManager.Button.B)) {
                _owner.State.StopBrake();
            }
            return false;
        }

        private bool CheckForBoost()
        {
            if(InputManager.Instance.Pressed(_owner.ControllerIndex, InputManager.Button.Y)) {
                _owner.State.StartBoost();
                return true;
            }

            if(_owner.State.IsBoosting && InputManager.Instance.Released(_owner.ControllerIndex, InputManager.Button.Y)) {
                _owner.State.StopBoost();
            }
            return false;
        }
#endregion

        private void RotateModel(Vector3 axes, float dt)
        {
            Quaternion rotation = _owner.Bird.transform.localRotation;
            Vector3 eulerAngles = _owner.Bird.transform.localRotation.eulerAngles;

            if(_owner.State.IsDead) {
                rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            } else if(_owner.State.IsStunned) {
                rotation = Quaternion.Euler(0.0f, eulerAngles.y, 0.0f);
            } else {
                Vector3 targetEuler = new Vector3();

                if(_horizontalBoundaryCollisions.Count < 1) {
                    if(axes.x < -Mathf.Epsilon) {
                        targetEuler.z = PlayerManager.Instance.PlayerData.TurnAnimationAngle;
                    } else if(axes.x > Mathf.Epsilon) {
                        targetEuler.z = -PlayerManager.Instance.PlayerData.TurnAnimationAngle;
                    }
                }

                if(null == _verticalBoundaryCollision) {
                    if(axes.y < -Mathf.Epsilon) {
                        targetEuler.x = PlayerManager.Instance.PlayerData.PitchAnimationAngle;
                    } else if(axes.y > Mathf.Epsilon) {
                        targetEuler.x = -PlayerManager.Instance.PlayerData.PitchAnimationAngle;
                    }
                }

                Quaternion targetRotation = Quaternion.Euler(targetEuler);
                rotation = Quaternion.Lerp(rotation, targetRotation, PlayerManager.Instance.PlayerData.RotationAnimationSpeed * dt);
            }

            _owner.Bird.transform.localRotation = rotation;
        }

#region Movement
        private void Turn(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                return;
            }

#if USE_ANGULAR_ACCEL
            float turnAcceleration = PlayerManager.Instance.PlayerData.BaseAngularAcceleration + _owner.Bird.Type.AngularAccelerationModifier;
            _angularAcceleration = Vector3.up * (turnAcceleration * axes.x);
            _rigidbody.angularVelocity += _angularAcceleration * dt;
            _angularVelocity = _rigidbody.angularVelocity;
#else
            float turnSpeed = PlayerManager.Instance.PlayerData.BaseTurnSpeed + _owner.Bird.Type.TurnSpeedModifier;
            Quaternion rotation = Quaternion.AngleAxis(axes.x * turnSpeed * dt, Vector3.up);
            _rigidbody.MoveRotation(_rigidbody.rotation * rotation);
#endif

            // adding a force opposite our current x velocity should help stop us drifting
            Vector3 relativeVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
            _bankForce = -relativeVelocity.x * _rigidbody.angularDrag * transform.right;
            _rigidbody.AddForce(_bankForce);
        }

        private void Move(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                if(GameManager.Instance.ConfigData.UseArcadeFlightControls) {
                    _rigidbody.velocity += Physics.gravity * dt;
                }
                // physical flight we just let gravity drop us
            } else if(_owner.State.IsStunned) {
// TODO: use a force for this
                _rigidbody.velocity = _owner.State.StunBounceDirection * PlayerManager.Instance.PlayerData.StunBounceSpeed;
            } else {
                if(GameManager.Instance.ConfigData.UseArcadeFlightControls) {
// TODO: calculate this stuff from the physical forces
// rather than having separate values to tweak (and get rid of those values)
                    float speed = PlayerManager.Instance.PlayerData.BaseSpeed + _owner.Bird.Type.SpeedModifier;

// TODO: what if we AddRelativeForce() just for these??
                    if(_owner.State.IsBraking) {
                        speed *= PlayerManager.Instance.PlayerData.BrakeFactor;
                    } else if(_owner.State.IsBoosting) {
                        speed *= PlayerManager.Instance.PlayerData.BoostFactor;
                    }

                    Vector3 velocity = transform.forward * speed;
                    velocity.y = 0.0f;

                    if(axes.y < -Mathf.Epsilon) {
                        velocity.y -= PlayerManager.Instance.PlayerData.BasePitchDownSpeed + _owner.Bird.Type.PitchSpeedModifier;
                    } else if(axes.y > Mathf.Epsilon) {
                        velocity.y += PlayerManager.Instance.PlayerData.BasePitchUpSpeed + _owner.Bird.Type.PitchSpeedModifier;
                    }

                    _rigidbody.velocity = velocity;
                } else {
                    // vertical acceleration
                    float verticalAcceleration = PlayerManager.Instance.PlayerData.BaseVerticalAcceleration + _owner.Bird.Type.VerticalAccelerationModifier;

                    // input
                    verticalAcceleration *= axes.y;

                    // only fight gravity if we're not falling
                    if(axes.y >= 0.0f) {
                        verticalAcceleration -= Physics.gravity.y;
                    }

                    // horizontal acceleration
                    float horizontalAcceleration = PlayerManager.Instance.PlayerData.BaseHorizontalAcceleration + _owner.Bird.Type.HorizontalAccelerationModifier;

                    // modififers
                    if(_owner.State.IsBraking) {
                        float brakeAcceleration = PlayerManager.Instance.PlayerData.BrakeAcceleration + _owner.Bird.Type.BrakeAccelerationModifier;
                        horizontalAcceleration -= brakeAcceleration;
                    }

                    if(_owner.State.IsBoosting) {
                        float boostAcceleration = PlayerManager.Instance.PlayerData.BoostAcceleration + _owner.Bird.Type.BoostAccelerationModifier;
                        horizontalAcceleration += boostAcceleration;
                    }

                    // take away some horizontal if we're not falling
                    if(verticalAcceleration > 0.0f) {
                        horizontalAcceleration -= verticalAcceleration;
                    }

                    if(horizontalAcceleration < 0.0f) {
                        horizontalAcceleration = 0.0f;
                    }

                    _linearAcceleration = (horizontalAcceleration * transform.forward) + (verticalAcceleration * Vector3.up);
                    _rigidbody.AddForce(_linearAcceleration * _rigidbody.mass);
                }
            }

            // cap our fall speed
            if(_rigidbody.velocity.y < -_owner.Bird.Type.TerminalVelocity) {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -_owner.Bird.Type.TerminalVelocity, _rigidbody.velocity.z);
            }
            _linearVelocity = _rigidbody.velocity;
        }
#endregion

#region Collision Handlers
        private bool CheckGoalCollision(Collider other)
        {
            Goal goal = other.GetComponentInParent<Goal>();
            return goal?.Collision(_owner) ?? false;
        }

        private bool CheckBuildingCollision(Collision collision)
        {
            Building building = collision.collider.GetComponent<Building>();
            return building?.Collision(_owner, collision) ?? false;
        }

        private bool CheckWorldCollisionEnter(Collision collision)
        {
            WorldBoundary boundary = collision.collider.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            if(boundary.IsVertical) {
                _verticalBoundaryCollision = boundary;
            } else {
                _horizontalBoundaryCollisions.Add(boundary);
            }

            return boundary.Collision(_owner);
        }

        private bool CheckWorldCollisionExit(Collision collision)
        {
            WorldBoundary boundary = collision.collider.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            if(boundary.IsVertical) {
                _verticalBoundaryCollision = null;
            } else {
                _horizontalBoundaryCollisions.Remove(boundary);
            }

            return true;
        }

        private bool CheckPlayerCollision(Collider other)
        {
            Player player = other.GetComponentInParent<Player>();
            if(null == player) {
                return false;
            }

            // TODO: hande this logic off to something else
            // maybe the Player or the PlayerState
            if(_owner.Bird.Type.IsPredator && player.Bird.Type.IsPrey) {
                player.State.PlayerKill(_owner, GetComponentInChildren<Collider>());
            } else if(_owner.Bird.Type.IsPrey && player.Bird.Type.IsPredator) {
                _owner.State.PlayerKill(player, other);
            } else {
                _owner.State.PlayerStun(player, other);
                player.State.PlayerStun(_owner, other);
            }

            return true;
        }
#endregion

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            if(GameManager.Instance.State.IsPaused) {
                _pauseState.Save(_rigidbody);
            } else {
                _pauseState.Restore(_rigidbody);
            }
            _rigidbody.isKinematic = GameManager.Instance.State.IsPaused;
        }
#endregion
    }
}
