using System;

using pdxpartyparrot.ggj2018.GameTypes;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Data
{
    [CreateAssetMenu(fileName="GameTypeData", menuName="ggj2018/Data/Game Type Data")]
    [Serializable]
    public sealed class GameTypeData : ScriptableObject
    {
        [SerializeField]
        private GameType.GameTypes _gameType;

        public GameType.GameTypes GameType => _gameType;

        [SerializeField]
        private string _name;

        public string Name => _name;

        [Space(10)]

#region Win/Loss Texts
        [Header("Win/Loss Text")]

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

        [Space(10)]

#region End Game
        [Header("End Game")]

        [SerializeField]
        private int _scoreLimit = -1;

        public int ScoreLimit => _scoreLimit;

        [SerializeField]
        private int _timeLimit = -1;

        public int TimeLimit => _timeLimit;

        [SerializeField]
        private string _timesUpText;

        public string TimesUpText => _timesUpText;
#endregion

        public string GetWinConditionDescription(BirdTypeData birdType)
        {
            return birdType.IsPredator ? PredatorWinConditionDescription : PreyWinConditionDescription;
        }

        public string GetWinText(BirdTypeData birdType)
        {
            return birdType.IsPredator ? PredatorWinText : PreyWinText;
        }

        public string GetLossText(BirdTypeData birdType)
        {
            return birdType.IsPredator ? PredatorLossText : PreyLossText;
        }
    }
}
