using System.Collections.Generic;

using pdxpartyparrot.Core.UI;
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

        private DebugWindow _window;

        private readonly List<DebugMenuNode> _nodes = new List<DebugMenuNode>();

        private DebugMenuNode _currentNode;

#region Unity Lifecycle
        private void Awake()
        {
            _window = new DebugWindow(new Rect(10, 10, 800, 600), RenderWindowContents)
            {
                Title = "Debug Menu"
            };

            AddNode("Test");
        }

        private void Update()
        {
            if(UnityEngine.Input.GetKeyDown(_enableKeyCode)) {
                _enabled = !_enabled;
            }

            if(_enabled) {
                _window.Update();
            }
        }

        private void OnGUI()
        {
            if(!_enabled) {
                return;
            }

            _window.Render();
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
            _window.Title = null == _currentNode ? "Debug Menu" : _currentNode.Title;
        }

        private void RenderWindowContents()
        {
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
        }
    }
}
