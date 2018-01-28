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

        public bool Collision(Collider other)
        {
            return true;
        }
    }
}
