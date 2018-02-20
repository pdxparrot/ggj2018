using ggj2018.Core.Util;

namespace ggj2018.Game.State
{
    public abstract class GameState : MonoBehavior
    {
// TODO: this is bunk, each state should handle start pressed on its own
// rather than looking for this in GameManager or whatever
        public abstract bool CanPause { get; }

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
