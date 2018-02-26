using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.UI
{
    [RequireComponent(typeof(Canvas))]
    public class TargetingReticle : MonoBehavior
    {
        [SerializeField]
        private float _offset = 2.5f;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Player _player;

#region Unity Lifecycle
        private void Update()
        {
            if(null != _player) {
                transform.position = _player.transform.position + (_player.Bird.transform.forward * _offset);
            }
        }
#endregion

        public void Initialize([CanBeNull] Player player)
        {
            _player = player;

            GetComponent<Canvas>().worldCamera = player?.Viewer.Camera;
        }
    }
}
