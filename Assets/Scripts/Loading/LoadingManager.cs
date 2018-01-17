using System.Collections;

using ggj2018.Audio;
using ggj2018.Data;
using ggj2018.Util;
using ggj2018.Util.ObjectPool;

using UnityEngine;

namespace ggj2018.Loading
{
    public sealed class LoadingManager : SingletonBehavior<LoadingManager>
    {
        [SerializeField]
        private LoadingScreen _loadingScreen;

#region Manager Prefabs
        [SerializeField]
        private ObjectPoolManager _objectPoolManagerPrefab;

        [SerializeField]
        private DataManager _dataManagerPrefab;

        [SerializeField]
        private AudioManager _audioManagerPrefab;
#endregion

        [SerializeField]
        [ReadOnly]
        private GameObject _managersObject;

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

            _loadingScreen.Progress.Percent = 1.0f;
            _loadingScreen.ProgressText = "Loading complete!";
            yield return null;
        }

        private void CreateManagers()
        {
            _managersObject = new GameObject("Managers");

            ObjectPoolManager.CreateFromPrefab(_objectPoolManagerPrefab.gameObject, _managersObject);
            DataManager.CreateFromPrefab(_dataManagerPrefab.gameObject, _managersObject);
            AudioManager.CreateFromPrefab(_audioManagerPrefab.gameObject, _managersObject);
        }

        private void Destroy()
        {
            Destroy(_loadingScreen.gameObject);
            _loadingScreen = null;

            Destroy(gameObject);
        }
    }
}
