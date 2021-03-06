﻿using System;

using pdxpartyparrot.ggj2018.Birds;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace pdxpartyparrot.ggj2018.Data
{
    [CreateAssetMenu(fileName="BirdTypeData", menuName="ggj2018/Data/Bird Type Data")]
    [Serializable]
    public sealed class BirdTypeData : ScriptableObject
    {
        [SerializeField]
        private string _id;

        public string Id => _id;

        [SerializeField]
        private string _name;

        public string Name => _name;

        [SerializeField]
        private Sprite _icon;

        public Sprite Icon => _icon;

        [SerializeField]
        private Bird _modelPrefab;

        public Bird ModelPrefab => _modelPrefab;

        [SerializeField]
        private BirdPhysicsData _physics;

        public BirdPhysicsData Physics => _physics;

#region Bird Type

        [SerializeField]
        private bool _isPredator;

        public bool IsPredator => _isPredator;

        public bool IsPrey => !_isPredator;
#endregion

        [Space(10)]

#region Viewer
        [Header("Viewer")]

        [SerializeField]
        private float _followOrbitRadius = 5.0f;

        public float FollowOrbitRadius => _followOrbitRadius;

        [SerializeField]
        private bool _showTargetingReticle;

        public bool ShowTargetingReticle => _showTargetingReticle;
#endregion

        [Space(10)]

#region Boost
        [Header("Boost")]

        [SerializeField]
        private bool _canBoost;

        public bool CanBoost => _canBoost;

        [SerializeField]
        private int _boostSeconds = 5;

        public int BoostSeconds => _boostSeconds;

        [SerializeField]
        private bool _boostRecharges;

        public bool BooseRecharges => _boostRecharges;

        [SerializeField]
        private float _boostRechargeCooldown = 5.0f;

        public float BoostRechargeCooldown => _boostRechargeCooldown;

        [SerializeField]
        private float _boostRechargeRate = 1.0f;

        public float BoostRechargeRate => _boostRechargeRate;
#endregion

        [Space(10)]

#region Audio
        [Header("Audio")]

        [SerializeField]
        private AudioClip _spawnAudioClip;

        public AudioClip SpawnAudioClip => _spawnAudioClip;

        [SerializeField]
        private AudioClip _flightAudioClip;

        public AudioClip FlightAudioClip => _flightAudioClip;

        [SerializeField]
        private AudioClip _boostAudioClip;

        public AudioClip BoostAudioClip => _boostAudioClip;

        [SerializeField]
        private AudioClip _boostFailAudioClip;

        public AudioClip BoostFailAudioClip => _boostFailAudioClip;

        [SerializeField]
        private AudioClip _winAudioClip;

        public AudioClip WinAudioClip => _winAudioClip;

        [SerializeField]
        private AudioClip _lossAudioClip;

        public AudioClip LossAudioClip => _lossAudioClip;

        [SerializeField]
        private AudioClip _hornAudioClip;

        public AudioClip HornAudioClip => _hornAudioClip;
#endregion

        private BirdData _birdData;

        public float ViewFOV => IsPredator
            ? _birdData.PredatorFOV
            : _birdData.PreyFOV;

        public float BoostFOVChange => IsPredator
            ? _birdData.PredatorBoostFOVChange
            : _birdData.PreyBoostFOVChange;

        public float BrakeFOVChange => IsPredator
            ? _birdData.PredatorBrakeFOVChange
            : _birdData.PreyBrakeFOVChange;

        public LayerMask PlayerLayer => IsPredator
            ? _birdData.PredatorPlayerLayer
            : _birdData.PreyPlayerLayer;

        // TODO: rename this
        public LayerMask OtherPlayerLayer => IsPrey
            ? _birdData.PredatorPlayerLayer
            : _birdData.PreyPlayerLayer;

        public LayerMask RenderLayer => IsPredator
            ? _birdData.PredatorRenderLayer
            : _birdData.PreyRenderLayer;

        // TODO: rename this
        public LayerMask OtherRenderLayer => IsPrey
            ? _birdData.PredatorRenderLayer
            : _birdData.PreyRenderLayer;

        public PostProcessProfile PostProcessProfile => IsPredator
            ? _birdData.PredatorPostProcessProfile
            : _birdData.PreyPostProcessProfile;

        public void Initialize(BirdData birdData)
        {
            _birdData = birdData;
        }
    }
}
