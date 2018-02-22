﻿using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.Util;

using JetBrains.Annotations;

using UnityEngine;

namespace pdxpartyparrot.Core.Camera
{
    public sealed class FollowCamera : MonoBehavior
    {
#region Orbit Config
        [Header("Orbit")]

        [SerializeField]
        private bool _enableOrbit = true;

        [SerializeField]
        private float _orbitSpeedX = 100.0f;

        [SerializeField]
        private float _orbitSpeedY = 100.0f;

        [SerializeField]
        private bool _returnToDefault;

        [SerializeField]
        private Vector2 _defaultOrbitRotation;

        [SerializeField]
        private float _defaultOrbitReturnTime = 0.5f;

        [SerializeField]
        [ReadOnly]
        private Vector2 _orbitReturnVelocity;

        [SerializeField]
        [ReadOnly]
        private Vector2 _orbitRotation;

        [SerializeField]
        private float _orbitRadius = 25.0f;

        public float OrbitRadius { get { return _orbitRadius; } set { _orbitRadius = value; } }
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

        [Space(10)]

#region Zoom Config
        [Header("Zoom")]

        [SerializeField]
        private bool _enableZoom = false;

        [SerializeField]
        private float _minZoomDistance = 5.0f;

        [SerializeField]
        private float _maxZoomDistance = 100.0f;

        [SerializeField]
        private float _zoomSpeed = 500.0f;
#endregion

        [Space(10)]

#region Look Config
        [Header("Look")]

        [SerializeField]
        private bool _enableLook = false;

        [SerializeField]
        private float _lookSpeedX = 100.0f;

        [SerializeField]
        private float _lookSpeedY = 100.0f;

        [SerializeField]
        [ReadOnly]
        private Vector2 _lookRotation;
#endregion

        [Space(10)]

#region Target
        [Header("Target")]

        [SerializeField]
        [CanBeNull]
        private IFollowTarget _target;

        [CanBeNull]
        public IFollowTarget Target => _target;
#endregion

        [Space(10)]

        [SerializeField]
        private bool _smooth;

        [SerializeField]
        private float _smoothTime = 0.05f;

        [SerializeField]
        [ReadOnly]
        private Vector3 _velocity;

#region Unity Lifecycle
        private void Update()
        {
            float dt = Time.deltaTime;

            HandleInput(dt);
        }

        private void LateUpdate()
        {
            if(_smooth) {
                return;
            }

            float dt = Time.deltaTime;

            FollowTarget(dt);
        }

        private void FixedUpdate()
        {
            if(!_smooth) {
                return;
            }

            float dt = Time.fixedDeltaTime;

            FollowTarget(dt);
        }
#endregion

        public void SetTarget(IFollowTarget target)
        {
            _target = target;
            _orbitRotation = _defaultOrbitRotation;
        }

        private void HandleInput(float dt)
        {
            if(null == Target) {
                return;
            }

            Vector3 axes = Target.LookAxis;

            Orbit(axes, dt);
            Zoom(axes, dt);
            Look(axes, dt);
        }

        private void Orbit(Vector3 axes, float dt)
        {
            if(!_enableOrbit) {
                return;
            }

            // TODO: this is fighting too hard at max rotation
            // (or maybe the max rotation clamping is killing it)
            if(_returnToDefault) {
                _orbitRotation = Vector2.SmoothDamp(_orbitRotation, _defaultOrbitRotation, ref _orbitReturnVelocity, _defaultOrbitReturnTime, Mathf.Infinity, dt);
            }

            _orbitRotation.x = Mathf.Clamp(MathUtil.WrapAngle(_orbitRotation.x + axes.x * _orbitSpeedX * dt), _orbitXMin, _orbitXMax);
            _orbitRotation.y = Mathf.Clamp(MathUtil.WrapAngle(_orbitRotation.y - axes.y * _orbitSpeedY * dt), _orbitYMin, _orbitYMax);
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
                Vector3 closestBoundsPoint = Target.Collider.ClosestPointOnBounds(transform.position);
                float distanceToPoint = (closestBoundsPoint - Target.GameObject.transform.position).magnitude;

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

            _lookRotation.x = MathUtil.WrapAngle(_lookRotation.x + axes.x * _lookSpeedX * dt);
            _lookRotation.y = MathUtil.WrapAngle(_lookRotation.y - axes.y * _lookSpeedY * dt);
        }

        private void FollowTarget(float dt)
        {
            if(null == Target) {
                return;
            }

            if(Target.IsPaused) {
                return;
            }

            Quaternion orbitRotation = Quaternion.Euler(_orbitRotation.y, _orbitRotation.x, 0.0f);
            Quaternion lookRotation = Quaternion.Euler(_lookRotation.y, _lookRotation.x, 0.0f);

            Quaternion targetRotation = Quaternion.Euler(0.0f, Target.GameObject.transform.eulerAngles.y, 0.0f);

            Quaternion finalOrbitRotation = targetRotation * orbitRotation;
            transform.rotation = finalOrbitRotation * lookRotation;

            // TODO: this doens't work if we free-look and zoom
            // because we're essentially moving the target position, not the camera position
            Vector3 targetPosition = Target.GameObject.transform.position;
            targetPosition = _smooth
                ? Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime)
                : targetPosition;

            transform.position = targetPosition + finalOrbitRotation * new Vector3(0.0f, 0.0f, -_orbitRadius);
        }
    }
}
