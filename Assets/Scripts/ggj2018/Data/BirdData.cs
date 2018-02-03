using System;
using System.Collections.Generic;

using UnityEngine;

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

#region Viewer
            [SerializeField]
            private float _followOrbitRadius = 15.0f;

            public float FollowOrbitRadius => _followOrbitRadius;
#endregion

#region Stat Modifiers
            [SerializeField]
            private float _speedModifier;

            public float SpeedModifier => _speedModifier;

            [SerializeField]
            private float _turnSpeedModifier;

            public float TurnSpeedModifier => _turnSpeedModifier;

            [SerializeField]
            private float _pitchSpeedModifier;

            public float PitchSpeedModifier => _pitchSpeedModifier;
#endregion

#region Bird Type
            [SerializeField]
            private bool _isPredator;

            public bool IsPredator => _isPredator;

            public bool IsPrey => !_isPredator;
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

            public LayerMask PostProcessLayerMask => IsPredator
                ? _birdData.PredatorPostProcessLayerMask
                : _birdData.PreyPostProcessLayerMask;

            public void Initialize(BirdData birdData)
            {
                _birdData = birdData;
            }
        }

#region FOVs
        [SerializeField]
        private float _predatorFOV = 90.0f;

        public float PredatorFOV => _predatorFOV;

        [SerializeField]
        private float _preyFOV = 60;

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

#region Post Processing Layers
        [SerializeField]
        private LayerMask _predatorPostProcessLayerMask;

        public LayerMask PredatorPostProcessLayerMask => _predatorPostProcessLayerMask;

        [SerializeField]
        private LayerMask _preyPostProcessLayerMask;

        public LayerMask PreyPostProcessLayerMask => _preyPostProcessLayerMask;
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
