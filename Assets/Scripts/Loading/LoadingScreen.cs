using ggj2018.UI;
using ggj2018.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.Loading
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
