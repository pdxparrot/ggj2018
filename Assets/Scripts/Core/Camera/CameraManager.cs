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

#region Viewers
        [SerializeField]
        private BaseViewer _viewerPrefab;

        private readonly List<BaseViewer> _viewers = new List<BaseViewer>();

        private readonly List<BaseViewer> _assignedViewers = new List<BaseViewer>();

        private readonly Queue<BaseViewer> _unassignedViewers = new Queue<BaseViewer>();
#endregion

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
                viewer.Initialize(i);
                viewer.gameObject.SetActive(false);

                _viewers.Add(viewer);
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

            //Debug.Log($"Acquired viewer {viewer.name}  (assigned: {_assignedViewers.Count}, unassigned: {_unassignedViewers.Count})");
            return viewer;
        }

        public void ReleaseViewer(BaseViewer viewer)
        {
            if(!_assignedViewers.Contains(viewer)) {
                return;
            }

            //Debug.Log($"Releasing viewer {viewer.name} (assigned: {_assignedViewers.Count}, unassigned: {_unassignedViewers.Count})");

            viewer.Reset();

            viewer.gameObject.SetActive(false);
            _assignedViewers.Remove(viewer);
            _unassignedViewers.Enqueue(viewer);
        }

        public void ResetViewers()
        {
            Debug.Log($"Releasing all ({_assignedViewers.Count}) viewers");

            // we loop through all of the viewers
            // because we can't loop over the assigned viewers
            foreach(BaseViewer viewer in _viewers) {
                ReleaseViewer(viewer);

                viewer.transform.position = Vector3.zero;
                viewer.transform.rotation = Quaternion.identity;
            }
            ResizeViewports();
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
                    if(viewerIdx >= viewers.Count) {
                        break;
                    }
                    viewers.ElementAt(viewerIdx).SetViewport(col, (gridRows - 1) - row, viewportWidth, viewportHeight);
                }
            }
        }
    }
}
