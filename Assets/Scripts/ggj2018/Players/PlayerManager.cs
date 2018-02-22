using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Birds;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.VFX;
using pdxpartyparrot.ggj2018.World;
using pdxpartyparrot.Game.Audio;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ggj2018.Players
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

        public bool HasAlivePlayer => Predators.Count > 0 || Prey.Count > 0;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");
        }

        private void Start()
        {
            StartCoroutine(UpdateMusicCrossfade());
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
                    player.Viewer.PlayerUI.PlayerHUD.EnableDebugVisualizer(true);
                }
            }
#endif
        }
#endregion

        [CanBeNull]
        public Player SpawnPlayer(GameTypeData gameTypeData, CharacterSelectState selectState)
        {
            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(gameTypeData, selectState.PlayerBirdData);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {selectState.PlayerBirdData.Id} in game type {gameTypeData.Name}");
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
        public Player GetNearestPlayer(Player player, out float distance)
        {
            return GetNearestPlayer(player, _players, out distance);
        }

        public Player GetNearestPredator(Player player, out float distance)
        {
            return GetNearestPlayer(player, _predators, out distance);
        }

        public Player GetNearestPrey(Player player, out float distance)
        {
            return GetNearestPlayer(player, _prey, out distance);
        }

        private Player GetNearestPlayer(Player player, List<Player> players, out float distance)
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
            }, out distance);
        }
#endregion

        private Player GetPlayerByComparison(Player player, List<Player> players, Comparison<Player> comparison, out float distance)
        {
            distance = float.MaxValue;

            if(players.Count < 1) {
                return null;
            }

            players.Sort(comparison);

            Player nearest = players[0];
            distance = (nearest.transform.position - player.transform.position).magnitude;

            return nearest;
        }

        private IEnumerator UpdateMusicCrossfade()
        {
            WaitForSeconds wait = new WaitForSeconds(NearestPlayerUpdateMs / 1000.0f);

            while(true) {
                Player closest = null;
                foreach(Player player in Predators) {
                    if(null == closest || player.NearestPreyDistance < closest.NearestPreyDistance) {
                        closest = player;
                    }
                }

                // TODO: this should be logarithmic rather than linear
                AudioManager.Instance.MusicCrossFade = 1.0f - ((closest?.NearestPreyDistance ?? float.MaxValue) / GameManager.Instance.GameTypeData.HawkAlertDistance);

                yield return wait;
            }
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
