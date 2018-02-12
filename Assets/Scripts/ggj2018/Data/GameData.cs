using System;
using System.Collections.Generic;

using ggj2018.ggj2018.GameTypes;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="ggj2018/Data/Game Data")]
    [Serializable]
    public sealed class GameData : ScriptableObject
    {
        [SerializeField]
        private float _gameStartImmuneTime = 2.0f;

        public float GameStartImmuneTime => _gameStartImmuneTime;

        [SerializeField]
        private float _immuneTime = 1.0f;

        public float ImmuneTime => _immuneTime;

        [Space(10)]

#region Game Over
        [Header("Game Over")]

        [SerializeField]
        private bool _restartOnGameOver;

        public bool RestartOnGameOver => _restartOnGameOver;

        [SerializeField]
        private int _gameOverWaitTime = 5;

        public int GameOverWaitTime => _gameOverWaitTime;
#endregion

// TODO: move to bird data
        [SerializeField]
        private float _hawkAlertDistance = 25.0f;

        public float HawkAlertDistance => _hawkAlertDistance;

        [SerializeField]
        private GameTypeData[] _gameTypes;

        public IReadOnlyCollection<GameTypeData> GameTypes => _gameTypes;

        private readonly Dictionary<GameType.GameTypes, GameTypeData> _gameTypeMap = new Dictionary<GameType.GameTypes, GameTypeData>();

        public IReadOnlyDictionary<GameType.GameTypes, GameTypeData> GameTypeMap => _gameTypeMap;

        public void Initialize()
        {
            foreach(GameTypeData gameType in GameTypes) {
                _gameTypeMap.Add(gameType.GameType, gameType);
            }
        }
    }
}
