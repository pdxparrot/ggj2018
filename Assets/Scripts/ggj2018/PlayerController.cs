//#define USE_PHYSICS

using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        private Bird _bird;

        public Bird Bird => _bird;

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
            if(GameManager.Instance.IsPaused) {
                return;
            }

            CheckForBoost();

            // when the rigidbody is not kinematic, collisions cause us to rotate weird, even with frozen rotations :\
            // this is the most consistent place to correct for that
#if USE_PHYSICS
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y , 0.0f));
#endif

            _lastMoveAxes = InputManager.Instance.GetMoveAxes(_owner.ControllerNumber);

            float dt = Time.deltaTime;

            Turn(_lastMoveAxes, dt);
            RotateModel(_lastMoveAxes, dt);
        }

        private void FixedUpdate()
        {
            if(GameManager.Instance.IsPaused) {
                return;
            }

            _boundaryCollision = null;

            float dt = Time.fixedDeltaTime;

            Move(_lastMoveAxes, dt);

            if(_owner != null && 
               _owner.State.IsDead && (_boundaryCollision == null || WorldBoundary.BoundaryType.Ground == _boundaryCollision.Type)) {
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
            _bird = bird;

            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.State.PlayerNumber} to {position}");
            transform.position = position;
        }

        private void CheckForBoost()
        {
            if(InputManager.Instance.Pressed(_owner.ControllerNumber, 1)) {
                _owner.State.StartBoost();
            } else if(_owner.State.IsBoosting && InputManager.Instance.Released(_owner.ControllerNumber, 1)) {
                _owner.State.StopBoost();
            }

            float boostPct = _owner.State.BoostRemainingSeconds /
                PlayerManager.Instance.PlayerData.BoostSeconds;

            UIManager.Instance.PlayerHud[ControllerNumber].SetSpeedAndBoost(
                Speed, boostPct);
        }

        private void Turn(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                return;
            }

            float turnSpeed = (PlayerManager.Instance.PlayerData.BaseTurnSpeed + _owner.State.BirdType.BirdDataEntry.TurnSpeedModifier) * dt;

            transform.RotateAround(transform.position, Vector3.up, axes.x * turnSpeed);
        }

        private void RotateModel(Vector3 axes, float dt)
        {
            if(_owner.State.IsDead) {
                _bird.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                return;
            }

            if(_owner.State.IsStunned) {
                return;
            }

            Vector3 targetEuler = new Vector3();

            if(null == _boundaryCollision || _boundaryCollision.IsVertical) {
                if(axes.x < -Mathf.Epsilon) {
                    targetEuler.z = 45.0f;
                } else if(axes.x > Mathf.Epsilon) {
                    targetEuler.z = -45.0f;
                }
            }

            if(null == _boundaryCollision || !_boundaryCollision.IsVertical) {
                if(axes.y < -Mathf.Epsilon) {
                    targetEuler.x = 45.0f;
                } else if(axes.y > Mathf.Epsilon) {
                    targetEuler.x = -45.0f;
                }
            }

            Quaternion targetRotation = Quaternion.Euler(targetEuler);
            Quaternion rotation = Quaternion.Lerp(_bird.transform.localRotation, targetRotation, dt * PlayerManager.Instance.PlayerData.RotationAnimationSpeed);
            _bird.transform.localRotation = rotation;
        }

        private void Move(Vector3 axes, float dt)
        {
            if(_owner.State.IsStunned) {
                transform.position += _owner.State.StunBounceDirection * PlayerManager.Instance.PlayerData.StunBounceSpeed * dt;
                return;
            }

            if(_owner.State.IsStunned || _owner.State.IsDead) {
                transform.position += Vector3.down * PlayerManager.Instance.PlayerData.TerminalVelocity * dt;
                return;
            }

            float speed = (PlayerManager.Instance.PlayerData.BaseSpeed + _owner.State.BirdType.BirdDataEntry.SpeedModifier) * dt;
            if(_owner.State.IsBoosting) {
                speed *= PlayerManager.Instance.PlayerData.BoostFactor;
            }

            _velocity = transform.forward * speed;
            _velocity.y = 0.0f;

            float pitchSpeed = (PlayerManager.Instance.PlayerData.BasePitchSpeed + _owner.State.BirdType.BirdDataEntry.PitchSpeedModifier) * dt;
            if(axes.y < -Mathf.Epsilon) {
                _velocity.y = -pitchSpeed;
            } else if(axes.y > Mathf.Epsilon) {
                _velocity.y = pitchSpeed;
            }

            transform.position += _velocity;
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
