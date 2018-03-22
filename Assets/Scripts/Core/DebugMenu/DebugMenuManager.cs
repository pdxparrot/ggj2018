using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public sealed class DebugMenuManager : SingletonBehavior<DebugMenuManager>
    {
        [SerializeField]
        private KeyCode _enableKeyCode = KeyCode.BackQuote;

        [SerializeField]
        private bool _enabled;

        [SerializeField]
        [ReadOnly]
        private int _windowId = 0;

        [SerializeField]
        private Rect _windowRect = new Rect(10, 10, 800, 600);

        private readonly List<DebugMenuNode> _nodes = new List<DebugMenuNode>();

        private DebugMenuNode _currentNode;

#region Unity Lifecycle
        private void Awake()
        {
            AddNode("Test");
        }

        private void Update()
        {
            if(UnityEngine.Input.GetKeyDown(_enableKeyCode)) {
                _enabled = !_enabled;
            }
        }

        private void OnGUI()
        {
            if(!_enabled) {
                return;
            }

            string title =  null == _currentNode ? "Debug Menu" : _currentNode.Title;

            _windowRect = GUILayout.Window(_windowId, _windowRect, id => {
                if(null == _currentNode) {
                    foreach(DebugMenuNode node in _nodes) {
                        node.Render();
                    }

                    if(GUILayout.Button("Quit", GUILayout.Width(100), GUILayout.Height(25))) {
                        Application.Quit();
                    }
                } else {
                    _currentNode.RenderChildren();
                    if(GUILayout.Button("Back", GUILayout.Width(100), GUILayout.Height(25))) {
                        SetCurrentNode(_currentNode.Parent);
                    }
                }

                GUI.DragWindow();
            }, title, GUILayout.MinWidth(200), GUILayout.MinHeight(200));
        }
#endregion

        public DebugMenuNode AddNode(string title)
        {
            DebugMenuNode node = new DebugMenuNode(title);
            _nodes.Add(node);
            return node;
        }

        public void SetCurrentNode(DebugMenuNode node)
        {
            _currentNode = node;
        }
    }
}
