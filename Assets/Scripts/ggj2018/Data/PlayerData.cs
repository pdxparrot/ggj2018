﻿using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Data
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

#region Boost
        [Header("Boost")]

        [SerializeField]
        private bool _enableBoostCameraShake = true;

        public bool EnableBoostCameraShake => _enableBoostCameraShake;

        [SerializeField]
        private Vector3 _boostCameraShakeStrength = new Vector3(3.0f, 3.0f, 3.0f);

        public Vector3 BoostCameraShakeStrength => _boostCameraShakeStrength;

        [SerializeField]
        private int _boostCameraShakeVibrato = 10;

        public int BoostCameraShakeVibrato => _boostCameraShakeVibrato;

        [SerializeField]
        private float _boostCameraShakeRandomness = 90.0f;

        public float BoostCameraShakeRandomness => _boostCameraShakeRandomness;
#endregion

        [Space(10)]

// TODO: move to game type data
#region Stun
        [Header("Stun")]

        [SerializeField]
        private int _stunTimeSeconds = 2;

        public int StunTimeSeconds => _stunTimeSeconds;

        [SerializeField]
        private float _stunBounceSpeed = 10.0f;

        public float StunBounceSpeed => _stunBounceSpeed;
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
