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

        [SerializeField]
        private GameObject _cameraStoragePrefab;

        private GameObject _cameraStorage;

        private readonly List<BaseViewer> _viewers = new List<BaseViewer>();

        private GameObject _cameraContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _cameraContainer = new GameObject("Cameras");

            _cameraStorage = Instantiate(_cameraStoragePrefab, transform);
        }

        protected override void OnDestroy()
        {
            Destroy(_cameraStorage);
            Destroy(_cameraContainer);
        }
#endregion

        public void SpawnViewers(int count)
        {
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

            for(int i=0; i<count; ++i) {
                BaseViewer viewer = Instantiate(_viewerPrefab, _cameraContainer.transform);
                viewer.name = $"Camera P{i}";
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
            return _viewers[i];
        }

        public void SetupCamera(int i, bool active)
        {
            _viewers[i].FollowCamera.enabled = active;

            // -- disabled cameras go in the black box under the world
            if(!active) {
                _viewers[i].transform.position = new Vector3(0,-100,0);
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
