using System.Collections.Generic;

using ggj2018.Core.Util;
using ggj2018.ggj2018.VFX;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    public sealed class GoalManager : SingletonBehavior<GoalManager>
    {
        [SerializeField]
        private string _goalLayerName;

        [SerializeField]
        private GodRay _goalGodRayPrefab;

        public GodRay GoalGodRayPrefab => _goalGodRayPrefab;

        public LayerMask GoalLayer => LayerMask.NameToLayer(_goalLayerName);

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
