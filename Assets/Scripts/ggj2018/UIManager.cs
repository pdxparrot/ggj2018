﻿using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField]
        private PauseMenu _pauseMenuPrefab;

        private PauseMenu _pauseMenu;

        private GameObject _uiContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _uiContainer = new GameObject("UI");

            _pauseMenu = Instantiate(_pauseMenuPrefab, _uiContainer.transform);
            _pauseMenu.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            Destroy(_uiContainer);
            _uiContainer = null;
        }
#endregion

        /*
        public void Countdown(int c)
        {
            for(int i = 0; i < PlayerHud.Length; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.SetCountdown(c);
            }
        }

        public void HideCountdown()
        {
            for(int i = 0; i < PlayerHud.Length; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.HideCountdown();
            }
        }

        public void HideMenu()
        {
            for(int i = 0; i < PlayerHud.Length; ++i) {
                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.Hide();
            }
        }
        */

        public void SwitchToMenu()
        {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Viewer viewer = CameraManager.Instance.Viewers.ElementAt(i) as Viewer;
                viewer?.PlayerUI.SwitchToMenu();
            }
        }

        public void SwitchToGame(GameType gameType)
        {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Player player = PlayerManager.Instance.Players.ElementAt(i);
                Viewer viewer = CameraManager.Instance.Viewers.ElementAt(i) as Viewer;
                if(null == player)
                    viewer?.PlayerUI.Hide();
                else
                    viewer?.PlayerUI.SwitchToGame(gameType, player.State.BirdType);
            }
        }

        public void SwitchToVictory(int winner)
        {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                Player player = PlayerManager.Instance.Players.ElementAt(i);
                Viewer viewer = CameraManager.Instance.Viewers.ElementAt(i) as Viewer;
                if(null == player)
                    viewer?.PlayerUI.Hide();
                else
                    viewer?.PlayerUI.SwitchToVictory(winner == i);
            }
        }

        public void EnablePauseUI(bool enable)
        {
            _pauseMenu.gameObject.SetActive(enable);
        }
    }
}

