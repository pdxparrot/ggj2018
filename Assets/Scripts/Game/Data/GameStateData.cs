using System;

using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [CreateAssetMenu(fileName="GameState", menuName="ggj2018/Data/Game State Data")]
    [Serializable]
    public sealed class GameStateData : ScriptableObject
    {
        public string Name => name;

        [SerializeField]
        private string _sceneName;

        public string SceneName => _sceneName;

        public bool HasScene => !string.IsNullOrWhiteSpace(SceneName);

        [SerializeField]
        private GameState _gameStatePrefab;

        public GameState GameStatePrefab => _gameStatePrefab;

        public GameState InstantiateGameState(Transform parent)
        {
            return Instantiate(_gameStatePrefab, parent);
        }
    }
}
