using System;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;
using ggj2018.ggj2018.World;

using UnityEngine;

namespace ggj2018.ggj2018.Game
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
            GameOver
        }

// TODO: GameState === GameStateMachine
// States == GameStateNode
// Split into classes rather than switching a bunch

        [SerializeField]
        [ReadOnly]
        private GameType _gameType;

        public GameType GameType => _gameType;

        [SerializeField]
        [ReadOnly]
        private States _state = States.Init;

        public States State { get { return _state; } private set { _state = value; } }

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

        public bool CanPause => States.Game == State || States.GameOver == State;

        public TimeSpan Timer { get; private set; }

        [SerializeField]
        [ReadOnly]
        private float _gameOverTimer;

        public void Update(float dt)
        {
            switch(State)
            {
            case States.Init:
                RunInit();
                break;
            case States.Menu:
                RunMenu();
                break;
            case States.CharacterSelect:
                RunCharacterSelect();
                break;
            case States.Game:
                RunGame(dt);
                break;
            case States.GameOver:
                RunGameOver(dt);
                break;
            }
        }

        public void SetState(States state)
        {
            FinishState();

            State = state;
            switch(State)
            {
            case States.Init:
                BeginInit();
                break;
            case States.Menu:
                BeginMenu();
                break;
            case States.CharacterSelect:
                BeginCharacterSelect();
                break;
            case States.Game:
                BeginGame();
                break;
            case States.GameOver:
                BeginGameOver();
                break;
            }

            Debug.Log($"State: {State}");
        }

        private void FinishState()
        {
// TODO
        }

#region Init State
        public void BeginInit()
        {
            PlayerManager.Instance.DespawnAllPlayers();

            SpawnManager.Instance.ReleaseSpawnPoints();

            CameraManager.Instance.ResetViewers();

            PlayerManager.Instance.ResetCharacterSelect();
        }

        public void RunInit()
        {
            SetState(States.Menu);
        }
#endregion

#region Menu State
        private void BeginMenu()
        {
        }

        private void RunMenu()
        {
            SetState(States.CharacterSelect);
        }
#endregion

#region Character Select State
        private void BeginCharacterSelect()
        {
            UIManager.Instance.SwitchToCharacterSelect();
        }

        private void RunCharacterSelect()
        {
            // Check for all players ready
            int ready = 0, joined = 0;
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                if(selectState.IsReady) {
                    ++ready;
                    ++joined;
                } else if(selectState.IsJoined) {
                    ++joined;
                }
            }

            if(ready == joined && ready > 0 && InputManager.Instance.StartPressed()) {
                if(!GameManager.Instance.NetworkManager.IsClientConnected()) {
                    Debug.LogError("Need to connect to a server!");
                } else {
                    SetState(States.Game);
                    return;
                }
            }

            // Check for player joins
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                if(selectState.IsReady) {
                    if(InputManager.Instance.Pressed(selectState.ControllerIndex, InputManager.Button.B)) {
                        selectState.PlayerJoinState = CharacterSelectState.JoinState.Joined;
                    }
                } else if(selectState.IsJoined) {
                    if(InputManager.Instance.Pressed(selectState.ControllerIndex, InputManager.Button.A)) {
                        selectState.PlayerJoinState = CharacterSelectState.JoinState.Ready;
                    } else if(InputManager.Instance.Pressed(selectState.ControllerIndex, InputManager.Button.B)) {
                        selectState.PlayerJoinState = CharacterSelectState.JoinState.None;
                    } else {
                        if(InputManager.Instance.DpadPressed(selectState.ControllerIndex, InputManager.DPadDir.Right)) {
                            selectState.NextBird();
                        } else if(InputManager.Instance.DpadPressed(selectState.ControllerIndex, InputManager.DPadDir.Left)) {
                            selectState.PrevBird();
                        }
                    }
                } else {
                    if(InputManager.Instance.PositivePressed(selectState.ControllerIndex)) {
                        selectState.PlayerJoinState = CharacterSelectState.JoinState.Joined;
                        selectState.SelectedBird = 0;
                    }
                }

                selectState.Viewer.PlayerUI.SetStatus(selectState, ready == joined);

                UIManager.Instance.SwitchToCharacterSelect();
            }

        }
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

            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                if(selectState.IsReady) {
                    playerCount++;
                }

                if(selectState.PlayerBirdData.IsPredator) {
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

            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                if(selectState.IsReady) {
                    selectState.Player = PlayerManager.Instance.SpawnPlayer(GameType.Type, selectState);
                } else {
                    selectState.Player = null;
                    CameraManager.Instance.ReleaseViewer(selectState.Viewer);
                    // TODO: release controller?
                }
            }

            CameraManager.Instance.ResizeViewports();

            UIManager.Instance.SwitchToGame(_gameType);

            Timer = GameType.GameTypeData.TimeLimit > 0
                ? TimeSpan.FromMinutes(GameType.GameTypeData.TimeLimit)
                : TimeSpan.Zero;
        }

        private void RunGame(float dt)
        {
            if(IsPaused) {
                return;
            }

            if(GameType.GameTypeData.TimeLimit > 0) {
                Timer = Timer.Subtract(TimeSpan.FromSeconds(dt));
                if(Timer.Seconds <= 0) {
                    Timer = TimeSpan.Zero;
                    GameType.TimerFinish();
                    SetState(States.GameOver);
                }
            }

            GameType.Update();
        }
#endregion

#region Game Over State
        private void BeginGameOver()
        {
            UIManager.Instance.SwitchToGameOver(GameType);

            _gameOverTimer = GameManager.Instance.GameTypeData.GameOverWaitTime;
        }

        private void RunGameOver(float dt)
        {
            _gameOverTimer -= dt;
            if(_gameOverTimer <= 0.0f) {
                _gameOverTimer = 0.0f;

                if(GameManager.Instance.GameTypeData.RestartOnGameOver) {
                    SetState(States.Init);
                }
            }
        }
#endregion
    }
}
