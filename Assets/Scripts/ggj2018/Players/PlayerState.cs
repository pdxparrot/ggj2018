using System;

using DG.Tweening;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Birds;
using pdxpartyparrot.ggj2018.Game;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Players
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
        private float _boostAmount;

        public float BoostAmount => _boostAmount;

        [SerializeField]
        [ReadOnly]
        private float _boostRemainingSeconds;

        public float BoostRemainingPercent => _boostRemainingSeconds / _owner.Bird.Type.BoostSeconds;

        [SerializeField]
        [ReadOnly]
        private float _boostRechargeCooldown;

        public bool IsBoostRechargeCooldown => _boostRechargeCooldown > 0.0f;

        public bool CanBoost => !GameManager.Instance.IsPaused && _owner.Bird.Type.CanBoost && !IsIncapacitated && (DebugManager.Instance.UseInfiniteBoost || _boostRemainingSeconds > 0.0f);

        private bool CanBoostRecharge => !GameManager.Instance.IsPaused && _owner.Bird.Type.CanBoost && !IsIncapacitated;

        private Tweener _boostTween;
#endregion

#region Brake
        [SerializeField]
        [ReadOnly]
        private bool _isBraking;

        public bool IsBraking => _isBraking;

        [SerializeField]
        [ReadOnly]
        private float _brakeAmount;

        public float BrakeAmount => _brakeAmount;

        public bool CanBrake => !GameManager.Instance.IsPaused && !IsIncapacitated;
#endregion

#region Score
        [SerializeField]
        [ReadOnly]
        private int _score;

        public int Score => _score;
#endregion

        [SerializeField]
        [ReadOnly]
        private GameOverType _gameOverState = GameOverType.Loss;

        public GameOverType GameOverState => _gameOverState;

        private readonly Player _owner;

        public PlayerState(Player owner)
        {
            _owner = owner;
        }

        public void Initialize()
        {
            _boostRemainingSeconds = _owner.Bird.Type.BoostSeconds;

            _boostTween = _owner.Viewer.Camera.DOShakePosition(
                    _owner.Bird.Type.BoostSeconds,
                    PlayerManager.Instance.PlayerData.BoostCameraShakeStrength,
                    PlayerManager.Instance.PlayerData.BoostCameraShakeVibrato,
                    PlayerManager.Instance.PlayerData.BoostCameraShakeRandomness,
                    false
                )
                .SetLoops(-1)
                .Pause();

            // make it easier to start the game
            _immuneTimer = GameManager.Instance.GameTypeData.GameStartImmuneTime;
        }

        public void Destroy()
        {
            _boostTween.Kill();
            _boostTween = null;
        }

        public void Update(float dt)
        {
            UpdateImmune(dt);
            UpdateBoost(dt);
            UpdateStun(dt);
        }

