using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    public sealed class NetworkPlayer : NetworkBehavior, IPlayer
    {
        public GameObject GameObject => gameObject;

        public PlayerState State { get; private set; }

        public PlayerController Controller { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            State = new PlayerState(this);

            Controller = GetComponent<PlayerController>();

            if(isLocalPlayer) {
                Core.Camera.CameraManager.Instance.GetFollowCamera().SetTarget(gameObject);
            }
        }
#endregion
    }
}
