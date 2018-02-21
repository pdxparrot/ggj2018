using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Loading
{
    [RequireComponent(typeof(Canvas))]
    public sealed class LoadingScreen : MonoBehavior
    {
        [SerializeField]
        private ProgressBar _progressBar;

        public ProgressBar Progress => _progressBar;

        [SerializeField]
        private Text _progressText;

        public string ProgressText
        {
            get { return _progressText.text; }

            set { _progressText.text = value; }
        }

#region Unity Lifecycle
        private void Awake()
        {
            GetComponent<Canvas>().sortingOrder = 9999;
        }
#endregion
    }
}
