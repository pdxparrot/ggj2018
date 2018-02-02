﻿using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
// TODO: rename this Player when NetworkPlayer is gone
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    //[RequireComponent(typeof(NetworkIdentity))]
    //[RequireComponent(typeof(NetworkTransform))]
    public sealed class Player : MonoBehaviour, IFollowTarget //NetworkBehavior
    {
        public GameObject GameObject => gameObject;

        [SerializeField]
        private PlayerState _playerState;

        public PlayerState State => _playerState;

        [SerializeField]
        private GameObject _godRay;

        public PlayerController Controller { get; private set; }

// TODO: assign this, don't assume it
        public int ControllerNumber => State.PlayerNumber;

#region Unity Lifecycle
        private void Awake()
        {
            _playerState = new PlayerState(this);

            Controller = GetComponent<PlayerController>();
        }

        public void Initialize()
        {
            //if(isLocalPlayer) {
                Debug.Log($"Setting follow cam {ControllerNumber}");
                CameraManager.Instance.Viewers.ElementAt(ControllerNumber).FollowCamera.SetTarget(GameObject);

                CameraManager.Instance.AddRenderLayer(ControllerNumber, State.BirdType.Layer);
                CameraManager.Instance.RemoveRenderLayer(ControllerNumber, State.BirdType.OtherLayer);       
            //}

            _godRay.GetComponent<GodRay>().Setup(this);
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);
        }
#endregion
    }
}