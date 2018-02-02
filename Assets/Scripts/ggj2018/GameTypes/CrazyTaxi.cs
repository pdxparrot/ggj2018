using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018.GameTypes
{
    public class CrazyTaxi : GameType
    {
        public override GameTypes Type => GameTypes.CrazyTaxi;

        public override bool BirdTypesShareSpawnpoints => true;

        public CrazyTaxi(GameTypeData.GameTypeDataEntry gameTypeData)
            : base(gameTypeData)
        {
        }

        public override void Initialize()
        {
            // TODO: spawn a random message goal
        }

        public override void GoalCollision(Player player)
        {
            // TODO: handle message goals as well

            Debug.Log($"Player {player.Id} has scored a goal!");

            player.State.Score++;

            if(player.State.Score >= GameTypeData.TargetGoalScore) {
                Debug.Log($"Player {player.Id} has reached the score limit!"); 
                GameManager.Instance.State.Winner = player.Id;
                GameManager.Instance.State.SetState(GameState.States.Victory);
            }

            // TODO: spawn the next appropriate goal type
        }
    }
}
