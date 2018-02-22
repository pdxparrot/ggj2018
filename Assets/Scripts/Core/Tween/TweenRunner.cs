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

        [SerializeField]
        [ReadOnly]
        private bool _firstRun = true;

        protected bool FirstRun => _firstRun;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Reset();

            if(_runOnAwake) {
                Run();

                _firstRun = false;
            }
        }

        protected virtual void OnEnable()
        {
            if(_resetOnEnable) {
                Reset();
                Run();

                _firstRun = false;
            }
        }
#endregion

        public abstract void Reset();

        public abstract void Run();
    }
}
