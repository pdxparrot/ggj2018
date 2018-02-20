using System;

using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;

using UnityEngine;

namespace ggj2018.ggj2018.Game
{
    [Serializable]
    public sealed class GameState
    {
        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

        [SerializeField]
        [ReadOnly]
        private GameType _gameType;

        public GameType GameType => _gameType;

        public void SetGameType(int playerCount, int predatorCount, int preyCount)
        {
// TODO: this is an awful hack :\
            if(DebugManager.Instance.SpawnMaxLocalPlayers) {
                _gameType = new Hunt(GameManager.Instance.GameTypeData.GameTypeMap.GetOrDefault(GameType.GameTypes.Hunt));
            } else if(1 == playerCount || 0 == predatorCount || 0 == preyCount) {
                _gameType = new CrazyTaxi(GameManager.Instance.GameTypeData.GameTypeMap.GetOrDefault(GameType.GameTypes.CrazyTaxi));
            } else if(playerCount > 1 && predatorCount > 0 && preyCount > 0) {
                _gameType = new Hunt(GameManager.Instance.GameTypeData.GameTypeMap.GetOrDefault(GameType.GameTypes.Hunt));
            } else {
                Debug.LogError($"No suitable gametype found! playerCount: {playerCount}, predatorCount: {predatorCount}, preyCount: {preyCount}");
            }
        }
    }
}
