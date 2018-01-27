using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Camera
{
    public sealed class CameraManager : SingletonBehavior<CameraManager>
    {
        public UnityEngine.Camera MainCamera { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
        }
#endregion
    }
}
