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

        public override void Run()
        {
            _image.DOFade(_to, _duration);
        }

        public override void Reset()
        {
            Color color = _image.color;
            color.a = _from;
            _image.color = color;
        }
    }
}
