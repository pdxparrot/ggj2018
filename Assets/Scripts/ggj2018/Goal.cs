﻿using ggj2018.Core.Util;

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

        public bool Collision(Player player)
        {
            GameManager.Instance.State.GameType.GoalCollision(player);

            return true;
        }
    }
}
