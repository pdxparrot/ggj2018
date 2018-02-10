using System;
using System.Collections.Generic;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="ggj2018/Data/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject
    {
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

        [SerializeField]
        private float _boostCooldown = 2.0f;

        public float BoostCooldown => _boostCooldown;

        [SerializeField]
        private float _boostRechargeRate = 1.0f;

        public float BoostRechargeRate => _boostRechargeRate;

        [SerializeField]
        private float _boostAcceleration = 5.0f;

        public float BoostAcceleration => _boostAcceleration;
#endregion

        [Space(10)]

// TODO: move to game type data
#region Braking
        [Header("Braking")]

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
