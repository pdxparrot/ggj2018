using System.Collections;

using ggj2018.Core.Assets;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Game.Data
{
    public sealed class DataManager : SingletonBehavior<DataManager>
    {
        [SerializeField]
        private string _gameDataPath = "Data/GameData.asset";

        [SerializeField]
        [ReadOnly]
        private GameData _gameData;

        public GameData GameData => _gameData;

        public IEnumerator InitializeRoutine()
        {
            _gameData = AssetManager.Instance.LoadAsset<GameData>(_gameDataPath);

            GameData.Initialize();
            GameData.DebugDump();

            yield break;
        }
    }
}
