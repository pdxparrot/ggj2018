using System.Collections;

using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ggj2018.Game.Scenes
{
    public sealed class GameReloader : MonoBehavior
    {
        public IEnumerator Reload(string mainSceneName, string reloadSceneName)
        {
            Debug.Log("Unload main scene...");

            AsyncOperation async = SceneManager.UnloadSceneAsync(mainSceneName);
            while(!async.isDone) {
                yield return null;
            }

            Debug.Log("Loading main scene...");

            async = SceneManager.LoadSceneAsync(mainSceneName);
            while(!async.isDone) {
                yield return null;
            }

            Debug.Log("Unloading reload scene...");

            async = SceneManager.UnloadSceneAsync(reloadSceneName);
            while(!async.isDone) {
                yield return null;
            }
        }
    }
}
