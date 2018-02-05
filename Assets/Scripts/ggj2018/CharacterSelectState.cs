using System;
using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [Serializable]
    public sealed class CharacterSelectState
    {
        public enum JoinState
        {
            None,
            Joined,
            Ready
        }

#region Join/Ready State
        [SerializeField]
        private JoinState _joinState = JoinState.None;

        public JoinState PlayerJoinState { get { return _joinState; } set { _joinState = value; } }

        public bool IsJoined => PlayerJoinState == JoinState.Joined;

        public bool IsJoinedOrReady => PlayerJoinState == JoinState.Joined || PlayerJoinState == JoinState.Ready;

        public bool IsReady => PlayerJoinState == JoinState.Ready;
#endregion

#region Selected Bird State
        [SerializeField]
        private int _selectedBird;

        public int SelectedBird { get { return _selectedBird; } set { _selectedBird = value; } }

        public BirdData.BirdDataEntry PlayerBirdData => GameManager.Instance.BirdData.Birds.ElementAt(SelectedBird);
#endregion

        [SerializeField]
        private Viewer _viewer;

        public Viewer Viewer => _viewer;

        [SerializeField]
        private int _controllerIndex;

        public int ControllerIndex => _controllerIndex;

        [SerializeField]
        private Player _player;

        public Player Player { get { return _player; } set { _player = value; } }

        public CharacterSelectState(int controllerIndex)
        {
            _controllerIndex = controllerIndex;
        }

        public void Reset()
        {
            _joinState = JoinState.None;
            _selectedBird = 0;
            _viewer = CameraManager.Instance.AcquireViewer() as Viewer;
            _player = null;
        }

        public void NextBird()
        {
            SelectedBird++;
            WrapBird();
        }

        public void PrevBird()
        {
            SelectedBird--;
            WrapBird();
        }

        private void WrapBird()
        {
            if(SelectedBird < 0) {
                SelectedBird = GameManager.Instance.BirdData.Birds.Count - 1;
            } else if(SelectedBird >= GameManager.Instance.BirdData.Birds.Count) {
                SelectedBird = 0;
            }
        }
    }
}
