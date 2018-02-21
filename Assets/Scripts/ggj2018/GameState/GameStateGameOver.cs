using ggj2018.Core.Input;
using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;
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
        private AudioClip _gameOverMusicAudioClip;

        [SerializeField]
        [ReadOnly]
        private float _gameOverTimer;

        public override void OnEnter()
        {
            base.OnEnter();

            foreach(Player player in PlayerManager.Instance.Players) {
                player.Viewer.PlayerUI.SwitchToGameOver(player, GameManager.Instance.GameType);
            }

            AudioManager.Instance.PlayMusic(_gameOverMusicAudioClip);

            _gameOverTimer = GameManager.Instance.GameTypeData.GameOverWaitTime;
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
            if(AudioManager.HasInstance) {
                AudioManager.Instance.StopMusic();
            }

            base.OnExit();
        }
    }
}
