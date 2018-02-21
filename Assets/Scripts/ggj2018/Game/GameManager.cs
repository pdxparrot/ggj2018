using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Network;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.GameTypes;
using pdxpartyparrot.ggj2018.UI;
using pdxpartyparrot.Game.Scenes;

using UnityEngine;
using UnityEngine.EventSystems;

namespace pdxpartyparrot.ggj2018.Game
{
    public sealed class GameManager : SingletonBehavior<GameManager>
    {
#region Events
        public event EventHandler<EventArgs> PauseEvent;
#endregion

#region Systems
        [Header("Systems")]

        [SerializeField]
        private GameObject _gvrPrefab;

        private GameObject _gvr;

        [SerializeField]
        private EventSystem _eventSystemPrefab;

        private EventSystem _eventSystem;

        public EventSystem EventSystem => _eventSystem;

        [SerializeField]
        private NetworkManager _networkManagerPrefab;

        private NetworkManager _networkManager;

        public NetworkManager NetworkManager => _networkManager;
#endregion

        [Space(10)]

#region Data
        [Header("Data")]

        [SerializeField]
        private ConfigData _configData;

        public ConfigData ConfigData => _configData;

        [SerializeField]
        private BirdData _birdData;

        public BirdData BirdData => _birdData;

        [SerializeField]
        private GameData _gameTypeData;

        public GameData GameTypeData => _gameTypeData;
#endregion

        [Space(10)]

#region Physics
        [Header("Physics")]

        [SerializeField]
        private PhysicMaterial _frictionlesssMaterial;

        public PhysicMaterial FrictionlessMaterial => _frictionlesssMaterial;
#endregion

        [Space(10)]

#region Game State
        [Header("Game State")]

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused => _isPaused;

        [SerializeField]
        [ReadOnly]
        private GameType _gameType;

        public GameType GameType => _gameType;

        [SerializeField]
        private bool _enableImmunity;

        public bool EnableImmunity => _enableImmunity;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = ConfigData.TargetFrameRate;
            Debug.Log($"Target frame rate: {Application.targetFrameRate}");

            Debug.Log($"Gravity: {Physics.gravity}");
            Physics.bounceThreshold = Mathf.Infinity;

            _birdData.Initialize();
            _gameTypeData.Initialize();

            if(ConfigData.EnableGVR) {
                Debug.Log("Creating GVR managers...");
                _gvr = Instantiate(_gvrPrefab);
            } else {
                Debug.Log("Creating EventSystem (no VR)...");
                _eventSystem = Instantiate(_eventSystemPrefab);
            }

            Debug.Log("Creating NetworkManager...");
            _networkManager = Instantiate(_networkManagerPrefab);
        }

        protected override void OnDestroy()
        {
            Destroy(_networkManager?.gameObject);
            _networkManager = null;

            Destroy(_eventSystem?.gameObject);
            _eventSystem = null;

            Destroy(_gvr);
            _gvr = null;

            base.OnDestroy();
        }

        private void Update()
        {
            CheckReload();
        }
#endregion

        public void Initialize()
        {
            NetworkManager.Initialize(ConfigData.EnableNetwork);

            CameraManager.Instance.SpawnViewers(ConfigData.MaxLocalPlayers);
        }

        public void SetGameType(int playerCount, int predatorCount, int preyCount)
        {
// TODO: this is an awful hack :\
            if(DebugManager.Instance.SpawnMaxLocalPlayers) {
                _gameType = new Hunt(GameTypeData.GameTypeMap.GetOrDefault(GameType.GameTypes.Hunt));
            } else if(1 == playerCount || 0 == predatorCount || 0 == preyCount) {
                _gameType = new CrazyTaxi(GameTypeData.GameTypeMap.GetOrDefault(GameType.GameTypes.CrazyTaxi));
            } else if(playerCount > 1 && predatorCount > 0 && preyCount > 0) {
                _gameType = new Hunt(GameTypeData.GameTypeMap.GetOrDefault(GameType.GameTypes.Hunt));
            } else {
                Debug.LogError($"No suitable gametype found! playerCount: {playerCount}, predatorCount: {predatorCount}, preyCount: {preyCount}");
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;

            UIManager.Instance.EnablePauseUI(IsPaused);

            PauseEvent?.Invoke(this, EventArgs.Empty);
        }

        private void CheckReload()
        {
            if(!IsPaused) {
                return;
            }

            if(InputManager.Instance.SelectPressed()) {
#if false
                Debug.Log("Restarting game!");
                GameSceneManager.Instance.ReloadMainScene();
#else
                Debug.Log("Quitting game!");
                Application.Quit();
#endif
            }
        }
    }
}

