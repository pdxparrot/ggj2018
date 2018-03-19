using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Players
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PlayerController : MonoBehavior
    {
        [SerializeField]
        private PlayerDriver _driver;

        public PlayerDriver Driver => _driver;

        public Rigidbody Rigidbody { get; private set; }

        protected Player Owner { get; private set; }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
#endregion

        public virtual void Initialize(Player owner)
        {
            Owner = owner;

            _driver.Initialize(owner, this);
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Teleporting player {Owner.Id} to {position}");
            Rigidbody.position = position;
        }
    }
}
