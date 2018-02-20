using ggj2018.Core.Util;
using ggj2018.Game.Data;

using UnityEngine;

namespace ggj2018.Game.State
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
