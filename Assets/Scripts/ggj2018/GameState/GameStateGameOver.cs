using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.Game.Audio;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.GameState
{
    public sealed class GameStateGameOver : pdxpartyparrot.Game.State.GameState
    {
        [SerializeField]
        private pdxpartyparrot.Game.State.GameState _startGameStatePrefab;

        [SerializeField]
        private AudioClip _gameOverMusicAudioClip;

        [SerializeField]
        [ReadOnly]
        private float _gameOverTimer;

        public override void OnEnter()
        {
            base.OnEnter();

            foreach(Player player in PlayerManager.Instance.Players) {
                player.Viewer.PlayerUI.PlayerUIPage.SwitchToGameOver(player, GameManager.Instance.GameType);
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
                    GameStateManager.Instance.TransitionState(_startGameStatePrefab, state => {
                        (state as GameStateStart)?.Initialize(GameManager.Instance.EnableDemoMode);
                    });
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
