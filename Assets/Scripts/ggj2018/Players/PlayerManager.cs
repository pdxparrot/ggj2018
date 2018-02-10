using System;
using System.Collections.Generic;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.VFX;
using ggj2018.ggj2018.World;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace ggj2018.ggj2018.Players
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
        [SerializeField]
        [FormerlySerializedAs("_localPlayerPrefab")]
        private Player _playerPrefab;

        [SerializeField]
        private GodRay _playerGodRayPrefab;

        public GodRay PlayerGodRayPrefab => _playerGodRayPrefab;

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

        private readonly List<Player> _deadPlayers = new List<Player>();
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

        private void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.V)) {
                foreach(Player player in Players) {
                    player.Viewer.PlayerUI.EnableDebugVisualizer(true);
                }
            }
#endif
        }
#endregion

        public void Initialize()
        {
            for(int i=0; i<GameManager.Instance.ConfigData.MaxLocalPlayers; ++i) {
                int controllerIndex = InputManager.Instance.AcquireController();
                Debug.Log($"Acquired controller {controllerIndex}");

                _characterSelectStates.Add(new CharacterSelectState(controllerIndex));
            }
            ResetCharacterSelect();
        }

        public void ResetCharacterSelect()
        {
            foreach(CharacterSelectState selectState in CharacterSelectStates) {
                selectState.Reset();
            }
            CameraManager.Instance.ResizeViewports();
        }

        [CanBeNull]
        public Player SpawnPlayer(GameType.GameTypes gameType, CharacterSelectState selectState)
        {
            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(gameType, selectState.PlayerBirdData);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {selectState.PlayerBirdData.Id} in game type {gameType}");
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

        public void KillPlayer(Player player)
        {
            if(player.Bird.Type.IsPredator) {
                _predators.Remove(player);
            } else {
                _prey.Remove(player);
            }

            _deadPlayers.Add(player);

            player.Viewer.PlayerUI.SwitchToDead();
            player.Died();
        }

        public void DespawnPlayer(Player player)
        {
            Debug.Log($"Despawning player {player.Id}");

            RemovePlayer(player, true);
        }

        public void DespawnAllPlayers()
        {
            Debug.Log($"Despawning all ({Players.Count}) players");
            foreach(Player player in Players) {
                RemovePlayer(player, false);
            }

            _players.Clear();
            _predators.Clear();
            _prey.Clear();
            _deadPlayers.Clear();
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

        private void RemovePlayer(Player player, bool removeFromPlayers)
        {
            if(player.State.IsDead) {
                _deadPlayers.Remove(player);
            }

            if(player.Bird.Type.IsPredator) {
                _predators.Remove(player);
            } else {
                _prey.Remove(player);
            }

            if(removeFromPlayers) {
                _players.Remove(player);
            }

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
