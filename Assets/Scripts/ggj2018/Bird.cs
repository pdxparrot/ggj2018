using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public abstract class Bird : MonoBehavior
    {
        [SerializeField]
        private ParticleSystem _stunParticles;

        [SerializeField]
        private ParticleSystem _featherParticles;

        [SerializeField]
        private TrailRenderer _boostTrail;

#region Unity Lifecycle
        protected virtual void Start()
        {
            ShowStun(false);
            ShowBoostTrail(false);
        }
#endregion

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
