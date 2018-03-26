using System;

using UnityEngine;

namespace pdxpartyparrot.Core.DebugMenu
{
    public class DebugMenuLabel : DebugMenuItem
    {
        public Func<string> Text { get; private set; }

        public DebugMenuLabel(Func<string> text)
        {
            Text = text;
        }

        public override void Render()
        {
            string text = Text();
            GUILayout.Label(text);
        }
    }
}
