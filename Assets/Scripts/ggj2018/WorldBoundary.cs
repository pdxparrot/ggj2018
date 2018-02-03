using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
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

#region Unity Lifecycle
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
        }
#endregion

        public bool Collision(Player player)
        {
            if(BoundaryType.Ground == Type && player.State.IsDead) {
                PlayerManager.Instance.DespawnPlayer(player);
            }
            return true;
        }
    }
}
