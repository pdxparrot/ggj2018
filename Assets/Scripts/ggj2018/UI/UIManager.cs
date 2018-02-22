using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.UI
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
#region UI Prefabs
        [SerializeField]
        private StartupLogo _startupLogoPrefab;

        private StartupLogo _startupLogo;

        [SerializeField]
        private PauseMenu _pauseMenuPrefab;

        private PauseMenu _pauseMenu;

        [SerializeField]
        private DebugUI _debugUIPrefab;

        private DebugUI _debugUI;
#endregion

        private GameObject _uiContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _uiContainer = new GameObject("UI");

            _startupLogo = Instantiate(_startupLogoPrefab, _uiContainer.transform);
            EnableStartupLogo(false);

            _pauseMenu = Instantiate(_pauseMenuPrefab, _uiContainer.transform);
            EnablePauseUI(false);

            _debugUI = Instantiate(_debugUIPrefab, _uiContainer.transform);
            EnableDebugUI(false);
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
                EnableDebugUI(!_debugUI.gameObject.activeInHierarchy);
            }
        }
#endregion

        public void EnableStartupLogo(bool enable)
        {
            _startupLogo.gameObject.SetActive(enable);
        }

        public void EnablePauseUI(bool enable)
        {
            _pauseMenu.gameObject.SetActive(enable);
        }

        private void EnableDebugUI(bool enable)
        {
            _debugUI.gameObject.SetActive(enable);
        }
    }
}
