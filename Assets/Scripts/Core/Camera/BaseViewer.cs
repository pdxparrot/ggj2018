using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Camera
{
    [RequireComponent(typeof(FollowCamera))]
    public abstract class BaseViewer : MonoBehavior
    {
        [SerializeField]
        private UnityEngine.Camera _camera;

        public UnityEngine.Camera Camera => _camera;

        [SerializeField]
        private UnityEngine.Camera _noPostProcessCamera;

        public UnityEngine.Camera NoPostProcessCamera => _noPostProcessCamera;

        [SerializeField]
        private UnityEngine.Camera _uiCamera;

        public UnityEngine.Camera UICamera => _uiCamera;

        private FollowCamera _followCamera;

        public FollowCamera FollowCamera => _followCamera;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _followCamera = GetComponent<FollowCamera>();
        }
#endregion

        public void SetViewport(int x, int y, float viewportWidth, float viewportHeight)
        {
            float viewportX = x * viewportWidth;
            float viewportY = y * viewportHeight;

            Rect viewport = new Rect(
                viewportX + CameraManager.Instance.ViewportEpsilon,
                viewportY + CameraManager.Instance.ViewportEpsilon,
                viewportWidth - (CameraManager.Instance.ViewportEpsilon * 2),
                viewportHeight - (CameraManager.Instance.ViewportEpsilon * 2));

            Camera.rect = viewport;
            NoPostProcessCamera.rect = viewport;
            UICamera.rect = viewport;

            AspectRatio aspectRatio = UICamera.GetComponent<AspectRatio>();
            if(null != aspectRatio) {
                aspectRatio.UpdateAspectRatio();
            }
        }

        public void SetFov(float fov)
        {
            Camera.fieldOfView = fov;
            NoPostProcessCamera.fieldOfView = fov;
        }
    }
}
