using System;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [Serializable]
    public sealed class PlayerState
    {
        [SerializeField]
        [ReadOnly]
        private BirdType _birdType;

        public BirdType BirdType => _birdType;

        [SerializeField]
        [ReadOnly]
        private int _playerNumber;

        public int PlayerNumber => _playerNumber;

        [SerializeField]
        [ReadOnly]
        private long _stunEndTimestamp;

        public bool Stunned => TimeManager.Instance.CurrentUnixSeconds <= _stunEndTimestamp;

        [SerializeField]
        [ReadOnly]
        private bool _alive = true;

        public bool Alive => _alive;

        public bool Dead => !_alive;

        private readonly IPlayer _owner;

        public PlayerState(IPlayer owner)
        {
            _owner = owner;
        }

        public void SetPlayerNumber(int playerNumber)
        {
            _playerNumber = playerNumber;
            _owner.GameObject.name = $"Player {PlayerNumber}";
        }

        public void SetBirdType(string id)
        {
            _birdType = new BirdType(id);
        }

        public void EnvironmentStun()
        {
            Debug.Log($"Player {PlayerNumber} stunned by the environment!");

            _stunEndTimestamp = TimeManager.Instance.CurrentUnixSeconds + PlayerManager.Instance.PlayerData.StunTimeSeconds;
        }

        public void PlayerStun(IPlayer stunner)
        {
            Debug.Log($"Player {PlayerNumber} stunned by player {stunner.State.PlayerNumber}!");

            _stunEndTimestamp = TimeManager.Instance.CurrentUnixSeconds + PlayerManager.Instance.PlayerData.StunTimeSeconds;
        }

        public void PlayerKill(IPlayer killer)
        {
            Debug.Log($"Player {PlayerNumber} killed by player {killer.State.PlayerNumber}!");

            _alive = false;
        }
    }
}
