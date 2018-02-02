using System;
using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [Serializable]
    public sealed class GameState
    {
        public enum States
        {
            Init,
            Menu,
            CharacterSelect,
            Game,
            Victory
        }

        [SerializeField]
        [ReadOnly]
        private GameType _gameType;

        public GameType GameType => _gameType;

        [SerializeField]
        [ReadOnly]
        private States _state = States.Init;

        public States State { get { return _state; } private set { _state = value; } }

// TODO: unused? bring intro back?
        [SerializeField]
        [ReadOnly]
        private int _countdown;

        [SerializeField]
        [ReadOnly]
        private int _winner;

        public int Winner { get { return _winner; } set { _winner = value; } }

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

        private TimeSpan _timer;

        public TimeSpan Timer => _timer;

        public void Update(float dt)
        {
            switch(State)
            {
            case States.Menu:
                RunMenu();
                break;
            case States.CharacterSelect:
                RunCharacterSelect();
                break;
            case States.Game:
                RunGame(dt);
                break;
            case States.Victory:
                RunVictory();
                break;
            }
        }

        public void SetState(States state)
        {
            FinishState();

            State = state;
            switch(State)
            {
            case States.Menu:
                BeginMenu();
                break;
            case States.CharacterSelect:
                BeginCharacterSelect();
                break;
            case States.Game:
                BeginGame();
                break;
            case States.Victory:
                BeginVictory();
                break;
            }

            Debug.Log($"State: {State}");
        }

        private void FinishState()
        {
// TODO
        }

#region Menu State
        private void BeginMenu()
        {
        }

        private void RunMenu()
        {
            SetState(States.CharacterSelect);
        }
#endregion

#region Menu State
        private void BeginCharacterSelect()
        {
        }

        private void RunCharacterSelect()
        {
            // Check for all players ready
            int ready = 0, joined = 0;
            for(int i=0; i<PlayerManager.Instance.CharacterSelectStates.Count; ++i) {
                PlayerManager.CharacterSelectState playerState = PlayerManager.Instance.CharacterSelectStates.ElementAt(i);

                if(playerState.IsReady) {
                    ++ready;
                    ++joined;
                } else if(playerState.IsJoined) {
                    ++joined;
                }
            }

            if(ready == joined && ready > 0 && InputManager.Instance.StartPressed()) {
                SetState(States.Game);
                return;
            }

            // Check for player joins
            for(int i=0; i<PlayerManager.Instance.CharacterSelectStates.Count; ++i) {
                PlayerManager.CharacterSelectState playerState = PlayerManager.Instance.CharacterSelectStates.ElementAt(i);

                if(playerState.IsReady) {
                    if(InputManager.Instance.Pressed(i, InputManager.Button.B)) {
                        playerState.PlayerJoinState = PlayerManager.CharacterSelectState.JoinState.Joined;
                    }
                } else if(playerState.IsJoined) {
                    if(InputManager.Instance.Pressed(i, InputManager.Button.A)) {
                        playerState.PlayerJoinState = PlayerManager.CharacterSelectState.JoinState.Ready;
                    } else if(InputManager.Instance.Pressed(i, InputManager.Button.B)) {
                        playerState.PlayerJoinState = PlayerManager.CharacterSelectState.JoinState.None;
                    } else {
                        if(InputManager.Instance.DpadPressed(i, InputManager.DPadDir.Right)) {
                            playerState.NextBird();
                        } else if(InputManager.Instance.DpadPressed(i, InputManager.DPadDir.Left)) {
                            playerState.PrevBird();
                        }
                    }
                } else {
                    if(InputManager.Instance.PositivePressed(i)) {
                        playerState.PlayerJoinState = PlayerManager.CharacterSelectState.JoinState.Joined;
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
        private void BeginIntro()
        {
            for(int i=0; i<PlayerManager.Instance.PlayerStates.Count; ++i) {
                PlayerManager.PlayerState playerState = PlayerManager.Instance.PlayerStates.ElementAt(i);
                if(playerState.IsReady) {
                    PlayerManager.Instance.SpawnLocalPlayer(i, playerState.PlayerBirdId);
                }
            }

            // $$$ making this instant for now
            UIManager.Instance.HideMenu();
            _countdown = 3;
            _timer = 1.0f;
            UIManager.Instance.Countdown(_countdown);
        }

        private void RunIntro()
        {
            SetState(States.Game);

            // $$$ making this instant for now
            _timer -= Time.deltaTime;
            if(_timer < 0) {
                --_countdown;
                _timer = 1.0f;

                if(_countdown == 0) {
                    SetState(States.Game);
                } else {
                    UIManager.Instance.Countdown(_countdown);
                }
            }
        }
*/
#endregion

#region Game State
        private void DetermineGameType()
        {
// TODO: player manager already has these counts
// if we can just determine the game type later in the process,
// after we spawn the players
            int playerCount = 0;
            int predatorCount = 0;
            int preyCount = 0;

            for(int i=0; i<PlayerManager.Instance.CharacterSelectStates.Count; ++i) {
                PlayerManager.CharacterSelectState playerState = PlayerManager.Instance.CharacterSelectStates.ElementAt(i);
                if(playerState.IsReady) {
                    playerCount++;
                }

                if(playerState.PlayerBirdData.IsPredator) {
                    predatorCount++;
                } else {
                    preyCount++;
                }
            }

            _gameType = GameType.GetGameType(playerCount, predatorCount, preyCount);
        }

        private void BeginGame()
        {
            DetermineGameType();
            Debug.Log($"Beginning game type {GameType.Type}");

            for(int i=0; i<PlayerManager.Instance.CharacterSelectStates.Count; ++i) {
                PlayerManager.CharacterSelectState playerState = PlayerManager.Instance.CharacterSelectStates.ElementAt(i);
                if(playerState.IsReady) {
                    PlayerManager.Instance.SpawnPlayer(GameType.Type, playerState.PlayerBirdId);
                    CameraManager.Instance.EnableViewer(i, true);
                } else {
                    CameraManager.Instance.EnableViewer(i, false);
                }
            }

            CameraManager.Instance.ResizeViewports();

            UIManager.Instance.SwitchToGame(_gameType);

            SpawnManager.Instance.ReleaseSpawnPoints();

            _timer = GameType.GameTypeData.TimeLimit > 0
                ? TimeSpan.FromMinutes(GameType.GameTypeData.TimeLimit)
                : TimeSpan.Zero;
        }

        private void RunGame(float dt)
        {
            if(IsPaused) {
                return;
            }

            if(GameType.GameTypeData.TimeLimit > 0) {
                _timer = _timer.Subtract(TimeSpan.FromSeconds(dt));
            }
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
#endregion
    }
}