#region Immunity
        private void UpdateImmune(float dt)
        {
            if(GameManager.Instance.IsPaused) {
                return;
            }

            if(_immuneTimer > 0.0f) {
                _immuneTimer -= dt;

                if(IsDead || !IsImmune) {
                    _immuneTimer = 0.0f;
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
        public void StartBoost(float amount)
        {
            if(!CanBoost) {
                _owner.Bird.PlayBoostFailAudio();
                return;
            }

            _boostAmount = amount;

            Debug.Log($"Player {_owner.Id} is boosting!");
            EnableBoost(true);

            if(PlayerManager.Instance.PlayerData.EnableBoostCameraShake) {
                _boostTween.Play();
            }

            _owner.Viewer.Camera.DOFieldOfView(_owner.Bird.Type.ViewFOV + (_owner.Bird.Type.BoostFOVChange * _boostAmount), 1.0f)
                .SetRecyclable(true);

            _owner.Bird.StartBoostAudio();
        }

        public void UpdateBoostAmount(float amount)
        {
            if(!CanBoost) {
                return;
            }

            _boostAmount = amount;

            _owner.Viewer.Camera.DOFieldOfView(_owner.Bird.Type.ViewFOV + (_owner.Bird.Type.BoostFOVChange * _boostAmount), 1.0f)
                .SetRecyclable(true);
        }

        public void StopBoost()
        {
            _boostAmount = 0.0f;

            Debug.Log($"Player {_owner.Id} slows down!");
            EnableBoost(false);

            _owner.Bird.StopBoostAudio();

            _owner.Viewer.Camera.DOFieldOfView(_owner.Bird.Type.ViewFOV, 1.0f)
                .SetRecyclable(true);

            if(PlayerManager.Instance.PlayerData.EnableBoostCameraShake) {
                _boostTween.Pause();
                _owner.Viewer.ResetCameraPosition();
            }

            _boostRechargeCooldown = _owner.Bird.Type.BoostRechargeCooldown;
        }

        private void EnableBoost(bool enable)
        {
            _isBoosting = enable;
            _owner.Bird.ShowBoostTrail(enable);
        }

        private void UpdateBoost(float dt)
        {
            if(!CanBoostRecharge) {
                return;
            }

            if(IsBoosting) {
                if(!DebugManager.Instance.UseInfiniteBoost) {
                    _boostRemainingSeconds -= dt;
                    if(_boostRemainingSeconds < 0.0f) {
                        _boostRemainingSeconds = 0.0f;
                    }
                }

                if(!CanBoost) {
                    StopBoost();
                    // should we maybe exhaust here?
                }
            } else if(IsBoostRechargeCooldown) {
                _boostRechargeCooldown -= dt;
                if(_boostRechargeCooldown < 0.0f) {
                    _boostRechargeCooldown = 0.0f;
                }
            } else {
                if(_boostRemainingSeconds < _owner.Bird.Type.BoostSeconds) {
                    _boostRemainingSeconds += _owner.Bird.Type.BoostRechargeRate * dt;
                } else {
                    _boostRemainingSeconds = _owner.Bird.Type.BoostSeconds;
                }
            }
        }
#endregion

#region Brake
        public void StartBrake(float amount)
        {
            if(!CanBrake) {
                return;
            }

            _brakeAmount = amount;

            Debug.Log($"Player {_owner.Id} is braking!");
            EnableBrake(true);

            _owner.Viewer.Camera.DOFieldOfView(_owner.Bird.Type.ViewFOV + (_owner.Bird.Type.BrakeFOVChange * _brakeAmount), 1.0f)
                .SetRecyclable(true);
        }

        public void UpdateBrakeAmount(float amount)
        {
            if(!CanBrake) {
                return;
            }

            _brakeAmount = amount;

            _owner.Viewer.Camera.DOFieldOfView(_owner.Bird.Type.ViewFOV + (_owner.Bird.Type.BrakeFOVChange * _brakeAmount), 1.0f)
                .SetRecyclable(true);
        }

        public void StopBrake()
        {
            _brakeAmount = 0.0f;

            Debug.Log($"Player {_owner.Id} stops braking!");
            EnableBrake(false);

            _owner.Viewer.Camera.DOFieldOfView(_owner.Bird.Type.ViewFOV, 1.0f)
                .SetRecyclable(true);
        }

        private void EnableBrake(bool enable)
        {
            _isBraking = enable;
        }
#endregion

#region Stun
        public void EnvironmentStun(Collider other)
        {
            if(IsDead) {
                PlayerManager.Instance.DespawnPlayer(_owner);
                return;
            } 

            if(IsImmune || IsStunned) {
                StunPush(other);
                return;
            }

            Debug.Log($"Player {_owner.Id} stunned by the environment!");

            Stun(other, true);
        }

        public void PlayerStun(Player stunner, Collider other)
        {
            if(IsImmune) {
                return;
            }

            // players can re-stun other players

            Debug.Log($"Player {_owner.Id} stunned by player {stunner.State._owner.Id}!");

            Stun(other, false);
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

        private void Stun(Collider other, bool push)
        {
            Stun();

            if(push) {
                StunPush(other);
            }
        }

        private void StunPush(Collider other)
        {
            Vector3 closestPoint = other.ClosestPoint(_owner.transform.position);

            Vector3 direction = (_owner.transform.position - closestPoint).normalized;
            _owner.Redirect(direction * PlayerManager.Instance.PlayerData.StunBounceSpeed);
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

            GameManager.Instance.GameType.PlayerKill(killer);
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

            PlayerManager.Instance.KillPlayer(_owner);
        }
#endregion

#region Score
        public void ScoreGoal()
        {
            _score++;
        }

        public void ScoreKill()
        {
            _owner.Viewer.PlayerUI.PlayerUIPage.PlayerHUD.AddKill();
        }
#endregion

#region Game Over
        public void GameOver(GameOverType state)
        {
            Debug.Log($"Player {_owner.Id} game over state: {state}");
            switch(state)
            {
            case GameOverType.Win:
                _owner.Bird.PlayWinAudio();
                break;
            case GameOverType.Loss:
                _owner.Bird.PlayLossAudio();
                break;
            case GameOverType.TimerUp:
                _owner.Bird.PlayLossAudio();
                break;
            }
            _gameOverState = state;
        }
#endregion
    }
}
