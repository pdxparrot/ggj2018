using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
        [SerializeField]
        private LocalPlayer _localPlayerPrefab;

        private GameObject _playerContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.S)) {
                LocalPlayer player = SpawnPlayer() as LocalPlayer;
                player.transform.position = new Vector3(0.0f, 25.0f, 0.0f);
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

        public IPlayer SpawnPlayer(Vector3 position=new Vector3(), Quaternion rotation=new Quaternion())
        {
            return Instantiate(_localPlayerPrefab, position, rotation, _playerContainer.transform);
        }
    }
}
