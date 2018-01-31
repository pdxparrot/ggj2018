using System;
using System.Collections.Generic;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="GameTypeData", menuName="ggj2018/Data/Game Type Data")]
    [Serializable]
    public sealed class GameTypeData : ScriptableObject
    {
        [Serializable]
        public sealed class GameTypeDataEntry
        {
            [SerializeField]
            private string _id;

            public string Id => _id;

            [SerializeField]
            private string _name;

            public string Name => _name;

            [SerializeField]
            private int _goalScore = -1;

            public int GoalScore => _goalScore;

            [SerializeField]
            private int _timeLimit = -1;

            public int TimeLimit => _timeLimit;

            [SerializeField]
            private bool _birdTypesShareSpawnPoints;

            public bool BirdTypesShareSpawnPoints => _birdTypesShareSpawnPoints;
        }

        [SerializeField]
        private GameTypeDataEntry[] _gameTypes;

        public IReadOnlyCollection<GameTypeDataEntry> GameTypes => _gameTypes;

        private readonly Dictionary<string, GameTypeDataEntry> _entries = new Dictionary<string, GameTypeDataEntry>();

        public IReadOnlyDictionary<string, GameTypeDataEntry> Entries => _entries;

        public void Initialize()
        {
            foreach(GameTypeDataEntry entry in GameTypes) {
                _entries.Add(entry.Id, entry);
            }
        }
    }
}
