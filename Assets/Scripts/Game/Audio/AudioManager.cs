using System.Collections;

using ggj2018.Core.Assets;
using ggj2018.Game.Data;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace ggj2018.Game.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private string _mainAudioMixerPath = "Audio/main.mixer";

        [SerializeField]
        [ReadOnly]
        private AudioMixer _audioMixer;

        [SerializeField]
        private AudioSource _oneShotAudioSource;

        public IEnumerator InitializeRoutine()
        {
            _audioMixer = AssetManager.Instance.LoadAsset<AudioMixer>(_mainAudioMixerPath);

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
