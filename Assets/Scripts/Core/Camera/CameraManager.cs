using ggj2018.Core.Util;

using UnityEngine;
using System.Collections.Generic;

namespace ggj2018.Core.Camera
{
    public sealed class CameraManager : SingletonBehavior<CameraManager>
    {
        [SerializeField] List<GameObject> Cameras;

        //public UnityEngine.Camera MainCamera { get; private set; }

#region Unity Lifecycle
        ////private void Awake()
        //{
            //MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
        //}
#endregion

        public FollowCamera GetFollowCamera(int i)
        {
            return Cameras[i].GetComponent<FollowCamera>();
        }

        public void SetupCamera(int i, bool active)
        {
            Cameras[i].GetComponent<UnityEngine.Camera>().enabled = active;
        }
    }
}
