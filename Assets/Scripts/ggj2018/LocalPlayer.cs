﻿using System.Linq;

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

        [SerializeField]
        private PlayerState _playerState;

        public PlayerState State => _playerState;

        [SerializeField]
        private GameObject _godRay;

        public PlayerController Controller { get; private set; }

        public int ControllerNumber => State.PlayerNumber;

#region Unity Lifecycle
        private void Awake()
        {
            _playerState = new PlayerState(this);

            Controller = GetComponent<PlayerController>();
        }

        public void Initialize()
        {
            Debug.Log($"Setting follow cam {ControllerNumber}");
            CameraManager.Instance.Viewers.ElementAt(ControllerNumber).FollowCamera.SetTarget(GameObject);

            _godRay.GetComponent<GodRay>().Setup(
                State.BirdType.IsPredator ?
                GodRay.Mode.Hawk : GodRay.Mode.Carrier);

            CameraManager.Instance.AddRenderLayer(ControllerNumber, State.BirdType.RenderLayerMask);
            CameraManager.Instance.RemoveRenderLayer(ControllerNumber, State.BirdType.OtherRenderLayerMask);       
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);
        }
#endregion
    }
}
