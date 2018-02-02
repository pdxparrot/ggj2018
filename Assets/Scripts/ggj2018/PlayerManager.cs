using System;
using System.Collections.Generic;
using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;

using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.GameTypes;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
// TODO: merge this with the other PlayerState
        [Serializable]
        public sealed class PlayerState
        {
            public enum JoinState
            {
                None,
                Joined,
                Ready
            }

            [SerializeField]
            private JoinState _joinState = JoinState.None;

            public JoinState PlayerJoinState { get { return _joinState; } set { _joinState = value; } }

            public bool IsJoined => PlayerJoinState == JoinState.Joined;

            public bool IsJoinedOrReady => PlayerJoinState == JoinState.Joined || PlayerJoinState == JoinState.Ready;

            public bool IsReady => PlayerJoinState == JoinState.Ready;

            [SerializeField]
            private int _selectedBird;

            public int SelectedBird { get { return _selectedBird; } set { _selectedBird = value; } }

            public BirdData.BirdDataEntry PlayerBirdData => GameManager.Instance.BirdData.Birds.ElementAt(SelectedBird);

            public string PlayerBirdId => PlayerBirdData.Id;

            public void NextBird()
            {
                SelectedBird++;
                WrapBird();
            }

            public void PrevBird()
            {
                SelectedBird--;
                WrapBird();
            }

            private void WrapBird()
            {
                if(SelectedBird < 0) {
                    SelectedBird = GameManager.Instance.BirdData.Birds.Count - 1;
                } else if(SelectedBird >= GameManager.Instance.BirdData.Birds.Count) {
                    SelectedBird = 0;
                }
            }
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

        public IReadOnlyCollection<IPlayer> Players => _players;

        [SerializeField]
        [ReadOnly]
        private PlayerState[] _playerStates;

        public IReadOnlyCollection<PlayerState> PlayerStates => _playerStates;

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

            _players = new IPlayer[InputManager.Instance.MaxControllers];

            _playerStates = new PlayerState[InputManager.Instance.MaxControllers];
            for(int i=0; i<_playerStates.Length; ++i) {
                _playerStates[i] = new PlayerState();
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

// TODO: separate instantiating/initializing/adding players
// from spawning them (disable their object after creating, basically)

        public void SpawnLocalPlayer(int playerNumber, GameType.GameTypes gameType, string birdTypeId)
        {
            if(null != _players[playerNumber]) {
                Debug.LogError("Cannot spawn a player on top of another player!");
                return;
            }

            BirdData.BirdDataEntry birdType = GameManager.Instance.BirdData.Entries.GetOrDefault(birdTypeId);

            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(gameType, birdType);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {birdTypeId} in game type {gameType}");
                return;
            }

            LocalPlayer player = Instantiate(_localPlayerPrefab, _playerContainer.transform);
            InitializePlayer(player, playerNumber, birdType, spawnPoint);

            Debug.Log($"Spawned {player.State.BirdType.Name} for local player {playerNumber} at {spawnPoint.name} ({player.transform.position})");

            AddPlayer(playerNumber, player);
        }

        public void SpawnNetworkPlayer(int playerNumber, GameType.GameTypes gameTypeId, string birdTypeId)
        {
            if(null != _players[playerNumber]) {
                Debug.LogError("Cannot spawn a player on top of another player!");
                return;
            }

            BirdData.BirdDataEntry birdType = GameManager.Instance.BirdData.Entries.GetOrDefault(birdTypeId);

            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(gameTypeId, birdType);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {birdType} in game type {gameTypeId}");
                return;
            }

            NetworkPlayer player = Instantiate(_networkPlayerPrefab, _playerContainer.transform);
            InitializePlayer(player, playerNumber, birdType, spawnPoint);

            Debug.Log($"Spawned {player.State.BirdType.Name} for local player {playerNumber} at {player.transform.position}");

            AddPlayer(playerNumber, player);
        }

        private void InitializePlayer(IPlayer player, int playerNumber, BirdData.BirdDataEntry birdType, SpawnPoint spawnPoint)
        {
            Bird model = Instantiate(
                birdType.IsPredator
                    ? (Bird)PredatorModelPrefab
                    : (Bird)PreyModelPrefab,
                player.GameObject.transform);

            player.Controller.Initialize(player, model);
            player.State.Initialize(playerNumber, birdType);
            player.Initialize();

            Viewer viewer = CameraManager.Instance.Viewers.ElementAt(playerNumber) as Viewer;
            viewer?.Initialize(player);

            spawnPoint.Spawn(player);
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
            if(player.State.BirdType.IsPrey) {
                _preyCount++;
            }
        }

        private void RemovePlayer(int playerNumber)
        {
            IPlayer player = _players[playerNumber];
            if(player.State.BirdType.IsPrey) {
                _preyCount--;
            }

            Destroy(player.GameObject);
            _players[playerNumber] = null;

            _playerCount--;
        }

#if UNITY_EDITOR
        public void DebugStunAll()
        {
            foreach(var player in _players) {
                player?.State.DebugStun();
            }
        }

        public void DebugKillAll()
        {
            foreach(var player in _players) {
                player?.State.DebugKill();
            }
        }
#endif
    }
}
