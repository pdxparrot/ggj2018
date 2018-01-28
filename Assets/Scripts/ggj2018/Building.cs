using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Building : MonoBehavior
    {
        public bool Collision(IPlayer player, Collider thisCollider)
        {
            player.State.EnvironmentStun(thisCollider);

            return true;
        }
    }
}
