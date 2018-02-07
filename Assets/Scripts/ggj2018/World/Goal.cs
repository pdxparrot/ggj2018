using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.VFX;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    public class Goal : MonoBehavior
    {
        [SerializeField]
        private Collider _collider;

        private GodRay _godRay;

#region Unity Lifecycle
        private void Awake()
        {
            GoalManager.Instance.RegisterGoal(this);

            _collider.gameObject.layer = GoalManager.Instance.GoalLayer;
            _collider.isTrigger = true;

            _godRay = Instantiate(GoalManager.Instance.GoalGodRayPrefab, transform);
            _godRay.SetupGoal();
        }

        private void OnDestroy()
        {
            Destroy(_godRay);
            _godRay = null;

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
