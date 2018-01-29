using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public abstract class Bird : MonoBehavior
    {
        [SerializeField]
        private ParticleSystem _featherParticles;

        [SerializeField]
        private GameObject _boostTrail;

#region Unity Lifecycle
        private void Awake()
        {
            _boostTrail.SetActive(false);
        }
#endregion

        public void ShowStun(bool show)
        {
        }

        public void EnableBoostTrail(bool enable)
        {
            var emission = _featherParticles.emission;
            emission.enabled = enable;

            _boostTrail.SetActive(enable);
        }
    }
}
