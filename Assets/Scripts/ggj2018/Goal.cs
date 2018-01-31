using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Goal : MonoBehavior
    {
#region Unity Lifecycle
        private void Awake()
        {
            GoalManager.Instance.RegisterGoal(this);
        }

        private void OnDestroy()
        {
            if(GoalManager.HasInstance) {
                GoalManager.Instance.UnregisterGoal(this);
            }
        }
#endregion

        public bool Collision(IPlayer player, Collider thisCollider)
        {
            GameManager.Instance.GoalCollision(player);

            return true;
        }
    }
}
