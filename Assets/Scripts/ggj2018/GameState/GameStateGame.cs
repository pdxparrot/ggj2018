using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
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
        private AudioClip _gameMusic1AudioClip;

        [SerializeField]
        private AudioClip _gameMusic2AudioClip;

        [SerializeField]
        [ReadOnly]
        private float _gameTimer;

        public float GameTimer => _gameTimer;

        public override void OnEnter()
        {
            base.OnEnter();

            CameraManager.Instance.ResizeViewports();

            AudioManager.Instance.PlayMusic(_gameMusic1AudioClip, _gameMusic2AudioClip);

            _gameTimer = GameManager.Instance.GameType.GameTypeData.TimeLimit * 60.0f;
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if(InputManager.Instance.StartPressed()) {
                GameManager.Instance.TogglePause();
            }

            if(GameManager.Instance.IsPaused) {
                return;
            }

            if(GameManager.Instance.GameType.GameTypeData.TimeLimit > 0) {
                _gameTimer -= dt;
                if(_gameTimer <= 0.0f) {
                    _gameTimer = 0.0f;

                    GameManager.Instance.GameType.TimerFinish();

                    GameStateManager.Instance.TransitionState(_gameOverGameStateData);
                }
            }

            GameManager.Instance.GameType.Update();
            if(GameManager.Instance.GameType.IsGameOver) {
                GameStateManager.Instance.TransitionState(_gameOverGameStateData);
            }
        }

        public override void OnExit()
        {
            if(AudioManager.HasInstance) {
                AudioManager.Instance.StopMusic();
            }

            base.OnExit();
        }
    }
}
