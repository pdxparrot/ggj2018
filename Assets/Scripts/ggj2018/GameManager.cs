using System;

using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.Core.Camera;
using ggj2018.ggj2018.Data;
using ggj2018.Game.Scenes;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
    public sealed class GameManager : SingletonBehavior<GameManager>
    {
#region Events
        public event EventHandler<EventArgs> PauseEvent;
#endregion

#region Systems
        [SerializeField]
        private GameObject _gvrPrefab;

        private GameObject _gvr;

        [SerializeField]
        private EventSystem _eventSystemPrefab;

        private EventSystem _eventSystem;

        [SerializeField]
        private NetworkManager _networkManagerPrefab;

        private NetworkManager _networkManager;
#endregion

#region Data
        [SerializeField]
        private ConfigData _configData;

        public ConfigData ConfigData => _configData;

        [SerializeField]
        private BirdData _birdData;

        public BirdData BirdData => _birdData;

        [SerializeField]
        private GameTypeData _gameTypeData;

        public GameTypeData GameTypeData => _gameTypeData;
#endregion

        [SerializeField]
        [ReadOnly]
        private /*readonly*/ GameState _gameState = new GameState();

        public GameState State => _gameState;

#region Unity Lifecycle
        private void Awake()
        {
            _birdData.Initialize();
            _gameTypeData.Initialize();

            if(ConfigData.EnableGVR) {
                _gvr = Instantiate(_gvrPrefab);
            } else {
                _eventSystem = Instantiate(_eventSystemPrefab);
            }

            if(ConfigData.EnableNetwork) {
                _networkManager = Instantiate(_networkManagerPrefab);
            }
        }

        protected override void OnDestroy()
        {
            Destroy(_networkManager);
            _networkManager = null;

            Destroy(_eventSystem);
            _eventSystem = null;

            Destroy(_gvr);
            _gvr = null;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            CheckPause();
            CheckReload();

            _gameState.Update(dt);
        }
#endregion

        public void Initialize()
        {
            CameraManager.Instance.SpawnViewers(PlayerManager.Instance.MaxLocalPlayers);
            State.SetState(GameState.States.Menu);
        }

        private void CheckPause()
        {
            if(!State.CanPause) {
                return;
            }

            if(InputManager.Instance.StartPressed()) {
                _gameState.IsPaused = !_gameState.IsPaused;

                UIManager.Instance.EnablePauseUI(_gameState.IsPaused);

                PauseEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CheckReload()
        {
            if(!_gameState.IsPaused) {
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

