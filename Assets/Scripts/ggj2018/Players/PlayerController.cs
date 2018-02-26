using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.World;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Players
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Serializable]
        private struct PauseState
        {
            private Vector3 _velocity;

            public Vector3 Velocity => _velocity;

            public void Save(Rigidbody rigidbody)
            {
                _velocity = rigidbody.velocity;
                rigidbody.velocity = Vector3.zero;
            }

            public void Restore(Rigidbody rigidbody)
            {
                rigidbody.velocity = _velocity;
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
        private Vector3 _bankForce;

        public Vector3 BankForce => _bankForce;

        public float Speed => GameManager.Instance.IsPaused ? _pauseState.Velocity.magnitude : _rigidbody.velocity.magnitude;

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

        private void Start()
        {
            // likely necessary because of our colliders and shit, I dunno
            _rigidbody.ResetCenterOfMass();
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }

        private void Update()
        {
            if(!_owner.IsLocalPlayer) {
                return;
            }

            if(InputManager.Instance.Pressed(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.InvertLookButton)) {
                InputManager.Instance.InvertLookAxis(_owner.ControllerIndex);
            } else if(InputManager.Instance.Pressed(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.InvertMoveButton)) {
                InputManager.Instance.InvertMoveAxis(_owner.ControllerIndex);
            }

#if UNITY_EDITOR
            CheckForDebug();
#endif

            if(!CheckForBrake()) {
                CheckForBoost();
            }

            if(GameManager.Instance.IsPaused) {
                return;
            }

// TODO: driver should do this
            _lastMoveAxes = InputManager.Instance.GetMoveAxes(_owner.ControllerIndex);

            float dt = Time.deltaTime;

            RotateModel(_lastMoveAxes, dt);
        }

        private void FixedUpdate()
        {
            if(!_owner.IsLocalPlayer) {
                return;
            }

            if(GameManager.Instance.IsPaused) {
                return;
            }

            float dt = Time.fixedDeltaTime;

            Turn(_lastMoveAxes, dt);
            Move(_lastMoveAxes, dt);

            // have to do this here so that it doesn't jump around
            _owner.Viewer.PlayerUI.PlayerHUD.DebugVisualizer.SetState(_owner);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!_owner.IsLocalPlayer) {
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
            if(!_owner.IsLocalPlayer) {
                return;
            }

            if(CheckPlayerCollision(other)) {
                return;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _rigidbody.angularVelocity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _rigidbody.velocity);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _bankForce);
        }
#endregion

        private void InitRigidbody()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.freezeRotation = true;
            _rigidbody.detectCollisions = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // we run the follow cam in FixedUpdate() and interpolation interferes with that
            _rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public void Initialize(Player owner)
        {
            _owner = owner;

            _rigidbody.mass = owner.Bird.Type.Physics.Mass;
            _rigidbody.drag = owner.Bird.Type.Physics.Drag;
            _rigidbody.angularDrag = owner.Bird.Type.Physics.AngularDrag;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.Id} to {position}");
            _rigidbody.position = position;
        }

        public void Redirect(Vector3 velocity)
        {
            Debug.Log($"Redirecting player {_owner.Id}: {velocity}");

            // unwind all of the rotations
            _owner.Bird.transform.localRotation = Quaternion.Euler(0.0f, _owner.Bird.transform.localEulerAngles.y, 0.0f);
            _rigidbody.transform.rotation = Quaternion.Euler(0.0f, _rigidbody.transform.eulerAngles.y, 0.0f);

            // stop moving
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            // move in an orderly fashion!
            _rigidbody.velocity = velocity;
        }

#region Input Handling
#if UNITY_EDITOR
        private void CheckForDebug()
        {
            if(Input.GetKey(KeyCode.B)) {
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
            }

            if(InputManager.Instance.Pressed(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.DebugStunButton)) {
                _owner.State.DebugStun();
            }

            if(InputManager.Instance.Pressed(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.DebugKillButton)) {
                _owner.State.DebugKill();
            }
        }
