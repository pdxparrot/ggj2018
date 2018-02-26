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

        private PlayerController _controller;

        public PlayerController Controller => _controller;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _controller = GetComponent<PlayerController>();
        }
#endregion

        public virtual void Initialize(int id)
        {
            _id = id;

            _controller.Initialize(this);
        }
    }
}
