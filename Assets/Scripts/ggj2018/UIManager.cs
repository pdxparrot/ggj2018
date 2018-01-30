using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField]
        private PauseMenu _pauseMenuPrefab;

        private GameObject _uiContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _uiContainer = new GameObject("UI");
        }

        private void Start()
        {
            SwitchToMenu();
        }

        protected override void OnDestroy()
        {
            Destroy(_uiContainer);
            _uiContainer = null;
        }
#endregion

        /*
        public void Countdown(int c) {
            for(int i = 0; i < PlayerHud.Length; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.SetCountdown(c);
            }
        }
        public void HideCountdown() {
            for(int i = 0; i < PlayerHud.Length; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.HideCountdown();
            }
        }

        public void HideMenu() {
            for(int i = 0; i < PlayerHud.Length; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.Hide();
            }
        }
        */

        public void SwitchToMenu() {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.SwitchToMenu();
            }
        }
        public void SwitchToGame() {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                if(!PlayerManager.Instance.HasPlayer(i))
                    viewer?.PlayerUI.Hide();
                else
                    viewer?.PlayerUI.SwitchToGame();
            }
        }
        public void SwitchToVictory(int winner) {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                if(!PlayerManager.Instance.HasPlayer(i))
                    viewer?.PlayerUI.Hide();
                else
                    viewer?.PlayerUI.SwitchToVictory(winner == i);
            }
        }

        public void EnablePauseUI(bool enable) {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.EnablePauseMenu(enable);
            }
        }
    }
}

