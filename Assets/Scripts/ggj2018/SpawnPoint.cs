using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class SpawnPoint : MonoBehavior
    {
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
    }
}
