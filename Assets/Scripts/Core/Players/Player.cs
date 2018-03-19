using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace pdxpartyparrot.Core.Players
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    public abstract class Player : NetworkBehavior
    {
        [SerializeField]
        [ReadOnly]
        private int _id = -1;

        public int Id => _id;

        [SerializeField]
        private PlayerController _controller;

        public PlayerController Controller => _controller;

        public virtual void Initialize(int id)
        {
            _id = id;

            _controller.Initialize(this);
        }
    }
}
