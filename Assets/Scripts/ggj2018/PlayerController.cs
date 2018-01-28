//#define USE_PHYSICS

using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        [SerializeField]
        private GameObject _model;

#region TODO: removeme
        [SerializeField]
        [ReadOnly]
        private bool _isGroundCollision;

        [SerializeField]
        [ReadOnly]
        private bool _isSkyCollision;
#endregion

        [SerializeField]
        [ReadOnly]
        private WorldBoundary _boundaryCollision;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastMoveAxes;

        [SerializeField]
        [ReadOnly]
        private Vector3 _velocity;

        [SerializeField]
        private Collider _collider;

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

            _lastMoveAxes = InputManager.Instance.GetMoveAxes(_owner.State.PlayerNumber);

            float dt = Time.deltaTime;

            Turn(_lastMoveAxes, dt);
            RotateModel(_lastMoveAxes, dt);
        }

        private void FixedUpdate()
        {
            if(GameManager.Instance.IsPaused) {
                return;
            }

            _isSkyCollision = false;
            _isGroundCollision = false;
            _boundaryCollision = null;

            float dt = Time.fixedDeltaTime;

            Move(_lastMoveAxes, dt);

            if(_owner.State.IsDead && transform.position.y < PlayerManager.Instance.PlayerData.MinHeight) {
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

        public void Initialize(IPlayer owner, SpawnPoint spawnPoint)
        {
            _owner = owner;

            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.State.PlayerNumber} to {position}");

            position.y = Mathf.Clamp(position.y, PlayerManager.Instance.PlayerData.MinHeight, PlayerManager.Instance.PlayerData.MaxHeight);
            transform.position = position;
        }

        private void CheckForBoost()
        {
            if(InputManager.Instance.Pressed(0, 1)) {
                _owner.State.StartBoost();
            } else if(_owner.State.IsBoosting && InputManager.Instance.Released(0, 1)) {
                _owner.State.StopBoost();
            }
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
                _model.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
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

            if(!_isSkyCollision && !_isGroundCollision && (null == _boundaryCollision || !_boundaryCollision.IsVertical)) {
                if(axes.y < -Mathf.Epsilon) {
                    targetEuler.x = 45.0f;
                } else if(axes.y > Mathf.Epsilon) {
                    targetEuler.x = -45.0f;
                }
            }

            Quaternion targetRotation = Quaternion.Euler(targetEuler);
            Quaternion rotation = Quaternion.Slerp(_model.transform.localRotation, targetRotation, dt * PlayerManager.Instance.PlayerData.RotationAnimationSpeed);
            _model.transform.localRotation = rotation;
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

            float prevY = transform.position.y;
            transform.position += _velocity;

            CheckWorldCollision(prevY);
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

            if(_owner.State.BirdType.BirdDataEntry.IsPredator && _owner.State.BirdType.BirdDataEntry.IsPrey) {
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
            return boundary.Collision(other);
        }

        private void CheckWorldCollision(float prevY)
        {
            if(transform.position.y < PlayerManager.Instance.PlayerData.MinHeight) {
                _isGroundCollision = true;
                transform.position = new Vector3(transform.position.x, prevY, transform.position.z);
            } else if(transform.position.y > PlayerManager.Instance.PlayerData.MaxHeight) {
                _isSkyCollision = true;
                transform.position = new Vector3(transform.position.x, prevY, transform.position.z);
            }
        }
#endregion
    }
}
