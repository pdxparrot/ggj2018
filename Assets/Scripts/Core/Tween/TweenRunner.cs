using DG.Tweening;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public abstract class TweenRunner : MonoBehavior
    {
        [SerializeField]
        private bool _runOnAwake = true;

        [SerializeField]
        private bool _resetOnEnable = true;

#region Duration
        [SerializeField]
        private float _duration = 1.0f;

        protected float Duration => _duration;
#endregion

#region Looping
        [SerializeField]
        private int _loops = 0;

        [SerializeField]
        LoopType _loopType = LoopType.Restart;
#endregion

#region Delay
        [SerializeField]
        private float _firstRunDelay = 0.0f;

        [SerializeField]
        private float _delay = 0.0f;
#endregion

#region Chaining
        [SerializeField]
        [CanBeNull]
        private TweenRunner _nextTween;
#endregion

        [SerializeField]
        [ReadOnly]
        private bool _firstRun = true;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            if(null != _nextTween) {
                _nextTween._runOnAwake = false;
            }

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

        public abstract void Reset();

        public void Run()
        {
            Tweener tweener = CreateTweener()
                .SetDelay(_firstRun ? (_firstRunDelay + _delay) : _delay)
                .SetLoops(_loops, _loopType);

            if(null != _nextTween) {
                tweener.OnComplete(() => {
                    _nextTween.Run();
                });
            }

            _firstRun = false;
        }

        protected abstract Tweener CreateTweener();
    }
}
