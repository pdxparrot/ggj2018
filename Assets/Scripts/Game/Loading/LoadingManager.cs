using System.Collections;

using ggj2018.Core.Assets;
using ggj2018.Game.Audio;
using ggj2018.Game.Data;
using ggj2018.Game.Scenes;
using ggj2018.Core.Util;
using ggj2018.Core.Util.ObjectPool;

using UnityEngine;

namespace ggj2018.Game.Loading
{
    public sealed class LoadingManager : SingletonBehavior<LoadingManager>
    {
        [SerializeField]
        private LoadingScreen _loadingScreen;

#region Manager Prefabs
        [SerializeField]    
        private AssetManager _assetManagerPrefab;

        [SerializeField]
        private ObjectPoolManager _objectPoolManagerPrefab;

        [SerializeField]
        private DataManager _dataManagerPrefab;

        [SerializeField]
        private AudioManager _audioManagerPrefab;

        [SerializeField]
        private GameSceneManager _gameSceneManagerPrefab;
#endregion

        [SerializeField]
        [ReadOnly]
        private GameObject _managersObject;

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
            _loadingScreen.ProgressText = "Creating Managers...";
            yield return null;

            CreateManagers();
            yield return null;

            _loadingScreen.Progress.Percent = 0.25f;
            _loadingScreen.ProgressText = "Loading assets...";
            yield return null;

            IEnumerator routine = AssetManager.Instance.LoadAssetsRoutine();
            while(routine.MoveNext()) {
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

        private void CreateManagers()
        {
            _managersObject = new GameObject("Managers");

            AssetManager.CreateFromPrefab(_assetManagerPrefab.gameObject, _managersObject);
            ObjectPoolManager.CreateFromPrefab(_objectPoolManagerPrefab.gameObject, _managersObject);
            DataManager.CreateFromPrefab(_dataManagerPrefab.gameObject, _managersObject);
            AudioManager.CreateFromPrefab(_audioManagerPrefab.gameObject, _managersObject);
            GameSceneManager.CreateFromPrefab(_gameSceneManagerPrefab.gameObject, _managersObject);
        }

        private void Destroy()
        {
            Destroy(_loadingScreen.gameObject);
            _loadingScreen = null;

            Destroy(gameObject);
        }
    }
}
