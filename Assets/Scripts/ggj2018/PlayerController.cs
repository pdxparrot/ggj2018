using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        [SerializeField]
        private float _baseSpeed = 10.0f;

        [SerializeField]
        private float _baseTurnSpeed = 5.0f;

        [SerializeField]
        private float _basePitchSpeed = 5.0f;

        [SerializeField]
        private GameObject _model;

        [SerializeField]
        [ReadOnly]
        private BaseAttributes _attributes = new BaseAttributes();

        public BaseAttributes Attributes { get { return _attributes; } set { _attributes = value ?? new BaseAttributes(); } }

#region Unity Lifecycle
        private void Update()
        {
            float dt = Time.deltaTime;

            Vector3 inputAxes = InputManager.Instance.GetAxes();

            Vector3 rotation = _model.transform.rotation.eulerAngles;
            rotation.x = Mathf.Clamp(rotation.x + inputAxes.x * dt, -45.0f, 45.0f);
            rotation.z = Mathf.Clamp(rotation.z + inputAxes.y * dt, -45.0f, 45.0f);

            float speed = _baseSpeed + _attributes.SpeedModifier * dt;
            float turnSpeed = _baseTurnSpeed + _attributes.TurnSpeedModifier * dt;
            float pitchSpeed = _basePitchSpeed + _attributes.PitchSpeedModifier * dt;

            _model.transform.rotation = Quaternion.Euler(rotation.x, 0.0f, rotation.y);
            transform.position += new Vector3(turnSpeed, pitchSpeed, speed);
        }
#endregion
    }
}
