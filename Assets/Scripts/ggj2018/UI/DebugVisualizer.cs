using ggj2018.Core.Util;
using ggj2018.ggj2018.Players;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
    public class DebugVisualizer : MonoBehavior
    {
        [SerializeField]
        private Text _debugVelocityText;

        [SerializeField]
        private Text _debugAngularVelocityText;

        [SerializeField]
        private Text _debugBankForceText;

        [SerializeField]
        private float _offset = 2.5f;

        public void SetState(Player player)
        {
            _debugVelocityText.text = $"Velocity: {player.Controller.Rigidbody.velocity} m/s";
            _debugAngularVelocityText.text = $"Angular Velocity: {player.Controller.Rigidbody.angularVelocity} m/s";
            _debugBankForceText.text = $"Bank Force: {player.Controller.BankForce}N";

            transform.position = player.transform.position + (player.transform.forward * _offset);
        }
    }
}
