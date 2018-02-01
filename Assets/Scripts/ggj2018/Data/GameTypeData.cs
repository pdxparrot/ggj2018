using System;
using System.Collections.Generic;

using ggj2018.ggj2018.GameTypes;

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
            private GameType.GameTypes _gameType;

            public GameType.GameTypes GameType => _gameType;

            [SerializeField]
            private string _name;

            public string Name => _name;

#region Win Conditions
            [SerializeField]
            private string _predatorWinConditionDescription;

            public string PredatorWinConditionDescription => _predatorWinConditionDescription;

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

            public string GetWinConditionDescription(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? PredatorWinConditionDescription : PreyWinConditionDescription;
            }
        }

        [SerializeField]
        private GameTypeDataEntry[] _gameTypes;

        public IReadOnlyCollection<GameTypeDataEntry> GameTypes => _gameTypes;

        private readonly Dictionary<GameType.GameTypes, GameTypeDataEntry> _entries = new Dictionary<GameType.GameTypes, GameTypeDataEntry>();

        public IReadOnlyDictionary<GameType.GameTypes, GameTypeDataEntry> Entries => _entries;

        public void Initialize()
        {
            foreach(GameTypeDataEntry entry in GameTypes) {
                _entries.Add(entry.GameType, entry);
            }
        }
    }
}
