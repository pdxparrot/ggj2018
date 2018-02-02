using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.Core.Camera
{
    [RequireComponent(typeof(FollowCamera))]
    public abstract class BaseViewer : MonoBehavior
    {
        [SerializeField]
        [ReadOnly]
        private int _id;

        public int Id => _id;

        [SerializeField]
        private UnityEngine.Camera _camera;

        protected UnityEngine.Camera Camera => _camera;

        [SerializeField]
        private UnityEngine.Camera _uiCamera;

        protected UnityEngine.Camera UICamera => _uiCamera;

        private FollowCamera _followCamera;

        public FollowCamera FollowCamera => _followCamera;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _followCamera = GetComponent<FollowCamera>();
        }
#endregion

        public void Initialize(int id)
        {
            _id = id;

            name = $"Viewer P{Id}";
            Camera.name = $"Camera P{Id}";
            UICamera.name = $"UI Camera P{Id}";
        }

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
            UICamera.rect = viewport;

            AspectRatio aspectRatio = UICamera.GetComponent<AspectRatio>();
            if(null != aspectRatio) {
                aspectRatio.UpdateAspectRatio();
            }
        }

        public void SetFov(float fov)
        {
            Camera.fieldOfView = fov;
        }

        public void SetPostProcessLayer(LayerMask postProcessLayerMask)
        {
            PostProcessLayer layer = Camera.GetComponent<PostProcessLayer>();
            layer.volumeLayer = postProcessLayerMask;
        }

#region Render Layers
        public void AddRenderLayer(string layer)
        {
            AddRenderLayer(LayerMask.NameToLayer(layer));
        }

        public void AddRenderLayer(LayerMask layer)
        {
            Camera.cullingMask |= (1 << layer.value);
        }

        public void RemoveRenderLayer(string layer)
        {
            RemoveRenderLayer(LayerMask.NameToLayer(layer));
        }

        public void RemoveRenderLayer(LayerMask layer)
        {
            Camera.cullingMask &= ~(1 << layer.value);
        }
#endregion
    }
}
