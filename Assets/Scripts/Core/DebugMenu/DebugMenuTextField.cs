using System;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuTextField : DebugMenuItem
    {
// TODO: flesh this out more
#region Events
        public event EventHandler<EventArgs> OnTextChanged;
#endregion

        public Func<string> Text { get; private set; }

        public DebugMenuTextField(Func<string> text)
        {
            Text = text;
        }

        public override void Render()
        {
            string text = Text();
            string newText = GUILayout.TextField(text);

            if(newText != text) {
                OnTextChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
