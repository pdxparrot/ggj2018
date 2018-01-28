using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(PlayerController))]
    public sealed class NetworkPlayer : NetworkBehavior, IPlayer
    {
#region Unity Lifecycle
        private void Awake()
        {
            if(isLocalPlayer) {
                Core.Camera.CameraManager.Instance.GetFollowCamera().SetTarget(gameObject);
            }
        }
#endregion

        public void MoveTo(Vector3 position)
        {
            transform.position = position;
        }
    }
}
