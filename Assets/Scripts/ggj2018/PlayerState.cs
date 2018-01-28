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
        private bool _alive = true;

        public bool Alive => _alive;

        public bool Dead => !_alive;

        [SerializeField]
        [ReadOnly]
        private long _stunEndTimestamp;

        public bool Stunned => TimeManager.Instance.CurrentUnixSeconds <= _stunEndTimestamp;

        [SerializeField]
        [ReadOnly]
        private Vector3 _stunBounceDirection;

        public Vector3 StunBounceDirection { get { return _stunBounceDirection; } set { _stunBounceDirection = value; } }

        public bool Incapacitated => Stunned || Dead;

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

        public void EnvironmentStun(Collider collider)
        {
            Debug.Log($"Player {PlayerNumber} stunned by the environment!");

            Stun(collider);
        }

        public void PlayerStun(IPlayer stunner, Collider collider)
        {
            Debug.Log($"Player {PlayerNumber} stunned by player {stunner.State.PlayerNumber}!");

            Stun(collider);
        }

        private void Stun(Collider collider)
        {
            _stunEndTimestamp = TimeManager.Instance.CurrentUnixSeconds + PlayerManager.Instance.PlayerData.StunTimeSeconds;

            Vector3 position = _owner.GameObject.transform.position;
            _stunBounceDirection = position - collider.ClosestPoint(position);
        }

        public void PlayerKill(IPlayer killer)
        {
            Debug.Log($"Player {PlayerNumber} killed by player {killer.State.PlayerNumber}!");

            _alive = false;
        }
    }
}
