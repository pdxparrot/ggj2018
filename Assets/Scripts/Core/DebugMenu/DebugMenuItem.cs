using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuItem
    {
        public string Title { get; private set; }

        public DebugMenuItem Parent { get; private set; }

        private readonly List<DebugMenuItem> _children = new List<DebugMenuItem>();

        public DebugMenuItem(string title)
        {
            Title = title;
        }

        public DebugMenuItem(string title, DebugMenuItem parent)
        {
            Title = title;
            Parent = parent;
        }

        public virtual void Render()
        {
            if(GUILayout.Button(Title, GUILayout.Width(100), GUILayout.Height(25))) {
                DebugMenuManager.Instance.SetCurrentChild(this);
            }
        }

        public void RenderChildren()
        {
            foreach(DebugMenuItem child in _children) {
                child.Render();
            }
        }
    }
}
