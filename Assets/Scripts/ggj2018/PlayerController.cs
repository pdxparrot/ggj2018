using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;
using ggj2018.Game.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class PlayerController : MonoBehavior
    {
        [SerializeField]
        private GameObject _model;

        [SerializeField]
        [ReadOnly]
        private BaseAttributes _attributes = new BaseAttributes();

        public BaseAttributes Attributes { get { return _attributes; } set { _attributes = value ?? new BaseAttributes(); } }

        private PlayerData _playerData;

#region Unity Lifecycle
        private void Awake()
        {
            _playerData = DataManager.Instance.GameData.Data.GetOrDefault(PlayerData.DataName) as PlayerData;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            float speed = (_playerData.BaseSpeed + _attributes.SpeedModifier) * dt;

            Vector3 moveAxes = InputManager.Instance.GetMoveAxes();
            Vector3 lookAxes = InputManager.Instance.GetLookAxes();

/*

            Vector3 rotation = _model.transform.rotation.eulerAngles;
            rotation.x = Mathf.Clamp(rotation.x + inputAxes.x * dt, -45.0f, 45.0f);
            rotation.z = Mathf.Clamp(rotation.z + inputAxes.y * dt, -45.0f, 45.0f);

            float turnSpeed = _playerData.BaseTurnSpeed + _attributes.TurnSpeedModifier * dt;
            float pitchSpeed = _playerData.BasePitchSpeed + _attributes.PitchSpeedModifier * dt;

            _model.transform.rotation = Quaternion.Euler(rotation.x, 0.0f, rotation.y);
*/

            Vector3 velocity = transform.forward * speed;
            transform.position += velocity;
        }
#endregion
    }
}
