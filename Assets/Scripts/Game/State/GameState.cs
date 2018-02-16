using ggj2018.Core.Util;

namespace ggj2018.Game.State
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
