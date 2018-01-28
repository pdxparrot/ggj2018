using ggj2018.Game.Loading;
using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GameLoadingManager : LoadingManager<GameLoadingManager>
    {
        [SerializeField]
        private PlayerManager _playerManagerPrefab;

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.Create(ManagersContainer);
            PlayerManager.CreateFromPrefab(_playerManagerPrefab, ManagersContainer);
        }
    }
}
