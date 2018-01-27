using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    public sealed class LocalPlayer : MonoBehavior
    {
#region Unity Lifecycle
        private void Awake()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;

            CameraManager.Instance.GetFollowCamera().SetTarget(gameObject);
        }
#endregion
    }
}
