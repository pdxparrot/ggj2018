﻿using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.GameTypes
{
    public class CrazyTaxi : GameType
    {
        public override GameTypes Type => GameTypes.CrazyTaxi;

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
                GameManager.Instance.State.SetState(Game.GameState.States.GameOver);
            }

            // TODO: spawn the next appropriate goal type
        }
    }
}
