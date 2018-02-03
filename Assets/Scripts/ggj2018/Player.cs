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
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    public sealed class Player : NetworkBehavior, IFollowTarget
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

        private PlayerController _controller;

        public Bird Bird => _controller.Bird;

        public Vector3 LookAxis => InputManager.Instance.GetLookAxes(ControllerIndex);

        [SerializeField]
        [ReadOnly]
        private int _controllerIndex;

        public int ControllerIndex => _controllerIndex;

        [SerializeField]
        [ReadOnly]
        private Viewer _viewer;

        public Viewer Viewer => _viewer;

        public bool IsLocalPlayer => !GameManager.Instance.ConfigData.EnableNetwork || isLocalPlayer;

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
            Debug.Log($"Initializing local player {id}");
            Initialize(id, birdModel, birdType);

            _controllerIndex = controllerIndex;

            _viewer = viewer;
            Viewer.Initialize(this);

            Viewer.FollowCamera.SetTarget(gameObject);

            Viewer.AddRenderLayer(State.BirdType.Layer);
            Viewer.RemoveRenderLayer(State.BirdType.OtherLayer);
        }

        public void InitializeNetwork(int id, Bird birdModel, BirdData.BirdDataEntry birdType)
        {
            Debug.Log($"Initializing network player {id}");
            Initialize(id, birdModel, birdType);
        }

        private void Initialize(int id, Bird birdModel, BirdData.BirdDataEntry birdType)
        {
            _id = id;
            name = $"Player {Id}";

            _controller.Initialize(this, birdType, birdModel);
            State.Initialize(birdType);

            _godRay.GetComponent<GodRay>().Setup(this);

            LogInfo();
        }

        private void LogInfo()
        {
            Debug.Log($@"Player(Id: {Id}, Networkid: {netId})
isLocalPlayer: {IsLocalPlayer}
isClient: {isClient}
isServer: {isServer}"
            );
        }
    }
}
