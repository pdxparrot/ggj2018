using ggj2018.Core.Input;
using ggj2018.Core.Util;

using JetBrains.Annotations;

using UnityEngine;

namespace ggj2018.Core.Camera
{
    public sealed class FollowCamera : MonoBehavior
    {
#region Zoom Config
        [SerializeField]
        private bool _enableZoom = false;

        [SerializeField]
        private float _minZoomDistance = 5.0f;

        [SerializeField]
        private float _maxZoomDistance = 100.0f;

        [SerializeField]
        private float _zoomSpeed = 500.0f;

        [SerializeField]
        private bool _invertZoomDirection = false;
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
        private float _orbitRadius = 25.0f;

#region Unity Lifecycle
        private void Update()
        {
            if(!InputManager.HasInstance) {
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
        }

        private void HandleInput(float dt)
        {
            Vector3 axes = InputManager.Instance.GetAxes();

            Zoom(axes, dt);
        }

        private void Zoom(Vector3 pointerAxis, float dt)
        {
            if(!_enableZoom) {
                return;
            }

            float zoomAmount = pointerAxis.z * _zoomSpeed * dt * (_invertZoomDirection ? -1 : 1);

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

        private void FollowTarget()
        {
            Vector3 targetPosition = null == Target ? (transform.position + (transform.forward * _orbitRadius)) : Target.transform.position;
            transform.position = targetPosition + new Vector3(0.0f, 0.0f, -_orbitRadius);
        }
    }
}