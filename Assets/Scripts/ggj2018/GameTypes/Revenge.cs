using ggj2018.ggj2018.Data;

namespace ggj2018.ggj2018.GameTypes
{
    public class Revenge : GameType
    {
        public override GameTypes Type => GameTypes.Revenge;

        public override bool BirdTypesShareSpawnpoints => false;

        public override bool PredatorsKillPrey => false;

        public Revenge(GameTypeData.GameTypeDataEntry gameTypeData)
            : base(gameTypeData)
        {
        }

        public override int ScoreLimit(BirdData.BirdDataEntry birdType)
        {
            return 0;
        }

        public override bool ShowScore(BirdData.BirdDataEntry birdType)
        {
            return false;
        }

// TODO
    }
}
