using System;
using System.Collections.Generic;
using System.Linq;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        public enum DPadDir
        {
            Up, Down, Left, Right,
            NumAxes
        }

        public enum DPadState
        {
            None,
            Down,
            Held,
            Up
        }

        public enum Button
        {
            A, B, X, Y,
            LeftBumper, RightBumper,
            LeftStick = 8, RightStick = 9
        }

        [Serializable]
        public sealed class ControllerState
        {
            [SerializeField]
            private bool _invertMoveX;

            public bool InvertMoveX { get { return _invertMoveX; } set { _invertMoveX = value; } }

            [SerializeField]
            private bool _invertMoveY;

            public bool InvertMoveY { get { return _invertMoveY; } set { _invertMoveY = value; } }

            [SerializeField]
            private bool _invertLookX;

            public bool InvertLookX { get { return _invertLookX; } set { _invertLookX = value; } }

            [SerializeField]
            private bool _invertLookY;

            public bool InvertLookY { get { return _invertLookY; } set { _invertLookY = value; } }

            [SerializeField]
            private bool _invertZoom;

            public bool InvertZoom { get { return _invertZoom; } set { _invertZoom = value; } }

            [SerializeField]
            private /*readonly*/ DPadState[] _dpadState = new DPadState[(int)DPadDir.NumAxes];

            public IReadOnlyCollection<DPadState> DPadStates => _dpadState;

            public ControllerState()
            {
                for(int i=0; i<_dpadState.Length; ++i) {
                    _dpadState[i] = DPadState.None;
                }
            }

            public void Reset()
            {
                InvertMoveX = false;
                InvertMoveY = false;

                InvertLookX = false;
                InvertLookY = false;

                InvertZoom = false;
            }

            public void UpdateDpadState(Vector3 axes)
            {
                UpdateDpadState(DPadDir.Left, axes.x < -Mathf.Epsilon);
                UpdateDpadState(DPadDir.Right, axes.x > Mathf.Epsilon);
                UpdateDpadState(DPadDir.Down, axes.y < -Mathf.Epsilon);
                UpdateDpadState(DPadDir.Up, axes.y > Mathf.Epsilon);
            }

            private void UpdateDpadState(DPadDir dir, bool pressed)
            {
                DPadState currentState = _dpadState[(int)dir];
                DPadState newState = currentState;

                switch(currentState)
                {
                case DPadState.None:
                    if(pressed) newState = DPadState.Down;
                    break;
                case DPadState.Down:
                    newState = pressed ? DPadState.Held : DPadState.Up;
                    break;
                case DPadState.Held:
                    if(!pressed) newState = DPadState.Up;
                    break;
                case DPadState.Up:
                    newState = pressed ? DPadState.Down : DPadState.None;
                    break;
                }

                _dpadState[(int)dir] = newState;
            }
        }

        private static string ButtonString(int controllerIndex, Button button)
        {
            return $"P{controllerIndex} Button{(int)button}";
        }

        [SerializeField]
        private int _maxControllers = 4;

        public int MaxControllers => _maxControllers;

        //[SerializeField]
        [ReadOnly]
        private bool[] _controllerAcquired;

        //[SerializeField]
        [ReadOnly]
        private ControllerState[] _controllerStates;

        public IReadOnlyCollection<ControllerState> ControllerStates => _controllerStates;

// TODO: detect joysticks connecting and disconnecting

#region Unity Lifecycle
        private void Awake()
        {
            _controllerAcquired = new bool[MaxControllers];

            _controllerStates = new ControllerState[MaxControllers];
            for(int i=0; i<MaxControllers; ++i) {
                _controllerStates[i] = new ControllerState();
            }
        }

        private void Start()
        {
            string[] joystickNames = UnityEngine.Input.GetJoystickNames();

            Debug.Log($"Detected {joystickNames.Length} joysticks:");
            foreach(string joystickName in joystickNames) {
                Debug.Log($"\t{joystickName}");
            }
        }

        private void Update()
        {
            for(int i=0; i<MaxControllers; ++i) {
                if(Pressed(i, Button.RightBumper)) {
                    Debug.Log($"Inverting controller {i} look");
                    _controllerStates[i].InvertLookY = !_controllerStates[i].InvertLookY;
                } else if(Pressed(i, Button.LeftBumper)) {
                    Debug.Log($"Inverting controller {i} move");
                    _controllerStates[i].InvertMoveY = !_controllerStates[i].InvertMoveY;
                }
            }

            UpdateDpadAxes();
        }
