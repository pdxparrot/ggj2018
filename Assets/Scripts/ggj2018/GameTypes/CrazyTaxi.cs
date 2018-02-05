using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

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

        public override int ScoreLimit(BirdData.BirdDataEntry birdType)
        {
            return GameTypeData.ScoreLimit;
        }

        public override bool ShowScore(BirdData.BirdDataEntry birdType)
        {
            return true;
        }

        public override void GoalCollision(Player player)
        {
            // TODO: handle message goals as well

            Debug.Log($"Player {player.Id} has scored a goal!");

            player.State.Score++;

            if(player.State.Score >= GameTypeData.ScoreLimit) {
                Debug.Log($"Player {player.Id} has reached the score limit!"); 
                player.State.GameOverState = PlayerState.GameOverType.Win;
                GameManager.Instance.State.SetState(GameState.States.GameOver);
            }

            // TODO: spawn the next appropriate goal type
        }
    }
}
