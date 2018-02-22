using pdxpartyparrot.ggj2018.Data;

namespace pdxpartyparrot.ggj2018.GameTypes
{
    public class Revenge : GameType
    {
        public override bool BirdTypesShareSpawnpoints => false;

        public override bool PredatorsKillPrey => false;

        public Revenge(GameTypeData gameTypeData)
            : base(gameTypeData)
        {
        }

        public override int ScoreLimit(BirdTypeData birdType)
        {
            return 0;
        }

        public override bool ShowScore(BirdTypeData birdType)
        {
            return false;
        }

// TODO
    }
}
