using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.ggj2018.UI;
using pdxpartyparrot.ggj2018.World;
using pdxpartyparrot.Game.Audio;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.GameState
{
    public sealed class GameStateStart : pdxpartyparrot.Game.State.GameState
    {
        [SerializeField]
        private pdxpartyparrot.Game.State.GameState _characterSelectGameStatePrefab;

        [SerializeField]
        private AudioClip _startupLogoMusicClip;

        private bool _showStartup = true;

        // NOTE: this is *not* called the first time we enter this state
        public void Initialize(bool showStartup)
        {
            _showStartup = showStartup;
        }

        public override void OnEnter()
        {
            base.OnEnter();

#region Reset Managers
            PlayerManager.Instance.DespawnAllPlayers();

            SpawnManager.Instance.ReleaseSpawnPoints();

            CameraManager.Instance.ResetViewers();

            InputManager.Instance.Reset();
#endregion

            if(!_showStartup) {
                GameStateManager.Instance.TransitionState(_characterSelectGameStatePrefab);
                return;
            }

            UIManager.Instance.EnableStartupLogo(true);

            AudioManager.Instance.PlayMusic(_startupLogoMusicClip);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if(InputManager.Instance.StartPressed()) {
                GameStateManager.Instance.TransitionState(_characterSelectGameStatePrefab);
            }
        }

        public override void OnExit()
        {
            if(AudioManager.HasInstance) {
                AudioManager.Instance.StopMusic();
            }

            if(UIManager.HasInstance) {
                UIManager.Instance.EnableStartupLogo(false);
            }

            base.OnExit();
        }
    }
}
