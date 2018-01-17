using UnityEngine;

namespace ggj2018.Util
{
    public static class Vector3Extensions
    {
        public static Vector3 FacingDirection2D(this Vector3 vector, Direction direction)
        {
            return Quaternion.AngleAxis((int)direction, Vector3.forward) * vector;
        }
    }
}
