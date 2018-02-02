using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

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
        [SerializeField]
        [ReadOnly]
        private int _id;

        public int Id => _id;

        [SerializeField]
        private PlayerState _playerState;

        public PlayerState State => _playerState;

        [SerializeField]
        private GameObject _godRay;

// TODO: probably don't need to expose this
        private PlayerController _controller;

        public PlayerController Controller => _controller;

        public int ControllerNumber { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            _playerState = new PlayerState(this);

            _controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);
        }
#endregion

        public void Initialize(int id, int controllerNumber, Bird birdModel, BirdData.BirdDataEntry birdType)
        {
            _id = id;
            ControllerNumber = controllerNumber;
            name = $"Player {Id}";

            _controller.Initialize(this, birdModel);
            State.Initialize(birdType);

            //if(isLocalPlayer) {
            Debug.Log($"Setting follow cam {ControllerNumber}");
            CameraManager.Instance.Viewers.ElementAt(ControllerNumber).FollowCamera.SetTarget(gameObject);

            CameraManager.Instance.AddRenderLayer(ControllerNumber, State.BirdType.Layer);
            CameraManager.Instance.RemoveRenderLayer(ControllerNumber, State.BirdType.OtherLayer);       
            //}

            _godRay.GetComponent<GodRay>().Setup(this);
        }
    }
}
