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

        private BoxCollider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }
#endregion

        public bool Collision(Player player, Collider other)
        {
            if(BoundaryType.Ground == Type && player.State.IsDead) {
                PlayerManager.Instance.DespawnPlayer(player.State.PlayerNumber);
                return true;
            }

            Vector3 position = player.GameObject.transform.position;
            switch(Type)
            {
            case BoundaryType.Ground:
                position.y = transform.position.y + _collider.size.y + GameManager.Instance.EnvironmentData.BoundaryCollisionPushback;
                break;
            case BoundaryType.Sky:
                position.y = transform.position.y - _collider.size.y - GameManager.Instance.EnvironmentData.BoundaryCollisionPushback;
                break;
            case BoundaryType.Wall:
                // TODO
                break;
            }
            player.GameObject.transform.position = position;

            return true;
        }
    }
}
