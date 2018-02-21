using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.VFX;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    [RequireComponent(typeof(Collider))]
    public class Goal : MonoBehavior
    {
        private Collider _collider;

        private GodRay _godRay;

#region Unity Lifecycle
        private void Awake()
        {
            GoalManager.Instance.RegisterGoal(this);

            gameObject.layer = GoalManager.Instance.GoalLayer;

            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;

            _godRay = Instantiate(GoalManager.Instance.GoalGodRayPrefab, transform.parent);
            _godRay.SetupGoal();
        }

        private void OnDestroy()
        {
            Destroy(_godRay.gameObject);
            _godRay = null;

            if(GoalManager.HasInstance) {
                GoalManager.Instance.UnregisterGoal(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.GetComponentInParent<Player>();
            if(null != player) {
                GameManager.Instance.GameType.GoalCollision(player);
            }
        }
#endregion
    }
}
