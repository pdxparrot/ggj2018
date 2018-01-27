using ggj2018.Core.Input;
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
            float dt = Time.deltaTime;

            Vector3 axes = InputManager.Instance.GetAxes();

            float zrot = Mathf.Clamp(axes.x, -1.0f, 1.0f) * dt;
            float xrot = Mathf.Clamp(axes.y, -1.0f, 1.0f) * dt;

            float speed = baseSpeed + _attributes.SpeedModifier * dt;

            transform.rotation = Quaternion.Euler(xrot, 0.0f, zrot);
            transform.position += new Vector3(axes.x, axes.y, speed);
        }
#endregion
    }
}
