using ggj2018.Core.Util;
using UnityEngine;

namespace ggj2018.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        public Vector3 GetAxes()
        {
            return new Vector3(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical"),
                0.0f
            );
        }
    }
}
