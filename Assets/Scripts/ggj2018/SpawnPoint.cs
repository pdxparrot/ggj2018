using System.Collections.Generic;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class SpawnPoint : MonoBehavior
    {
        [SerializeField]
        private string[] _gameTypeIds;

        public IReadOnlyCollection<string> GameTypeIds => _gameTypeIds;

        [SerializeField]
        private bool _isPredatorSpawn;

        public bool IsPredatorSpawn => _isPredatorSpawn;

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

        public void Spawn(IPlayer player)
        {
            player.GameObject.transform.position = transform.position;
            player.GameObject.transform.rotation = transform.rotation;
        }
    }
}
