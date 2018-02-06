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
        private float _horizontalForce = 25.0f;

        [SerializeField]
        private float _verticalSpeed = 10.0f;

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
        private Vector3 _horizontalAcceleration;

        [SerializeField]
        [ReadOnly]
        private Vector3 _horizontalVelocity;

        [SerializeField]
        [ReadOnly]
        private Vector3 _verticalVelocity;

        [SerializeField]
        [ReadOnly]
        private float _lastVerticalSpeed;

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
            float turnAccelerationMagnitude = _turnForce / _rigidbody.mass;

            _angularAcceleration = Vector3.up * (turnAccelerationMagnitude * _lastInput.x);
            _rigidbody.angularVelocity += _angularAcceleration * dt;
#endregion

#region Linear Acceleration
            // horizontal acceleration
            float horizontalAcceleration = _horizontalForce / _rigidbody.mass;

            if(_isBraking) {
                float brakeAcceleration = _brakeForce / _rigidbody.mass;
                horizontalAcceleration -= brakeAcceleration;
            }

            if(_isBoosting) {
                float boostAcceleration = _boostForce / _rigidbody.mass;
                horizontalAcceleration += boostAcceleration;
            }

            _horizontalAcceleration = transform.forward * horizontalAcceleration;

            // vertical speed
            float verticalPct = _lastInput.y * (_invertMoveY ? -1.0f : 1.0f);
            _verticalSpeed = _verticalSpeed * verticalPct;

            // fight gravity
            _verticalSpeed -= Physics.gravity.y;

            // clamp
            _verticalSpeed = Mathf.Clamp(_verticalSpeed, -_terminalVelocity, Mathf.Infinity);

            // adjust
            _verticalVelocity = Vector3.up * _verticalSpeed * dt;
            _horizontalVelocity = (_horizontalAcceleration * dt) - (transform.forward * _verticalVelocity.magnitude);

            _rigidbody.velocity += _horizontalVelocity + _verticalVelocity;


/*
            float maxHorizontalAcceleration = _maxHorizontalForce / _rigidbody.mass;
            float maxVerticalAcceleration = _maxVerticalForce / _rigidbody.mass;

            // start with 100% horizontal acceleration
            float linearAccelerationMagnitudeX = maxHorizontalAcceleration;
            float linearAccelerationMagnitudeY = 0.0f;

            // use some vertical to fight gravity unless we're falling
            float verticalPct = _lastInput.y * (_invertMoveY ? -1.0f : 1.0f);
            if(verticalPct >= -float.Epsilon) {
                linearAccelerationMagnitudeY -= Physics.gravity.y;
            }

            // the rest of the veritcal is determined by the stick
            linearAccelerationMagnitudeY += verticalPct * maxVerticalAcceleration;

            if(_isBraking) {
                float brakeAccelerationMagnitude = _brakeForce / _rigidbody.mass;
                linearAccelerationMagnitudeX -= brakeAccelerationMagnitude;
            }

            if(_isBoosting) {
                float boostAccelerationMagnitude = _boostForce / _rigidbody.mass;
                linearAccelerationMagnitudeX += boostAccelerationMagnitude;
            }


            linearAccelerationMagnitudeX -= Mathf.Abs(linearAccelerationMagnitudeY);
            _linearAcceleration = (transform.forward * linearAccelerationMagnitudeX) + (Vector3.up * linearAccelerationMagnitudeY);

            _rigidbody.velocity += _linearAcceleration * dt;
*/

/*
            // gravity
            float gravityPct = Physics.gravity.magnitude / linearAccelerationMagnitude;
            linearAccelerationDirection += Vector3.up * gravityPct;

            // up/down
            float verticalPct = _lastInput.y * (_invertMoveY ? -1.0f : 1.0f);
            linearAccelerationDirection += Vector3.up * verticalPct;

            // braking
            if(_isBraking) {
                float brakeAccelerationMagnitude = _brakeForce / _rigidbody.mass;
                linearAccelerationMagnitude -= brakeAccelerationMagnitude;
            }

            // boosting
            if(_isBoosting) {
                float boostAccelerationMagnitude = _boostForce / _rigidbody.mass;
                linearAccelerationMagnitude += boostAccelerationMagnitude;
            }

            _linearAcceleration = linearAccelerationDirection.normalized * linearAccelerationMagnitude;

            _rigidbody.velocity += _linearAcceleration * dt;
*/

            // overcome gravity
            //_linearAcceleration = -Physics.gravity;

            // up/down
            //_linearAcceleration += Vector3.up * flapAcceleration * (_lastInput.y * (_invertMoveY ? -1.0f : 1.0f));

            // move forward
            //_linearAcceleration += Vector3.forward * flapAcceleration;

            //float brakeAcceleration = _brakeForce / _rigidbody.mass;

            // modifiers
            //if(_isBraking) {
                //_linearAcceleration += Vector3.forward * brakeAcceleration;
            //}

            //float boostAcceleration = _boostForce / _rigidbody.mass;

            //if(_isBoosting) {
                //_linearAcceleration += Vector3.forward * boostAcceleration;
            //}

            //_rigidbody.velocity += (transform.rotation * _linearAcceleration) * _rigidbody.mass * dt;
#endregion
        }
    }
}
