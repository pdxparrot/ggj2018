using System;
using System.Collections;
using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [Serializable]
    public sealed class GameState
    {
        public enum EState {
            eInit,
            eMenu,
            //eIntro,
            eGame,
            eVictory,
        }

        [SerializeField]
        [ReadOnly]
        private string _gameType;

        [SerializeField]
        [ReadOnly]
        private EState _state = EState.eInit;

        public EState State { get { return _state; } private set { _state = value; } }

        [SerializeField]
        [ReadOnly]
        private int _countdown;

        [SerializeField]
        [ReadOnly]
        private float _timer;

        [SerializeField]
        [ReadOnly]
        private int _winner;

        public int Winner { get { return _winner; } set { _winner = value; } }

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

        public void Update()
        {
            switch(State) {
            case EState.eMenu:      RunMenu();      break;
            //case EState.eIntro:     RunIntro();     break;
            case EState.eGame:      RunGame();      break;
            case EState.eVictory:   RunVictory();   break;
            }
        }

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

#region Menu State
        private void BeginMenu()
        {
        }

        private void RunMenu()
        {
            // Check for all players ready
            int ready = 0;
            int joined = 0;
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                PlayerManager.PlayerState playerState = PlayerManager.Instance.PlayerStates.ElementAt(i);

                if(playerState.IsReady) {
                    ++ready;
                    ++joined;
                } else if(playerState.IsJoined) {
                    ++joined;
                }
            }

            if(ready == joined && ready > 0 && InputManager.Instance.StartPressed()) {
                SetState(EState.eGame);
                return;
            }

            // Check for player joins
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                PlayerManager.PlayerState playerState = PlayerManager.Instance.PlayerStates.ElementAt(i);

                if(playerState.IsReady) {
                    if(InputManager.Instance.Pressed(i, InputManager.Button.B)) {
                        playerState.PlayerJoinState = PlayerManager.PlayerState.JoinState.Joined;
                    }
                } else if(playerState.IsJoined) {
                    if(InputManager.Instance.Pressed(i, InputManager.Button.A)) {
                        playerState.PlayerJoinState = PlayerManager.PlayerState.JoinState.Ready;
                    } else if(InputManager.Instance.Pressed(i, InputManager.Button.B)) {
                        playerState.PlayerJoinState = PlayerManager.PlayerState.JoinState.None;
                    } else {
                        if(InputManager.Instance.DpadPressed(i, InputManager.DPadDir.Right)) {
                            playerState.NextBird();
                        } else if(InputManager.Instance.DpadPressed(i, InputManager.DPadDir.Left)) {
                            playerState.PrevBird();
                        }
                    }
                } else {
                    if(InputManager.Instance.PositivePressed(i)) {
                        playerState.PlayerJoinState = PlayerManager.PlayerState.JoinState.Joined;
                        playerState.SelectedBird = 0;
                    }
                }

                Viewer viewer = CameraManager.Instance.Viewers.ElementAt(i) as Viewer;
                viewer?.PlayerUI.SetStatus(playerState, ready == joined);

                UIManager.Instance.SwitchToMenu();
            }

        }
#endregion

#region Intro State
        /*
        private void BeginIntro() {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i)
                if(PlayerManager.Instance.GetPlayerState(i).PlayerReady)
                    PlayerManager.Instance.SpawnLocalPlayer(i, BirdType(PlayerManager.Instance.GetPlayerState(i).PlayerBird));

            // $$$ making this instant for now
            UIManager.Instance.HideMenu();
            _countdown = 3;
            _timer = 1.0f;
            UIManager.Instance.Countdown(_countdown);
        }

        private void RunIntro() {
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
#endregion

#region Game State
        private bool SinglePlayer()
        {
            int players = 0;
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                if(PlayerManager.Instance.PlayerStates.ElementAt(i).IsReady) {
                    ++players;
                }
            }
            return players == 1;
        }

        private void BeginGame()
        {
            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                if(PlayerManager.Instance.PlayerStates.ElementAt(i).IsReady) {
                    PlayerManager.Instance.SpawnLocalPlayer(i, PlayerManager.Instance.PlayerStates.ElementAt(i).PlayerBirdId);
                }
            }

            UIManager.Instance.SwitchToGame();

            for(int i = 0; i < InputManager.Instance.MaxControllers; ++i) {
                CameraManager.Instance.SetupCamera(i, PlayerManager.Instance.PlayerStates.ElementAt(i).IsReady);
            }
            CameraManager.Instance.ResizeViewports();

            if(!SinglePlayer()) {
                GameManager.Instance.StartCoroutine(CheckPredatorVictoryCondition());
            }
        }

        private void RunGame()
        {
        }
#endregion

#region Victory State
        private void BeginVictory()
        {
            UIManager.Instance.SwitchToVictory(Winner);
        }

        private void RunVictory()
        {
        }

        private IEnumerator CheckPredatorVictoryCondition()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            while(true) {
                if(PlayerManager.Instance.PreyCount < 1) {
                    Winner = PlayerManager.Instance.HawkIndex();
                    SetState(EState.eVictory);
                    yield break;
                }

                yield return wait;
            }
        }
#endregion
    }
}
