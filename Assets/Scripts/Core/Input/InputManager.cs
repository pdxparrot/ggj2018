using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        public Vector3 GetMoveAxes()
        {
            return new Vector3(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical"),
                0.0f
            );
        }

        public Vector3 GetLookAxes()
        {
            return new Vector3(
                UnityEngine.Input.GetAxis("Horizontal Look"),
                UnityEngine.Input.GetAxis("Vertical Look"),
                0.0f
            );
        }
    }
}
