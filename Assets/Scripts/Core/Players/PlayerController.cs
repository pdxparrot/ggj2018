using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Players
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PlayerController : MonoBehavior
    {
        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField]
        private PlayerDriver _driver;

        protected PlayerDriver Driver => _driver;

        protected Player Owner { get; private set; }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
#endregion

        public virtual void Initialize(Player owner)
        {
            Owner = owner;
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {Owner.Id} to {position}");
            Rigidbody.position = position;
        }
    }
}
