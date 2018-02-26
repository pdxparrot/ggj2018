using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Camera;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.UI
{
    public class PlayerUI : MonoBehavior
    {
        [SerializeField]
        private PlayerUIPage _playerUIPage;

        public PlayerUIPage PlayerUIPage => _playerUIPage;

        [SerializeField]
        private TargetingReticle _targetingReticle;

        public void Initialize(Viewer viewer)
        {
            PlayerUIPage.Initialize(viewer);

            _targetingReticle.Initialize(null);
        }

        public void Initialize(Player player)
        {
            PlayerUIPage.Initialize(player.Viewer);

            _targetingReticle.Initialize(player);
        }

        public void Hide()
        {
            PlayerUIPage.Hide();

            _targetingReticle.gameObject.SetActive(false);
        }

        public void ShowTargetingReticle(bool show)
        {
            _targetingReticle.gameObject.SetActive(show);
        }
    }
}
