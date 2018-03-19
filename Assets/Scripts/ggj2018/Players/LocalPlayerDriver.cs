using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Players
{
    public class LocalPlayerDriver : Core.Players.PlayerDriver
    {
        [SerializeField]
        [ReadOnly]
        private int _controllerIndex;

        public int ControllerIndex { get { return _controllerIndex; } set { _controllerIndex = value; } }

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastMoveAxes;

        public Vector3 LookAxis => InputManager.Instance.GetLookAxes(ControllerIndex);

        private new PlayerController PlayerController => (PlayerController)base.PlayerController;

        private new Player Owner => (Player)base.Owner;

#region Unity Lifecycle
        private void Update()
        {
            if(!Owner.IsLocalPlayer) {
                return;
            }

            if(InputManager.Instance.Pressed(ControllerIndex, PlayerManager.Instance.PlayerData.InvertLookButton)) {
                InputManager.Instance.InvertLookAxis(ControllerIndex);
            } else if(InputManager.Instance.Pressed(ControllerIndex, PlayerManager.Instance.PlayerData.InvertMoveButton)) {
                InputManager.Instance.InvertMoveAxis(ControllerIndex);
            }

#if UNITY_EDITOR
            CheckForDebug();
#endif

            CheckForBrake();
            CheckForBoost();

            if(GameManager.Instance.IsPaused) {
                return;
            }

// TODO: driver should do this
            _lastMoveAxes = InputManager.Instance.GetMoveAxes(ControllerIndex);

            float dt = Time.deltaTime;

            PlayerController.RotateModel(_lastMoveAxes, dt);
        }

        private void FixedUpdate()
        {
            if(!Owner.IsLocalPlayer) {
                return;
            }

            if(GameManager.Instance.IsPaused) {
                return;
            }

            float dt = Time.fixedDeltaTime;

            PlayerController.Turn(_lastMoveAxes, dt);
            PlayerController.Move(_lastMoveAxes, dt);
        }
#endregion

#region Input Handling
#if UNITY_EDITOR
        private void CheckForDebug()
        {
            if(InputManager.Instance.Pressed(ControllerIndex, PlayerManager.Instance.PlayerData.DebugStunButton)) {
                Owner.State.DebugStun();
            }

            if(InputManager.Instance.Pressed(ControllerIndex, PlayerManager.Instance.PlayerData.DebugKillButton)) {
                Owner.State.DebugKill();
            }
        }
#endif

        private void CheckForBrake()
        {
            float brakeAmount = 0.0f;

            if(PlayerManager.Instance.PlayerData.UseBoostBrakeAxes) {
                brakeAmount = InputManager.Instance.GetTriggerAxis(ControllerIndex, InputManager.TriggerAxis.Trigger);
                if(brakeAmount <= 0.0f) {
                    brakeAmount = 0.0f;
                }
                brakeAmount = Mathf.Abs(brakeAmount);
            } else if(InputManager.Instance.Held(ControllerIndex, PlayerManager.Instance.PlayerData.BrakeButton)) {
                brakeAmount = 1.0f;
            }

            if(Owner.State.IsBraking) {
                if(brakeAmount > 0.0f) {
                    Owner.State.UpdateBrakeAmount(brakeAmount);
                } else {
                    Owner.State.StopBrake();
                }
            } else if(brakeAmount > 0.0f) {
                Owner.State.StartBrake(brakeAmount);
            }
        }

        private void CheckForBoost()
        {
            float boostAmount = 0.0f;

            if(PlayerManager.Instance.PlayerData.UseBoostBrakeAxes) {
                boostAmount = InputManager.Instance.GetTriggerAxis(ControllerIndex, InputManager.TriggerAxis.Trigger);
                if(boostAmount >= 0.0f) {
                    boostAmount = 0.0f;
                }
                boostAmount = Mathf.Abs(boostAmount);
            } else if(InputManager.Instance.Held(ControllerIndex, PlayerManager.Instance.PlayerData.BoostButton)) {
                boostAmount = 1.0f;
            }

            if(Owner.State.IsBoosting) {
                if(boostAmount > 0.0f) {
                    Owner.State.UpdateBoostAmount(boostAmount);
                } else {
                    Owner.State.StopBoost();
                }
            } else if(boostAmount > 0.0f) {
                Owner.State.StartBoost(boostAmount);
            }
        }
#endregion
    }
}
