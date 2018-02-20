using ggj2018.Core.Input;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;
using ggj2018.Game.Audio;
using ggj2018.Game.Data;
using ggj2018.Game.State;

using UnityEngine;

namespace ggj2018.ggj2018.GameState
{
    public sealed class GameStateCharacterSelect : global::ggj2018.Game.State.GameState
    {
        [SerializeField]
        private GameStateData _gameGameStateData;

// TODO: move the character select states to this object

        public override void OnEnter()
        {
            base.OnEnter();

            UIManager.Instance.SwitchToCharacterSelect();

// TODO: move the audio clip to this object
            AudioManager.Instance.PlayMusic(GameManager.Instance.CharacterSelectMusicClip);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

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
                    GameStateManager.Instance.TransitionState(_gameGameStateData);
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

// TODO: why is this necessary???
selectState.Viewer.PlayerUI.SwitchToCharacterSelect(selectState);
                selectState.Viewer.PlayerUI.CharacterSelect.SetState(selectState, ready == joined);
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
