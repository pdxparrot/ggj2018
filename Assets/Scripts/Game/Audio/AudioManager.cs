using System.Collections;

using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace ggj2018.Game.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private AudioMixer _mixer;

#region Mixer Groups
        [Header("Mixer Groups")]

        [SerializeField]
        private string _music1MixerGroupName = "Music1";

        [SerializeField]
        private string _music2MixerGroupName = "Music2";

        [SerializeField]
        private string _sfxMixerGroupName = "SFX";
#endregion

        [Space(10)]

        [Header("SFX")]

        [SerializeField]
        private AudioSource _oneShotAudioSource;

        [Space(10)]

#region Music
        [Header("Music")]

        [SerializeField]
        private AudioSource _music1AudioSource;

        [SerializeField]
        private AudioSource _music2AudioSource;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _musicCrossFade = 0.0f;

        [SerializeField]
        private float _updateCrossfadeUpdateMs = 100.0f;

        private float _minVolume, _volumeDistance;
#endregion

        [Space(10)]

#region Expoded Vars
        [Header("Mixer Variables")]

        [SerializeField]
        private string _music1VolumeParamName = "Music Volume 1";

        [SerializeField]
        private string _music2VolumeParamName = "Music Volume 2";
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            InitAudioMixerGroup(_oneShotAudioSource, _sfxMixerGroupName);

            InitAudioMixerGroup(_music1AudioSource, _music1MixerGroupName);
            _music1AudioSource.loop = true;

            float music1Volume;
            _mixer.GetFloat(_music1VolumeParamName, out music1Volume);

            InitAudioMixerGroup(_music2AudioSource, _music2MixerGroupName);
            _music2AudioSource.loop = true;

            float music2Volume;
            _mixer.GetFloat(_music2VolumeParamName, out music2Volume);

            _minVolume = Mathf.Min(music1Volume, music2Volume);
            _volumeDistance = Mathf.Max(music1Volume, music2Volume) - _minVolume;
        }

        private void Start()
        {
            StartCoroutine(UpdateMusicCrossfade());
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

        public void PlayMusic(AudioClip musicAudioClip)
        {
            StopMusic();

            _music1AudioSource.clip = musicAudioClip;
            _music1AudioSource.Play();
        }

        public void PlayMusic(AudioClip music1AudioClip, AudioClip music2AudioClip)
        {
            StopMusic();

            _music1AudioSource.clip = music1AudioClip;
            _music1AudioSource.Play();

            _music2AudioSource.clip = music2AudioClip;
            _music2AudioSource.Play();
        }

        public void StopMusic()
        {
            _music1AudioSource.Stop();
            _music2AudioSource.Stop();
        }

        private IEnumerator UpdateMusicCrossfade()
        {
            WaitForSeconds wait = new WaitForSeconds(_updateCrossfadeUpdateMs / 1000.0f);
            while(true) {
                _mixer.SetFloat(_music1VolumeParamName, _minVolume + (_volumeDistance * (1.0f - _musicCrossFade)));
                _mixer.SetFloat(_music2VolumeParamName, _minVolume + (_volumeDistance * _musicCrossFade));

                yield return wait;
            }
        }
    }
}