#endif

        private bool CheckForBrake()
        {
            float brakeAmount = 0.0f;

            if(PlayerManager.Instance.PlayerData.UseBrakeAxis) {
                brakeAmount = InputManager.Instance.GetTriggerAxes(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.BrakeAxis);
            } else if(InputManager.Instance.Held(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.BrakeButton)) {
                brakeAmount = 1.0f;
            }

            if(_owner.State.IsBraking) {
                if(brakeAmount >= Mathf.Epsilon) {
                    _owner.State.UpdateBrakeAmount(brakeAmount);
                    return true;
                }

                _owner.State.StopBrake();
                return false;
            }

            if(brakeAmount >= Mathf.Epsilon) {
                _owner.State.StartBrake(brakeAmount);
                return true;
            }

            return false;
        }

        private bool CheckForBoost()
        {
            float boostAmount = 0.0f;

            if(PlayerManager.Instance.PlayerData.UseBoostAxis) {
                boostAmount = InputManager.Instance.GetTriggerAxes(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.BoostAxis);
            } else if(InputManager.Instance.Held(_owner.ControllerIndex, PlayerManager.Instance.PlayerData.BoostButton)) {
                boostAmount = 1.0f;
            }

            if(_owner.State.IsBoosting) {
                if(boostAmount >= Mathf.Epsilon) {
                    _owner.State.UpdateBoostAmount(boostAmount);
                    return true;
                }

                _owner.State.StopBoost();
                return false;
            }

            if(boostAmount >= Mathf.Epsilon) {
                _owner.State.StartBoost(boostAmount);
                return true;
            }

            return false;
        }
#endregion

        private void RotateModel(Vector3 axes, float dt)
        {
            Quaternion rotation = _owner.Bird.transform.localRotation;
            Vector3 eulerAngles = _owner.Bird.transform.localEulerAngles;

            if(_owner.State.IsDead) {
                rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            } else if(_owner.State.IsStunned) {
                rotation = Quaternion.Euler(0.0f, eulerAngles.y, 0.0f);
            } else {
                Vector3 targetEuler = new Vector3();

                if(_horizontalBoundaryCollisions.Count < 1) {
                    targetEuler.z = axes.x * -GameManager.Instance.BirdData.MaxBankAngle;
                }

                if(null == _verticalBoundaryCollision) {
                    targetEuler.x = axes.y * -GameManager.Instance.BirdData.MaxAttackAngle;
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

#if true
            float turnSpeed = _owner.Bird.Type.Physics.TurnSpeed * axes.x;
            Quaternion rotation = Quaternion.AngleAxis(turnSpeed * dt, Vector3.up);
            _rigidbody.MoveRotation(_rigidbody.rotation * rotation);
#else
            // TODO: this only works if Y rotatoin is unconstrained
            // it also breaks because the model rotates :(
            const float AngularThrust = 0.5f;
            _rigidbody.AddRelativeTorque(Vector3.up * AngularThrust * axes.x);
#endif

            // adding a force opposite our current x velocity should help stop us drifting
            Vector3 relativeVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
            _bankForce = -relativeVelocity.x * _rigidbody.angularDrag * transform.right;
            _rigidbody.AddForce(_bankForce);
        }

        private void Move(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                // just let gravity drop us (this feels slow tho)
            } else if(_owner.State.IsStunned) {
                // just don't fall
                _rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
            } else {
                float attackAngle = axes.y * -GameManager.Instance.BirdData.MaxAttackAngle;
                Vector3 attackVector = Quaternion.AngleAxis(attackAngle, Vector3.right) * Vector3.forward;
                _rigidbody.AddRelativeForce(attackVector * _owner.Bird.Type.Physics.LinearThrust);

                if(_owner.State.IsBraking) {
                    _rigidbody.AddRelativeForce(Vector3.forward * -_owner.Bird.Type.Physics.BrakeThrust * _owner.State.BrakeAmount);
                }

                if(_owner.State.IsBoosting) {
                    _rigidbody.AddRelativeForce(Vector3.forward * _owner.Bird.Type.Physics.BoostThrust * _owner.State.BoostAmount);
                }

                // lift if we're not falling
                if(axes.y >= 0.0f) {
                    _rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
                }
            }

            // cap our fall speed
            if(_rigidbody.velocity.y < -_owner.Bird.Type.Physics.TerminalVelocity) {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -_owner.Bird.Type.Physics.TerminalVelocity, _rigidbody.velocity.z);
            }
        }
#endregion

#region Collision Handlers
        private bool CheckWorldCollisionEnter(Collision collision)
        {
            WorldBoundary boundary = collision.gameObject.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            if(boundary.IsVertical) {
                _verticalBoundaryCollision = boundary;
            } else {
                _horizontalBoundaryCollisions.Add(boundary);
            }

            return true;
        }

        private bool CheckWorldCollisionExit(Collision collision)
        {
            WorldBoundary boundary = collision.gameObject.GetComponent<WorldBoundary>();
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

            // TODO: both players are going to get this so we should
            // probably do something here so that we don't double-collide them

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
            if(GameManager.Instance.IsPaused) {
                _pauseState.Save(_rigidbody);
            } else {
                _pauseState.Restore(_rigidbody);
            }
            _rigidbody.isKinematic = GameManager.Instance.IsPaused;
        }
#endregion
    }
}
