using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.World
{
    [RequireComponent(typeof(Collider))]
    public class Building : MonoBehavior
    {
        private Collider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            gameObject.layer = GameManager.Instance.ObstacleLayer;
// TODO: building prefabs don't need to be set to the obstacle layer by hand
        }

        private void OnCollisionEnter(Collision collision)
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            player?.State.EnvironmentStun(_collider);
        }
#endregion
    }
}
