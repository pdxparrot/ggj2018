using System;

using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [Serializable]
    public sealed class PlayerState
    {
        [SerializeField]
        [ReadOnly]
        private BirdData.BirdDataEntry _birdType;

        public BirdData.BirdDataEntry BirdType => _birdType;

        [SerializeField]
        [ReadOnly]
        private int _playerNumber;

        public int PlayerNumber => _playerNumber;

#region Alive
        [SerializeField]
        [ReadOnly]
        private bool _isAlive = true;

        public bool IsAlive => _isAlive;

        public bool IsDead => !_isAlive;
#endregion

#region Stun
        [SerializeField]
        [ReadOnly]
        private float _stunTimer;

        public bool IsStunned => _stunTimer > 0.0f;

        [SerializeField]
        [ReadOnly]
        private Vector3 _stunBounceDirection;

        public Vector3 StunBounceDirection { get { return _stunBounceDirection; } set { _stunBounceDirection = value; } }
#endregion

        public bool IsIncapacitated => IsStunned || IsDead;

#region Boost
        [SerializeField]
        [ReadOnly]
        private bool _isBoosting;

        public bool IsBoosting => _isBoosting;

        [SerializeField]
        [ReadOnly]
        private float _boostRemainingSeconds;

        public float BoostRemainingSeconds => _boostRemainingSeconds;

        public bool CanBoost => !IsIncapacitated && BoostRemainingSeconds > 0.0f;
#endregion

#region Brake
        [SerializeField]
        [ReadOnly]
        private bool _isBraking;

        public bool IsBraking => _isBraking;

        public bool CanBrake => !IsIncapacitated;
#endregion

        [SerializeField]
        [ReadOnly]
        private int _score;

        public int Score { get { return _score; } set { _score = value; } }

        private readonly IPlayer _owner;

        public PlayerState(IPlayer owner)
        {
            _owner = owner;
        }

        public void Initialize(int playerNumber, BirdData.BirdDataEntry birdType)
        {
            _playerNumber = playerNumber;
            _owner.GameObject.name = $"Player {PlayerNumber}";
            _birdType = birdType;

            _boostRemainingSeconds = PlayerManager.Instance.PlayerData.BoostSeconds;
        }

        public void Update(float dt)
        {
            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            UpdateBoost(dt);
            UpdateStun(dt);
        }

#region Boost
        public void StartBoost()
        {
            if(!CanBoost) {
                Debug.Log($"TODO: Player a shitty sound because YOU CAN'T BOOST FOOL");
                return;
            }

            Debug.Log($"Player {PlayerNumber} is boosting!");
            EnableBoost(true);
        }

        public void StopBoost()
        {
            Debug.Log($"Player {PlayerNumber} slows down!");
            EnableBoost(false);
        }

        private void EnableBoost(bool enable)
        {
            _isBoosting = enable;
            _owner.Controller.Bird.ShowBoostTrail(enable);
        }

        private void UpdateBoost(float dt)
        {
            if(!IsBoosting) {
                return;
            }

            _boostRemainingSeconds -= dt;
            if(_boostRemainingSeconds < 0.0f) {
                _boostRemainingSeconds = 0.0f;
            }

            if(!CanBoost) {
                StopBoost();
                // should we maybe exhaust here?
            }
        }
#endregion

#region Brake
        public void StartBrake()
        {
            if(!CanBrake) {
                return;
            }

            Debug.Log($"Player {PlayerNumber} is braking!");
            _isBraking = true;
        }

        public void StopBrake()
        {
            Debug.Log($"Player {PlayerNumber} is stops braking!");
            _isBraking = false;
        }
#endregion

#region Stun
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

            // players can re-stun other players

            Debug.Log($"Player {PlayerNumber} stunned by player {stunner.State.PlayerNumber}!");

            Stun(collider);
        }

#if UNITY_EDITOR
        public void DebugStun()
        {
            if(IsStunned) {
                return;
            }

            Debug.Log("Ouch!");

            Stun();
        }
#endif

        private void Stun(Collider collider)
        {
            Stun();

            Vector3 playerPosition = _owner.GameObject.transform.position;
            Vector3 collisionPosition = collider.transform.position;
            collisionPosition.y = playerPosition.y;

            _stunBounceDirection = (playerPosition - collisionPosition).normalized;

            if(PlayerManager.Instance.PlayerData.StunBounceRotation) {
                _owner.GameObject.transform.forward = _stunBounceDirection;
                _stunBounceDirection = _owner.GameObject.transform.rotation * _stunBounceDirection;
            }
        }

        private void Stun()
        {
            _stunTimer = PlayerManager.Instance.PlayerData.StunTimeSeconds;

            _owner.Controller.Bird.ShowStun(true);
        }

        private void UpdateStun(float dt)
        {
            if(!IsStunned) {
                return;
            }

            _stunTimer -= dt;
            if(!IsStunned) {
                _owner.Controller.Bird.ShowStun(false);
            }
        }
#endregion

#region Kill
        public void EnvironmentKill(Collider collider)
        {
            if(IsDead) {
                return;
            }

            Debug.Log($"Player {PlayerNumber} killed by environment!");

            Kill();
        }

        public void PlayerKill(IPlayer killer, Collider collider)
        {
            if(IsDead) {
                return;
            }

            if(killer.State.IsDead) {
                PlayerStun(killer, collider);
                return;
            }

            Debug.Log($"Player {PlayerNumber} killed by player {killer.State.PlayerNumber}!");

            Kill();

            GameManager.Instance.PlayerKilled(killer);
        }

#if UNITY_EDITOR
        public void DebugKill()
        {
            if(IsDead) {
                return;
            }

            Debug.Log("You monster!");

            Kill();
        }
#endif

        private void Kill()
        {
            _isAlive = false;

            Prey prey = _owner.Controller.Bird as Prey;
            if(null != prey) {
                prey.ShowBlood(true);
            }
        }
#endregion
    }
}
