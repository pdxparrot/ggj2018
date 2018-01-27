using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(PlayerController))]
    public sealed class Player : NetworkBehavior
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
