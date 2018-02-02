using ggj2018.Core.Camera;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Viewer : BaseViewer
    {
        [SerializeField]
        private PlayerUIPage _playerUIPrefab;

        public PlayerUIPage PlayerUI { get; private set; }

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

        public void Initialize(Player owner)
        {
            SetFov(owner.State.BirdType.ViewFOV);

            SetPostProcessLayer(owner.State.BirdType.PostProcessLayerMask);

            PlayerUI.Initialize(owner);

            PlayerUI.SetScore(0, GameManager.Instance.State.GameType.ScoreLimit(owner.State.BirdType));
            PlayerUI.SetTimer(GameManager.Instance.State.Timer);
        }
    }
}
