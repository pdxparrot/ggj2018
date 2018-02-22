using DG.Tweening;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public class TweenScale : TweenRunner
    {
        [SerializeField]
        [ReadOnly]
        private Vector3 _from;

        [SerializeField]
        private Vector3 _to = new Vector3();

#region Unity Lifecycle
        protected override void Awake()
        {
            _from = transform.localScale;

            base.Awake();
        }
#endregion

        public override void Reset()
        {
            base.Reset();

            transform.localScale = _from;
        }

        protected override Tweener CreateTweener()
        {
            return transform.DOScale(_to, Duration);
        }
    }
}
