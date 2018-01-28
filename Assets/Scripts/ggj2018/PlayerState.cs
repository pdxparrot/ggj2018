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
        private bool _isAlive = true;

        public bool IsAlive => _isAlive;

        public bool IsDead => !_isAlive;

        [SerializeField]
        [ReadOnly]
        private long _stunEndTimestamp;

        public bool IsStunned => TimeManager.Instance.CurrentUnixSeconds <= _stunEndTimestamp;

        [SerializeField]
        [ReadOnly]
        private Vector3 _stunBounceDirection;

        public Vector3 StunBounceDirection { get { return _stunBounceDirection; } set { _stunBounceDirection = value; } }

        public bool Incapacitated => IsStunned || IsDead;

        private readonly IPlayer _owner;

        public PlayerState(IPlayer owner)
        {
            _owner = owner;
        }

        public void Initialize(int playerNumber, BirdType birdType)
        {
            _playerNumber = playerNumber;
            _owner.GameObject.name = $"Player {PlayerNumber}";

            _birdType = birdType;
        }

        public void EnvironmentStun(Collider collider)
        {
            if(IsDead) {
                PlayerManager.Instance.DespawnLocalPlayer(PlayerNumber);
                return;
            } 

            if(IsStunned) {
                return;
            }

            Debug.Log($"Player {PlayerNumber} stunned by the environment!");

            Stun(collider);
        }

        public void PlayerStun(IPlayer stunner, Collider collider)
        {
            if(IsDead) {
                return;
            }

            Debug.Log($"Player {PlayerNumber} stunned by player {stunner.State.PlayerNumber}!");

            Stun(collider);
        }

        private void Stun(Collider collider)
        {
            _stunEndTimestamp = TimeManager.Instance.CurrentUnixSeconds + PlayerManager.Instance.PlayerData.StunTimeSeconds;

            Vector3 position = _owner.GameObject.transform.position;
            _stunBounceDirection = position - collider.ClosestPoint(position);
        }

        public void EnvironmentKill(Collider collider)
        {
            if(IsDead) {
                return;
            }

            Debug.Log($"Player {PlayerNumber} killed by environment!");

            _isAlive = false;
        }

        public void PlayerKill(IPlayer killer, Collider collider)
        {
            if(killer.State.IsDead) {
                PlayerStun(killer, collider);
                return;
            }

            Debug.Log($"Player {PlayerNumber} killed by player {killer.State.PlayerNumber}!");

            _isAlive = false;
        }

        public void DebugKill()
        {
            Debug.Log("You monster!");

            if(IsDead) {
                return;
            }

            _isAlive = false;
        }
    }
}
