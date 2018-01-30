using System;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;

using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
        [Serializable]
        public sealed class PlayerState
        {
            [SerializeField]
            private bool _playerJoined;

            public bool PlayerJoined { get { return _playerJoined; } set { _playerJoined = value; } }

            [SerializeField]
            private bool _playerReady;

            public bool PlayerReady { get { return _playerReady; } set { _playerReady = value; } }

            [SerializeField]
            private int _playerBird;

            public int PlayerBird { get { return _playerBird; } set { _playerBird = value; } }
        }

#region Models
        [SerializeField]
        private Predator _predatorModelPrefab;

        public Predator PredatorModelPrefab => _predatorModelPrefab;

        [SerializeField]
        private Prey _preyModelPrefab;

        public Prey PreyModelPrefab => _preyModelPrefab;
#endregion

        [SerializeField]
        private LocalPlayer _localPlayerPrefab;

        [SerializeField]
        private NetworkPlayer _networkPlayerPrefab;

        private GameObject _playerContainer;

        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

        private IPlayer[] _players;

        [SerializeField]
        [ReadOnly]
        private PlayerState[] _playerStates;

        [SerializeField]
        [ReadOnly]
        private int _playerCount;

        public int PlayerCount => _playerCount;

        [SerializeField]
        [ReadOnly]
        private int _preyCount;

        public int PreyCount => _preyCount;

        public int PredatorCount => PlayerCount - PreyCount;

        public IPlayer Player(int i) {
            return _players[i];
        }
        public bool HasPlayer(int i) {
            return _players[i] != null;
        }
        public PlayerState GetPlayerState(int i) {
            return _playerStates[i];
        }

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");

            _players = new IPlayer[InputManager.Instance.MaxControllers];

            _playerStates = new PlayerState[InputManager.Instance.MaxControllers];
            for(int i=0; i<InputManager.Instance.MaxControllers; ++i) {
                _playerStates[i] = new PlayerState();
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K)) {
                foreach(var player in _players) {
                    player?.State.DebugKill();
                }
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

            Debug.Log($"Spawned {player.State.BirdType.BirdDataEntry.Name} for local player {playerNumber} at {spawnPoint.name} ({player.transform.position})");

            AddPlayer(playerNumber, player);
        }

        public void SpawnNetworkPlayer(int playerNumber, string birdTypeId)
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

            NetworkPlayer player = Instantiate(_networkPlayerPrefab, _playerContainer.transform);
            InitializePlayer(player, playerNumber, birdType, spawnPoint);

            Debug.Log($"Spawned {player.State.BirdType.BirdDataEntry.Name} for local player {playerNumber} at {player.transform.position}");

            AddPlayer(playerNumber, player);
        }

        private void InitializePlayer(IPlayer player, int playerNumber, BirdType birdType, SpawnPoint spawnPoint)
        {
            Bird model = Instantiate(
                birdType.BirdDataEntry.IsPredator
                    ? (Bird)PredatorModelPrefab
                    : (Bird)PreyModelPrefab,
                player.GameObject.transform);

            player.Controller.Initialize(player, spawnPoint, model);
            player.State.Initialize(playerNumber, birdType);
            player.Initialize();

            Viewer viewer = CameraManager.Instance.GetViewer(playerNumber) as Viewer;
            viewer?.Initialize(birdType);
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

        public int HawkIndex()
        {
            for(int i = 0; i < _players.Length; ++i) {
                if(null != _players[i] &&
                   _players[i].State.BirdType.BirdDataEntry.IsPredator)
                    return i;
            }

            return 0;
        }
    }
}
