using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Building : MonoBehavior
    {
        public bool Collision(Player player, Collision collision)
        {
            player.State.EnvironmentStun(collision);

            return true;
        }
    }
}
