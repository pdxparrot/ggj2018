using System.Collections;

using ggj2018.Game.Data;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace ggj2018.Game.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private AudioSource _oneShotAudioSource;

        [SerializeField]
        private AudioData _audioData;

        public AudioData AudioData => _audioData;

        public IEnumerator InitializeRoutine()
        {
            // TODO: why isn't this working???
            _oneShotAudioSource.outputAudioMixerGroup = _audioMixer.outputAudioMixerGroup;

            yield break;
        }

#region Unity Lifecycle
        private void Awake()
        {
            _audioData.Initialize();
        }
#endregion

        public void PlayAudioOneShot(string id)
        {
            AudioClip audioClip = _audioData.Entries.GetOrDefault(id)?.AudioClip;
            if(null != audioClip) {
                PlayAudioOneShot(audioClip);
            }
        }

        public void PlayAudioOneShot(AudioClip audioClip)
        {
            _oneShotAudioSource.PlayOneShot(audioClip);
        }
    }
}
