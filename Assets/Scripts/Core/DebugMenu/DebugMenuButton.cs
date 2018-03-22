using System;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuButton : DebugMenuItem
    {
#region Events
        public event EventHandler<EventArgs> OnClick;
#endregion

        public string Title { get; private set; }

        public DebugMenuButton(string title)
        {
            Title = title;
        }

        public override void Render()
        {
            if(GUILayout.Button(Title, GUILayout.Width(100), GUILayout.Height(25))) {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
