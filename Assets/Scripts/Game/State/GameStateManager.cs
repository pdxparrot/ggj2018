using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public sealed class GameStateManager : SingletonBehavior<GameStateManager>
    {
        [SerializeField]
        private GameStateData _initialGameStateData;

        [SerializeField]
        [ReadOnly]
        private GameState _currentGameState;

        public GameState CurrentState => _currentGameState;

#region Unity Lifecycle
        private void Start()
        {
            TransitionState(_initialGameStateData);
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

        public void TransitionState(GameStateData gameStateData)
        {
            ExitCurrentState();

            _currentGameState = gameStateData.InstantiateGameState(transform);
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
