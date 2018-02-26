using System;
using System.Linq;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018
{
    [Serializable]
    public sealed class CharacterSelectState
    {
        private enum JoinState
        {
            None,
            Joined,
            Ready
        }

#region Join/Ready State
        [Header("Join State")]

        [SerializeField]
        [ReadOnly]
        private JoinState _joinState = JoinState.None;

        public bool IsJoined => _joinState == JoinState.Joined;

        public bool IsReady => _joinState == JoinState.Ready;

        public bool IsJoinedOrReady => _joinState == JoinState.Joined || _joinState == JoinState.Ready;
#endregion

        [Space(10)]

#region Selected Bird State
        [Header("Bird Selection")]

        [SerializeField]
        [ReadOnly]
        private int _selectedBird;

        public int SelectedBird { get { return _selectedBird; } set { _selectedBird = value; } }

        public BirdTypeData PlayerBirdData => GameManager.Instance.BirdData.Birds.ElementAt(SelectedBird);
#endregion

        [SerializeField]
        [ReadOnly]
        private Camera.Viewer _viewer;

        public Camera.Viewer Viewer => _viewer;

        [SerializeField]
        [ReadOnly]
        private int _controllerIndex;

        public int ControllerIndex => _controllerIndex;

        [SerializeField]
        [ReadOnly]
        private Player _player;

        public Player Player { get { return _player; } set { _player = value; } }

        public CharacterSelectState(int controllerIndex)
        {
            _controllerIndex = controllerIndex;
            _viewer = CameraManager.Instance.AcquireViewer() as Camera.Viewer;

            Viewer.PlayerUI.Hide();
            Viewer.PlayerUI.PlayerUIPage.SwitchToCharacterSelect(this);
        }

        public void Update(bool allReady)
        {
            if(IsReady) {
                if(InputManager.Instance.Pressed(ControllerIndex, InputManager.Button.B)) {
                    _joinState = JoinState.Joined;
                }
            } else if(IsJoined) {
                if(InputManager.Instance.Pressed(ControllerIndex, InputManager.Button.A)) {
                    _joinState = JoinState.Ready;
                } else if(InputManager.Instance.Pressed(ControllerIndex, InputManager.Button.B)) {
                    _joinState = JoinState.None;
                } else {
                    if(InputManager.Instance.DpadPressed(ControllerIndex, InputManager.DPadDir.Right)) {
                        SelectedBird = MathUtil.WrapMod(SelectedBird + 1, GameManager.Instance.BirdData.Birds.Count);
                    } else if(InputManager.Instance.DpadPressed(ControllerIndex, InputManager.DPadDir.Left)) {
                        SelectedBird = MathUtil.WrapMod(SelectedBird - 1, GameManager.Instance.BirdData.Birds.Count);
                    }
                }
            } else {
                if(InputManager.Instance.PositivePressed(ControllerIndex)) {
                    _joinState = JoinState.Joined;
                    SelectedBird = 0;
                }
            }

            Viewer.PlayerUI.PlayerUIPage.CharacterSelect.SetState(this, allReady);
        }

        public void Finish()
        {
            Player = null;
            if(IsReady) {
                Player = PlayerManager.Instance.SpawnPlayer(GameManager.Instance.GameType.GameTypeData, this);
            } else if(DebugManager.Instance.SpawnMaxLocalPlayers) {
                SelectedBird = ControllerIndex % GameManager.Instance.BirdData.Birds.Count;
                Player = PlayerManager.Instance.SpawnPlayer(GameManager.Instance.GameType.GameTypeData, this);
            }

            if(null == Player) {
                Viewer.PlayerUI.Hide();

                CameraManager.Instance.ReleaseViewer(Viewer);
                return;
            }

            Player.Viewer.PlayerUI.PlayerUIPage.SwitchToGame(Player, GameManager.Instance.GameType);
            Player.Viewer.PlayerUI.ShowTargetingReticle(Player.Bird.Type.ShowTargetingReticle);
        }
    }
}
