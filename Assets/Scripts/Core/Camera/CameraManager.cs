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

        public UnityEngine.Camera GetCamera(int i)
        {
            return Cameras[i].GetComponent<UnityEngine.Camera>();
        }

        public FollowCamera GetFollowCamera(int i)
        {
            return Cameras[i].GetComponent<FollowCamera>();
        }

        public void SetupCamera(int i, bool active)
        {
            GetFollowCamera(i).enabled = active;

            // -- disabled cameras go in the black box under the world
            if(!active) {
                Cameras[i].gameObject.transform.position = new Vector3(0,-100,0);
            }
        }

        public enum Layer { Hawk, Carrier }
        public void SetLayer(int cam, Layer layer) {
            Cameras[cam].GetComponent<UnityEngine.Camera>().cullingMask &= ~(1 << 8);
            Cameras[cam].GetComponent<UnityEngine.Camera>().cullingMask &= ~(1 << 9);

            if(layer == Layer.Hawk) {
                Cameras[cam].GetComponent<UnityEngine.Camera>().cullingMask |= (1 << 8);
            }
            else {
                Cameras[cam].GetComponent<UnityEngine.Camera>().cullingMask |= (1 << 9);
            }
        }
    }
}
