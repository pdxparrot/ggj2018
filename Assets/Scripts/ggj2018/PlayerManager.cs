using System;
using System.Collections.Generic;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.GameTypes;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
        [SerializeField]
        [FormerlySerializedAs("_localPlayerPrefab")]
        private Player _playerPrefab;

        private GameObject _playerContainer;

        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

        [SerializeField]
        private string _playerCollisionLayerName;

        public LayerMask PlayerCollisionLayer => LayerMask.NameToLayer(_playerCollisionLayerName);

        [SerializeField]
        private float _nearestPlayerUpdateMs = 100.0f;

        public float NearestPlayerUpdateMs => _nearestPlayerUpdateMs;

#region Character Selection
        private readonly List<CharacterSelectState> _characterSelectStates = new List<CharacterSelectState>();

        public IReadOnlyCollection<CharacterSelectState> CharacterSelectStates => _characterSelectStates;
#endregion

#region Players
        // TODO: if we wrap this it would be easier to track alive and dead player counts

        private readonly List<Player> _players = new List<Player>();

        public IReadOnlyCollection<Player> Players => _players;

        private readonly List<Player> _predators = new List<Player>();

        public IReadOnlyCollection<Player> Predators => _predators;

        private readonly List<Player> _prey = new List<Player>();

        public IReadOnlyCollection<Player> Prey => _prey;

        public int PlayerCount => _predators.Count + _prey.Count;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

        public void Initialize()
        {
            for(int i=0; i<GameManager.Instance.ConfigData.MaxLocalPlayers; ++i) {
                Viewer viewer = CameraManager.Instance.AcquireViewer() as Viewer;
                int controllerIndex = InputManager.Instance.AcquireController();
                Debug.Log($"Acquired viewer {viewer?.name} and controller {controllerIndex}");

                _characterSelectStates.Add(new CharacterSelectState(controllerIndex, viewer));
            }
            CameraManager.Instance.ResizeViewports();
        }

        [CanBeNull]
        public Player SpawnPlayer(GameType.GameTypes gameType, CharacterSelectState selectState)
        {
            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(gameType, selectState.PlayerBirdData);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {selectState.PlayerBirdId} in game type {gameType}");
                return null;
            }

            Player player = Instantiate(_playerPrefab, _playerContainer.transform);
            InitializePlayer(player, PlayerCount, selectState, spawnPoint);
            //NetworkServer.Spawn(player.gameObject);

            Debug.Log($"Spawned {player.Bird.Type.Name} for local player {player.Id} at {spawnPoint.name} ({player.transform.position})");

            AddPlayer(player);
            return player;
        }

        private void InitializePlayer(Player player, int playerId, CharacterSelectState selectState, SpawnPoint spawnPoint)
        {
            Bird birdModel = Instantiate(selectState.PlayerBirdData.ModelPrefab, player.transform);

            if(player.IsLocalPlayer) {
                player.InitializeLocal(playerId, selectState.ControllerIndex, selectState.Viewer, birdModel, selectState.PlayerBirdData);
            } else {
                player.InitializeNetwork(playerId, birdModel, selectState.PlayerBirdData);
            }

            spawnPoint.Spawn(player);
        }

        public void DespawnPlayer(Player player)
        {
            Debug.Log($"Despawning player {player.Id}");

            RemovePlayer(player);
        }

        private void AddPlayer(Player player)
        {
            if(player.Bird.Type.IsPredator) {
                _predators.Add(player);
            } else {
                _prey.Add(player);
            }
            _players.Add(player);
        }

        private void RemovePlayer(Player player)
        {
            if(player.Bird.Type.IsPredator) {
                _predators.Remove(player);
            } else {
                _prey.Remove(player);
            }
            _players.Remove(player);

            Destroy(player.gameObject);
        }

#region Players by Distance
        public Player GetNearestPlayer(Player player)
        {
            return GetNearestPlayer(player, _players);
        }

        public Player GetNearestPredator(Player player)
        {
            return GetNearestPlayer(player, _predators);
        }

        public Player GetNearestPrey(Player player)
        {
            return GetNearestPlayer(player, _prey);
        }

        private Player GetNearestPlayer(Player player, List<Player> players)
        {
            return GetPlayerByComparison(player, players, (x, y) => {
                float xd = (x.transform.position - player.transform.position).sqrMagnitude;
                float yd = (y.transform.position - player.transform.position).sqrMagnitude;

                if(xd < yd) {
                    return -1;
                }

                if(xd > yd) {
                    return 1;
                }

                return 0;
            });
        }
#endregion

        private Player GetPlayerByComparison(Player player, List<Player> players, Comparison<Player> comparison)
        {
            if(players.Count < 1) {
                return null;
            }

            players.Sort(comparison);
            return players[0];
        }

#if UNITY_EDITOR
        public void DebugStunAll()
        {
            foreach(Player player in Players) {
                player.State.DebugStun();
            }
        }

        public void DebugKillAll()
        {
            foreach(Player player in Players) {
                player.State.DebugKill();
            }
        }
#endif
    }
}
