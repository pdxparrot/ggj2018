using UnityEngine;

namespace ggj2018.ggj2018
{
    public class BaseAttributes
    {
        [SerializeField]
        private float _speedModifier;

        public float SpeedModifier => _speedModifier;
    }
}
