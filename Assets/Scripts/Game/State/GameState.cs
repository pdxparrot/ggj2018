using pdxpartyparrot.Core.Util;

namespace pdxpartyparrot.Game.State
{
    public abstract class GameState : MonoBehavior
    {
        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}
