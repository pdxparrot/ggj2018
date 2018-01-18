using ggj2018.Core.UI;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.Game.Loading
{
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
    }
}
