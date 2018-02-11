using System;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;
using ggj2018.ggj2018.World;
using ggj2018.Game.Audio;

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

        public bool CanScore => States.Game == State;

        [SerializeField]
        [ReadOnly]
        private float _gameTimer;

        public float GameTimer => _gameTimer;

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
            switch(State)
            {
                case States.Init:
                    FinishInit();
                    break;
                case States.Menu:
                    FinishMenu();
                    break;
                case States.CharacterSelect:
                    FinishCharacterSelect();
                    break;
                case States.Game:
                    FinishGame();
                    break;
                case States.GameOver:
                    FinishGameOver();
                    break;
            }
        }

#region Init State
        private void BeginInit()
        {
            PlayerManager.Instance.DespawnAllPlayers();

            SpawnManager.Instance.ReleaseSpawnPoints();

            CameraManager.Instance.ResetViewers();

            PlayerManager.Instance.ResetCharacterSelect();
        }

        private void RunInit()
        {
            SetState(States.Menu);
        }

        private void FinishInit()
        {
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

        private void FinishMenu()
        {
        }
#endregion

#region Character Select State
        private void BeginCharacterSelect()
        {
            UIManager.Instance.SwitchToCharacterSelect();

            AudioManager.Instance.PlayMusic(GameManager.Instance.CharacterSelectMusicClip);
        }

        private void RunCharacterSelect()
        {
            // TODO: do this in a way that we don't have to loop every frame
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

            // TODO: move the guts of this loop into CharacterSelectState
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
            }
        }

        private void FinishCharacterSelect()
        {
            AudioManager.Instance.StopMusic();
        }
#endregion

#region Game State
        private void DetermineGameType()
        {
// TODO: player manager already has these counts, right?
// if we can just determine the game type later in the process,
// after we spawn the players then this can go away
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

            // TODO: move the guts of this loop into CharacterSelectState
            foreach(CharacterSelectState selectState in PlayerManager.Instance.CharacterSelectStates) {
                if(selectState.IsReady) {
                    selectState.Player = PlayerManager.Instance.SpawnPlayer(GameType.Type, selectState);
                } else if(DebugManager.Instance.SpawnMaxLocalPlayers) {
                    selectState.SelectedBird = selectState.ControllerIndex % GameManager.Instance.BirdData.Birds.Count;
                    selectState.Player = PlayerManager.Instance.SpawnPlayer(GameType.Type, selectState);
                } else {
                    selectState.Player = null;
                    CameraManager.Instance.ReleaseViewer(selectState.Viewer);
                    // TODO: release controller?
                }
            }

            CameraManager.Instance.ResizeViewports();

            UIManager.Instance.SwitchToGame(_gameType);

            AudioManager.Instance.PlayMusic(GameManager.Instance.GameMusic1AudioClip, GameManager.Instance.GameMusic2AudioClip);

            _gameTimer = GameType.GameTypeData.TimeLimit * 60.0f;
        }

        private void RunGame(float dt)
        {
            if(IsPaused) {
                return;
            }

            if(GameType.GameTypeData.TimeLimit > 0) {
                _gameTimer -= dt;
                if(_gameTimer <= 0.0f) {
                    _gameTimer = 0.0f;

                    GameType.TimerFinish();
                    SetState(States.GameOver);
                }
            }

            GameType.Update();
        }

        private void FinishGame()
        {
            AudioManager.Instance.StopMusic();
        }
#endregion

#region Game Over State
        private void BeginGameOver()
        {
            UIManager.Instance.SwitchToGameOver(GameType);

            AudioManager.Instance.PlayMusic(GameManager.Instance.GameOverMusicAudioClip);

            _gameOverTimer = GameManager.Instance.GameTypeData.GameOverWaitTime;
        }

        private void RunGameOver(float dt)
        {
            if(IsPaused) {
                return;
            }

            _gameOverTimer -= dt;
            if(_gameOverTimer <= 0.0f) {
                _gameOverTimer = 0.0f;

                if(GameManager.Instance.GameTypeData.RestartOnGameOver) {
                    SetState(States.Init);
                }
            }
        }

        private void FinishGameOver()
        {
            AudioManager.Instance.StopMusic();
        }
#endregion
    }
}
