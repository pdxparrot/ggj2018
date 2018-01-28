using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
        [SerializeField]
        private LocalPlayer _localPlayerPrefab;

        private GameObject _playerContainer;

        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

        private IPlayer _player;

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");

            _playerData.Initialize();
        }

        private void Update()
        {
            if(null == _player && Input.GetKeyUp(KeyCode.P) || InputManager.Instance.Pressed(0, 3)) {
                _player = SpawnPlayer(0);
                _player.Controller.MoveTo(new Vector3(0.0f, 225.0f, 0.0f));
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

        public IPlayer SpawnPlayer(int playerNumber, Vector3 position=new Vector3(), Quaternion rotation=new Quaternion())
        {
            LocalPlayer player = Instantiate(_localPlayerPrefab, position, rotation, _playerContainer.transform);
            player.Controller.Initialize(player);
            player.State.SetPlayerNumber(playerNumber);
            return player;
        }
    }
}
