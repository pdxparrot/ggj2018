using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;
using ggj2018.Game.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        [SerializeField]
        private GameObject _model;

        [SerializeField]
        [ReadOnly]
        private BaseAttributes _attributes = new BaseAttributes();

        public BaseAttributes Attributes { get { return _attributes; } set { _attributes = value ?? new BaseAttributes(); } }

        private PlayerData _playerData;

#region Unity Lifecycle
        private void Awake()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            _playerData = DataManager.Instance.GameData.Data.GetOrDefault(PlayerData.DataName) as PlayerData;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            Vector3 moveAxes = InputManager.Instance.GetMoveAxes();

            Turn(moveAxes, dt);
            Move(moveAxes, dt);
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: ouch... no no no
            if(null != other.GetComponentInParent<Building>()) {
Debug.Log("Building collision");
            } else if(null != other.GetComponentInParent<PlayerController>()) {
Debug.Log("Player collision");
            } else if(null != other.GetComponentInParent<Ground>()) {
Debug.Log("Ground collision!");
            }
        }
#endregion

        public void MoveTo(Vector3 position)
        {
            transform.position = position;
        }

        private void Turn(Vector3 axes, float dt)
        {
            float turnSpeed = (_playerData.BaseTurnSpeed + _attributes.TurnSpeedModifier) * dt;
            transform.Rotate(0.0f, axes.x * turnSpeed, 0.0f);

            RotateModel(axes, dt);
        }

        private void RotateModel(Vector3 axes, float dt)
        {
            // TODO: smooth this
            Vector3 rotation = new Vector3();

            if(axes.x < -Mathf.Epsilon) {
                rotation.z = 45.0f;
            } else if(axes.x > Mathf.Epsilon) {
                rotation.z = -45.0f;
            }

            if(axes.y < -Mathf.Epsilon) {
                rotation.x = 45.0f;
            } else if(axes.y > Mathf.Epsilon) {
                rotation.x = -45.0f;
            }

            _model.transform.localRotation = Quaternion.Euler(rotation);
        }

        private void Move(Vector3 axes, float dt)
        {
            float speed = (_playerData.BaseSpeed + _attributes.SpeedModifier) * dt;

            Vector3 velocity = transform.forward * speed;

            float pitchSpeed = (_playerData.BasePitchSpeed + _attributes.PitchSpeedModifier) * dt;
            if(axes.y < -Mathf.Epsilon) {
                velocity.y -= pitchSpeed;
            } else if(axes.y > Mathf.Epsilon) {
                velocity.y += pitchSpeed;
            }

            transform.position += velocity;
        }
    }
}
