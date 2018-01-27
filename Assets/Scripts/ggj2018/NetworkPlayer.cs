using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(PlayerController))]
    public sealed class NetworkPlayer : NetworkBehavior
    {
#region Unity Lifecycle
        private void Awake()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
        }
#endregion
    }
}
