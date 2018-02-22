using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Birds;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018
{
    public class DebugManager : SingletonBehavior<DebugManager>
    {
#region Debug Models
        [SerializeField]
        private Predator _hawkModelPrefab;

        [SerializeField]
        private Vector3 _hawkSpawnPosition;

        [SerializeField]
        private Prey _pigeonModelPrefab;

        [SerializeField]
        private Vector3 _pigeonSpawnPosition;
#endregion

        [SerializeField]
        private bool _spawnMaxLocalPlayers;

        public bool SpawnMaxLocalPlayers => _spawnMaxLocalPlayers;

        [SerializeField]
        private bool _useInfiniteBoost;

        public bool UseInfiniteBoost => _useInfiniteBoost;

        private GameObject _debugContainer;

#region Unity Lifecycle
        protected override void OnDestroy()
        {
            Destroy(_debugContainer);
            _debugContainer = null;

            base.OnDestroy();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(null == _debugContainer && Input.GetKeyDown(KeyCode.D)) {
                _debugContainer = new GameObject("Debug");

                Instantiate(_hawkModelPrefab, _hawkSpawnPosition, Quaternion.identity, _debugContainer.transform);
                Instantiate(_pigeonModelPrefab, _pigeonSpawnPosition, Quaternion.identity, _debugContainer.transform);
            }

            if(Input.GetKeyDown(KeyCode.K)) {
                PlayerManager.Instance.DebugKillAll();
            }

            if(Input.GetKeyDown(KeyCode.S)) {
                PlayerManager.Instance.DebugStunAll();
            }
#endif
        }
#endregion
    }
}
