using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.Players;

using UnityEngine;

namespace ggj2018.ggj2018.World
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

        [SerializeField]
        private bool _useSolidGizmos;

        public BoundaryType Type => _boundaryType;

        public bool IsVertical => BoundaryType.Ground == _boundaryType || BoundaryType.Sky == _boundaryType;

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
            if(_useSolidGizmos) {
                Gizmos.DrawCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
            } else {
                Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
            }
        }
#endregion
    }
}
