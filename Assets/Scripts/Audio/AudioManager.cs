using ggj2018.Data;
using ggj2018.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace ggj2018.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private AudioSource _oneShotAudioSource;

#region Unity Lifecycle
        private void Start()
        {
            // TODO: why isn't this working???
            _oneShotAudioSource.outputAudioMixerGroup = _audioMixer.outputAudioMixerGroup;
        }
#endregion

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
