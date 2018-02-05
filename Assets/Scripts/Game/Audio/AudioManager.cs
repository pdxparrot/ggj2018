using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace ggj2018.Game.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private AudioMixer _mixer;

        [SerializeField]
        private string _musicMixerGroupName = "Music";

        [SerializeField]
        private string _sfxMixerGroupName = "SFX";

        [SerializeField]
        private AudioSource _oneShotAudioSource;

        [SerializeField]
        private AudioSource _musicAudioSource;

#region Unity Lifecycle
        private void Awake()
        {
            InitAudioMixerGroup(_oneShotAudioSource, _sfxMixerGroupName);

            InitAudioMixerGroup(_musicAudioSource, _musicMixerGroupName);
            _musicAudioSource.loop = true;
        }
#endregion

        private void InitAudioMixerGroup(AudioSource source, string mixerGroupName)
        {
            var mixerGroups = _mixer.FindMatchingGroups(mixerGroupName);
            source.outputAudioMixerGroup = mixerGroups.Length > 0 ? mixerGroups[0] : _mixer.outputAudioMixerGroup;
        }

        public void PlayAudioOneShot(AudioClip audioClip)
        {
            _oneShotAudioSource.PlayOneShot(audioClip);
        }

        public void PlayMusic(AudioClip audioClip)
        {
            StopMusic();

            _musicAudioSource.clip = audioClip;
            _musicAudioSource.Play();
        }

        public void StopMusic()
        {
            _musicAudioSource.Stop();
        }
    }
}
