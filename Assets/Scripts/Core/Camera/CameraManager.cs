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

            int gridSizeX = Mathf.CeilToInt(Mathf.Sqrt(count));
            int gridSizeY = gridSizeX;

// TODO: cut off unused rows
            /*int extra = (gridSizeX * gridSizeY) - count;
            gridSizeY = extra / gridSizeY;*/

            for(int i=0; i<count; ++i) {
                BaseViewer viewer = Instantiate(_viewerPrefab, _viewerContainer.transform);
                viewer.name = $"Viewer P{i}";
                _viewers.Add(viewer);
            }

// TODO: this maybe should go in ResizeViewports() ?
            float viewportWidth = (1.0f / gridSizeX);
            float viewportHeight = (1.0f / gridSizeY);

            for(int y=0; y<gridSizeY; ++y) {
                for(int x=0; x<gridSizeX; ++x) {
                    int viewerIdx = (gridSizeX * y) + x;
                    _viewers[viewerIdx].SetViewport(x, gridSizeY - 1 - y, viewportWidth, viewportHeight);
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

        public void ResizeViewports()
        {
            Debug.Log("Resizing viewports...");

            // TODO: probably a better way to do this...
            List<int> activeViewers = new List<int>();
            for(int i=0; i<_viewers.Count; ++i) {
                if(_viewers[i].gameObject.activeInHierarchy) {
                    activeViewers.Add(i);
                }
            }

            if(1 == activeViewers.Count) {
                _viewers[activeViewers[0]].SetViewport(0, 0, 1.0f, 1.0f);
            } else {
                // TODO
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
