using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    public sealed class LocalPlayer : MonoBehavior, IPlayer
    {
        public GameObject GameObject => gameObject;

        public PlayerState State { get; private set; }

        public PlayerController Controller { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            State = new PlayerState(this);

            Controller = GetComponent<PlayerController>();

            Core.Camera.CameraManager.Instance.GetFollowCamera().SetTarget(gameObject);
        }
#endregion
    }
}
