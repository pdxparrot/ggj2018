using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Collider))]
    public abstract class Bird : MonoBehavior
    {
        [SerializeField]
        private GameObject _model;

        public GameObject Model => _model;

        [SerializeField]
        private ParticleSystem _stunParticles;

        [SerializeField]
        private ParticleSystem _featherParticles;

        [SerializeField]
        private ParticleSystem _immunityParticles;

        [SerializeField]
        private TrailRenderer _boostTrail;

        [SerializeField]
        private SphereCollider _playerCollider;

        public Player Owner { get; private set; }

        public BirdData.BirdDataEntry Type { get; private set; }

        private Collider _collider;

#region Unity Lifecycle      
        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.sharedMaterial = GameManager.Instance.FrictionlessMaterial;

            _playerCollider.gameObject.layer = PlayerManager.Instance.PlayerCollisionLayer;
            _playerCollider.isTrigger = true;
        }

        protected virtual void Start()
        {
            ShowImmunity(false);
            ShowStun(false);
            ShowBoostTrail(false);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + _playerCollider.center, _playerCollider.radius);
        }
#endregion

        public void Initialize(Player owner, BirdData.BirdDataEntry birdType)
        {
            Owner = owner;
            Type = birdType;
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
    }
}
