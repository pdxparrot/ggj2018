using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
// TODO: collect logs in a log window here also

    public sealed class DebugMenuManager : SingletonBehavior<DebugMenuManager>
    {
        [SerializeField]
        private KeyCode _enableKeyCode = KeyCode.BackQuote;

        [SerializeField]
        private bool _enabled;

        private DebugWindow _window;

        private readonly List<DebugMenuNode> _nodes = new List<DebugMenuNode>();

        [CanBeNull]
        private DebugMenuNode _currentNode;

#region Unity Lifecycle
        private void Awake()
        {
            _window = new DebugWindow(new Rect(10, 10, 800, 600), RenderWindowContents)
            {
                Title = () => {
                    string title = "Debug Menu";

                    if(null != _currentNode) {
                        if(null != _currentNode.Parent) {
                            title = $"{_currentNode.Parent.Title()}";
                        }
                        title += $" => {_currentNode.Title()}";
                    }

                    return title;
                }
            };
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

        public DebugMenuNode AddNode(Func<string> title)
        {
            DebugMenuNode node = new DebugMenuNode(title);
            _nodes.Add(node);
            return node;
        }

        public void SetCurrentNode(DebugMenuNode node)
        {
            _currentNode = node;
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
