using System;
using System.Collections;

using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.Core.Camera;
using ggj2018.ggj2018.Data;
using ggj2018.Game.Scenes;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GameManager : SingletonBehavior<GameManager>
    {
#region Events
        public event EventHandler<EventArgs> PauseEvent;
#endregion

        [SerializeField]
        private int _maxPlayers = 4;

        public int MaxPlayers => _maxPlayers;

        [SerializeField]
        private EnvironmentData _environmentData;

        public EnvironmentData EnvironmentData => _environmentData;

        [SerializeField]
        private BirdData _birdData;

        public BirdData BirdData => _birdData;

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused => _isPaused;

#region Unity Lifecycle
        private void Awake()
        {
            _playerJoined = new bool[MaxPlayers];
            _playerReady = new bool[MaxPlayers];
            _playerBird = new int[MaxPlayers];
        }

        private void Update()
        {
            CheckPause();
            CheckReload();

            switch(State) {
            case EState.eMenu:      RunMenu();      break;
            //case EState.eIntro:     RunIntro();     break;
            case EState.eGame:      RunGame();      break;
            case EState.eVictory:   RunVictory();   break;
            }
        }
#endregion

        public void Initialize()
        {
            _birdData.Initialize();

            CameraManager.Instance.SpawnViewers(MaxPlayers);
        }

        private void CheckPause()
        {
            if(State != EState.eGame || !InputManager.Instance.StartPressed()) {
                return;
            }

            _isPaused = !_isPaused;

            UIManager.Instance.EnablePauseUI(_isPaused);

            PauseEvent?.Invoke(this, EventArgs.Empty);
        }

        private void CheckReload()
        {
            if(!IsPaused) {
                return;
            }

            if(!InputManager.Instance.SelectPressed()) {
                return;
            }

#if false
            Debug.Log("Restarting game!");
            GameSceneManager.Instance.ReloadMainScene();
#else
            Debug.Log("Quitting game!");
            Application.Quit();
#endif
        }

        // Game State
        public enum EState {
            eMenu,
            //eIntro,
            eGame,
            eVictory,
        }
        public EState State { get; private set; }

        public void SetState(EState state) {
            State = state;
            switch(State) {
            case EState.eMenu:      BeginMenu();    break;
            //case EState.eIntro:     BeginIntro();   break;
            case EState.eGame:      BeginGame();    break;
            case EState.eVictory:   BeginVictory(); break;
            }

            Debug.Log($"State: {State}");
        }

        // Local Variables
        int _countdown;
        float _timer;


        // Player bits
        bool[] _playerJoined;
        bool[] _playerReady;
        int[] _playerBird;


        // Bird Logic
        const int NumBirds = 4;
        void DefaultBird(int player) {
            _playerBird[player] = player;
            if(!ValidBird(player))
                NextBird(player);
        }
        void NextBird(int player) {
            //do {
                ++_playerBird[player];
                WrapBird(player);
            //} while(ValidBird(player));
        }
        void PrevBird(int player) {
            //do {
                --_playerBird[player];
                WrapBird(player);
            //} while(ValidBird(player));
        }
        bool ValidBird(int player) {
            // TODO: 'only one hawk' logic here
            return true;
        }
        void WrapBird(int player) {
            if(_playerBird[player] < 0)
                _playerBird[player] = NumBirds - 1;
            else if(_playerBird[player] >= NumBirds)
                _playerBird[player] = 0;
        }

        string BirdType(int i) {
            switch(i) {
            default:
            case 0: return "hawk";
            case 1: return "carrier1";
            case 2: return "carrier2";
            case 3: return "carrier3";
            }
        }

        // Menu State
        void BeginMenu() {
        }
        void RunMenu() {
            // Check for all players ready
            int ready = 0;
            int total = 0;
            for(int i = 0; i < MaxPlayers; ++i) {
                if(_playerReady[i])
                    ++ready;
                if(_playerJoined[i])
                    ++total;
            }
            if (ready == total && ready > 0) {
                for (int i = 0; i < MaxPlayers; ++i)
                    if (InputManager.Instance.StartPressed(i)) {
                        SetState(EState.eGame);
                        return;
                }
            }

            // Check for player joins
            for(int i = 0; i < MaxPlayers; ++i) {
                if(_playerReady[i]) {
                    if(InputManager.Instance.Pressed(i, 1))
                        _playerReady[i] = false;
                }
                else if(_playerJoined[i]) {
                    if(InputManager.Instance.Pressed(i, 0))
                        _playerReady[i] = true;

                    else if(InputManager.Instance.Pressed(i, 1))
                        _playerJoined[i] = false;
                    
                    else {
                        if(InputManager.Instance.DpadPressed(i, Dir.Left)) {
                            NextBird(_playerBird[i]);
                        }
                        else if(InputManager.Instance.DpadPressed(i, Dir.Right)) {
                            PrevBird(_playerBird[i]);
                        }
                    }
                }
                else {
                    if(InputManager.Instance.Pressed(i, 0) ||
                       InputManager.Instance.StartPressed(i)) {
                        _playerJoined[i] = true;
                        DefaultBird(i);
                    }
                }

                Viewer viewer = CameraManager.Instance.GetViewer(i) as Viewer;
                viewer?.PlayerUI.SetStatus(_playerJoined[i],
                                       _playerReady[i],
                                       BirdType(_playerBird[i]),
                                       ready == total);

                UIManager.Instance.SwitchToMenu();
            }

        }

        // Intro State
        /*
        void BeginIntro() {
            for(int i = 0; i < MaxPlayers; ++i)
                if(_playerReady[i])
                    PlayerManager.Instance.SpawnLocalPlayer(i, BirdType(_playerBird[i]));

            // $$$ making this instant for now
            UIManager.Instance.HideMenu();
            _countdown = 3;
            _timer = 1.0f;
            UIManager.Instance.Countdown(_countdown);
        }
        void RunIntro() {
            SetState(EState.eGame);

            // $$$ making this instant for now
            _timer -= Time.deltaTime;
            if(_timer < 0) {
                --_countdown;
                _timer = 1.0f;

                if(_countdown == 0)
                    SetState(EState.eGame);
                else
                    UIManager.Instance.Countdown(_countdown);
            }
        }*/

        bool SinglePlayer() {
            int players = 0;
            for(int i = 0; i < MaxPlayers; ++i)
                if(_playerReady[i])
                    ++players;
            return players == 1;
        }

        // Game State
        void BeginGame() {
            for(int i = 0; i < MaxPlayers; ++i) {
                if(_playerReady[i]) {
                    PlayerManager.Instance.SpawnLocalPlayer(i, BirdType(_playerBird[i]));
                }
            }

            UIManager.Instance.SwitchToGame();

            for(int i = 0; i < MaxPlayers; ++i) {
                CameraManager.Instance.SetupCamera(i, _playerReady[i]);
            }
            CameraManager.Instance.ResizeViewports();

            if(!SinglePlayer()) {
                StartCoroutine(CheckPredatorVictoryCondition());
            }
        }
        void RunGame() {
        }

        // Victory State
        void BeginVictory() {
            UIManager.Instance.SwitchToVictory(winner);
        }
        void RunVictory() {
        }

        private IEnumerator CheckPredatorVictoryCondition()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            while(true) {
                if(PlayerManager.Instance.PreyCount < 1) {
                    winner = PlayerManager.Instance.HawkIndex();
                    SetState(EState.eVictory);
                    yield break;
                }

                yield return wait;
            }
        }

        private int winner;

        public int Winner { get { return winner; } set { winner = value; } }
    }
}

