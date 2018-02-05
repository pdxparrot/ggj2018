using System;

using ggj2018.Core.Util;
using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Game;

using UnityEngine;

namespace ggj2018.ggj2018.Players
{
    [Serializable]
    public sealed class PlayerState
    {
        public enum GameOverType
        {
            Win,
            Loss,
            TimerUp
        }

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

        [SerializeField]
        [ReadOnly]
        private float _immuneTimer;

        public bool IsImmune => IsDead || _immuneTimer > 0.0f;

#region Boost
        [SerializeField]
        [ReadOnly]
        private bool _isBoosting;

        public bool IsBoosting => _isBoosting;

        [SerializeField]
        [ReadOnly]
        private float _boostRemainingSeconds;

        public float BoostRemainingSeconds => _boostRemainingSeconds;

        public bool CanBoost => _owner.Bird.Type.CanBoost && !IsIncapacitated && BoostRemainingSeconds > 0.0f;
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

        [SerializeField]
        [ReadOnly]
        private GameOverType _gameOverState = GameOverType.Loss;

        public GameOverType GameOverState { get { return _gameOverState; } set { _gameOverState = value; } }

        private readonly Player _owner;

        public PlayerState(Player owner)
        {
            _owner = owner;
        }

        public void Initialize()
        {
            _boostRemainingSeconds = PlayerManager.Instance.PlayerData.BoostSeconds;
        }

        public void Update(float dt)
        {
            if(GameManager.Instance.State.IsPaused) {
                return;
            }

            UpdateImmune(dt);
            UpdateBoost(dt);
            UpdateStun(dt);
        }

#region Immunity
        private void UpdateImmune(float dt)
        {
            if(_immuneTimer > 0.0f) {
                _immuneTimer -= dt;

                if(!IsImmune) {
                    _owner.Bird.ShowImmunity(false);
                }
            }
        }

        private void MakeImmune()
        {
            if(!GameManager.Instance.EnableImmunity) {
                return;
            }

            _immuneTimer = GameManager.Instance.GameTypeData.ImmuneTime;
            _owner.Bird.ShowImmunity(true);
        }
#endregion

#region Boost
        public void StartBoost()
        {
            if(!CanBoost) {
                Debug.Log($"TODO: Player a shitty sound because YOU CAN'T BOOST FOOL");
                return;
            }

            Debug.Log($"Player {_owner.Id} is boosting!");
            EnableBoost(true);
        }

        public void StopBoost()
        {
            Debug.Log($"Player {_owner.Id} slows down!");
            EnableBoost(false);
        }

        private void EnableBoost(bool enable)
        {
            _isBoosting = enable;
            _owner.Bird.ShowBoostTrail(enable);
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

            Debug.Log($"Player {_owner.Id} is braking!");
            _isBraking = true;
        }

        public void StopBrake()
        {
            Debug.Log($"Player {_owner.Id} is stops braking!");
            _isBraking = false;
        }
#endregion

#region Stun
        public void EnvironmentStun(Collision collision)
        {
            if(IsDead) {
                PlayerManager.Instance.DespawnPlayer(_owner);
                return;
            } 

            if(IsImmune || IsStunned) {
                return;
            }

            Debug.Log($"Player {_owner.Id} stunned by the environment!");

            Stun(collision.collider);
        }

        public void PlayerStun(Player stunner, Collider other)
        {
            if(IsImmune) {
                return;
            }

            // players can re-stun other players

            Debug.Log($"Player {_owner.Id} stunned by player {stunner.State._owner.Id}!");

            Stun(other);
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

        private void Stun(Collider other)
        {
            Stun();

            Vector3 playerPosition = _owner.transform.position;
            Vector3 collisionPosition = other.transform.position;
            collisionPosition.y = playerPosition.y;

            _stunBounceDirection = (playerPosition - collisionPosition).normalized;

            if(PlayerManager.Instance.PlayerData.StunBounceRotation) {
                _owner.transform.forward = _stunBounceDirection;
                _stunBounceDirection = _owner.transform.rotation * _stunBounceDirection;
            }
        }

        private void Stun()
        {
            _stunTimer = PlayerManager.Instance.PlayerData.StunTimeSeconds;

            _owner.Bird.ShowStun(true);
        }

        private void UpdateStun(float dt)
        {
            if(!IsStunned) {
                return;
            }

            _stunTimer -= dt;
            if(!IsStunned) {
                _owner.Bird.ShowStun(false);
                MakeImmune();
            }
        }
#endregion

#region Kill
        public void EnvironmentKill()
        {
            if(IsImmune) {
                return;
            }

            Debug.Log($"Player {_owner.Id} killed by environment!");

            Kill();
        }

        public void PlayerKill(Player killer, Collider other)
        {
            if(IsImmune) {
                return;
            }

            if(killer.State.IsDead) {
                PlayerStun(killer, other);
                return;
            }

            if(killer.State.IsStunned) {
                return;
            }

            Debug.Log($"Player {_owner.Id} killed by player {killer.Id}!");

            Kill();

            killer.State.MakeImmune();

            GameManager.Instance.State.GameType.PlayerKill(killer);
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

            Prey prey = _owner.Bird as Prey;
            if(null != prey) {
                prey.ShowBlood(true);
            }

            _owner.Viewer.PlayerUI.SwitchToDead();
        }
#endregion
    }
}
