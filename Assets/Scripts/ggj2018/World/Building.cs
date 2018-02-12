using ggj2018.Core.Util;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    [RequireComponent(typeof(Collider))]
    public class Building : MonoBehavior
    {
        private Collider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            player?.State.EnvironmentStun(_collider);
        }
#endregion
    }
}
