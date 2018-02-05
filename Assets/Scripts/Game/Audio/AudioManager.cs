using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Game.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private AudioSource _oneShotAudioSource;

        [SerializeField]
        private AudioSource _musicAudioSource;

#region Unity Lifecycle
        private void Awake()
        {
            _musicAudioSource.loop = true;
        }
#endregion

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
