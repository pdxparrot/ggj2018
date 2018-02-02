using System.Collections.Generic;

using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class SpawnPoint : MonoBehavior
    {
        public enum SpawnPointType
        {
            Predator,
            Prey,
            Message
        }

        [SerializeField]
        private GameType.GameTypes[] _gameTypes;

        public IReadOnlyCollection<GameType.GameTypes> GameTypes => _gameTypes;

        [SerializeField]
        private SpawnPointType _type;

        public SpawnPointType Type => _type;

#region Unity Lifecycle
        private void Awake()
        {
            SpawnManager.Instance.RegisterSpawnPoint(this);
        }

        private void OnDestroy()
        {
            if(SpawnManager.HasInstance) {
                SpawnManager.Instance.UnregisterSpawnPoint(this);
            }
        }
#endregion

        public void Spawn(Player player)
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }
}
