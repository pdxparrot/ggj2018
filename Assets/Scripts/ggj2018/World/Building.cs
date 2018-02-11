using ggj2018.Core.Util;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    public class Building : MonoBehavior
    {
#region Unity Lifecycle
        private void OnCollisionEnter(Collision collision)
        {
            Player player = collision.collider.GetComponentInParent<Player>();
            player?.State.EnvironmentStun(collision);
        }
#endregion
    }
}
