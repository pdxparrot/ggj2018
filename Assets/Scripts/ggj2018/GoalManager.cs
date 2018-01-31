using System.Collections.Generic;

using ggj2018.Core.Util;

namespace ggj2018.ggj2018
{
    public sealed class GoalManager : SingletonBehavior<GoalManager>
    {
        private readonly List<Goal> _goals = new List<Goal>();

        public void RegisterGoal(Goal goal)
        {
            _goals.Add(goal);
        }

        public void UnregisterGoal(Goal goal)
        {
            _goals.Remove(goal);
        }
    }
}
