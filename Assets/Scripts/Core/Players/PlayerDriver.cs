using pdxpartyparrot.Core.Util;

namespace pdxpartyparrot.Core.Players
{
    public abstract class PlayerDriver : MonoBehavior
    {
        protected PlayerController PlayerController { get; private set; }

        protected Player Owner { get; private set; }

        public void Initialize(Player owner, PlayerController playerController)
        {
            Owner = owner;
            PlayerController = playerController;
        }
    }
}
