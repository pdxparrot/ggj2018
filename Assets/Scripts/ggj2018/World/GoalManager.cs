using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.ggj2018.VFX;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.World
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

#region Goals by Distance
        public Goal GetNearestGoal(Player player, out float distance)
        {
            return GetNearestGoal(player, _goals, out distance);
        }

        private Goal GetNearestGoal(Player player, List<Goal> goals, out float distance)
        {
            return GetGoalByComparison(player, goals, (x, y) => {
                float xd = (x.transform.position - player.transform.position).sqrMagnitude;
                float yd = (y.transform.position - player.transform.position).sqrMagnitude;

                if(xd < yd) {
                    return -1;
                }

                if(xd > yd) {
                    return 1;
                }

                return 0;
            }, out distance);
        }
#endregion

        private Goal GetGoalByComparison(Player player, List<Goal> goals, Comparison<Goal> comparison, out float distance)
        {
            distance = float.MaxValue;

            if(goals.Count < 1) {
                return null;
            }

            goals.Sort(comparison);

            Goal nearest = goals[0];
            distance = (nearest.transform.position - player.transform.position).magnitude;

            return nearest;
        }
    }
}
