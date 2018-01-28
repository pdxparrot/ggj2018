using System.Collections;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.UI;
using ggj2018.Game.Audio;
using ggj2018.Game.Scenes;
using ggj2018.Core.Util;
using ggj2018.Core.Util.ObjectPool;

using UnityEngine;

namespace ggj2018.Game.Loading
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
#endregion

        protected GameObject ManagersContainer { get; private set; }

        [SerializeField]
        private string _defaultSceneName;

#region Unity Lifecycle
        private void Awake()
        {
            ManagersContainer = new GameObject("Managers");
        }

        private void Start()
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

            _loadingScreen.Progress.Percent = 0.75f;
            _loadingScreen.ProgressText = "Loading default scene...";
            GameSceneManager.Instance.LoadScene(_defaultSceneName, () => {
                _loadingScreen.Progress.Percent = 1.0f;
                _loadingScreen.ProgressText = "Loading complete!";

                Destroy();
            });
        }

        protected virtual void CreateManagers()
        {
            TimeManager.Create(ManagersContainer);
            AudioManager.CreateFromPrefab(_audioManagerPrefab, ManagersContainer);
            ObjectPoolManager.Create(ManagersContainer);
            CameraManager.CreateFromPrefab(_cameraManagerPrefab, ManagersContainer);
            InputManager.CreateFromPrefab(_inputManagerPrefab, ManagersContainer);
            GameSceneManager.CreateFromPrefab(_gameSceneManagerPrefab, ManagersContainer);
            UIManager.Create(ManagersContainer);
        }

        protected virtual void InitializeManagers()
        {
            AudioManager.Instance.Initialize();
        }

        private void Destroy()
        {
            Destroy(_loadingScreen.gameObject);
            _loadingScreen = null;

            Destroy(gameObject);
        }
    }
}
