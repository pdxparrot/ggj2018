using System.Collections.Generic;
using System.Linq;

using ggj2018.Core.Util;

using UnityEngine;

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

        [SerializeField]
        private GameObject[] _postProcessObjectPrefabs;

        public IReadOnlyCollection<BaseViewer> Viewers => _viewers;

        private GameObject _viewerContainer, _postProcessContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _viewerContainer = new GameObject("Viewers");
            _postProcessContainer = new GameObject("Post Process");

            foreach(GameObject ppp in _postProcessObjectPrefabs) {
                Instantiate(ppp, _postProcessContainer.transform);
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_postProcessContainer);
            _postProcessContainer = null;

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
                viewer.UICamera.name = $"UI Camera P{i}";
                _viewers.Add(viewer);
            }

            ResizeViewports();
        }

        public void EnableCamera(int i, bool active)
        {
            _viewers[i].gameObject.SetActive(active);
        }

        public void ResizeViewports()
        {
            var activeViewers = Viewers.Where(x => x.gameObject.activeInHierarchy).ToList();

            Debug.Log($"Resizing {_viewers.Count} viewports, found {activeViewers.Count} active...");

            int gridCols = Mathf.CeilToInt(Mathf.Sqrt(activeViewers.Count));
            int gridRows = gridCols;

            // remove any extra full colums
            int extraCols = (gridCols * gridRows) - activeViewers.Count;
            gridCols -= extraCols / gridRows;

            float viewportWidth = (1.0f / gridCols);
            float viewportHeight = (1.0f / gridRows);

            for(int row=0; row<gridRows; ++row) {
                for(int col=0; col<gridCols; ++col) {
                    int viewerIdx = (row * gridCols) + col;
                    activeViewers[viewerIdx].SetViewport(col, (gridRows - 1) - row, viewportWidth, viewportHeight);
                }
            }
        }

        public void AddRenderLayer(int viewerIndex, string layer)
        {
            AddRenderLayer(viewerIndex, LayerMask.NameToLayer(layer));
        }

        public void AddRenderLayer(int viewerIndex, LayerMask layer)
        {
            _viewers[viewerIndex].Camera.cullingMask |= (1 << layer.value);
        }

        public void RemoveRenderLayer(int viewerIndex, string layer)
        {
            RemoveRenderLayer(viewerIndex, LayerMask.NameToLayer(layer));
        }

        public void RemoveRenderLayer(int viewerIndex, LayerMask layer)
        {
            _viewers[viewerIndex].Camera.cullingMask &= ~(1 << layer.value);
        }
    }
}
