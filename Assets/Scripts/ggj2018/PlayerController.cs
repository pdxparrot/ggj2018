using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        [SerializeField]
        private float baseSpeed = 10.0f;

        [SerializeField]
        private BaseAttributes _attributes = new BaseAttributes();

        public BaseAttributes Attributes { get { return _attributes; } set { _attributes = value ?? new BaseAttributes(); } }

#region Unity Lifecycle
        private void Update()
        {
            float strafe = Input.GetAxis("Horizontal");
            float pitch = Input.GetAxis("Vertical");

            float zrot = Mathf.Clamp(strafe, -1.0f, 1.0f);
            float xrot = Mathf.Clamp(pitch, -1.0f, 1.0f);

            float speed = baseSpeed + _attributes.SpeedModifier;

            transform.rotation = Quaternion.Euler(xrot, 0.0f, zrot);
            transform.position += new Vector3(strafe, pitch, speed);
        }
#endregion
    }
}
