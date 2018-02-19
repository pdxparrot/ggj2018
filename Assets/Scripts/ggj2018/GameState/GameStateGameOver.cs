using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.UI;
using ggj2018.Game.Audio;
using ggj2018.Game.Data;
using ggj2018.Game.State;

using UnityEngine;

namespace ggj2018.ggj2018.GameState
{
    public sealed class GameStateGameOver : global::ggj2018.Game.State.GameState
    {
        [SerializeField]
        private GameStateData _startGameStateData;

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

        [SerializeField]
        [ReadOnly]
        private float _gameOverTimer;

        private GameType _gameType;

        public void Initialize(GameType gameType)
        {
            _gameType = gameType;
        }

        public override void OnEnter()
        {
            UIManager.Instance.SwitchToGameOver(_gameType);

// TODO: move the audio clip to this object
            AudioManager.Instance.PlayMusic(GameManager.Instance.GameOverMusicAudioClip);

            _gameOverTimer = GameManager.Instance.GameTypeData.GameOverWaitTime;
        }

        public override void OnUpdate(float dt)
        {
            if(IsPaused) {
                return;
            }

            _gameOverTimer -= dt;
            if(_gameOverTimer <= 0.0f) {
                _gameOverTimer = 0.0f;

                if(GameManager.Instance.GameTypeData.RestartOnGameOver) {
                    GameStateManager.Instance.TransitionState(_startGameStateData);
                }
            }
        }

        public override void OnExit()
        {
            AudioManager.Instance.StopMusic();
        }
    }
}
