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
        private int _menuId = 0;

        [SerializeField]
        private Rect _menuRect = new Rect(10, 10, 800, 600);

        private readonly List<DebugMenuItem> _children = new List<DebugMenuItem>();

        private DebugMenuItem _currentChild;

#region Unity Lifecycle
        private void Awake()
        {
            AddItem("Test");    // TODO: we need to make a distinction between top-level menu items and menu items that do things
            AddItem("Quit");    // TODO: pass an action or something here to quit the game
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

            string title =  null == _currentChild ? "Debug Menu" : _currentChild.Title;

            _menuRect = GUILayout.Window(_menuId, _menuRect, id => {
                if(null == _currentChild) {
                    foreach(DebugMenuItem child in _children) {
                        child.Render();
                    }
                } else {
                    _currentChild.RenderChildren();
                    if(GUILayout.Button("Back", GUILayout.Width(100), GUILayout.Height(25))) {
                        SetCurrentChild(_currentChild.Parent);
                    }
                }

                GUI.DragWindow();
            }, title, GUILayout.MinWidth(200), GUILayout.MinHeight(200));
        }
#endregion

        public DebugMenuItem AddItem(string title)
        {
            DebugMenuItem item = new DebugMenuItem(title);
            _children.Add(item);
            return item;
        }

        public DebugMenuItem InsertItem(int index, string title)
        {
            DebugMenuItem item = new DebugMenuItem(title);
            _children.Insert(index, item);
            return item;
        }

        public void SetCurrentChild(DebugMenuItem child)
        {
            _currentChild = child;
        }
    }
}
