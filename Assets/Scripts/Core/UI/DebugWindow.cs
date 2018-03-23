using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    public class DebugWindow
    {
// TODO: this should support a lot more window-type options
// like "allow resize" and "allow move" and all that usual stuff

        private const int WindowHandleSize = 25;

        private static int _windowIdGenerator = 0;

        private static int NextId => ++_windowIdGenerator;

        [SerializeField]
        [ReadOnly]
        private int _id = NextId;

        public int Id => _id;

        [SerializeField]
        private Rect _rect;

#region Resizing
        [SerializeField]
        [ReadOnly]
        private bool _isResizing;

        [SerializeField]
        [ReadOnly]
        private Rect _resizeOriginalRect;

        [SerializeField]
        [ReadOnly]
        private Vector2 _resizeStartPosition;
#endregion

        public string Title { get; set; } = "Debug Window";

        private readonly Action _renderContents;

        public DebugWindow(Rect rect, Action renderContents)
        {
            _rect = rect;
            _renderContents = renderContents;
        }

        public void Update()
        {
            Resize();
        }

        public void Render()
        {
            _rect = GUILayout.Window(_id, _rect, id => {
                _renderContents();

                // TODO: we should limit dragging only to when it's done from the title bar
                if(!_isResizing) {
                    GUI.DragWindow();
                }
            }, Title);
        }

        private void Resize()
        {
            // https://forum.unity.com/threads/is-there-a-resize-equivalent-to-gui-dragwindow.10144/
            var mousePosition = UnityEngine.Input.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y;

            Rect windowHandle = new Rect(_rect.x + _rect.width - WindowHandleSize, _rect.y + _rect.height - WindowHandleSize, WindowHandleSize, WindowHandleSize);

            if(UnityEngine.Input.GetMouseButtonDown(0) && windowHandle.Contains(mousePosition)) {
                _isResizing = true;
                _resizeStartPosition = mousePosition;
                _resizeOriginalRect = _rect;
            }

            if(_isResizing) {
                if(UnityEngine.Input.GetMouseButton(0)) {
                    _rect.width = Mathf.Clamp(_resizeOriginalRect.width + (mousePosition.x - _resizeStartPosition.x), 0, Screen.width);
                    _rect.height = Mathf.Clamp(_resizeOriginalRect.height + (mousePosition.y - _resizeStartPosition.y), 0, Screen.height);
                }

                if(UnityEngine.Input.GetMouseButtonUp(0)) {
                    _isResizing = false;
                }
            }
        }
    }
}
