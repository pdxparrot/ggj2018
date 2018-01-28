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

        private IPlayer[] _players;

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");

            _players = new IPlayer[GameManager.Instance.MaxPlayers];
        }

        private void Update()
        {
            if(null == _players[0] && (Input.GetKeyUp(KeyCode.P) || InputManager.Instance.Pressed(0, 3))) {
                _players[0] = SpawnLocalPlayer(0, "hawk") as LocalPlayer;
                _players[0].Controller.MoveTo(new Vector3(0.0f, 125.0f, 0.0f));
            }

            if(null != _players[0] && Input.GetKeyDown(KeyCode.K)) {
                _players[0].State.EnvironmentKill();
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

        public LocalPlayer SpawnLocalPlayer(int playerNumber, string birdType, Vector3 position=new Vector3(), Quaternion rotation=new Quaternion())
        {
            LocalPlayer player = Instantiate(_localPlayerPrefab, position, rotation, _playerContainer.transform);
            player.Controller.Initialize(player);
            player.State.SetPlayerNumber(playerNumber);
            player.State.SetBirdType(birdType);

            Debug.Log($"Spawned {player.State.BirdType.BirdDataEntry.Name} for local player {playerNumber} at {position}");

            return player;
        }

        public void DespawnLocalPlayer(int playerNumber)
        {
            Debug.Log($"Despawning player {playerNumber}");

            if(null == _players[0]) {
                return;
            }

            Destroy(_players[0].GameObject);
            _players[0] = null;
        }
    }
}
