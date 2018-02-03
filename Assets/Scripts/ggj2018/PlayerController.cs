using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

// TODO: use the rigidbody for physics

namespace ggj2018.ggj2018
{
// TODO: this should be a NetworkBehavior
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehavior//NetworkBehavior
    {
        public Bird Bird { get; private set; }

        [SerializeField]
        [ReadOnly]
        private WorldBoundary _boundaryCollision;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastMoveAxes;

        [SerializeField]
        [ReadOnly]
        private Vector3 _acceleration;

        [SerializeField]
        [ReadOnly]
        private Vector3 _velocity;

        public float Speed => _velocity.magnitude;

        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;

        private Player _owner;

#region Unity Lifecycle
        private void Awake()
        {
            InitRigidbody();
        }

        private void Update()
        {
            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            /*if(!isLocalPlayer) {
                return;
            }*/

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
            /*if(!isLocalPlayer) {
                return;
            }*/

            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            _boundaryCollision = null;

            float dt = Time.fixedDeltaTime;

            Move(_lastMoveAxes, dt);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // this fixes the weird rotation that occurs on collision
            _rigidbody.ResetCenterOfMass();

            /*if(!isLocalPlayer) {
                return;
            }*/

            if(CheckGoalCollision(collision)) {
                return;
            }

            if(CheckBuildingCollision(collision)) {
                return;
            }

            if(CheckPlayerCollision(collision)) {
                return;
            }

            if(CheckWorldCollision(collision)) {
                return;
            }
        }
#endregion

        public void Initialize(Player owner, Bird bird)
        {
            _owner = owner;
            Bird = bird;
        }

        private void InitRigidbody()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.Id} to {position}");
            transform.position = position;
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

            float turnSpeed = PlayerManager.Instance.PlayerData.BaseTurnSpeed + _owner.State.BirdType.TurnSpeedModifier;

            transform.RotateAround(transform.position, Vector3.up, axes.x * turnSpeed * dt);
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

        private void Move(Vector3 axes, float dt)
        {
            if(_owner.State.IsStunned) {
                _acceleration = Vector3.zero;
                _velocity = Vector3.zero;

                transform.position += _owner.State.StunBounceDirection * PlayerManager.Instance.PlayerData.StunBounceSpeed * dt;
                return;
            }

            if(_owner.State.IsStunned || _owner.State.IsDead) {
                _acceleration = Vector3.zero;
                _velocity = Vector3.zero;

                transform.position += Vector3.down * PlayerManager.Instance.PlayerData.TerminalVelocity * dt;
                return;
            }

#if false
            float acceleration = PlayerManager.Instance.PlayerData.BaseAccleration + _owner.State.BirdType.BirdDataEntry.AccelerationModifier;
            if(_owner.State.IsBraking) {
                acceleration -= PlayerManager.Instance.PlayerData.BaseBrakeDeceleration;
            } else if(_owner.State.IsBoosting) {
                acceleration += PlayerManager.Instance.PlayerData.BaseBoostAcceleration;
            }

            float velocity = PlayerManager.Instance.PlayerData.BaseSpeed + _owner.State.BirdType.BirdDataEntry.SpeedModifier + (acceleration * dt);
            float speed = velocity.magnitude;
// TODO: we need to bound this, I dunno... polish shit
#else
            float velocity = PlayerManager.Instance.PlayerData.BaseSpeed + _owner.State.BirdType.SpeedModifier;
            if(_owner.State.IsBraking) {
                velocity *= PlayerManager.Instance.PlayerData.BrakeFactor;
            } else if(_owner.State.IsBoosting) {
                velocity *= PlayerManager.Instance.PlayerData.BoostFactor;
            }
#endif

            _velocity = transform.forward * velocity;
            _velocity.y = 0.0f;

            if(axes.y < -Mathf.Epsilon) {
                _velocity.y = -PlayerManager.Instance.PlayerData.BasePitchDownSpeed + _owner.State.BirdType.PitchSpeedModifier;
            } else if(axes.y > Mathf.Epsilon) {
                _velocity.y = PlayerManager.Instance.PlayerData.BasePitchUpSpeed + _owner.State.BirdType.PitchSpeedModifier;
            }

            transform.position += _velocity * dt;
        }

#region Collision Handlers
        private bool CheckGoalCollision(Collision collision)
        {
            Goal goal = collision.collider.GetComponent<Goal>();
            return goal?.Collision(_owner) ?? false;
        }

        private bool CheckBuildingCollision(Collision collision)
        {
            Building building = collision.collider.GetComponent<Building>();
            return building?.Collision(_owner, collision) ?? false;
        }

        private bool CheckPlayerCollision(Collision collision)
        {
            Player player = collision.collider.GetComponentInParent<Player>();
            if(null == player) {
                return false;
            }

            if(_owner.State.BirdType.IsPredator && player.State.BirdType.IsPrey) {
                player.State.PlayerKill(_owner, collision);
            } else {
                _owner.State.PlayerStun(player, collision);
                player.State.PlayerStun(_owner, collision);
            }

            return true;
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
#endregion
    }
}
