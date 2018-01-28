using ggj2018.Core.Input;
using ggj2018.Core.Math;
using ggj2018.Core.Util;

using JetBrains.Annotations;

using UnityEngine;

namespace ggj2018.Core.Camera
{
    public sealed class FollowCamera : MonoBehavior
    {
#region Orbit Config
        [SerializeField]
        private bool _enableOrbit = true;

        [SerializeField]
        private float _orbitSpeedX = 100.0f;

        [SerializeField]
        private float _orbitSpeedY = 100.0f;
#endregion

#region Orbit Constraints
        [SerializeField]
        private float _orbitXMin = -360.0f;

        [SerializeField]
        private float _orbitXMax = 360.0f;

        [SerializeField]
        private float _orbitYMin = -360.0f;

        [SerializeField]
        private float _orbitYMax = 360.0f;
#endregion

#region Zoom Config
        [SerializeField]
        private bool _enableZoom = false;

        [SerializeField]
        private float _minZoomDistance = 5.0f;

        [SerializeField]
        private float _maxZoomDistance = 100.0f;

        [SerializeField]
        private float _zoomSpeed = 500.0f;
#endregion

#region Look Config
        [SerializeField]
        private bool _enableLook = false;

        [SerializeField]
        private float _lookSpeedX = 100.0f;

        [SerializeField]
        private float _lookSpeedY = 100.0f;
#endregion

#region Target
        [SerializeField]
        [CanBeNull]
        private GameObject _target;

        [CanBeNull]
        public GameObject Target => _target;

        [SerializeField]
        [CanBeNull]
        private Collider _targetCollider;
#endregion

#region Controller
        [SerializeField]
        private int _controllerIndex = 0;
#endregion

        [SerializeField]
        [ReadOnly]
        private Vector2 _orbitRotation;

        [SerializeField]
        [ReadOnly]
        private float _orbitRadius = 25.0f;

        [SerializeField]
        [ReadOnly]
        private Vector2 _lookRotation;

#region Unity Lifecycle
        private void Update()
        {
            if(null == Target) {
                return;
            }

            HandleInput(Time.deltaTime);
        }

        private void LateUpdate()
        {
            FollowTarget();
        }
#endregion

        public void SetTarget(GameObject target)
        {
            _target = target;
            _targetCollider = Target?.GetComponentInChildren<Collider>();   // :(

            IFollowTarget followTarget = Target?.GetComponent<IFollowTarget>();
            if(null != followTarget) {
                _controllerIndex = followTarget.ControllerNumber;
            }
        }

        private void HandleInput(float dt)
        {
            if(!InputManager.HasInstance) {
                return;
            }

            Vector3 axes = InputManager.Instance.GetLookAxes(_controllerIndex);

            Orbit(axes, dt);
            Zoom(axes, dt);
            Look(axes, dt);
        }

        private void Orbit(Vector3 axes, float dt)
        {
            if(!_enableOrbit) {
                return;
            }

            _orbitRotation.x = Mathf.Clamp(MathHelper.WrapAngle(_orbitRotation.x + axes.x * _orbitSpeedX * dt), _orbitXMin, _orbitXMax);
            _orbitRotation.y = Mathf.Clamp(MathHelper.WrapAngle(_orbitRotation.y - axes.y * _orbitSpeedY * dt), _orbitYMin, _orbitYMax);
        }

        private void Zoom(Vector3 axes, float dt)
        {
            if(!_enableZoom) {
                return;
            }

            float zoomAmount = axes.z * _zoomSpeed * dt;

            float minDistance = _minZoomDistance, maxDistance = _maxZoomDistance;
            if(null != Target) {
                // avoid zooming into the target
                Vector3 closestBoundsPoint = _targetCollider?.ClosestPointOnBounds(transform.position) ?? Target.transform.position;
                float distanceToPoint = (closestBoundsPoint - Target.transform.position).magnitude;

                minDistance += distanceToPoint;
                maxDistance += distanceToPoint;

                _orbitRadius = Mathf.Clamp(_orbitRadius + zoomAmount, minDistance, maxDistance);
            } else {
                _orbitRadius += zoomAmount;
            }
        }

        private void Look(Vector3 axes, float dt)
        {
            if(!_enableLook) {
                return;
            }

            _lookRotation.x = MathHelper.WrapAngle(_lookRotation.x + axes.x * _lookSpeedX * dt);
            _lookRotation.y = MathHelper.WrapAngle(_lookRotation.y - axes.y * _lookSpeedY * dt);
        }

        private void FollowTarget()
        {
            Quaternion orbitRotation = Quaternion.Euler(_orbitRotation.y, _orbitRotation.x, 0.0f);
            Quaternion lookRotation = Quaternion.Euler(_lookRotation.y, _lookRotation.x, 0.0f);
            Quaternion targetRotation = null == Target ? Quaternion.identity : Target.transform.rotation;

            Quaternion finalOrbitRotation = orbitRotation * targetRotation;
            transform.rotation = finalOrbitRotation * lookRotation;

            // TODO: this doens't work if we free-look and zoom
            // because we're essentially moving the target position, not the camera position
            Vector3 targetPosition = null == Target ? (transform.position + (transform.forward * _orbitRadius)) : Target.transform.position;
            transform.position = targetPosition + finalOrbitRotation * new Vector3(0.0f, 0.0f, -_orbitRadius);
        }
    }
}
