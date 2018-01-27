using System.Collections;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.UI;
using ggj2018.Game.Audio;
using ggj2018.Game.Data;
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
        private DataManager _dataManagerPrefab;

        [SerializeField]
        private AudioManager _audioManagerPrefab;

        [SerializeField]
        private Core.Camera.CameraManager _cameraManagerPrefab;

        [SerializeField]
        private InputManager _inputManagerPrefab;

        [SerializeField]
        private GameSceneManager _gameSceneManagerPrefab;
#endregion

        [SerializeField]
        [ReadOnly]
        private GameObject _managersContainer;

        protected GameObject ManagersContainer => _managersContainer;

        [SerializeField]
        private string _defaultSceneName;

#region Unity Lifecycle
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

            IEnumerator runner = InitializeManagers();
            while(runner.MoveNext()) {
                yield return null;
            }

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
            _managersContainer = new GameObject("Managers");

            DataManager.CreateFromPrefab(_dataManagerPrefab.gameObject, ManagersContainer);
            AudioManager.CreateFromPrefab(_audioManagerPrefab.gameObject, ManagersContainer);
            ObjectPoolManager.Create(ManagersContainer);
            Core.Camera.CameraManager.CreateFromPrefab(_cameraManagerPrefab.gameObject, ManagersContainer);
            InputManager.CreateFromPrefab(_inputManagerPrefab.gameObject, ManagersContainer);
            GameSceneManager.CreateFromPrefab(_gameSceneManagerPrefab.gameObject, ManagersContainer);
            UIManager.Create(ManagersContainer);
        }

        // TODO: virtualize
        private IEnumerator InitializeManagers()
        {
            IEnumerator runner = DataManager.Instance.InitializeRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            runner = AudioManager.Instance.InitializeRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }
        }

        private void Destroy()
        {
            Destroy(_loadingScreen.gameObject);
            _loadingScreen = null;

            Destroy(gameObject);
        }
    }
}
