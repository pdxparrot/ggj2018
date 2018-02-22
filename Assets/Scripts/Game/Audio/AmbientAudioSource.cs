﻿using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AmbientAudioSource : MonoBehavior
    {
        private AudioSource[] _audioSources;

#region Unity Lifecycle
        private void Awake()
        {
            _audioSources = GetComponents<AudioSource>();
            foreach(AudioSource source in _audioSources) {
                AudioManager.Instance.InitSFXAudioMixerGroup(source);
            }
        }
#endregion
    }
}
