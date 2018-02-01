using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

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

        public static GameType GetGameType(GameState gameState)
        {
// TODO: this is an awful hack :\
            if(1 == gameState.PlayerCount || 0 == gameState.PredatorCount || 0 == gameState.PreyCount) {
                return new CrazyTaxi(GameManager.Instance.GameTypeData.Entries.GetOrDefault(GameTypes.CrazyTaxi));
            }
            if(gameState.PlayerCount > 1 && gameState.PredatorCount > 0 && gameState.PreyCount > 0) {
                return new Hunt(GameManager.Instance.GameTypeData.Entries.GetOrDefault(GameTypes.Hunt));
            }
            Debug.LogError($"No suitable gametype found! playerCount: {gameState.PlayerCount}, predatorCount: {gameState.PredatorCount}, preyCount: {gameState.PreyCount}");
            return null;
        }

        public abstract GameTypes Type { get; }

        public GameTypeData.GameTypeDataEntry GameTypeData { get; }

        public abstract bool BirdTypesShareSpawnpoints { get; }

        public virtual void GoalCollision(IPlayer player)
        {
        }

        public virtual void PlayerKill(IPlayer killer)
        {
        }

        protected GameType(GameTypeData.GameTypeDataEntry gameTypeData)
        {
            GameTypeData = gameTypeData;
        }
    }
}
