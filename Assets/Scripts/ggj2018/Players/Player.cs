using System.Collections;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Camera;
using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.VFX;
using ggj2018.ggj2018.World;
using ggj2018.Game.Audio;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.ggj2018.Players
{
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
        [ReadOnly]
        private  PlayerState _playerState;

        public PlayerState State => _playerState;

        private GodRay _godRay;

        private PlayerController _controller;

        public PlayerController Controller => _controller;

        public Bird Bird { get; private set; }

        public Color PlayerColor => PlayerManager.Instance.PlayerData.GetPlayerColor(Id);

#region Input
        [SerializeField]
        [ReadOnly]
        private int _controllerIndex;

        public int ControllerIndex => _controllerIndex;

        public Vector3 LookAxis => InputManager.Instance.GetLookAxes(ControllerIndex);
#endregion

        public bool IsPaused => GameManager.Instance.State.IsPaused;

#region Camera
        [SerializeField]
        [ReadOnly]
        private Viewer _viewer;

        public Viewer Viewer => _viewer;
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

#region Unity Lifecycle
        private void Awake()
        {
            _playerState = new PlayerState(this);

            _controller = GetComponent<PlayerController>();
        }

        private void OnDestroy()
        {
            Destroy(_godRay);
            _godRay = null;
        }

        private void Start()
        {
            StartCoroutine(UpdateNearestPlayers());
            StartCoroutine(UpdateNearestGoal());
            StartCoroutine(PlayFlightAnimation());
        }

        private void Update()
        {
            _playerState.Update(Time.deltaTime);

            Viewer.PlayerUI.PlayerHUD.SetState(this);
        }
#endregion

        public void InitializeLocal(int id, int controllerIndex, Viewer viewer, Bird bird, BirdTypeData birdType)
        {
            Debug.Log($"Initializing local player {id}");
            Initialize(id, bird, birdType);

            _controllerIndex = controllerIndex;

            _viewer = viewer;
            Viewer.Initialize(this);

            Viewer.FollowCamera.SetTarget(gameObject);

            Viewer.AddRenderLayer(Bird.Type.Layer);
            Viewer.RemoveRenderLayer(Bird.Type.OtherLayer);
        }

        public void InitializeNetwork(int id, Bird bird, BirdTypeData birdType)
        {
            Debug.Log($"Initializing network player {id}");
            Initialize(id, bird, birdType);
        }

        private void Initialize(int id, Bird bird, BirdTypeData birdType)
        {
            _id = id;
            name = $"Player {Id}";
            Bird = bird;

            Bird.Initialize(this, birdType);
            _controller.Initialize(this);
            State.Initialize();

            _godRay = Instantiate(PlayerManager.Instance.PlayerGodRayPrefab, transform);
            _godRay.SetupPlayer(this);

            LogInfo();
        }

        public void Spawned()
        {
            AudioManager.Instance.PlayAudioOneShot(Bird.Type.SpawnAudioClip);
        }

        public void Died()
        {
            StopAllCoroutines();
        }

        public void Redirect(Vector3 velocity)
        {
            _controller.Redirect(velocity);
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

        private IEnumerator PlayFlightAnimation()
        {
            System.Random random = new System.Random();
            while(true) {
                // TODO: animate

                AudioManager.Instance.PlayAudioOneShot(Bird.Type.FlightAudioClip);

                float wait = random.NextSingle(PlayerManager.Instance.PlayerData.MinFlightAnimationCooldown, PlayerManager.Instance.PlayerData.MaxFlightAnimationCooldown);
                yield return new WaitForSeconds(wait);
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
