using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.GameTypes
{
    public class Hunt : GameType
    {
        public override GameTypes Type => GameTypes.Hunt;

        public override bool BirdTypesShareSpawnpoints => false;

        public override bool PredatorsKillPrey => true;

        public Hunt(GameTypeData gameTypeData)
            : base(gameTypeData)
        {
        }

        public override int ScoreLimit(BirdTypeData birdType)
        {
            return birdType.IsPredator ? PlayerManager.Instance.Prey.Count : 1;
        }

        public override bool ShowScore(BirdTypeData birdType)
        {
            return birdType.IsPredator;
        }

        public override void GoalCollision(Player player)
        {
            if(!CanScore) {
                return;
            }

            if(player.Bird.Type.IsPredator) {
                return;
            }

            Debug.Log($"Player {player.Id} has reached the goal!");
            foreach(Player p in PlayerManager.Instance.Prey) {
                p.State.GameOver(PlayerState.GameOverType.Win);
            }

            IsGameOver = true;
        }

        public override void PlayerKill(Player killer)
        {
            if(!CanScore) {
                return;
            }

            killer.State.ScoreKill();

            Debug.Log($"Player {killer.Id} has scored a kill! {PlayerManager.Instance.Prey.Count} remain");

            if(PlayerManager.Instance.Prey.Count < 1) {
                Debug.Log($"Player {killer.Id} has killed all the messengers!");
                killer.State.GameOver(PlayerState.GameOverType.Win);
                IsGameOver = true;
            }
        }
    }
}
