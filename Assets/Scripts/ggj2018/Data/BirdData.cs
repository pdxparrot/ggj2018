using System;
using System.Collections.Generic;

using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Game;

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

#region Physics
            [Header("Physics")]

            [SerializeField]
            private float _mass = 1.0f;

            public float Mass => _mass;

            [SerializeField]
            private float _drag = 1.0f;

            public float Drag => _drag;

            [SerializeField]
            private float _angularDrag = 1.0f;

            public float AngularDrag => _angularDrag;

            [SerializeField]
            private float _terminalVelocity = 10.0f;

            public float TerminalVelocity => _terminalVelocity;
#endregion

            [Space(10)]

#region Stat Modifiers
            [Header("Stat Modifiers")]

            [SerializeField]
            private bool _canBoost;

            public bool CanBoost => _canBoost;

            [Header("Arcade Stat Modifiers")]

            [SerializeField]
            private float _speedModifier;

            public float SpeedModifier => _speedModifier;

            [SerializeField]
            private float _turnSpeedModifier;

            public float TurnSpeedModifier => _turnSpeedModifier;

            [SerializeField]
            private float _pitchSpeedModifier;

            public float PitchSpeedModifier => _pitchSpeedModifier;

            [Header("Physical Stat Modifiers")]

            [SerializeField]
            private float _linearThrust = 10.0f;

            public float LinearThrust => _linearThrust;

            [SerializeField]
            private float _angularThrust = 10.0f;

            public float AngularThrust => _angularThrust;

            [SerializeField]
            private float _brakeThrust = 5.0f;

            public float BrakeThrust => _brakeThrust;

            [SerializeField]
            private float _boostThrust = 5.0f;

            public float BoostThrust => _boostThrust;
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
        [SerializeField]
        private float _predatorFOV = 60.0f;

        public float PredatorFOV => _predatorFOV;

        [SerializeField]
        private float _preyFOV = 90.0f;

        public float PreyFOV => _preyFOV;
#endregion

#region Layers
        [SerializeField]
        private string _predatorLayer;

        public string PredatorLayer => _predatorLayer;

        [SerializeField]
        private string _preyLayer;

        public string PreyLayer => _preyLayer;
#endregion

#region Post Processing
        [SerializeField]
        private PostProcessProfile _predatorPostProcessProfile;

        public PostProcessProfile PredatorPostProcessProfile => _predatorPostProcessProfile;

        [SerializeField]
        private PostProcessProfile _preyPostProcessProfile;

        public PostProcessProfile PreyPostProcessProfile => _preyPostProcessProfile;
#endregion

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
