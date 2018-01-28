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
        private BaseAttributes _attributes = new BaseAttributes();

        public BaseAttributes Attributes { get { return _attributes; } set { _attributes = value ?? new BaseAttributes(); } }

        [SerializeField]
        [ReadOnly]
        private int _playerNumber;

        public int PlayerNumber => _playerNumber;

        [SerializeField]
        [ReadOnly]
        private long _stunEndTimestamp;

        public bool Stunned => TimeManager.Instance.CurrentUnixSeconds <= _stunEndTimestamp;

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

        public void Stun()
        {
            Debug.Log($"Player {PlayerNumber} stunned!");

            _stunEndTimestamp = TimeManager.Instance.CurrentUnixSeconds + PlayerManager.Instance.PlayerData.StunTimeSeconds;
        }
    }
}
