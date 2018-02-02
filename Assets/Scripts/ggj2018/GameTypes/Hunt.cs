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

        public override int ScoreLimit(BirdData.BirdDataEntry birdType)
        {
            return birdType.IsPredator ? PlayerManager.Instance.PreyCount : 1;
        }

        public override bool ShowScore(BirdData.BirdDataEntry birdType)
        {
            return birdType.IsPredator;
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

            killer.State.Score++;

            if(killer.State.Score == PlayerManager.Instance.PreyCount) {
                Debug.Log($"Player {killer.Id} has killed all the messengers!"); 
                GameManager.Instance.State.Winner = killer.Id;
                GameManager.Instance.State.SetState(GameState.States.Victory);
            }
        }
    }
}
