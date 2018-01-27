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

        [SerializeField]
        private bool _invertOrbitY = false;
#endregion

#region Zoom Config
        [SerializeField]
        private bool _enableZoom = true;

        [SerializeField]
        private float _minZoomDistance = 5.0f;

        [SerializeField]
        private float _maxZoomDistance = 100.0f;

        [SerializeField]
        private float _zoomSpeed = 500.0f;

        [SerializeField]
        private bool _invertZoomDirection = false;
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
        }

        private void HandleInput(float dt)
        {
            Vector3 axes = InputManager.Instance.GetLookAxes();
            Orbit(axes, dt);
            Zoom(axes, dt);
            Look(axes, dt);
        }

        private void Orbit(Vector3 axes, float dt)
        {
            if(!_enableOrbit) {
                return;
            }

            float orbitXAmount = _orbitRotation.x + axes.x * _orbitSpeedX * dt;
            _orbitRotation.x = MathHelper.WrapAngle(orbitXAmount);

            float orbitYAmount = _orbitRotation.y - axes.y * _orbitSpeedY * dt * (_invertOrbitY ? -1 : 1);
            _orbitRotation.y = MathHelper.WrapAngle(orbitYAmount);
        }

        private void Zoom(Vector3 axes, float dt)
        {
            if(!_enableZoom) {
                return;
            }

            float zoomAmount = axes.z * _zoomSpeed * dt * (_invertZoomDirection ? -1 : 1);

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

        private void Look(Vector3 pointerAxis, float dt)
        {
            if(!_enableLook) {
                return;
            }

            _lookRotation.x = MathHelper.WrapAngle(_lookRotation.x + pointerAxis.x * _lookSpeedX * dt);
            _lookRotation.y = MathHelper.WrapAngle(_lookRotation.y - pointerAxis.y * _lookSpeedY * dt);
        }

        private void FollowTarget()
        {
            Quaternion orbitRotation = Quaternion.Euler(_orbitRotation.y, _orbitRotation.x, 0.0f);
            Quaternion lookRotation = Quaternion.Euler(_lookRotation.y, _lookRotation.x, 0.0f);

            transform.rotation = orbitRotation * lookRotation;

            // TODO: this doens't work if we free-look and zoom
            // because we're essentially moving the target position, not the camera position
            Vector3 targetPosition = null == Target ? (transform.position + (transform.forward * _orbitRadius)) : Target.transform.position;
            transform.position = targetPosition + orbitRotation * new Vector3(0.0f, 0.0f, -_orbitRadius);
        }
    }
}