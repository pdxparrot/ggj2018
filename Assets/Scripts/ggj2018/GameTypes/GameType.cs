using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.GameTypes
{
    public abstract class GameType
    {
        public enum GameTypes
        {
            CrazyTaxi,
            Hunt,
            Revenge
        }

        public static GameType GetGameType(int playerCount, int predatorCount, int preyCount)
        {
// TODO: this is an awful hack :\
            if(DebugManager.Instance.SpawnMaxLocalPlayers) {
                return new Hunt(GameManager.Instance.GameTypeData.Entries.GetOrDefault(GameTypes.Hunt));
            }
            if(1 == playerCount || 0 == predatorCount || 0 == preyCount) {
                return new CrazyTaxi(GameManager.Instance.GameTypeData.Entries.GetOrDefault(GameTypes.CrazyTaxi));
            }
            if(playerCount > 1 && predatorCount > 0 && preyCount > 0) {
                return new Hunt(GameManager.Instance.GameTypeData.Entries.GetOrDefault(GameTypes.Hunt));
            }
            Debug.LogError($"No suitable gametype found! playerCount: {playerCount}, predatorCount: {predatorCount}, preyCount: {preyCount}");
            return null;
        }

        public abstract GameTypes Type { get; }

        public GameTypeData.GameTypeDataEntry GameTypeData { get; }

        public abstract bool BirdTypesShareSpawnpoints { get; }

        public bool ShowTimer => GameTypeData.TimeLimit > 0;

        public bool CanScore => GameManager.Instance.State.CanScore;

        public abstract bool PredatorsKillPrey { get; }

        public virtual void Initialize()
        {
        }

        public void Update()
        {
            if(!PlayerManager.Instance.HasAlivePlayer) {
                GameManager.Instance.State.SetState(GameState.States.GameOver);
            }
        }

        public abstract int ScoreLimit(BirdData.BirdDataEntry birdType);

        public abstract bool ShowScore(BirdData.BirdDataEntry birdType);

        public virtual void GoalCollision(Player player)
        {
        }

        public virtual void PlayerKill(Player killer)
        {
        }

        public virtual void TimerFinish()
        {
            // TODO: move this into the PlayerManager
            foreach(Player player in PlayerManager.Instance.Players) {
                player.State.GameOver(PlayerState.GameOverType.TimerUp);
            }
        }

        protected GameType(GameTypeData.GameTypeDataEntry gameTypeData)
        {
            GameTypeData = gameTypeData;
        }
    }
}
