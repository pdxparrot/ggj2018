using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuNode : DebugMenuItem
    {
        public Func<string> Title { get; private set; }

        public DebugMenuNode Parent { get; private set; }

        private readonly List<DebugMenuItem> _children = new List<DebugMenuItem>();

        public DebugMenuNode(Func<string> title)
        {
            Title = title;
        }

        public DebugMenuNode(Func<string> title, DebugMenuNode parent)
        {
            Title = title;
            Parent = parent;
        }

        public override void Render()
        {
            string title = Title();

            // TODO: calculate width/height

            if(GUILayout.Button(title, GUILayout.Width(100), GUILayout.Height(25))) {
                DebugMenuManager.Instance.SetCurrentNode(this);
            }
        }

        public void RenderChildren()
        {
            foreach(DebugMenuItem child in _children) {
                child.Render();
            }
        }

        public DebugMenuNode AddNode(Func<string> title)
        {
            DebugMenuNode node = new DebugMenuNode(title, this);
            _children.Add(node);
            return node;
        }

        public DebugMenuLabel AddLabel(Func<string> text)
        {
            DebugMenuLabel label = new DebugMenuLabel(text);
            _children.Add(label);
            return label;
        }

        public DebugMenuButton AddButton(Func<string> title)
        {
            DebugMenuButton button = new DebugMenuButton(title);
            _children.Add(button);
            return button;
        }
    }
}
