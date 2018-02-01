using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    public sealed class NetworkPlayer : NetworkBehavior, IPlayer
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
            if(isLocalPlayer) {
                Debug.Log($"Setting local follow cam {ControllerNumber}");
                CameraManager.Instance.Viewers.ElementAt(ControllerNumber).FollowCamera.SetTarget(gameObject);
            }

            _godRay.GetComponent<GodRay>().Setup(this);

            CameraManager.Instance.AddRenderLayer(ControllerNumber, State.BirdType.Layer);
            CameraManager.Instance.RemoveRenderLayer(ControllerNumber, State.BirdType.OtherLayer);    
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);
        }
#endregion
    }
}
