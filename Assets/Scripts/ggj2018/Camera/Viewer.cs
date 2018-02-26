using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.VFX;
using pdxpartyparrot.ggj2018.Players;
using pdxpartyparrot.ggj2018.UI;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Camera
{
    public class Viewer : FollowViewer
    {
        [Header("UI")]

        [SerializeField]
        private PlayerUI _playerUIPrefab;

        public PlayerUI PlayerUI { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            PlayerUI = Instantiate(_playerUIPrefab, transform);
            PlayerUI.Initialize(this);
        }

        protected override void OnDestroy()
        {
            Destroy(PlayerUI.gameObject);
            PlayerUI = null;

            base.OnDestroy();
        }
#endregion

        public void Initialize(Player owner)
        {
            PlayerUI.Initialize(owner);

            SetFov(owner.Bird.Type.ViewFOV);
            SetOrbitRadius(owner.Bird.Type.FollowOrbitRadius);

            SetGlobalPostProcessProfile(owner.Bird.Type.PostProcessProfile.Clone());
        }
    }
}
