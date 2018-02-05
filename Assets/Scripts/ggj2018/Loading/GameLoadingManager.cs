using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;
using ggj2018.ggj2018.World;
using ggj2018.Game.Loading;

using UnityEngine;

namespace ggj2018.ggj2018.Loading
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
            PlayerManager.Instance.Initialize();
        }
    }
}
