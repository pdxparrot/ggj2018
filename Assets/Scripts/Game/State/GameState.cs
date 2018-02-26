using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public abstract class GameState : MonoBehavior
    {
        public string Name => name;

        [SerializeField]
        private string _sceneName;

        public string SceneName => _sceneName;

        public bool HasScene => !string.IsNullOrWhiteSpace(SceneName);

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}
