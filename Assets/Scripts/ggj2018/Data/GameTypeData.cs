using System;
using System.Collections.Generic;

using ggj2018.Core.Util;

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

            public bool IsGameType(int playerCount, int predatorCount, int preyCount)
            {
// TODO: this is an awful hack :\
                if(1 == playerCount || 0 == predatorCount || 0 == preyCount && Id == "crazy_taxi") {
                    return true;
                }
                if(playerCount > 1 && predatorCount > 0 && preyCount > 0 && Id == "hunt") {
                    return true;
                }
                Debug.LogError($"No suitable gametype found! playerCount: {playerCount}, predatorCount: {predatorCount}, preyCount: {preyCount}");
                return false;
            }

            public string GetGoalDescription(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? PredatorWinConditionDescription : PreyWinConditionDescription;
            }

            public bool IsGoalWinCondition(WinCondition winCondition)
            {
                return WinCondition.SingleGoal == winCondition
                    || WinCondition.MultiGoal == winCondition;
            }

            public bool IsKillWinCondition(WinCondition winCondition)
            {
                return WinCondition.Kills == winCondition;
            }

            public bool ShouldCountGoalScore(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? IsGoalWinCondition(PredatorWinCondition) : IsGoalWinCondition(PreyWinCondition);
            }

            public bool ShouldCountKillScore(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator ? IsKillWinCondition(PredatorWinCondition) : IsKillWinCondition(PreyWinCondition);
            }
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

        public GameTypeDataEntry GetGameType(int playerCount, int predatorCount, int preyCount)
        {
            foreach(GameTypeDataEntry entry in GameTypes) {
                if(entry.IsGameType(playerCount, predatorCount, preyCount)) {
                    return entry;
                }
            }
            return null;
        }
    }
}
