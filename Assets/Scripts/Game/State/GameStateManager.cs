using System;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    // TODO: need to handle game state scene setup/transitioning
    // TODO: need to be able to have a loading screen where necessary
    // TODO: all game states should be created at startup and enabled/disabled
    // as necessary (if we even need to enable/disable them, since we control updates and shit)
    // so that we can avoid instantiating new objects
    public sealed class GameStateManager : SingletonBehavior<GameStateManager>
    {
        [SerializeField]
        private GameState _initialGameStatePrefab;

        [SerializeField]
        [ReadOnly]
        private GameState _currentGameState;

        public GameState CurrentState => _currentGameState;

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            ExitCurrentState();

            base.OnDestroy();
        }

        private void Update()
        {
            _currentGameState?.OnUpdate(Time.deltaTime);
        }
#endregion

        public void TransitionToInitialState(Action<GameState> initializeState=null)
        {
            TransitionState(_initialGameStatePrefab, initializeState);
        }

        public void TransitionState(GameState gameStatePrefab, Action<GameState> initializeState=null)
        {
            ExitCurrentState();

            _currentGameState = Instantiate(gameStatePrefab, transform);
            initializeState?.Invoke(_currentGameState);

            _currentGameState.OnEnter();

            Debug.Log($"State: {_currentGameState.Name}");
        }

        private void ExitCurrentState()
        {
            _currentGameState?.OnExit();

            Destroy(_currentGameState?.gameObject);
            _currentGameState = null;
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "GameStateManager");

            debugMenuNode.AddLabel(() => $"Current Game State: {CurrentState.name}");
        }
    }
}
