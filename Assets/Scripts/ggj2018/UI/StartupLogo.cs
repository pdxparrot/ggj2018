using pdxpartyparrot.Core.Tween;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.UI
{
    [RequireComponent(typeof(Canvas))]
    public class StartupLogo : MonoBehavior
    {
        [SerializeField]
        private TweenImageFade _fader;

#region Unity Lifecycle
        private void Awake()
        {
            GetComponent<Canvas>().sortingOrder = 9990;
        }

        private void OnEnable()
        {
            _fader.Reset();

            _fader.Run();
        }
#endregion
    }
}
