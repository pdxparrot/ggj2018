using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="ggj2018/Data/Game Data")]
    [Serializable]
    public sealed class GameData : ScriptableObject
    {
#region Immunity
        [SerializeField]
        private float _gameStartImmuneTime = 2.0f;

        public float GameStartImmuneTime => _gameStartImmuneTime;

        [SerializeField]
        private float _immuneTime = 1.0f;

        public float ImmuneTime => _immuneTime;
#endregion

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

// TODO: move to bird data and rename predatorAlertDistance
        [SerializeField]
        private float _hawkAlertDistance = 25.0f;

        public float HawkAlertDistance => _hawkAlertDistance;

        [SerializeField]
        private GameTypeData[] _gameTypes;

        public IReadOnlyCollection<GameTypeData> GameTypes => _gameTypes;

        private readonly Dictionary<string, GameTypeData> _gameTypeMap = new Dictionary<string, GameTypeData>();

        public IReadOnlyDictionary<string, GameTypeData> GameTypeMap => _gameTypeMap;

        public void Initialize()
        {
            foreach(GameTypeData gameType in GameTypes) {
                Debug.Log($"Registering game type {gameType.Name}");
                _gameTypeMap.Add(gameType.Name, gameType);
            }
        }
    }
}
