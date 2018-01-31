//#define USE_PHYSICS

using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
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
        private Vector3 _velocity;

        public float Speed => _velocity.magnitude;

        private IPlayer _owner;

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

#if UNITY_EDITOR
            CheckForDebug();
#endif

            if(!CheckForBrake()) {
                CheckForBoost();
            }

            // when the rigidbody is not kinematic, collisions cause us to rotate weird, even with frozen rotations :\
            // this is the most consistent place to correct for that
#if USE_PHYSICS
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y , 0.0f));
#endif

            _lastMoveAxes = InputManager.Instance.GetMoveAxes(_owner.ControllerNumber);

            float dt = Time.deltaTime;

            Turn(_lastMoveAxes, dt);
            RotateModel(_lastMoveAxes, dt);

            float boostPct = _owner.State.BoostRemainingSeconds /
                PlayerManager.Instance.PlayerData.BoostSeconds;

            Viewer viewer = CameraManager.Instance.Viewers.ElementAt(_owner.ControllerNumber) as Viewer;
            viewer?.PlayerUI.SetSpeedAndBoost((int)Speed, boostPct);
        }

        private void FixedUpdate()
        {
            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            _boundaryCollision = null;

            float dt = Time.fixedDeltaTime;

            Move(_lastMoveAxes, dt);

            bool groundCollision = null != _boundaryCollision && WorldBoundary.BoundaryType.Ground == _boundaryCollision.Type;
            if(_owner != null && _owner.State.IsDead && groundCollision) {
                PlayerManager.Instance.DespawnLocalPlayer(_owner.State.PlayerNumber);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(CheckGoalCollision(other)) {
                return;
            }

            if(CheckBuildingCollision(other)) {
                return;
            }

            if(CheckPlayerCollision(other)) {
                return;
            }

            if(CheckWorldCollision(other)) {
                return;
            }
        }
#endregion

        private void InitRigidbody()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
#if !USE_PHYSICS
            rigidbody.isKinematic = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
#endif
            rigidbody.useGravity = false;
        }

        public void Initialize(IPlayer owner, SpawnPoint spawnPoint, Bird bird)
        {
            _owner = owner;
            Bird = bird;

            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.State.PlayerNumber} to {position}");
            transform.position = position;
        }

#if UNITY_EDITOR
        private void CheckForDebug()
        {
            if(InputManager.Instance.Pressed(_owner.ControllerNumber, InputManager.Button.LeftStick)) {
                _owner.State.DebugStun();
            }

            if(InputManager.Instance.Pressed(_owner.ControllerNumber, InputManager.Button.RightStick)) {
                _owner.State.DebugKill();
            }
        }
#endif

        private bool CheckForBrake()
        {
            if(InputManager.Instance.Pressed(_owner.ControllerNumber, InputManager.Button.B)) {
                _owner.State.StartBrake();
                return true;
            }

            if(_owner.State.IsBraking && InputManager.Instance.Released(_owner.ControllerNumber, InputManager.Button.B)) {
                _owner.State.StopBrake();
            }
            return false;
        }

        private bool CheckForBoost()
        {
            if(InputManager.Instance.Pressed(_owner.ControllerNumber, InputManager.Button.Y)) {
                _owner.State.StartBoost();
                return true;
            }

            if(_owner.State.IsBoosting && InputManager.Instance.Released(_owner.ControllerNumber, InputManager.Button.Y)) {
                _owner.State.StopBoost();
            }
            return false;
        }

        private void Turn(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                return;
            }

            float turnSpeed = PlayerManager.Instance.PlayerData.BaseTurnSpeed + _owner.State.BirdType.BirdDataEntry.TurnSpeedModifier;

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
                _velocity = Vector3.zero;

                transform.position += _owner.State.StunBounceDirection * PlayerManager.Instance.PlayerData.StunBounceSpeed * dt;
                return;
            }

            if(_owner.State.IsStunned || _owner.State.IsDead) {
                _velocity = Vector3.zero;

                transform.position += Vector3.down * PlayerManager.Instance.PlayerData.TerminalVelocity * dt;
                return;
            }

            float speed = PlayerManager.Instance.PlayerData.BaseSpeed + _owner.State.BirdType.BirdDataEntry.SpeedModifier;
            if(_owner.State.IsBraking) {
                speed *= PlayerManager.Instance.PlayerData.BrakeFactor;
            } else if(_owner.State.IsBoosting) {
                speed *= PlayerManager.Instance.PlayerData.BoostFactor;
            }

            _velocity = transform.forward * speed;
            _velocity.y = 0.0f;

            if(axes.y < -Mathf.Epsilon) {
                _velocity.y = -PlayerManager.Instance.PlayerData.BasePitchDownSpeed + _owner.State.BirdType.BirdDataEntry.PitchSpeedModifier;
            } else if(axes.y > Mathf.Epsilon) {
                _velocity.y = PlayerManager.Instance.PlayerData.BasePitchUpSpeed + _owner.State.BirdType.BirdDataEntry.PitchSpeedModifier;
            }

            transform.position += _velocity * dt;
        }

#region Collision Handlers
        private bool CheckGoalCollision(Collider other)
        {
            Goal goal = other.GetComponent<Goal>();
            return goal?.Collision(_owner, other) ?? false;
        }

        private bool CheckBuildingCollision(Collider other)
        {
            Building building = other.GetComponent<Building>();
            return building?.Collision(_owner, other) ?? false;
        }

        private bool CheckPlayerCollision(Collider other)
        {
            IPlayer player = other.GetComponentInParent<IPlayer>();
            if(null == player) {
                return false;
            }

            if(_owner.State.BirdType.BirdDataEntry.IsPredator && player.State.BirdType.BirdDataEntry.IsPrey) {
                player.State.PlayerKill(_owner, other);
            } else {
                _owner.State.PlayerStun(player, other);
                player.State.PlayerStun(_owner, other);
            }

            return true;
        }

        private bool CheckWorldCollision(Collider other)
        {
            WorldBoundary boundary = other.GetComponent<WorldBoundary>();
            if(null == boundary) {
                return false;
            }

            _boundaryCollision = boundary;
            return boundary.Collision(_owner, other);
        }
#endregion
    }
}
