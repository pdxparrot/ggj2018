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

        public Hunt(GameTypeData.GameTypeDataEntry gameTypeData)
            : base(gameTypeData)
        {
        }

        public override int ScoreLimit(BirdData.BirdDataEntry birdType)
        {
            return birdType.IsPredator ? PlayerManager.Instance.Prey.Count : 1;
        }

        public override bool ShowScore(BirdData.BirdDataEntry birdType)
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
            GameManager.Instance.State.SetState(GameState.States.GameOver);
        }

        public override void PlayerKill(Player killer)
        {
            if(!CanScore) {
                return;
            }

            Debug.Log($"Player {killer.Id} has scored a kill!");

            killer.State.Score++;

            if(killer.State.Score >= PlayerManager.Instance.Prey.Count) {
                Debug.Log($"Player {killer.Id} has killed all the messengers!");
                killer.State.GameOver(PlayerState.GameOverType.Win);
                GameManager.Instance.State.SetState(GameState.States.GameOver);
            }
        }
    }
}
