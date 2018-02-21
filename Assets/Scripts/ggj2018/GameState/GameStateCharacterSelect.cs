using System.Collections.Generic;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.Game.Audio;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.GameState
{
    public sealed class GameStateCharacterSelect : pdxpartyparrot.Game.State.GameState
    {
        [SerializeField]
        private GameStateData _gameGameStateData;

        [SerializeField]
        private AudioClip _characterSelectMusicClip;

        private readonly List<CharacterSelectState> _characterSelectStates = new List<CharacterSelectState>();

        public override void OnEnter()
        {
            base.OnEnter();

            AudioManager.Instance.PlayMusic(_characterSelectMusicClip);

            for(int i=0; i<GameManager.Instance.ConfigData.MaxLocalPlayers; ++i) {
                int controllerIndex = InputManager.Instance.AcquireController();
                Debug.Log($"Acquired controller {controllerIndex}");

                CharacterSelectState selectState = new CharacterSelectState(controllerIndex);
                _characterSelectStates.Add(selectState);

                selectState.Viewer.PlayerUI.SwitchToCharacterSelect(selectState);
            }

            CameraManager.Instance.ResizeViewports();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            // TODO: do this in a way that we don't have to loop every frame
            int ready = 0, joined = 0;
            foreach(CharacterSelectState selectState in _characterSelectStates) {
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
                    GameStateManager.Instance.TransitionState(_gameGameStateData);
                    return;
                }
            }

            foreach(CharacterSelectState selectState in _characterSelectStates) {
                selectState.Update(ready == joined);
            }
        }

        public override void OnExit()
        {
            if(AudioManager.HasInstance) {
                AudioManager.Instance.StopMusic();
            }

            // TODO: this sucks
            DetermineGameType();
            Debug.Log($"Determined game type {GameManager.Instance.GameType.Type}");

            foreach(CharacterSelectState selectState in _characterSelectStates) {
                selectState.Finish();
            }

            base.OnExit();
        }

        private void DetermineGameType()
        {
// TODO: player manager already has these counts, right?
// if we can just determine the game type later in the process,
// after we spawn the players then this can go away
            int playerCount = 0;
            int predatorCount = 0;
            int preyCount = 0;

            foreach(CharacterSelectState selectState in _characterSelectStates) {
                if(selectState.IsReady) {
                    playerCount++;
                }

                if(selectState.PlayerBirdData.IsPredator) {
                    predatorCount++;
                } else {
                    preyCount++;
                }
            }

            GameManager.Instance.SetGameType(playerCount, predatorCount, preyCount);
        }
    }
}
