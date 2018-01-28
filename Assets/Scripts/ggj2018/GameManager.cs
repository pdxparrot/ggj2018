using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GameManager : SingletonBehavior<GameManager>
    {
        [SerializeField]
        public const int MaxPlayers = 4;

        [SerializeField]
        private BirdData _birdData;

        public BirdData BirdData => _birdData;

        public void Initialize()
        {
            _birdData.Initialize();
        }
        // Game State
        public enum EState {
            eMenu,
            eIntro,
            eGame,
            eVictory,
        }
        public EState State { get; private set; }

        void SetState(EState state) {
            State = state;
            switch(State) {
            case EState.eMenu:      BeginMenu();    break;
            case EState.eIntro:     BeginIntro();   break;
            case EState.eGame:      BeginGame();    break;
            case EState.eVictory:   BeginVictory(); break;
            }

            Debug.Log($"State: {State}");
        }

        void Update() {
            switch(State) {
            case EState.eMenu:      RunMenu();      break;
            case EState.eIntro:     RunIntro();     break;
            case EState.eGame:      RunGame();      break;
            case EState.eVictory:   RunVictory();   break;
            }
        }

        // Local Variables
        int _countdown;
        float _timer;


        // Player bits
        [SerializeField] PlayerUIPage[] playerHud;
        bool[] _playerJoined = new bool[MaxPlayers];
        bool[] _playerReady = new bool[MaxPlayers];
        int[] _playerBird = new int[MaxPlayers];


        // Bird Logic
        const int NumBirds = 4;
        void DefaultBird(int player) {
            _playerBird[player] = player;
            if(!ValidBird(player))
                NextBird(player);
        }
        void NextBird(int player) {
            do {
                ++_playerBird[player];
                WrapBird(player);
            } while(ValidBird(player));
        }
        void PrevBird(int player) {
            do {
                --_playerBird[player];
                WrapBird(player);
            } while(ValidBird(player));
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
            if(ready == total) {// && ready > 2) {
                for(int i = 0; i < MaxPlayers; ++i)
                    if(InputManager.Instance.Pressed(i, 0) ||
                       InputManager.Instance.StartPressed(i)) 
                        SetState(EState.eIntro);
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
                UIManager.Instance.
                PlayerHud[i].SetStatus(_playerJoined[i],
                                       _playerReady[i],
                                       _playerBird[i]);
            }

        }

        // Intro State
        void BeginIntro() {
            for(int i = 0; i < MaxPlayers; ++i)
                if(_playerReady[i])
                    PlayerManager.Instance.SpawnLocalPlayer(i, BirdType(_playerBird[i]));

            UIManager.Instance.HideMenu();
            _countdown = 3;
            _timer = 1.0f;
            UIManager.Instance.Countdown(_countdown);
        }
        void RunIntro() {
            _timer -= Time.deltaTime;
            if(_timer < 0) {
                --_countdown;
                _timer = 1.0f;

                if(_countdown == 0)
                    SetState(EState.eGame);
                else
                    UIManager.Instance.Countdown(_countdown);
            }
        }

        // Game State
        void BeginGame() {
            UIManager.Instance.HideCountdown();
        }
        void RunGame() {
        }

        // Victory State
        void BeginVictory() {
        }
        void RunVictory() {
        }
    }
}

