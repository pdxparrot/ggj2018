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

        public IEnumerator InitializeRoutine()
        {
            // TODO: why isn't this working???
            _oneShotAudioSource.outputAudioMixerGroup = _audioMixer.outputAudioMixerGroup;

            yield break;
        }

        public void PlayAudioOneShot(string id)
        {
            AudioClip audioClip = DataManager.Instance.GameData.Audio.Entries.GetOrDefault(id)?.AudioClip;
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
