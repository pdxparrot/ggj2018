using System;
using System.Collections.Generic;

using ggj2018.ggj2018.GameTypes;

using UnityEngine;
using UnityEngine.Serialization;

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

#region Win/Loss Texts
            [SerializeField]
            private string _predatorWinConditionDescription;

            public string PredatorWinConditionDescription => _predatorWinConditionDescription;

            [SerializeField]
            private string _predatorWinText;

            public string PredatorWinText => _predatorWinText;

            [SerializeField]
            private string _predatorLossText;

            public string PredatorLossText => _predatorLossText;

            [SerializeField]
            private string _preyWinConditionDescription;

            public string PreyWinConditionDescription => _preyWinConditionDescription;

            [SerializeField]
            private string _preyWinText;

            public string PreyWinText => _preyWinText;

            [SerializeField]
            private string _preyLossText;

            public string PreyLossText => _preyLossText;
#endregion

            [SerializeField]
            private int _scoreLimit = -1;

            public int ScoreLimit => _scoreLimit;

            [SerializeField]
            private int _timeLimit = -1;

            public int TimeLimit => _timeLimit;

            [SerializeField]
            private string _timesUpText;

            public string TimesUpText => _timesUpText;

            public string GetWinConditionDescription(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? PredatorWinConditionDescription : PreyWinConditionDescription;
            }

            public string GetWinText(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? PredatorWinText : PreyWinText;
            }

            public string GetLossText(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? PredatorLossText : PreyLossText;
            }
        }

        [SerializeField]
        private float _immuneTime = 1.0f;

        public float ImmuneTime => _immuneTime;

        [SerializeField]
        private int _gameOverWaitTime = 5;

        public int GameOverWaitTime => _gameOverWaitTime;

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
