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

        public bool Collision(IPlayer player, Collider other)
        {
            Vector3 position = player.GameObject.transform.position;
            switch(Type)
            {
            case BoundaryType.Ground:
                position.y = transform.position.y + _collider.size.y + 2.0f;
                break;
            case BoundaryType.Sky:
                position.y = transform.position.y - _collider.size.y - 2.0f;
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
