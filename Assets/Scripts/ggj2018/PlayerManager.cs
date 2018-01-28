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

        [SerializeField]
        [ReadOnly]
        private int _playerCount;

        public int PlayerCount => _playerCount;

        [SerializeField]
        [ReadOnly]
        private int _preyCount;

        public int PreyCount => _preyCount;

        public int PredatorCount => PlayerCount - PreyCount;

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");

            _players = new IPlayer[GameManager.MaxPlayers];
        }

        private void Update()
        {
#if false
            if(null != _players[0] && UnityEngine.Input.GetKeyDown(KeyCode.K)) {
                _players[0].State.DebugKill();
            }
#endif
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

            AddPlayer(playerNumber, player);
        }

        private void InitializePlayer(IPlayer player, int playerNumber, BirdType birdType, SpawnPoint spawnPoint)
        {
            player.Controller.Initialize(player, spawnPoint);
            player.State.Initialize(playerNumber, birdType);
        }

        public void DespawnLocalPlayer(int playerNumber)
        {
            if(null == _players[playerNumber]) {
                return;
            }

            Debug.Log($"Despawning player {playerNumber}");

            RemovePlayer(playerNumber);
        }

        public void DespawnAllPlayers()
        {
            Debug.Log("Despawning everybody");
            for(int i=0; i<_players.Length; ++i) {
                if(null == _players[i]) {
                    continue;
                }
                RemovePlayer(i);
            }
        }

        private void AddPlayer(int playerNumber, IPlayer player)
        {
            _players[playerNumber] = player;

            _playerCount++;
            if(player.State.BirdType.BirdDataEntry.IsPrey) {
                _preyCount++;
            }
        }

        private void RemovePlayer(int playerNumber)
        {
            IPlayer player = _players[playerNumber];
            if(player.State.BirdType.BirdDataEntry.IsPrey) {
                _preyCount--;
            }

            Destroy(player.GameObject);
            _players[playerNumber] = null;
            _playerCount--;
        }
    }
}
