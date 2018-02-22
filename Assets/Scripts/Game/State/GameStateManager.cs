using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    // TODO: need to handle game state scene setup/transitioning
    // TODO: need to be able to have a loading screen where necessary
    public sealed class GameStateManager : SingletonBehavior<GameStateManager>
    {
        [SerializeField]
        private GameStateData _initialGameStateData;

        [SerializeField]
        [ReadOnly]
        private GameState _currentGameState;

        public GameState CurrentState => _currentGameState;

#region Unity Lifecycle
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
            TransitionState(_initialGameStateData, initializeState);
        }

        public void TransitionState(GameStateData gameStateData, Action<GameState> initializeState=null)
        {
            ExitCurrentState();

            _currentGameState = gameStateData.InstantiateGameState(transform);
            initializeState?.Invoke(_currentGameState);

            _currentGameState?.OnEnter();

            Debug.Log($"State: {gameStateData.Name}");
        }

        private void ExitCurrentState()
        {
            _currentGameState?.OnExit();

            Destroy(_currentGameState?.gameObject);
            _currentGameState = null;
        }
    }
}
