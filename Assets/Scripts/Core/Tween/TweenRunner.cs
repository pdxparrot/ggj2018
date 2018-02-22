using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public abstract class TweenRunner : MonoBehavior
    {
        [SerializeField]
        private bool _runOnAwake = true;

        [SerializeField]
        private bool _resetOnEnable;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Reset();

            if(_runOnAwake) {
                Run();
            }
        }

        protected virtual void OnEnable()
        {
            if(_resetOnEnable) {
                Reset();
                Run();
            }
        }
#endregion

        public abstract void Run();

        public abstract void Reset();
    }
}
