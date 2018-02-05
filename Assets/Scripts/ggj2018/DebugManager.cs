using ggj2018.Core.Util;
using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class DebugManager : SingletonBehavior<DebugManager>
    {
        [SerializeField]
        private Predator _hawkModelPrefab;

        [SerializeField]
        private Vector3 _hawkSpawnPosition;

        [SerializeField]
        private Prey _pigeonModelPrefab;

        [SerializeField]
        private Vector3 _pigeonSpawnPosition;

        private GameObject _debugContainer;

#region Unity Lifecycle
        protected override void OnDestroy()
        {
            Destroy(_debugContainer);
            _debugContainer = null;
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
