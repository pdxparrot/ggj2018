using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.World
{
    [RequireComponent(typeof(Collider))]
    public class WorldBoundary : MonoBehavior
    {
        public enum BoundaryType
        {
            Ground,
            Sky,
            Wall
        }

        [SerializeField]
        private BoundaryType _boundaryType = BoundaryType.Wall;

        public BoundaryType Type => _boundaryType;

        public bool IsVertical => BoundaryType.Ground == _boundaryType || BoundaryType.Sky == _boundaryType;

        [SerializeField]
        private bool _isPredatorBound = true;

        [SerializeField]
        private bool _isPreyBound = true;

        [SerializeField]
        private bool _useSolidGizmos;

        private Collider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.material = GameManager.Instance.FrictionlessMaterial;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            if(null == player) {
                return;
            }

            if(BoundaryType.Ground == Type && player.State.IsDead) {
                PlayerManager.Instance.DespawnPlayer(player);
                return;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            if(!_isPredatorBound) {
                Gizmos.color = Color.blue;
            } else if(!_isPreyBound) {
                Gizmos.color = Color.red;
            }

            BoxCollider boxCollider = _collider as BoxCollider ?? GetComponent<BoxCollider>();
            if(_useSolidGizmos) {
                Gizmos.DrawCube(transform.position + boxCollider.center, boxCollider.size);
            } else {
                Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
            }
        }
#endregion
    }
}
