using System.Collections;

using DG.Tweening;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Game.Audio;
using pdxpartyparrot.Game.Scenes;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.Util.ObjectPool;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.Loading
{
    public abstract class LoadingManager<T> : SingletonBehavior<T> where T: LoadingManager<T>
    {
        [SerializeField]
        private LoadingScreen _loadingScreen;

#region Manager Prefabs
        [SerializeField]
        private AudioManager _audioManagerPrefab;

        [SerializeField]
        private CameraManager _cameraManagerPrefab;

        [SerializeField]
        private InputManager _inputManagerPrefab;

        [SerializeField]
        private GameSceneManager _gameSceneManagerPrefab;

        [SerializeField]
        private GameStateManager _gameStateManagerPrefab;
#endregion

        protected GameObject ManagersContainer { get; private set; }

        [SerializeField]
        private string _defaultSceneName;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            ManagersContainer = new GameObject("Managers");
        }

        protected virtual void Start()
        {
            StartCoroutine(Load());
        }
#endregion

        private IEnumerator Load()
        {
            _loadingScreen.Progress.Percent = 0.0f;
            _loadingScreen.ProgressText = "Creating managers...";
            yield return null;

            CreateManagers();
            yield return null;

            _loadingScreen.Progress.Percent = 0.25f;
            _loadingScreen.ProgressText = "Initializing managers...";
            yield return null;

            InitializeManagers();
            yield return null;

            // TODO: default scene should come from the game state
            _loadingScreen.Progress.Percent = 0.75f;
            _loadingScreen.ProgressText = "Loading default scene...";
            GameSceneManager.Instance.LoadDefaultScene(() => {
                _loadingScreen.Progress.Percent = 1.0f;
                _loadingScreen.ProgressText = "Loading complete!";

                GameStateManager.Instance.TransitionToInitialState();

                Destroy();
            });
        }

        protected virtual void CreateManagers()
        {
            // TODO: this should be initialized by a Core something
            DOTween.Init();

            TimeManager.Create(ManagersContainer);
            AudioManager.CreateFromPrefab(_audioManagerPrefab, ManagersContainer);
            ObjectPoolManager.Create(ManagersContainer);
            CameraManager.CreateFromPrefab(_cameraManagerPrefab, ManagersContainer);
            InputManager.CreateFromPrefab(_inputManagerPrefab, ManagersContainer);
            GameSceneManager.CreateFromPrefab(_gameSceneManagerPrefab, ManagersContainer);
            GameStateManager.CreateFromPrefab(_gameStateManagerPrefab, ManagersContainer);
        }

        protected virtual void InitializeManagers()
        {
            GameSceneManager.Instance.DefaultSceneName = _defaultSceneName;
        }

        private void Destroy()
        {
            Destroy(_loadingScreen.gameObject);
            _loadingScreen = null;

            Destroy(gameObject);
        }
    }
}
