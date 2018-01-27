using System.Collections;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Game.Data
{
    public sealed class DataManager : SingletonBehavior<DataManager>
    {
        [SerializeField]
        private GameData _gameData;

        public GameData GameData => _gameData;

        public IEnumerator InitializeRoutine()
        {
            GameData.Initialize();
            GameData.DebugDump();

            yield break;
        }
    }
}
