using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.UI
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

        public void SwitchToCharacterSelect()
        {
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                selectState.Viewer.PlayerUI.Initialize(selectState);
                selectState.Viewer.PlayerUI.SwitchToCharacterSelect();
            }
        }

        public void SwitchToGame(GameType gameType)
        {
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                selectState.Viewer.PlayerUI.Hide();
            }

            foreach(Player player in PlayerManager.Instance.Players) {
                player.Viewer.PlayerUI.SwitchToGame(gameType);
            }
        }

        public void SwitchToGameOver(GameType gameType)
        {
            foreach(Player player in PlayerManager.Instance.Players) {
                player.Viewer.PlayerUI.SwitchToGameOver(gameType);
            }
        }

        public void EnablePauseUI(bool enable)
        {
            _pauseMenu.gameObject.SetActive(enable);
        }
    }
}
