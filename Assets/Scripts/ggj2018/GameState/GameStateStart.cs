using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;
using ggj2018.ggj2018.World;
using ggj2018.Game.Audio;
using ggj2018.Game.Data;
using ggj2018.Game.State;

using UnityEngine;

namespace ggj2018.ggj2018.GameState
{
    public sealed class GameStateStart : global::ggj2018.Game.State.GameState
    {
        [SerializeField]
        private GameStateData _characterSelectGameStateData;

        public override void OnEnter()
        {
            base.OnEnter();

#region Reset Managers
            PlayerManager.Instance.DespawnAllPlayers();

            SpawnManager.Instance.ReleaseSpawnPoints();

            CameraManager.Instance.ResetViewers();

            PlayerManager.Instance.ResetCharacterSelect();

            InputManager.Instance.Reset();
#endregion

            UIManager.Instance.EnableStartupLogo(true);

// TODO: move the audio clip to this object
            AudioManager.Instance.PlayMusic(GameManager.Instance.StartupLogoMusicClip);
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
