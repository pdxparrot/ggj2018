using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    [RequireComponent(typeof(Collider))]
    public class Goal : MonoBehavior
    {
        private Collider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            GoalManager.Instance.RegisterGoal(this);

            _collider = GetComponent<Collider>();
            _collider.gameObject.layer = GoalManager.Instance.GoalLayer;
            _collider.isTrigger = true;
        }

        private void OnDestroy()
        {
            if(GoalManager.HasInstance) {
                GoalManager.Instance.UnregisterGoal(this);
            }
        }
#endregion

        public bool Collision(Player player)
        {
            GameManager.Instance.State.GameType.GoalCollision(player);

            return true;
        }
    }
}
