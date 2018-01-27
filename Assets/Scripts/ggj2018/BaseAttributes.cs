using UnityEngine;

namespace ggj2018.ggj2018
{
    public class BaseAttributes
    {
        [SerializeField]
        private float _speedModifier;

        public float SpeedModifier => _speedModifier;

        [SerializeField]
        private float _turnSpeedModifier;

        public float TurnSpeedModifier => _turnSpeedModifier;

        [SerializeField]
        private float _pitchSpeedModifier;

        public float PitchSpeedModifier => _pitchSpeedModifier;
    }
}
