using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public abstract class Bird : MonoBehavior
    {
        [SerializeField]
        private GameObject _stunParticles;

        [SerializeField]
        private GameObject _featherParticles;

        [SerializeField]
        private GameObject _boostTrail;

#region Unity Lifecycle
        protected virtual void Start()
        {
            ShowStun(false);
            ShowBoostTrail(false);
        }
#endregion

        public void ShowStun(bool show)
        {
            _stunParticles.SetActive(show);
        }

        public void ShowBoostTrail(bool enable)
        {
            _featherParticles.SetActive(enable);
            _boostTrail.SetActive(enable);
        }
    }
}
