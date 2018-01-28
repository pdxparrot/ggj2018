using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
        [SerializeField]
        private LocalPlayer _localPlayerPrefab;

        private GameObject _playerContainer;

        private IPlayer _player;

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");
        }

        private void Update()
        {
            if(null == _player && Input.GetKeyUp(KeyCode.P)) {
                _player = SpawnPlayer();
                _player.MoveTo(new Vector3(0.0f, 25.0f, 0.0f));
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
