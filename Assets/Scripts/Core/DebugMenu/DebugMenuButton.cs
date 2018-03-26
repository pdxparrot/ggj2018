using System;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuButton : DebugMenuItem
    {
#region Events
        public event EventHandler<EventArgs> OnClick;
#endregion

        public Func<string> Title { get; private set; }

        public DebugMenuButton(Func<string> title)
        {
            Title = title;
        }

        public override void Render()
        {
            string title = Title();

            // TODO: calculate width/height

            if(GUILayout.Button(title, GUILayout.Width(100), GUILayout.Height(25))) {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
