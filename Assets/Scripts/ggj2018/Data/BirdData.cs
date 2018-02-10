using System;
using System.Collections.Generic;

using ggj2018.ggj2018.Birds;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="BirdData", menuName="ggj2018/Data/Bird Data")]
    [Serializable]
    public sealed class BirdData : ScriptableObject
    {
        [Serializable]
        public sealed class BirdDataEntry
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
            private float _followOrbitRadius = 15.0f;

            public float FollowOrbitRadius => _followOrbitRadius;
#endregion

            [Space(10)]

#region Abilities
            [Header("Abilities")]

            [SerializeField]
            private bool _canBoost;

            public bool CanBoost => _canBoost;
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
            private AudioClip _winAudioClip;

            public AudioClip WinAudioClip => _winAudioClip;

            [SerializeField]
            private AudioClip _lossAudioClip;

            public AudioClip LossAudioClip => _lossAudioClip;
#endregion

            private BirdData _birdData;

            public float ViewFOV => IsPredator
                ? _birdData.PredatorFOV
                : _birdData.PreyFOV;

            public string Layer => IsPredator
                ? _birdData.PredatorLayer
                : _birdData.PreyLayer;

            // TODO: rename this
            public string OtherLayer => IsPrey
                ? _birdData.PredatorLayer
                : _birdData.PreyLayer;

            public PostProcessProfile PostProcessProfile => IsPredator
                ? _birdData.PredatorPostProcessProfile
                : _birdData.PreyPostProcessProfile;

            public void Initialize(BirdData birdData)
            {
                _birdData = birdData;
            }
        }

#region Flight
        [Header("Flight")]

        [SerializeField]
        private float _maxAttackAngle = 45.0f;

        public float MaxAttackAngle => _maxAttackAngle;

        [SerializeField]
        private float _maxBankAngle = 45.0f;

        public float MaxBankAngle => _maxBankAngle;
#endregion

        [Space(10)]

#region FOVs
        [Header("FOV")]

        [SerializeField]
        private float _predatorFOV = 60.0f;

        public float PredatorFOV => _predatorFOV;

        [SerializeField]
        private float _preyFOV = 90.0f;

        public float PreyFOV => _preyFOV;
#endregion

        [Space(10)]

#region Layers
        [Header("Layers")]

        [SerializeField]
        private string _predatorLayer;

        public string PredatorLayer => _predatorLayer;

        [SerializeField]
        private string _preyLayer;

        public string PreyLayer => _preyLayer;
#endregion

        [Space(10)]

#region Post Processing
        [Header("Post Processing")]

        [SerializeField]
        private PostProcessProfile _predatorPostProcessProfile;

        public PostProcessProfile PredatorPostProcessProfile => _predatorPostProcessProfile;

        [SerializeField]
        private PostProcessProfile _preyPostProcessProfile;

        public PostProcessProfile PreyPostProcessProfile => _preyPostProcessProfile;
#endregion

        [Space(10)]

        [Header("Birds")]

        [SerializeField]
        private BirdDataEntry[] _birds;

        public IReadOnlyCollection<BirdDataEntry> Birds => _birds;

        private readonly Dictionary<string, BirdDataEntry> _entries = new Dictionary<string, BirdDataEntry>();

        public IReadOnlyDictionary<string, BirdDataEntry> Entries => _entries;

        public void Initialize()
        {
            foreach(BirdDataEntry entry in Birds) {
                entry.Initialize(this);
                _entries.Add(entry.Id, entry);
            }
        }
    }
}
