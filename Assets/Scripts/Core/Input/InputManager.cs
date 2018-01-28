using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public enum Dir {
        Up, Down, Left, Right
    }

    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        private static string ButtonString(int controllerIndex, int buttonIndex)
        {
            return $"P{controllerIndex} Button{buttonIndex}";
        }

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

        public Vector3 GetMoveAxes(int controllerIndex)
        {
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal") * (InvertMoveX ? -1.0f : 1.0f),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical") * (InvertMoveY ? -1.0f : 1.0f),
                0.0f
            );
        }

        public Vector3 GetLookAxes(int controllerIndex)
        {
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal Look") * (InvertLookX ? -1.0f : 1.0f),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical Look") * (InvertLookY ? -1.0f : 1.0f),
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

        public bool StartPressed(int controllerIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Start");
        }

        public float GetZoomZxis()
        {
            return UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        }

        // Axis pressed methods
        private const int NumPlayers = 4;
        private const int NumAxes = 4;
        private bool[] _dpadPressed = new bool[NumPlayers * NumAxes];

        public bool DpadPressed(int controllerIndex, Dir dir) {
            float val = (dir == Dir.Up || dir == Dir.Down)
                ? UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal")
                : UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical");

            bool down = (dir == Dir.Down || dir == Dir.Left) ? (val < -0.8f) : (val > 0.8f);
            bool pressed = (down && !_dpadPressed[controllerIndex * NumAxes + (int)dir]);
            _dpadPressed[controllerIndex * NumAxes + (int)dir] = down;

            return pressed;
        }
    }
}

