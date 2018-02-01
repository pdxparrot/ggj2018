using ggj2018.ggj2018.Data;

namespace ggj2018.ggj2018.GameTypes
{
    public class Revenge : GameType
    {
        public override GameTypes Type => GameTypes.Revenge;

        public override bool BirdTypesShareSpawnpoints => false;

        public Revenge(GameTypeData.GameTypeDataEntry gameTypeData)
            : base(gameTypeData)
        {
        }

// TODO
    }
}
