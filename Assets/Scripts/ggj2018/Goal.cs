using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Goal : MonoBehavior
    {
        public bool Collision(IPlayer player, Collider thisCollider)
        {
            if(player.State.BirdType.BirdDataEntry.IsPredator) {
                return false;
            }

            Debug.Log($"Player {player.State.PlayerNumber} has reached the goal!");
            GameManager.Instance.winner = player.ControllerNumber;
            GameManager.Instance.SetState(GameManager.EState.eVictory);
            return true;
        }
    }
}
