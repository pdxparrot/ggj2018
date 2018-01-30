using System;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="ggj2018/Data/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject
    {
#region Base Stats
        [SerializeField]
        private float _baseSpeed = 1.0f;

        public float BaseSpeed => _baseSpeed;

        [SerializeField]
        private float _baseTurnSpeed = 50.0f;

        public float BaseTurnSpeed => _baseTurnSpeed;

        [SerializeField]
        private float _basePitchUpSpeed = 25.0f;

        public float BasePitchUpSpeed => _basePitchUpSpeed;

        [SerializeField]
        private float _basePitchDownSpeed = 50.0f;

        public float BasePitchDownSpeed => _basePitchDownSpeed;
#endregion

#region Animations
        [SerializeField]
        private float _pitchAnimationAngle = 45.0f;

        public float PitchAnimationAngle => _pitchAnimationAngle;

        [SerializeField]
        private float _turnAnimationAngle = 45.0f;

        public float TurnAnimationAngle => _turnAnimationAngle;

        [SerializeField]
        private float _rotationAnimationSpeed = 5.0f;

        public float RotationAnimationSpeed => _rotationAnimationSpeed;
#endregion

#region Stuns
        [SerializeField]
        private int _stunTimeSeconds = 2;

        public int StunTimeSeconds => _stunTimeSeconds;

        [SerializeField]
        private float _stunBounceSpeed = 1.0f;

        public float StunBounceSpeed => _stunBounceSpeed;

        [SerializeField]
        private bool _stunBounceRotation = false;

        public bool StunBounceRotation => _stunBounceRotation;
#endregion

        [SerializeField]
        private float _terminalVelocity = 100.0f;

        public float TerminalVelocity => _terminalVelocity;

#region Boost
        [SerializeField]
        private float _boostFactor = 2.0f;

        public float BoostFactor => _boostFactor;

        [SerializeField]
        private int _boostSeconds = 5;

        public int BoostSeconds => _boostSeconds;
#endregion
    }
}
