using System;
using System.Collections.Generic;

using ggj2018.ggj2018.Game;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="ggj2018/Data/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject
    {
// TODO: move to bird data
#region Base Stats
        [Header("Arcade Base Stats")]

        [SerializeField]
        private float _baseSpeed = 10.0f;

        public float BaseSpeed => _baseSpeed;

        [SerializeField]
        private float _baseTurnSpeed = 10.0f;

        public float BaseTurnSpeed => _baseTurnSpeed;

        [SerializeField]
        private float _basePitchUpSpeed = 5.0f;

        public float BasePitchUpSpeed => _basePitchUpSpeed;

        [SerializeField]
        private float _basePitchDownSpeed = 10.0f;

        public float BasePitchDownSpeed => _basePitchDownSpeed;

        [Header("Physical Base Stats")]

        [SerializeField]
        private float _baseHorizontalAcceleration = 25.0f;

        public float BaseHorizontalAcceleration => _baseHorizontalAcceleration;

        [SerializeField]
        private float _baseVerticalAcceleration = 5.0f;

        public float BaseVerticalAcceleration => _baseVerticalAcceleration;

        [SerializeField]
        private float _baseAngularAcceleration = 1.0f;

        public float BaseAngularAcceleration => _baseAngularAcceleration;
#endregion

        [Space(10)]

// TODO: move to bird data
#region Animations
        [Header("Animations")]

        [SerializeField]
        private float _minFlightAnimationCooldown = 5.0f;

        public float MinFlightAnimationCooldown => _minFlightAnimationCooldown;

        [SerializeField]
        private float _maxFlightAnimationCooldown = 15.0f;

        public float MaxFlightAnimationCooldown => _maxFlightAnimationCooldown;

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

        [Space(10)]

// TODO: move to game type data
#region Stun
        [Header("Stun")]

        [SerializeField]
        private int _stunTimeSeconds = 2;

        public int StunTimeSeconds => _stunTimeSeconds;

        // TODO: make this a force
        [SerializeField]
        private float _stunBounceSpeed = 1.0f;

        public float StunBounceSpeed => _stunBounceSpeed;

        [SerializeField]
        private bool _stunBounceRotation = false;

        public bool StunBounceRotation => _stunBounceRotation;
#endregion

        [Space(10)]

// TODO: move to game type data
#region Boost
        [Header("Boost")]

        [SerializeField]
        private int _boostSeconds = 5;

        public int BoostSeconds => _boostSeconds;

        [Header("Arcade Boost")]

        [SerializeField]
        private float _boostFactor = 2.0f;

        public float BoostFactor => _boostFactor;

        [Header("Physical Boost")]

        [SerializeField]
        private float _boostAcceleration = 5.0f;

        public float BoostAcceleration => _boostAcceleration;
#endregion

        [Space(10)]

// TODO: move to game type data
#region Braking
        [Header("Arcade Braking")]

        [SerializeField]
        private float _brakeFactor = 0.5f;

        public float BrakeFactor => _brakeFactor;

        [Header("Physical Braking")]

        [SerializeField]
        private float _brakeAcceleration = 5.0f;

        public float BrakeAcceleration => _brakeAcceleration;
#endregion

        [Space(10)]

#region Player Colors
        [Header("Player Colors")]

        [SerializeField]
        private Color _defaultPlayerColor = Color.magenta;

        public Color DefaultPlayerColor => _defaultPlayerColor;

        [SerializeField]
        private Color[] _playerColors;

        public IReadOnlyCollection<Color> PlayerColors => _playerColors;

        [SerializeField]
        private string _playerColorProperty = "_PlayerColor";

        public string PlayerColorProperty => _playerColorProperty;
#endregion

        public Color GetPlayerColor(int playerId)
        {
            return _playerColors.Length < 1 ? DefaultPlayerColor : _playerColors[playerId % _playerColors.Length];
        }
    }
}
