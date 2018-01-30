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
            _viewerContainer = null;
        }
#endregion

        public void SpawnViewers(int count)
        {
            Debug.Log($"Spawning {count} viewers...");

            for(int i=0; i<count; ++i) {
                BaseViewer viewer = Instantiate(_viewerPrefab, _viewerContainer.transform);
                viewer.name = $"Viewer P{i}";
                viewer.Camera.name = $"Camera P{i}";
                viewer.NoPostProcessCamera.name = $"No PP Camera P{i}";
                viewer.UICamera.name = $"UI Camera P{i}";
                _viewers.Add(viewer);
            }

            ResizeViewports();
        }

        public BaseViewer GetViewer(int i)
        {
            return i >= _viewers.Count ? null : _viewers[i];
        }

        public void SetupCamera(int i, bool active)
        {
            _viewers[i].gameObject.SetActive(active);
        }

        public void ResizeViewports()
        {
            // TODO: probably a better way to do this...
            var activeViewers = new List<int>();
            for(int i=0; i<_viewers.Count; ++i) {
                if(_viewers[i].gameObject.activeInHierarchy) {
                    activeViewers.Add(i);
                }
            }

            Debug.Log($"Resizing {_viewers.Count} viewports, found {activeViewers.Count} active...");

            // TODO: would be cooler if we favored wider rows rather than taller columns

            int gridSizeX = Mathf.CeilToInt(Mathf.Sqrt(activeViewers.Count));
            int gridSizeY = gridSizeX;

            int extra = (gridSizeX * gridSizeY) - activeViewers.Count;
            gridSizeY = gridSizeY - (extra / gridSizeX);

            float viewportWidth = (1.0f / gridSizeX);
            float viewportHeight = (1.0f / gridSizeY);

            for(int y=0; y<gridSizeY; ++y) {
                for(int x=0; x<gridSizeX; ++x) {
                    int viewerIdx = (gridSizeX * y) + x;
                    _viewers[activeViewers[viewerIdx]].SetViewport(x, gridSizeY - 1 - y, viewportWidth, viewportHeight);
                }
            }
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
