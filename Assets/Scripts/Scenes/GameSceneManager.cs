using System;
using System.Collections;
using System.Collections.Generic;

using ggj2018.Util;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ggj2018.Scenes
{
    public /*abstract*/ class GameSceneManager : SingletonBehavior<GameSceneManager>
    {
        private readonly List<string> _loadedScenes = new List<string>();

        [SerializeField]
        [ReadOnly]
        private bool _isGameStarted;

        public bool IsGameStarted { get { return _isGameStarted; } set { _isGameStarted = value; } }

        protected IEnumerator LoadSceneRoutine(string sceneName, Action callback)
        {
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while(!asyncOp.isDone) {
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            _loadedScenes.Add(sceneName);

            callback?.Invoke();
        }

        protected IEnumerator UnloadScene(string sceneName, Action callback)
        {
            AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            while(!asyncOp.isDone) {
                yield return null;
            }
            // TODO: active scene?

            _loadedScenes.Remove(sceneName);

            callback?.Invoke();
        }

        protected void UnloadScenes()
        {
            foreach(string sceneName in _loadedScenes) {
                SceneManager.UnloadSceneAsync(sceneName);
            }
            _loadedScenes.Clear();

            // TODO: active scene?
        }
    }
}
