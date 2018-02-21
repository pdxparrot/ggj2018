using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.ggj2018.UI;
using pdxpartyparrot.ggj2018.World;
using pdxpartyparrot.Game.Audio;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.GameState
{
    public sealed class GameStateStart : pdxpartyparrot.Game.State.GameState
    {
        [SerializeField]
        private GameStateData _characterSelectGameStateData;

        [SerializeField]
        private AudioClip _startupLogoMusicClip;

        public override void OnEnter()
        {
            base.OnEnter();

#region Reset Managers
            PlayerManager.Instance.DespawnAllPlayers();

            SpawnManager.Instance.ReleaseSpawnPoints();

            CameraManager.Instance.ResetViewers();

            InputManager.Instance.Reset();
#endregion

            UIManager.Instance.EnableStartupLogo(true);

            AudioManager.Instance.PlayMusic(_startupLogoMusicClip);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if(InputManager.Instance.StartPressed()) {
                GameStateManager.Instance.TransitionState(_characterSelectGameStateData);
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
