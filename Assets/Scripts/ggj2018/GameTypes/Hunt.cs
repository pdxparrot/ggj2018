using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018.GameTypes
{
    public class Hunt : GameType
    {
        public override GameTypes Type => GameTypes.Hunt;

        public override bool BirdTypesShareSpawnpoints => false;

        public Hunt(GameTypeData.GameTypeDataEntry gameTypeData)
            : base(gameTypeData)
        {
        }

        public override void GoalCollision(Player player)
        {
            if(player.State.BirdType.IsPredator) {
                return;
            }

            Debug.Log($"Player {player.Id} has reached the goal!"); 
            GameManager.Instance.State.Winner = player.Id;
            GameManager.Instance.State.SetState(GameState.States.Victory);
        }

        public override void PlayerKill(Player killer)
        {
            Debug.Log($"Player {killer.Id} has scored a kill!");

            killer.State.Kills++;

            if(killer.State.Kills == PlayerManager.Instance.PreyCount) {
                Debug.Log($"Player {killer.Id} has killed all the messengers!"); 
                GameManager.Instance.State.Winner = killer.Id;
                GameManager.Instance.State.SetState(GameState.States.Victory);
            }
        }
    }
}
