using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        [SerializeField]
        private GameObject _model;

        [SerializeField]
        [ReadOnly]
        private bool _groundCollision;

        [SerializeField]
        [ReadOnly]
        private bool _skyCollision;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastMoveAxes;

        [SerializeField]
        [ReadOnly]
        private Vector3 _velocity;

        private IPlayer _owner;

#region Unity Lifecycle
        private void Awake()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void Update()
        {
            _lastMoveAxes = InputManager.Instance.GetMoveAxes(_owner.State.PlayerNumber);

            if(_owner.State.Stunned || _owner.State.Dead) {
                _velocity = Vector3.zero;
                return;
            }

            float dt = Time.deltaTime;

            Turn(_lastMoveAxes, dt);
            RotateModel(_lastMoveAxes, dt);

            // sometimes collisions cause us to rotate weird, even with frozen rotations :\
            // this is the most consistent place to correct for that
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y , 0.0f));
        }

        private void FixedUpdate()
        {
            _skyCollision = false;
            _groundCollision = false;

            if(_owner.State.Stunned) {
                return;
            }

            float dt = Time.fixedDeltaTime;

            Move(_lastMoveAxes, dt);
        }

        private void OnTriggerEnter(Collider collider)
        {
            // TODO: ouch... no no no
            if(null != collider.GetComponentInParent<Building>()) {
                _owner.State.EnvironmentStun();
            } else if(null != collider.GetComponentInParent<PlayerController>()) {
                IPlayer player = collider.GetComponentInParent<IPlayer>();
                if(_owner.State.BirdType.BirdDataEntry.IsPredator && !_owner.State.BirdType.BirdDataEntry.IsPredator) {
                    player.State.PlayerKill(_owner);
                } else {
                    player.State.EnvironmentStun();
                }
            }
        }
#endregion

        public void Initialize(IPlayer owner)
        {
            _owner = owner;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {_owner.State.PlayerNumber} to {position}");

            position.y = Mathf.Clamp(position.y, PlayerManager.Instance.PlayerData.MinHeight, PlayerManager.Instance.PlayerData.MaxHeight);
            transform.position = position;
        }

        private void Turn(Vector3 axes, float dt)
        {
            float turnSpeed = (PlayerManager.Instance.PlayerData.BaseTurnSpeed + _owner.State.BirdType.BirdDataEntry.TurnSpeedModifier) * dt;

            transform.RotateAround(transform.position, Vector3.up, axes.x * turnSpeed);
        }

        private void RotateModel(Vector3 axes, float dt)
        {
            Vector3 targetEuler = new Vector3();

            if(axes.x < -Mathf.Epsilon) {
                targetEuler.z = 45.0f;
            } else if(axes.x > Mathf.Epsilon) {
                targetEuler.z = -45.0f;
            }

            if(!_skyCollision && !_groundCollision) {
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
            float speed = (PlayerManager.Instance.PlayerData.BaseSpeed + _owner.State.BirdType.BirdDataEntry.SpeedModifier) * dt;

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

        private void CheckWorldCollision(float prevY)
        {
            if(transform.position.y < PlayerManager.Instance.PlayerData.MinHeight) {
                _groundCollision = true;
                transform.position = new Vector3(transform.position.x, prevY, transform.position.z);
            } else if(transform.position.y > PlayerManager.Instance.PlayerData.MaxHeight) {
                _skyCollision = true;
                transform.position = new Vector3(transform.position.x, prevY, transform.position.z);
            }
        }
    }
}
