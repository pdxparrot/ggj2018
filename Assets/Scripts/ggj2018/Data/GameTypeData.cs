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
            public enum WinCondition
            {
                SingleGoal,
                MultiGoal,
                Kills,
                Survive
            }

            [SerializeField]
            private string _id;

            public string Id => _id;

            [SerializeField]
            private string _name;

            public string Name => _name;

#region Predator Win Condition
            [SerializeField]
            private WinCondition _predatorWinCondition;

            public WinCondition PredatorWinCondition => _predatorWinCondition;

            [SerializeField]
            private string _predatorWinConditionDescription;

            public string PredatorWinConditionDescription => _predatorWinConditionDescription;
#endregion


#region Prey Win Condition
            [SerializeField]
            private WinCondition _preyWinCondition;

            public WinCondition PreyWinCondition => _preyWinCondition;

            [SerializeField]
            private string _preyWinConditionDescription;

            public string PreyWinConditionDescription => _preyWinConditionDescription;
#endregion

            [SerializeField]
            private int _targetGoalScore = -1;

            public int TargetGoalScore => _targetGoalScore;

            [SerializeField]
            private int _timeLimit = -1;

            public int TimeLimit => _timeLimit;

            [SerializeField]
            private bool _useTeams;

            public bool UseTeams => _useTeams;

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
