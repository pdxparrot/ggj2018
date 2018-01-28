using System;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="ggj2018/Data/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject
    {
#region world shit (removeme when we get real walls)
        [SerializeField]
        private float _minHeight = 25.0f;

        public float MinHeight => _minHeight;

        [SerializeField]
        private float _maxHeight = 220.0f;

        public float MaxHeight => _maxHeight;
#endregion

        [SerializeField]
        private float _baseSpeed = 1.0f;

        public float BaseSpeed => _baseSpeed;

        [SerializeField]
        private float _baseTurnSpeed = 50.0f;

        public float BaseTurnSpeed => _baseTurnSpeed;

        [SerializeField]
        private float _basePitchSpeed = 50.0f;

        public float BasePitchSpeed => _basePitchSpeed;

        [SerializeField]
        private float _rotationAnimationSpeed = 5.0f;

        public float RotationAnimationSpeed => _rotationAnimationSpeed;

        [SerializeField]
        private int _stunTimeSeconds = 2;

        public int StunTimeSeconds => _stunTimeSeconds;

        [SerializeField]
        private float _stunBounceSpeed = 1.0f;

        public float StunBounceSpeed => _stunBounceSpeed;

        [SerializeField]
        private float _terminalVelocity = 100.0f;

        public float TerminalVelocity => _terminalVelocity;
    }
}
