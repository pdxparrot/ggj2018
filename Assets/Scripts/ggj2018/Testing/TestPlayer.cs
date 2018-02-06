using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.Testing
{
    [RequireComponent(typeof(Rigidbody))]
    public class TestPlayer : MonoBehavior
    {
        private Rigidbody _rigidbody;

#region UI
        [SerializeField]
        private Text _velocityText;

        [SerializeField]
        private Text _angularVelocityText;
#endregion

#region Physics
        [SerializeField]
        private float _maxHorizontalForce = 25.0f;

        [SerializeField]
        private float _maxVerticalForce = 10.0f;

        [SerializeField]
        private float _terminalVelocity = 50.0f;

        [SerializeField]
        private float _turnForce = 1.0f;

        [SerializeField]
        private float _brakeForce = 5.0f;

        [SerializeField]
        private float _boostForce = 5.0f;

        public float Speed => _rigidbody.velocity.magnitude;

        public float AngularSpeed => _rigidbody.angularVelocity.magnitude;
#endregion

#region State
        [SerializeField]
        [ReadOnly]
        private Vector2 _lastInput;

        [SerializeField]
        [ReadOnly]
        private Vector3 _angularAcceleration;

        [SerializeField]
        [ReadOnly]
        private Vector3 _acceleration;

        [SerializeField]
        [ReadOnly]
        private bool _isBraking;

        [SerializeField]
        [ReadOnly]
        private bool _isBoosting;
#endregion

#region Model/Animations
        [SerializeField]
        private GameObject _model;

        [SerializeField]
        private float _turnAnimationSpeed = 5.0f;

        [SerializeField]
        private float _turnAnimationAngle = 45.0f;
#endregion

#region Controls
        [SerializeField]
        private bool _invertMoveY = true;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.detectCollisions = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Update()
        {
            _lastInput.x = Input.GetAxis("P0 Horizontal");
            _lastInput.y = Input.GetAxis("P0 Vertical");

            _isBraking = Input.GetButton("P0 Button1");
            _isBoosting = Input.GetButton("P0 Button3");

            float dt = Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(_lastInput.y * -_turnAnimationAngle * (_invertMoveY ? -1.0f : 1.0f), 0.0f, _lastInput.x * -_turnAnimationAngle);
            _model.transform.localRotation = Quaternion.Lerp(_model.transform.localRotation, targetRotation, _turnAnimationSpeed * dt);

            _velocityText.text = $"Speed: {Speed:0.##} m/s";
            //_angularVelocityText.text = $"Angular Speed: {AngularSpeed:0.##} m/s";
            _angularVelocityText.text = $"Angular Velocity: {_rigidbody.angularVelocity} m/s";
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            Move(dt);
        }

        private void OnCollisionEnter(Collision collision)
        {
            _rigidbody.ResetCenterOfMass();
        }
#endregion

        private void Move(float dt)
        {
#region Angular Acceleration
            float turnAcceleration = _turnForce / _rigidbody.mass;

#if false
            _angularAcceleration = Vector3.up * (turnAcceleration * _lastInput.x);
            _rigidbody.angularVelocity += _angularAcceleration * dt;
#else
            Quaternion rotation = Quaternion.AngleAxis(turnAcceleration * _lastInput.x * dt, Vector3.up);
            _rigidbody.MoveRotation(transform.rotation * rotation);
#endif
#endregion

#region Linear Acceleration
            // vertical acceleration
            float verticalAcceleration = _maxVerticalForce / _rigidbody.mass;

            float ypct = _lastInput.y * (_invertMoveY ? -1.0f : 1.0f);
            verticalAcceleration *= ypct;

            // only fight gravity if we're not falling
            if(ypct >= 0.0f) {
                verticalAcceleration -= Physics.gravity.y;
            }

            // cap our fall speed
            if(verticalAcceleration < -_terminalVelocity) {
                verticalAcceleration = -_terminalVelocity;
            }

            // horizontal acceleration
            float horizontalAcceleration = _maxHorizontalForce / _rigidbody.mass;

            // modififers
            if(_isBraking) {
                float brakeAcceleration = _brakeForce / _rigidbody.mass;
                horizontalAcceleration -= brakeAcceleration;
            }

            if(_isBoosting) {
                float boostAcceleration = _boostForce / _rigidbody.mass;
                horizontalAcceleration += boostAcceleration;
            }

            // take away some horizontal if we're not falling
            if(verticalAcceleration > 0.0f) {
                horizontalAcceleration -= verticalAcceleration;
            }

            _acceleration = (horizontalAcceleration * transform.forward) + (verticalAcceleration * Vector3.up);
            _rigidbody.velocity += _acceleration * dt;
#endregion
        }
    }
}