#endregion

        public void Reset()
        {
            foreach(ControllerState controllerState in ControllerStates) {
                controllerState.Reset();
            }
        }

        public int AcquireController()
        {
            for(int i=0; i<_controllerAcquired.Length; ++i) {
                if(!_controllerAcquired[i]) {
                    _controllerAcquired[i] = true;
                    return i;
                }
            }
            return -1;
        }

#region Axes
        public Vector3 GetMoveAxes(int controllerIndex)
        {
            ControllerState state = _controllerStates[controllerIndex];
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal") * (state.InvertMoveX ? -1.0f : 1.0f),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical") * (state.InvertMoveY ? -1.0f : 1.0f),
                0.0f
            );
        }

        public Vector3 GetLookAxes(int controllerIndex)
        {
            ControllerState state = _controllerStates[controllerIndex];
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal Look") * (state.InvertLookX ? -1.0f : 1.0f),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical Look") * (state.InvertLookY ? -1.0f : 1.0f),
                0.0f
            );
        }

        public Vector3 GetDpadAxes(int controllerIndex)
        {
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal DPad"),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical DPad")
            );
        }

        public Vector3 GetTriggerAxes(int controllerIndex)
        {
            // TODO
            return Vector3.zero;
        }

        private void UpdateDpadAxes()
        {
            for(int i=0; i<_controllerStates.Length; ++i) {
                UpdateDpadAxes(i);
            }
        }

        private void UpdateDpadAxes(int controllerIndex)
        {
            ControllerState state = _controllerStates[controllerIndex];
            Vector3 axes = GetDpadAxes(controllerIndex);

            state.UpdateDpadState(axes);
        }
#endregion

#region Buttons
        public bool Pressed(int controllerIndex, Button button)
        {
            return UnityEngine.Input.GetButtonDown(ButtonString(controllerIndex, button));
        }

        public bool Released(int controllerIndex, Button button)
        {
            return UnityEngine.Input.GetButtonUp(ButtonString(controllerIndex, button));
        }

        public bool Held(int controllerIndex, Button button)
        {
            return UnityEngine.Input.GetButton(ButtonString(controllerIndex, button));
        }

        public bool StartPressed()
        {
            for(int i=0; i<MaxControllers; ++i) {
                if(StartPressed(i)) {
                    return true;
                }
            }
            return false;
        }

        public bool StartPressed(int controllerIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Start");
        }

        public bool SelectPressed()
        {
            for(int i=0; i<MaxControllers; ++i) {
                if(SelectPressed(i)) {
                    return true;
                }
            }
            return false;
        }

        public bool SelectPressed(int controllerIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Select");
        }

        public bool PositivePressed()
        {
            for(int i=0; i<MaxControllers; ++i) {
                if(PositivePressed(i)) {
                    return true;
                }
            }
            return false;
        }

        public bool PositivePressed(int controllerIndex)
        {
            return Pressed(controllerIndex, Button.A) || StartPressed(controllerIndex);
        }

        public bool NegativePressed()
        {
            for(int i=0; i<MaxControllers; ++i) {
                if(NegativePressed(i)) {
                    return true;
                }
            }
            return false;
        }

        public bool NegativePressed(int controllerIndex)
        {
            return Pressed(controllerIndex, Button.B) || SelectPressed(controllerIndex);
        }
#endregion

#region Dpad
        public bool DpadPressed(int controllerIndex, DPadDir dir)
        {
            ControllerState state = _controllerStates[controllerIndex];
            return DPadState.Down == state.DPadStates.ElementAt((int)dir);
        }

        public bool DpadReleased(int controllerIndex, DPadDir dir)
        {
            ControllerState state = _controllerStates[controllerIndex];
            return DPadState.Up == state.DPadStates.ElementAt((int)dir);
        }

        public bool DpadHeld(int controllerIndex, DPadDir dir)
        {
            ControllerState state = _controllerStates[controllerIndex];
            return DPadState.Held == state.DPadStates.ElementAt((int)dir);
        }
#endregion
    }
}

