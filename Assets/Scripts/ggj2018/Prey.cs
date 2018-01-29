using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Prey : Bird
    {
        [SerializeField]
        private ParticleSystem _bloodParticles;

        public void ShowBlood()
        {
            var emission = _bloodParticles.emission;
            emission.enabled = true;
        }
    }
}
