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

            public override string ToString()
            {
                return $"Bird({Id})";
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

        [SerializeField]
        private BirdDataEntry[] _birds;

        public IReadOnlyCollection<BirdDataEntry> Birds => _birds;

        private readonly Dictionary<string, BirdDataEntry> _entries = new Dictionary<string, BirdDataEntry>();

        public IReadOnlyDictionary<string, BirdDataEntry> Entries => _entries;

        public void Initialize()
        {
            foreach(BirdDataEntry entry in Birds) {
                _entries.Add(entry.Id, entry);
            }
        }
    }
}
