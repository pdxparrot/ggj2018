using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace pdxpartyparrot.Game.Scenes
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

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();
        }
#endregion

#region Load Scene
        public void SetScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

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

        public void UnloadAllScenes(Action callback)
        {
            foreach(string sceneName in _loadedScenes) {
                SceneManager.UnloadSceneAsync(sceneName);
            }
            _loadedScenes.Clear();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainSceneName));
        }

        public IEnumerator UnloadAllScenesRoutine(Action callback)
        {
            foreach(string sceneName in _loadedScenes) {
                IEnumerator runner = UnloadSceneRoutine(sceneName, null);
                while(runner.MoveNext()) {
                    yield return null;
                }
            }
            _loadedScenes.Clear();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainSceneName));

            callback?.Invoke();
        }
#endregion

#region Reload Scene
        public void ReloadMainScene()
        {
            UnloadAllScenes(() => {
                SceneManager.LoadScene(_mainSceneName);
            });
        }

        public void ReloadDefaultScene(Action callback)
        {
            ReloadScene(DefaultSceneName, callback);
        }

        public void ReloadScene(string sceneName, Action callback)
        {
            UnloadScene(sceneName, () => {
                LoadScene(sceneName, callback);
            });
        }
#endregion

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "GameSceneManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Loaded Scenes", GUI.skin.box);
                    foreach(string loadedScene in _loadedScenes) {
                        GUILayout.Label(loadedScene);
                    }
                GUILayout.EndVertical();
            };
        }
    }
}
