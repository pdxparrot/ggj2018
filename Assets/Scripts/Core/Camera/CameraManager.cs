using System;
using ggj2018.Core.Util;

using UnityEngine;
using System.Collections.Generic;

namespace ggj2018.Core.Camera
{
    public sealed class CameraManager : SingletonBehavior<CameraManager>
    {
        [SerializeField]
        private float _viewportEpsilon = Mathf.Epsilon;

        public float ViewportEpsilon => _viewportEpsilon;

        [SerializeField]
        private BaseViewer _viewerPrefab;

        private readonly List<BaseViewer> _viewers = new List<BaseViewer>();

        private GameObject _viewerContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _viewerContainer = new GameObject("Viewers");
        }

        protected override void OnDestroy()
        {
            Destroy(_viewerContainer);
        }
#endregion

        public void SpawnViewers(int count)
        {
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

            for(int i=0; i<count; ++i) {
                BaseViewer viewer = Instantiate(_viewerPrefab, _viewerContainer.transform);
                viewer.name = $"Viewer P{i}";
                _viewers.Add(viewer);
            }

            float viewportWidth = (1.0f / gridSize);
            float viewportHeight = (1.0f / gridSize);

            for(int y=0; y<gridSize; ++y) {
                for(int x=0; x<gridSize; ++x) {
                    int viewerIdx = (gridSize * y) + x;
                    _viewers[viewerIdx].SetViewport(x, gridSize - 1 - y, viewportWidth, viewportHeight);
                }
            }
        }

        public BaseViewer GetViewer(int i)
        {
            return i >= _viewers.Count ? null : _viewers[i];
        }

        public void SetupCamera(int i, bool active)
        {
            _viewers[i].gameObject.SetActive(active);
        }

        public enum Layer { Hawk, Carrier }

        public void SetLayer(int cam, Layer layer) {
            _viewers[cam].Camera.cullingMask &= ~(1 << 8);
            _viewers[cam].Camera.cullingMask &= ~(1 << 9);

            if(layer == Layer.Hawk) {
                _viewers[cam].Camera.cullingMask |= (1 << 8);
            }
            else {
                _viewers[cam].Camera.cullingMask |= (1 << 9);
            }
        }
    }
}
