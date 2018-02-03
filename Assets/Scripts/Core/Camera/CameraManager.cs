using System.Collections.Generic;
using System.Linq;

using ggj2018.Core.Util;

using JetBrains.Annotations;

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

        private readonly List<BaseViewer> _assignedViewers = new List<BaseViewer>();

        private readonly Queue<BaseViewer> _unassignedViewers = new Queue<BaseViewer>();

        [SerializeField]
        private GameObject[] _postProcessObjectPrefabs;

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
                viewer.Initialize(i);
                viewer.gameObject.SetActive(false);

                _unassignedViewers.Enqueue(viewer);
            }

            ResizeViewports();
        }

        [CanBeNull]
        public BaseViewer AcquireViewer()
        {
            if(_unassignedViewers.Count < 1) {
                return null;
            }

            BaseViewer viewer = _unassignedViewers.Dequeue();
            viewer.gameObject.SetActive(true);
            _assignedViewers.Add(viewer);

            //Debug.Log($"Acquired viewer {viewer.name}");
            return viewer;
        }

        public void ReleaseViewer(BaseViewer viewer)
        {
            //Debug.Log($"Releasing viewer {viewer.name}");

            viewer.gameObject.SetActive(false);
            _assignedViewers.Remove(viewer);
            _unassignedViewers.Enqueue(viewer);
        }

        public void ResizeViewports()
        {
            if(_assignedViewers.Count > 0) {
                ResizeViewports(_assignedViewers);
            } else if(_unassignedViewers.Count > 0) {
                ResizeViewports(_unassignedViewers);
            }
        }

        private void ResizeViewports(IReadOnlyCollection<BaseViewer> viewers)
        {
            int gridCols = Mathf.CeilToInt(Mathf.Sqrt(viewers.Count));
            int gridRows = gridCols;

            // remove any extra full colums
            int extraCols = (gridCols * gridRows) - viewers.Count;
            gridCols -= extraCols / gridRows;

            float viewportWidth = (1.0f / gridCols);
            float viewportHeight = (1.0f / gridRows);

            Debug.Log($"Resizing {viewers.Count} viewports, Grid Size: {gridCols}x{gridRows} Viewport Size: {viewportWidth}x{viewportHeight}");

            for(int row=0; row<gridRows; ++row) {
                for(int col=0; col<gridCols; ++col) {
                    int viewerIdx = (row * gridCols) + col;
                    viewers.ElementAt(viewerIdx).SetViewport(col, (gridRows - 1) - row, viewportWidth, viewportHeight);
                }
            }
        }
    }
}
