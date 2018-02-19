using ggj2018.Core.Camera;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;
using ggj2018.Game.Audio;
using ggj2018.Game.Data;
using ggj2018.Game.State;

using UnityEngine;

namespace ggj2018.ggj2018.GameState
{
    public sealed class GameStateGame : global::ggj2018.Game.State.GameState
    {
        [SerializeField]
        private GameStateData _gameOverGameStateData;

        [SerializeField]
        [ReadOnly]
        private GameType _gameType;

        public GameType GameType => _gameType;

        [SerializeField]
        [ReadOnly]
        private float _gameTimer;

        public float GameTimer => _gameTimer;

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

        public override void OnEnter()
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

// TODO: move the audio clips to this object
            AudioManager.Instance.PlayMusic(GameManager.Instance.GameMusic1AudioClip, GameManager.Instance.GameMusic2AudioClip);

            _gameTimer = GameType.GameTypeData.TimeLimit * 60.0f;
        }

        public override void OnUpdate(float dt)
        {
            if(IsPaused) {
                return;
            }

            if(GameType.GameTypeData.TimeLimit > 0) {
                _gameTimer -= dt;
                if(_gameTimer <= 0.0f) {
                    _gameTimer = 0.0f;

                    GameType.TimerFinish();

                    GameStateManager.Instance.TransitionState(_gameOverGameStateData, gameState =>
                    {
                        (gameState as GameStateGameOver)?.Initialize(_gameType);
                    });
                }
            }

            GameType.Update();
        }

        public override void OnExit()
        {
            AudioManager.Instance.StopMusic();
        }

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
    }
}
