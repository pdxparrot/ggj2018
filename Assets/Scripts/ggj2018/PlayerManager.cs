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

            _players = new IPlayer[GameManager.MaxPlayers];
        }

        private void Update()
        {
            if(null == _players[0] && (Input.GetKeyUp(KeyCode.P) || InputManager.Instance.Pressed(0, 3))) {
                SpawnLocalPlayer(0, "hawk");
            } else if(null != _players[0] && Input.GetKeyDown(KeyCode.K)) {
                _players[0].State.DebugKill();
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

        public void SpawnLocalPlayer(int playerNumber, string birdTypeId)
        {
            if(null != _players[playerNumber]) {
                Debug.LogError("Cannot spawn a player on top of another player!");
                return;
            }

            BirdType birdType = new BirdType(birdTypeId);

            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(birdType);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {birdType}");
                return;
            }

            LocalPlayer player = Instantiate(_localPlayerPrefab, _playerContainer.transform);
            InitializePlayer(player, playerNumber, birdType, spawnPoint);

            Debug.Log($"Spawned {player.State.BirdType.BirdDataEntry.Name} for local player {playerNumber} at {player.transform.position}");

            _players[playerNumber] = player;
        }

        private void InitializePlayer(IPlayer player, int playerNumber, BirdType birdType, SpawnPoint spawnPoint)
        {
            player.Controller.Initialize(player, spawnPoint);
            player.State.Initialize(playerNumber, birdType);
            player.Initialize();
        }

        public void DespawnLocalPlayer(int playerNumber)
        {
            Debug.Log($"Despawning player {playerNumber}");

            if(null == _players[playerNumber]) {
                return;
            }

            Destroy(_players[playerNumber].GameObject);
            _players[playerNumber] = null;
        }
    }
}
