using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.GameState;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.Game.State;

namespace pdxpartyparrot.ggj2018.GameTypes
{
    public abstract class GameType
    {
// TODO: is it possible to kill this enum?
        public enum GameTypes
        {
            CrazyTaxi,
            Hunt,
            Revenge
        }

        public abstract GameTypes Type { get; }

        public GameTypeData GameTypeData { get; }

        public abstract bool BirdTypesShareSpawnpoints { get; }

        public bool ShowTimer => GameTypeData.TimeLimit > 0;

        public bool CanScore => GameStateManager.Instance.CurrentState is GameStateGame;

        public bool IsGameOver { get; protected set; }

        public abstract bool PredatorsKillPrey { get; }

        public virtual void Initialize()
        {
        }

        public void Update()
        {
            if(!PlayerManager.Instance.HasAlivePlayer) {
                IsGameOver = true;
            }
        }

        public abstract int ScoreLimit(BirdTypeData birdType);

        public abstract bool ShowScore(BirdTypeData birdType);

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

        protected GameType(GameTypeData gameTypeData)
        {
            GameTypeData = gameTypeData;
        }
    }
}
