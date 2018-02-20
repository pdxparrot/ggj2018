using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.UI
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField]
        private StartupLogo _startupLogoPrefab;

        private StartupLogo _startupLogo;

        [SerializeField]
        private PauseMenu _pauseMenuPrefab;

        private PauseMenu _pauseMenu;

        [SerializeField]
        private DebugUI _debugUIPrefab;

        private DebugUI _debugUI;

        private GameObject _uiContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _uiContainer = new GameObject("UI");

            _startupLogo = Instantiate(_startupLogoPrefab, _uiContainer.transform);
            _startupLogo.gameObject.SetActive(false);

            _pauseMenu = Instantiate(_pauseMenuPrefab, _uiContainer.transform);
            _pauseMenu.gameObject.SetActive(false);

            _debugUI = Instantiate(_debugUIPrefab, _uiContainer.transform);
            _debugUI.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            Destroy(_uiContainer);
            _uiContainer = null;

            base.OnDestroy();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F)) {
                _debugUI.gameObject.SetActive(!_debugUI.gameObject.activeInHierarchy);
            }
        }
#endregion

        public void SwitchToCharacterSelect()
        {
// TODO: move into the player manager
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                selectState.Viewer.PlayerUI.SwitchToCharacterSelect(selectState);
            }
        }

        public void SwitchToGame(GameType gameType)
        {
// TODO: move into the player manager
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                selectState.Viewer.PlayerUI.Hide();
            }

            foreach(Player player in PlayerManager.Instance.Players) {
                player.Viewer.PlayerUI.SwitchToGame(player, gameType);
            }
        }

        public void SwitchToGameOver(GameType gameType)
        {
// TODO: move into the player manager
            foreach(Player player in PlayerManager.Instance.Players) {
                player.Viewer.PlayerUI.SwitchToGameOver(player, gameType);
            }
        }

        public void EnableStartupLogo(bool enable)
        {
            _startupLogo.gameObject.SetActive(enable);
        }

        public void EnablePauseUI(bool enable)
        {
            _pauseMenu.gameObject.SetActive(enable);
        }
    }
}
