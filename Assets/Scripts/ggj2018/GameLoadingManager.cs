using ggj2018.Game.Loading;

namespace ggj2018.ggj2018
{
    public sealed class GameLoadingManager : LoadingManager<GameLoadingManager>
    {
        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.Create(ManagersContainer);
        }
    }
}
