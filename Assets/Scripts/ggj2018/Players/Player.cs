using System.Collections;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Birds;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.VFX;
using pdxpartyparrot.ggj2018.World;

using JetBrains.Annotations;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Players
{
    public sealed class Player : Core.Players.Player, IFollowTarget
    {
        public GameObject GameObject => gameObject;

        [SerializeField]
        [ReadOnly]
        private  PlayerState _playerState;

        public PlayerState State => _playerState;

        private GodRay _godRay;

        public Bird Bird { get; private set; }

        public Color PlayerColor => PlayerManager.Instance.PlayerData.GetPlayerColor(Id);

        public new PlayerController Controller => (PlayerController)base.Controller;

// TODO: driver class should handle input
#region Input
        [SerializeField]
        [ReadOnly]
        private int _controllerIndex;

        public int ControllerIndex => _controllerIndex;

        public Vector3 LookAxis => InputManager.Instance.GetLookAxes(ControllerIndex);
#endregion

        public bool IsPaused => GameManager.Instance.IsPaused;

#region Viewer
        [SerializeField]
        [ReadOnly]
        private Camera.Viewer _viewer;

        public Camera.Viewer Viewer => _viewer;
#endregion

#region Network
        public bool IsLocalPlayer => !GameManager.Instance.ConfigData.EnableNetwork || isLocalPlayer;
#endregion

#region Nearest Other Objects
        [CanBeNull]
        public Player NearestPredator { get; private set; }

        private float _nearestPredatorDistance;

        public float NearestPredatorDistance => _nearestPredatorDistance;

        [CanBeNull]
        public Player NearestPrey { get; private set; }

        private float _nearestPreyDistance;

        public float NearestPreyDistance => _nearestPreyDistance;

        [CanBeNull]
        public Goal NearestGoal { get; private set; }

        private float _nearestGoalDistance;

        public float NearestGoalDistance => _nearestGoalDistance;
#endregion

        public Collider Collider => Bird.Collider;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _playerState = new PlayerState(this);
        }

        private void OnDestroy()
        {
            State.Destroy();

            Viewer.FollowCamera.SetTarget(null);

            Destroy(_godRay.gameObject);
            _godRay = null;
        }

        private void Start()
        {
            StartCoroutine(UpdateNearestPlayers());
            StartCoroutine(UpdateNearestGoal());
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);

            Viewer.PlayerUI.PlayerUIPage.PlayerHUD.SetState(this);

            if(InputManager.Instance.Pressed(Id, PlayerManager.Instance.PlayerData.HornButton)) {
                Bird.PlayHornAudio();
            }
        }
#endregion

        public void InitializeLocal(int id, int controllerIndex, Camera.Viewer viewer, Bird bird, BirdTypeData birdType)
        {
            _controllerIndex = controllerIndex;
            _viewer = viewer;

            Debug.Log($"Initializing local player {id}");
            Initialize(id, bird, birdType);

            Viewer.Initialize(this);

            Viewer.FollowCamera.SetTarget(this);

            // ouch.... why not just render everything except for OtherRenderLayer ?
            Viewer.AddRenderLayer(Bird.Type.PlayerLayer);
            Viewer.AddRenderLayer(Bird.Type.OtherPlayerLayer);
            Viewer.AddRenderLayer(Bird.Type.RenderLayer);
            Viewer.RemoveRenderLayer(Bird.Type.OtherRenderLayer);
            Viewer.AddRenderLayer(GameManager.Instance.ObstacleLayer);
        }

        public void InitializeNetwork(int id, Bird bird, BirdTypeData birdType)
        {
            Debug.Log($"Initializing network player {id}");
            Initialize(id, bird, birdType);
        }

        private void Initialize(int id, Bird bird, BirdTypeData birdType)
        {
            Initialize(id);

            name = $"Player {Id}";
            Bird = bird;

            Bird.Initialize(this, birdType);
            Controller.Initialize(this);
            State.Initialize();

            _godRay = Instantiate(PlayerManager.Instance.PlayerGodRayPrefab, transform);
            _godRay.SetupPlayer(this);

            LogInfo();
        }

        public void Spawned()
        {
            Bird.PlaySpawnAudio();
        }

        public void Died()
        {
            StopAllCoroutines();

            Bird.StopAllCoroutines();
        }

        public void Redirect(Vector3 velocity)
        {
            Controller.Redirect(velocity);
        }

        private IEnumerator UpdateNearestPlayers()
        {
            WaitForSeconds wait = new WaitForSeconds(PlayerManager.Instance.NearestPlayerUpdateMs / 1000.0f);
            while(true) {
                yield return wait;

                NearestPredator = PlayerManager.Instance.GetNearestPredator(this, out _nearestPredatorDistance);
                NearestPrey = PlayerManager.Instance.GetNearestPrey(this, out _nearestPreyDistance);
            }
        }

        private IEnumerator UpdateNearestGoal()
        {
            WaitForSeconds wait = new WaitForSeconds(PlayerManager.Instance.NearestPlayerUpdateMs / 1000.0f);
            while(true) {
                yield return wait;

                NearestGoal = GoalManager.Instance.GetNearestGoal(this, out _nearestGoalDistance);
            }
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
