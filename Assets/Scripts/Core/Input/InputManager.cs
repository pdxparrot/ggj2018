using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        public Vector3 GetMoveAxes(int controllerIndex)
        {
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal"),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical"),
                0.0f
            );
        }

        public Vector3 GetLookAxes(int controllerIndex)
        {
            return new Vector3(
                UnityEngine.Input.GetAxis($"P{controllerIndex} Horizontal Look"),
                UnityEngine.Input.GetAxis($"P{controllerIndex} Vertical Look"),
                0.0f
            );
        }

        public bool Pressed(int controllerIndex, int buttonIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Button{buttonIndex}");
        }

        public bool Held(int controllerIndex, int buttonIndex)
        {
            return UnityEngine.Input.GetButton($"P{controllerIndex} Button{buttonIndex}");
        }

        public bool StartPressed(int controllerIndex)
        {
            return UnityEngine.Input.GetButtonDown($"P{controllerIndex} Start");
        }
    }
}

