namespace ggj2018.ggj2018
{
    public sealed class PlayerState
    {
        public int PlayerNumber { get; private set; }

        private readonly IPlayer _owner;

        public PlayerState(IPlayer owner)
        {
            _owner = owner;
        }

        public void SetPlayerNumber(int playerNumber)
        {
            PlayerNumber = playerNumber;
            _owner.GameObject.name = $"Player {PlayerNumber}";
        }
    }
}
