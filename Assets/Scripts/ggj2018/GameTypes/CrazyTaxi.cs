using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.GameTypes
{
    public sealed class CrazyTaxi : GameType
    {
        public override bool BirdTypesShareSpawnpoints => true;

        public override bool PredatorsKillPrey => false;

        public CrazyTaxi(GameTypeData gameTypeData)
            : base(gameTypeData)
        {
        }

        public override void Initialize()
        {
            // TODO: spawn a random message goal
        }

        public override int ScoreLimit(BirdTypeData birdType)
        {
            return GameTypeData.ScoreLimit;
        }

        public override bool ShowScore(BirdTypeData birdType)
        {
            return true;
        }

        public override void GoalCollision(Player player)
        {
            if(!CanScore) {
                return;
            }

            // TODO: handle message goals as well

            Debug.Log($"Player {player.Id} has scored a goal!");

            player.State.ScoreGoal();

            if(player.State.Score >= GameTypeData.ScoreLimit) {
                Debug.Log($"Player {player.Id} has reached the score limit!"); 
                player.State.GameOver(PlayerState.GameOverType.Win);
                IsGameOver = true;
            }

            // TODO: spawn the next appropriate goal type
        }
    }
}
