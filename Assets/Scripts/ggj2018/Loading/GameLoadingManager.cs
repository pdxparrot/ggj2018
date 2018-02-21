using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.ggj2018.UI;
using pdxpartyparrot.ggj2018.World;
using pdxpartyparrot.Game.Loading;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Loading
{
    public sealed class GameLoadingManager : LoadingManager<GameLoadingManager>
    {
        [SerializeField]
        private DebugManager _debugManagerPrefab;

        [SerializeField]
        private GameManager _gameManagerPrefab;

        [SerializeField]
        private PlayerManager _playerManagerPrefab;

        [SerializeField]
        private GoalManager _goalManagerPrefab;

        [SerializeField]
        private UIManager _uiManagerPrefab;

        protected override void CreateManagers()
        {
            base.CreateManagers();

            DebugManager.CreateFromPrefab(_debugManagerPrefab, ManagersContainer);
            GameManager.CreateFromPrefab(_gameManagerPrefab, ManagersContainer);
            PlayerManager.CreateFromPrefab(_playerManagerPrefab, ManagersContainer);
            SpawnManager.Create(ManagersContainer);
            GoalManager.CreateFromPrefab(_goalManagerPrefab, ManagersContainer);
            UIManager.CreateFromPrefab(_uiManagerPrefab, ManagersContainer);
        }

        protected override void InitializeManagers()
        {
            base.InitializeManagers();

            GameManager.Instance.Initialize();
        }
    }
}
