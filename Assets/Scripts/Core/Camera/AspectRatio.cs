using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Camera
{
    // http://2sa-studio.blogspot.com/2015/01/handling-aspect-ratio-in-unity2d.html
    [RequireComponent(typeof(UnityEngine.Camera))]
    public sealed class AspectRatio : MonoBehavior
    {
        [SerializeField]
        private int _aspectWidth = 16;

        [SerializeField]
        private int _aspectHeight = 9;

        [SerializeField]
        [ReadOnly]
        private float _targetAspectRatio;

        private UnityEngine.Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();

            _targetAspectRatio = _aspectWidth / (float)_aspectHeight;
        }

        private void Start()
        {
            float screenAspectRatio = Screen.width / (float)Screen.height;
            float scaleHeight = screenAspectRatio / _targetAspectRatio;

            if(scaleHeight < 1.0f) {
                _camera.orthographicSize = _camera.orthographicSize / scaleHeight;
            }
        }
    }
}
