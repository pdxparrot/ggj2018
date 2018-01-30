using ggj2018.Core.Camera;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Viewer : BaseViewer
    {
        [SerializeField]
        private PlayerUIPage _ui;

        public PlayerUIPage PlayerUI => _ui;

        [SerializeField]
        private BirdVisionSelector _visionSelector;

        public void Initialize(BirdType birdType)
        {
            SetFov(birdType.ViewFOV);

            _visionSelector.SetVision(birdType);
        }
    }
}
