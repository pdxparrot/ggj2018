using ggj2018.Core.Camera;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Viewer : BaseViewer
    {
        [SerializeField]
        private PlayerUIPage _playerUIPrefab;

        public PlayerUIPage PlayerUI { get; private set; }

        [SerializeField]
        private BirdVisionSelector _visionSelector;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            PlayerUI = Instantiate(_playerUIPrefab, transform);
            PlayerUI.GetComponent<Canvas>().worldCamera = UICamera;
        }

        private void OnDestroy()
        {
            Destroy(PlayerUI);
            PlayerUI = null;
        }
#endregion

        public void Initialize(BirdType birdType)
        {
            SetFov(birdType.ViewFOV);

            _visionSelector.SetVision(birdType);
        }
    }
}
