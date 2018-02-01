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

        public override void GoalCollision(IPlayer player)
        {
            Debug.Log($"Player {player.State.PlayerNumber} has scored a goal!");

            player.State.Score++;

            if(player.State.Score >= GameTypeData.TargetGoalScore) {
                Debug.Log($"Player {player.State.PlayerNumber} has reached the score limit!"); 
                GameManager.Instance.State.Winner = player.State.PlayerNumber;
                GameManager.Instance.State.SetState(GameState.States.Victory);
            }
        }
    }
}
