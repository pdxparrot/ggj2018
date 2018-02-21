using UnityEngine;

namespace ggj2018.Core.Util
{
    public static class MathUtil
    {
        public static int WrapMod(int n, int m)
        {
            return (int)WrapMod((float)n, (float)m);
        }

        public static float WrapMod(float n, float m)
        {
            return n - m * Mathf.Floor(n / m);
        }
    }
}
