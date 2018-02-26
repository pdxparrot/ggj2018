using System.Collections;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.Game.Audio;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Birds
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class Bird : MonoBehavior
    {
#region Animations
        [Header("Animations")]

        [SerializeField]
        private Animator _animator;
#endregion

        [Space(10)]

#region VFX
        [Header("VFX")]

        [SerializeField]
        private ParticleSystem _stunParticles;

        [SerializeField]
        private ParticleSystem _featherParticles;

        [SerializeField]
        private ParticleSystem _immunityParticles;

        [SerializeField]
        private TrailRenderer _boostTrail;

        [SerializeField]
        private Renderer[] _coloredRenderers;
#endregion

        [Space(10)]

        [SerializeField]
        private SphereCollider _playerCollider;

        public Player Owner { get; private set; }

        public BirdTypeData Type { get; private set; }

        private Collider _collider;

        public Collider Collider => _collider;

        private AudioSource _audioSource;

#region Unity Lifecycle      
        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.sharedMaterial = GameManager.Instance.FrictionlessMaterial;

            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0.0f;
            AudioManager.Instance.InitSFXAudioMixerGroup(_audioSource);

            _playerCollider.gameObject.layer = PlayerManager.Instance.PlayerCollisionLayer;
            _playerCollider.isTrigger = true;
        }

        protected virtual void Start()
        {
            ShowImmunity(false);
            ShowStun(false);
            ShowBoostTrail(false);

            StartCoroutine(PlayFlightAnimation());
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _playerCollider.center, _playerCollider.radius);
        }
#endregion

        public void Initialize(Player owner, BirdTypeData birdType)
        {
            Owner = owner;
            Type = birdType;

            foreach(Renderer r in _coloredRenderers) {
                r.material.SetColor(PlayerManager.Instance.PlayerData.PlayerColorProperty, Owner.PlayerColor);
            }
        }

        public void ShowImmunity(bool show)
        {
            _immunityParticles.gameObject.SetActive(show);
        }

        public void ShowStun(bool show)
        {
            _stunParticles.gameObject.SetActive(show);
        }

        public void ShowBoostTrail(bool show)
        {
            if(show) {
                _featherParticles.Play();
            } else {
                _featherParticles.Stop();

            }
            _boostTrail.gameObject.SetActive(show);
        }

#region Audio
        public void StartBoostAudio()
        {
            _audioSource.clip = Type.BoostAudioClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        public void StopBoostAudio()
        {
            _audioSource.Stop();
        }

        public void PlayBoostFailAudio()
        {
            _audioSource.PlayOneShot(Type.BoostFailAudioClip);
        }

        public void PlaySpawnAudio()
        {
            _audioSource.PlayOneShot(Type.SpawnAudioClip);
        }

        public void PlayHornAudio()
        {
// TODO: cooldown this
            _audioSource.PlayOneShot(Type.HornAudioClip);
        }

        public void PlayWinAudio()
        {
            _audioSource.PlayOneShot(Type.WinAudioClip);
        }

        public void PlayLossAudio()
        {
            _audioSource.PlayOneShot(Type.LossAudioClip);
        }
#endregion

        private IEnumerator PlayFlightAnimation()
        {
            System.Random random = new System.Random();
            while(true) {
                // TODO: animate

                _audioSource.PlayOneShot(Type.FlightAudioClip);

                float wait = random.NextSingle(PlayerManager.Instance.PlayerData.MinFlightAnimationCooldown, PlayerManager.Instance.PlayerData.MaxFlightAnimationCooldown);
                yield return new WaitForSeconds(wait);
            }
        }
    }
}
