using ggj2018.Core.Util;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.World
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
