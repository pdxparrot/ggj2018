using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace ggj2018.Core
{
    public class SplashScreen : MonoBehavior
    {
        [SerializeField]
        private VideoClip[] _splashScreens;

        [SerializeField]
        private string _mainSceneName = "main";

        [SerializeField]
        private UnityEngine.Camera _camera;

        [SerializeField]
        [ReadOnly]
        private int _currentSplashScreen;

        private VideoPlayer _videoPlayer;

#region Unity Lifecycle
        private void Awake()
        {
            _videoPlayer = _camera.gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
            _videoPlayer.isLooping = false;
        }

        private void Start()
        {
            PlayNextSplashScreen();
        }
#endregion


        private void PlayNextSplashScreen()
        {
            if(_currentSplashScreen >= _splashScreens.Length) {
                SceneManager.LoadScene(_mainSceneName);
                return;
            }

            VideoPlayer.EventHandler eventHandler = null;
            eventHandler = vp => {
                _videoPlayer.loopPointReached -= eventHandler;

                _currentSplashScreen++;
                PlayNextSplashScreen();
            };

            _videoPlayer.clip = _splashScreens[_currentSplashScreen];
            _videoPlayer.loopPointReached += eventHandler;
            _videoPlayer.Play();
        }
    }
}
