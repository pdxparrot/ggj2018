using System;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        public enum DPadDir {
            Up, Down, Left, Right,
            NumAxes
        }

        public enum Button {
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

            public bool InvertX { set { _invertLookX = value; _invertMoveX = value; } }

            public bool InvertY { set { _invertLookY = value; _invertMoveY = value; } }

            [SerializeField]
            private bool _invertZoom;

            public bool InvertZoom { get { return _invertZoom; } set { _invertZoom = value; } }

            [SerializeField]
            private readonly bool[] _dpadPressed = new bool[(int)DPadDir.NumAxes];

            public bool GetDPadPressed(DPadDir dir)
            {
                return _dpadPressed[(int)dir];
            }

            public void SetDPadPressed(DPadDir dir, bool pressed)
            {
                _dpadPressed[(int)dir] = pressed;
            }
        }

        private static string ButtonString(int controllerIndex, Button button)
        {
            return $"P{controllerIndex} Button{(int)button}";
        }

        [SerializeField]
        private int _maxControllers = 4;

        public int MaxControllers => _maxControllers;

        [SerializeField]
        [ReadOnly]
        private int _connectedJoystickCount;

        public int ConnectedJoystickCount => _connectedJoystickCount;

        public bool HasJoystickConnected => _connectedJoystickCount > 0;

        [SerializeField]
        [ReadOnly]
        private ControllerState[] _controllerStates;

// TODO: detect joysticks connecting and disconnecting

#region Unity Lifecycle
        private void Awake()
        {
            _controllerStates = new ControllerState[MaxControllers];
            for(int i=0; i<MaxControllers; ++i) {
                _controllerStates[i] = new ControllerState();
            }
        }

        private void Start()
        {
            string[] joystickNames = UnityEngine.Input.GetJoystickNames();
            _connectedJoystickCount = joystickNames.Length;

            Debug.Log($"Detected {ConnectedJoystickCount} joysticks:");
            foreach(string joystickName in joystickNames) {
                Debug.Log($"\t{joystickName}");
            }
        }

        private void Update()
        {
            for(int i=0; i<MaxControllers; ++i) {
                if(Pressed(i, Button.LeftBumper)) {
                    Debug.Log($"Inverting controller {i}");
                    _controllerStates[i].InvertY = true;
                } else if(Pressed(i, Button.RightBumper)) {
                    Debug.Log($"Uninverting controller {i}");
                    _controllerStates[i].InvertY = false;
                }
            }
        }
#endregion

        public ControllerState GetControllerState(int controllerIndex)
        {
            return _controllerStates[controllerIndex];
        }

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
#endregion

        // Axis pressed methods
        public bool DpadPressed(int controllerIndex, DPadDir dir) {
            ControllerState state = _controllerStates[controllerIndex];

            float val = (dir == DPadDir.Up || dir == DPadDir.Down)
                ? UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal")
                : UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical");

            bool down = (dir == DPadDir.Down || dir == DPadDir.Left) ? (val < -0.8f) : (val > 0.8f);
            bool pressed = (down && !state.GetDPadPressed(dir));
            state.SetDPadPressed(dir, down);

            return pressed;
        }
    }
}

