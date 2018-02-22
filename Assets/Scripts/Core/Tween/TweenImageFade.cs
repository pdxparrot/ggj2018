using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace pdxpartyparrot.Core.Tween
{
    public class TweenImageFade : TweenRunner
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private float _from = 0.0f;

        [SerializeField]
        private float _to = 1.0f;

        [SerializeField]
        private float _duration = 1.0f;

        [SerializeField]
        private float _firstRunDelay = 5.0f;

        public override void Reset()
        {
            Color color = _image.color;
            color.a = _from;
            _image.color = color;
        }

        public override void Run()
        {
            Tweener tween = _image.DOFade(_to, _duration);
            if(FirstRun) {
                tween.SetDelay(_firstRunDelay);
            }
        }
    }
}
