using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.World
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
        private GameTypeData[] _gameTypes;

        public IReadOnlyCollection<GameTypeData> GameTypes => _gameTypes;

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

            player.gameObject.SetActive(true);
            player.Spawned();
        }
    }
}
