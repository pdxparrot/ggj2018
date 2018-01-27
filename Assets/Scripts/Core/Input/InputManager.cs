using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
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

        public Vector3 GetMoveAxes()
        {
            return new Vector3(
                UnityEngine.Input.GetAxis("Horizontal") * (InvertMoveX ? -1.0f : 1.0f),
                UnityEngine.Input.GetAxis("Vertical") * (InvertMoveY ? -1.0f : 1.0f),
                0.0f
            );
        }

        public Vector3 GetLookAxes()
        {
            return new Vector3(
                UnityEngine.Input.GetAxis("Horizontal Look") * (InvertLookX ? -1.0f : 1.0f),
                UnityEngine.Input.GetAxis("Vertical Look") * (InvertLookY ? -1.0f : 1.0f),
                0.0f
            );
        }

        public float GetZoomZxis()
        {
            return UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        }
    }
}
