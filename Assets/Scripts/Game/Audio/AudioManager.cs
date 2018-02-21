using System.Collections;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Audio;

namespace pdxpartyparrot.Game.Audio
{
    public sealed class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        private AudioMixer _mixer;

#region Mixer Groups
        [Header("Mixer Groups")]

        [SerializeField]
        private string _musicMixerGroupName = "Music";

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
        private float _musicCrossFade;

        // 0 == music1, 1 = music2
        public float MusicCrossFade { get { return _musicCrossFade; } set { _musicCrossFade = Mathf.Clamp01(value); } }

        [SerializeField]
        private float _updateCrossfadeUpdateMs = 100.0f;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            InitAudioMixerGroup(_oneShotAudioSource, _sfxMixerGroupName);

            InitAudioMixerGroup(_music1AudioSource, _musicMixerGroupName);
            _music1AudioSource.loop = true;

            InitAudioMixerGroup(_music2AudioSource, _musicMixerGroupName);
            _music2AudioSource.loop = true;
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
                _music1AudioSource.volume = 1.0f - _musicCrossFade;
                _music2AudioSource.volume = _musicCrossFade;

                yield return wait;
            }
        }
    }
}
