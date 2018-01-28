using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    public sealed class LocalPlayer : MonoBehavior, IPlayer
    {
#region Unity Lifecycle
        private void Awake()
        {
            Core.Camera.CameraManager.Instance.GetFollowCamera().SetTarget(gameObject);
        }
#endregion

        public void MoveTo(Vector3 position)
        {
            transform.position = position;
        }
    }
}
