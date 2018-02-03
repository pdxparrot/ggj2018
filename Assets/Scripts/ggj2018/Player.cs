using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018
{
// TODO: rename this Player when NetworkPlayer is gone
    [RequireComponent(typeof(PlayerController))]
    //[RequireComponent(typeof(NetworkIdentity))]
    //[RequireComponent(typeof(NetworkTransform))]
    public sealed class Player : MonoBehaviour, IFollowTarget //NetworkBehavior
    {
        [SerializeField]
        [ReadOnly]
        private int _id = -1;

        public int Id => _id;

        [SerializeField]
        private PlayerState _playerState;

        public PlayerState State => _playerState;

        [SerializeField]
        private GameObject _godRay;

// TODO: probably don't need to expose this
        private PlayerController _controller;

        public PlayerController Controller => _controller;

        public Vector3 LookAxis => InputManager.Instance.GetLookAxes(ControllerIndex);

        public int ControllerIndex { get; private set; }

        public Viewer Viewer { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            _playerState = new PlayerState(this);

            _controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);

            Viewer?.PlayerUI.SetScore(State.Score, GameManager.Instance.State.GameType.ScoreLimit(State.BirdType)); 
            Viewer?.PlayerUI.SetTimer(GameManager.Instance.State.Timer); 
        }
#endregion

        public void InitializeLocal(int id, int controllerIndex, Viewer viewer, Bird birdModel, BirdData.BirdDataEntry birdType)
        {
            Initialize(id, birdModel, birdType);

            ControllerIndex = controllerIndex;

            Viewer = viewer;
            Viewer.Initialize(this);

            Viewer.FollowCamera.SetTarget(gameObject);

            Viewer.AddRenderLayer(State.BirdType.Layer);
            Viewer.RemoveRenderLayer(State.BirdType.OtherLayer);
        }

        public void InitializeNetwork(int id, Bird birdModel, BirdData.BirdDataEntry birdType)
        {
            Initialize(id, birdModel, birdType);
        }

        private void Initialize(int id, Bird birdModel, BirdData.BirdDataEntry birdType)
        {
            _id = id;
            name = $"Player {Id}";

            _controller.Initialize(this, birdModel);
            State.Initialize(birdType);

            _godRay.GetComponent<GodRay>().Setup(this);
        }
    }
}
