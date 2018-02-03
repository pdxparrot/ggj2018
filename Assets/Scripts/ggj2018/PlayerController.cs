using System;

using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
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

// TODO: Player class should own this
        public Bird Bird { get; private set; }

        [SerializeField]
        [ReadOnly]
        private WorldBoundary _boundaryCollision;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastMoveAxes;

        public float Speed => _rigidbody.velocity.magnitude;

        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;

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

            Turn(_lastMoveAxes, dt);
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

            _boundaryCollision = null;

            Move(_lastMoveAxes);
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

            if(CheckWorldCollision(collision)) {
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
#endregion

        private void InitRigidbody()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            _rigidbody.freezeRotation = true;
            //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.detectCollisions = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void Initialize(Player owner, BirdData.BirdDataEntry birdType, Bird bird)
        {
            _owner = owner;
            Bird = bird;

            _rigidbody.mass = birdType.Mass;
            _rigidbody.drag = birdType.Drag;
            _rigidbody.angularDrag = birdType.AngularDrag;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.Id} to {position}");
            _rigidbody.position = position;
        }

#if UNITY_EDITOR
        private void CheckForDebug()
        {
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

        private void Turn(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                return;
            }

            // TODO: torque
            float turnSpeed = PlayerManager.Instance.PlayerData.BaseTurnSpeed + _owner.State.BirdType.TurnSpeedModifier;
            Quaternion rotation = Quaternion.AngleAxis(axes.x * turnSpeed * dt, Vector3.up);
            _rigidbody.MoveRotation(_rigidbody.rotation * rotation);
        }

        private void RotateModel(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                Bird.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                return;
            }

            if(_owner.State.IsStunned) {
                Vector3 modelRotation = Bird.transform.localRotation.eulerAngles;
                Bird.transform.localRotation = Quaternion.Euler(0.0f, modelRotation.y, 0.0f);
                return;
            }

            Vector3 targetEuler = new Vector3();

            if(null == _boundaryCollision || _boundaryCollision.IsVertical) {
                if(axes.x < -Mathf.Epsilon) {
                    targetEuler.z = PlayerManager.Instance.PlayerData.TurnAnimationAngle;
                } else if(axes.x > Mathf.Epsilon) {
                    targetEuler.z = -PlayerManager.Instance.PlayerData.TurnAnimationAngle;
                }
            }

            if(null == _boundaryCollision || !_boundaryCollision.IsVertical) {
                if(axes.y < -Mathf.Epsilon) {
                    targetEuler.x = PlayerManager.Instance.PlayerData.PitchAnimationAngle;
                } else if(axes.y > Mathf.Epsilon) {
                    targetEuler.x = -PlayerManager.Instance.PlayerData.PitchAnimationAngle;
                }
            }

            Quaternion targetRotation = Quaternion.Euler(targetEuler);
            Quaternion rotation = Quaternion.Lerp(Bird.transform.localRotation, targetRotation, PlayerManager.Instance.PlayerData.RotationAnimationSpeed * dt);
            Bird.transform.localRotation = rotation;
        }

        private void Move(Vector3 axes)
        {
            if(_owner.State.IsStunned) {
                _rigidbody.velocity = _owner.State.StunBounceDirection * PlayerManager.Instance.PlayerData.StunBounceSpeed;
                return;
            }

            if(_owner.State.IsStunned || _owner.State.IsDead) {
                _rigidbody.velocity = new Vector3(0.0f, PlayerManager.Instance.PlayerData.TerminalVelocity, 0.0f);
                return;
            }

            float speed = PlayerManager.Instance.PlayerData.BaseSpeed + _owner.State.BirdType.SpeedModifier;
            if(_owner.State.IsBraking) {
                speed *= PlayerManager.Instance.PlayerData.BrakeFactor;
            } else if(_owner.State.IsBoosting) {
                speed *= PlayerManager.Instance.PlayerData.BoostFactor;
            }

            Vector3 velocity = transform.forward * speed;
            velocity.y = 0.0f;

            if(axes.y < -Mathf.Epsilon) {
                velocity.y -= PlayerManager.Instance.PlayerData.BasePitchDownSpeed + _owner.State.BirdType.PitchSpeedModifier;
            } else if(axes.y > Mathf.Epsilon) {
                velocity.y += PlayerManager.Instance.PlayerData.BasePitchUpSpeed + _owner.State.BirdType.PitchSpeedModifier;
            }

            _rigidbody.velocity = velocity;
        }

#region Collision Handlers
        private bool CheckGoalCollision(Collider other)
        {
            Goal goal = other.GetComponent<Goal>();
            return goal?.Collision(_owner) ?? false;
        }

        private bool CheckBuildingCollision(Collision collision)
        {
            Building building = collision.collider.GetComponent<Building>();
            return building?.Collision(_owner, collision) ?? false;
        }

        private bool CheckWorldCollision(Collision collision)
        {
            WorldBoundary boundary = collision.collider.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            _boundaryCollision = boundary;
            return boundary.Collision(_owner);
        }

        private bool CheckPlayerCollision(Collider other)
        {
            Player player = other.GetComponentInParent<Player>();
            if(null == player) {
                return false;
            }

            if(_owner.State.BirdType.IsPredator && player.State.BirdType.IsPrey) {
                player.State.PlayerKill(_owner, GetComponentInChildren<Collider>());
            } else if(_owner.State.BirdType.IsPrey && player.State.BirdType.IsPredator) {
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
