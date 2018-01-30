using System;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public enum Dir {
        Up, Down, Left, Right,
        NumAxes
    }

    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        [Serializable]
        private sealed class ControllerState
        {
            [SerializeField]
            private bool _invertMoveX = false;

            public bool InvertMoveX => _invertMoveX;

            [SerializeField]
            private bool _invertMoveY = false;

            public bool InvertMoveY => _invertMoveY;

            [SerializeField]
            private bool _inverLookX = false;

            public bool InvertLookX => _inverLookX;

            [SerializeField]
            private bool _invertLookY = false;

            public bool InvertLookY => _invertLookY;

            [SerializeField]
            private bool _invertZoom = false;

            public bool InvertZoom => _invertZoom;

            [SerializeField]
            private readonly bool[] _dpadPressed = new bool[(int)Dir.NumAxes];

            public bool GetDPadPressed(Dir dir)
            {
                return _dpadPressed[(int)dir];
            }

            public void SetDPadPressed(Dir dir, bool pressed)
            {
                _dpadPressed[(int)dir] = pressed;
            }
        }

        private static string ButtonString(int controllerIndex, int buttonIndex)
        {
            return $"P{controllerIndex} Button{buttonIndex}";
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
#endregion

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

        public bool Pressed(int controllerIndex, int buttonIndex)
        {
            return UnityEngine.Input.GetButtonDown(ButtonString(controllerIndex, buttonIndex));
        }

        public bool Released(int controllerIndex, int buttonIndex)
        {
            return UnityEngine.Input.GetButtonUp(ButtonString(controllerIndex, buttonIndex));
        }

        public bool Held(int controllerIndex, int buttonIndex)
        {
            return UnityEngine.Input.GetButton(ButtonString(controllerIndex, buttonIndex));
        }

        public bool StartPressed()
        {
            return StartPressed(0) ||
                StartPressed(1) ||
                StartPressed(2) ||
                StartPressed(3);
        }

        public bool StartPressed(int controllerIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Start");
        }

        public bool SelectPressed()
        {
            return SelectPressed(0) ||
                SelectPressed(1) ||
                SelectPressed(2) ||
                SelectPressed(3);
        }

        public bool SelectPressed(int controllerIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Select");
        }

        // Axis pressed methods
        public bool DpadPressed(int controllerIndex, Dir dir) {
            ControllerState state = _controllerStates[controllerIndex];

            float val = (dir == Dir.Up || dir == Dir.Down)
                ? UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal")
                : UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical");

            bool down = (dir == Dir.Down || dir == Dir.Left) ? (val < -0.8f) : (val > 0.8f);
            bool pressed = (down && !state.GetDPadPressed(dir));
            state.SetDPadPressed(dir, down);

            return pressed;
        }
    }
}

