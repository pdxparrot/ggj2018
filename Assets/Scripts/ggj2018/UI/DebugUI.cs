﻿using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
    [RequireComponent(typeof(Canvas))]
    public class DebugUI : MonoBehavior
    {
        [SerializeField]
        private Text _fpsText;

#region Unity Lifecycle
        private void Awake()
        {
            GetComponent<Canvas>().sortingOrder = 9990;
        }

        private void Update()
        {
            _fpsText.text = $"FPS: {(int)(1.0f / Time.unscaledDeltaTime)}";
        }
#endregion
    }
}