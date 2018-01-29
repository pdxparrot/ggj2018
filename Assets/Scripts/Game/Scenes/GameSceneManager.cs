using System;
using System.Collections;
using System.Collections.Generic;

using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ggj2018.Game.Scenes
{
    public sealed class GameSceneManager : SingletonBehavior<GameSceneManager>
    {
        [SerializeField]
        private string _mainSceneName = "main";

        [SerializeField]
        [ReadOnly]
        private string _defaultSceneName;

        public string DefaultSceneName { get; set; }

        private readonly List<string> _loadedScenes = new List<string>();

#region Load Scene
        public void LoadDefaultScene(Action callback)
        {
            LoadScene(DefaultSceneName, callback, true);
        }

        public void LoadScene(string sceneName, Action callback, bool setActive=false)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, () => {
                callback?.Invoke();
            }, setActive));
        }

        public IEnumerator LoadSceneRoutine(string sceneName, Action callback, bool setActive=false)
        {
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while(!asyncOp.isDone) {
                yield return null;
            }

            if(setActive) {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            }

            _loadedScenes.Add(sceneName);

            callback?.Invoke();
        }
#endregion

#region Unload Scene
        public void UnloadDefaultScene(Action callback)
        {
            UnloadScene(DefaultSceneName, callback);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainSceneName));
        }

        public void UnloadScene(string sceneName, Action callback)
        {
            StartCoroutine(UnloadSceneRoutine(sceneName, () => {
                callback?.Invoke();
            }));
        }

        public IEnumerator UnloadSceneRoutine(string sceneName, Action callback)
        {
            AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            while(!asyncOp.isDone) {
                yield return null;
            }

            // TODO: active scene?

            _loadedScenes.Remove(sceneName);

            callback?.Invoke();
        }

        public void UnloadAllScenes()
        {
            foreach(string sceneName in _loadedScenes) {
                SceneManager.UnloadSceneAsync(sceneName);
            }
            _loadedScenes.Clear();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainSceneName));
        }
#endregion

#region Reload Scene
        public void ReloadMainScene()
        {
            UnloadAllScenes();
            SceneManager.LoadScene(_mainSceneName);
        }

        public void ReloadDefaultScene(Action callback)
        {
            UnloadDefaultScene(() => {
                LoadDefaultScene(callback);
            });
        }

        public void ReloadScene(string sceneName, Action callback)
        {
            UnloadScene(sceneName, () => {
                LoadScene(sceneName, callback);
            });
        }
#endregion
    }
}
