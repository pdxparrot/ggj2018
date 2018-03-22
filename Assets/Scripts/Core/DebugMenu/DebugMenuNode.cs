using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuNode : DebugMenuItem
    {
        public string Title { get; private set; }

        public DebugMenuNode Parent { get; private set; }

        private readonly List<DebugMenuItem> _children = new List<DebugMenuItem>();

        public DebugMenuNode(string title)
        {
            Title = title;
        }

        public DebugMenuNode(string title, DebugMenuNode parent)
        {
            Title = title;
            Parent = parent;
        }

        public override void Render()
        {
            if(GUILayout.Button(Title, GUILayout.Width(100), GUILayout.Height(25))) {
                DebugMenuManager.Instance.SetCurrentNode(this);
            }
        }

        public void RenderChildren()
        {
            foreach(DebugMenuItem child in _children) {
                child.Render();
            }
        }

        public DebugMenuNode AddNode(string title)
        {
            DebugMenuNode node = new DebugMenuNode(title, this);
            _children.Add(node);
            return node;
        }

        public DebugMenuButton AddButton(string title)
        {
            DebugMenuButton button = new DebugMenuButton(title);
            _children.Add(button);
            return button;
        }
    }
}
